using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AssetsLib;
using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using QoLMod.Beacon;
using QoLMod.GadgetScrapper;
using QoLMod.OffTarrMusicOption;
using SRML;
using SRML.Console;
using SRML.SR;
using SRML.SR.Patches;
using SRML.SR.SaveSystem;
using SRML.SR.Translation;
using SRML.Utils;
using UnityEngine;
using UnityEngine.UI;
using Console = SRML.Console.Console;

namespace QoLMod
{
	public class EntryPoint : ModEntryPoint
	{
		public override void PreLoad()
		{
			ConsoleInstance = base.ConsoleInstance;
			foreach (string resourceName in from name in execAssembly.GetManifestResourceNames() 
			         where name.Contains("iconBeacon")
			         select name)
			{
				Sprite beaconSprite = CreateSpriteFromTexture2D(CreateTexture2DWithFullNameFromImage(resourceName));
				string beaconName = beaconSprite.name.Replace("iconBeacon", string.Empty);
				TranslationPatcher.AddPediaTranslation("m.beacon.name." + beaconName.ToLowerInvariant(), beaconName + " Beacon");
				BeaconActivator.PersistentKeysWithSprites.Add(beaconSprite.name, beaconSprite);
			}
			HarmonyInstance.PatchAll();
			TranslationPatcher.AddUITranslation("l.tarr_music", "Tarr Music");
			TranslationPatcher.AddUITranslation("m.count_of_scrap", "<color=#0000FF> {0} </color> > {1}");
			TranslationPatcher.AddUITranslation("t.scrap_gadget", "Gadget Scrapper");
			TranslationPatcher.AddUITranslation("t.multiteleporter", "Multi-Teleporter");
			TranslationPatcher.AddUITranslation("b.scrap_gadget", "Scrap Gadget");
			TranslationPatcher.AddUITranslation("l.returns", "Returns: ");
			TranslationPatcher.AddUITranslation("b.disable_all_ss", "Disable All SS");
			TranslationPatcher.AddUITranslation("b.active_all_ss", "Enable All SS");
			TranslationPatcher.AddUITranslation("w.not_on_ranch_decorizer_link", "Decorizer Link can only be installed on the Ranch with Decorizers");
			TranslationPatcher.AddUITranslation("m.gadget.decorizerlink", "Decorizer Link");
			TranslationPatcher.AddUITranslation("m.gadget.multiteleport", "Multi Teleport");
			TranslationPatcher.AddUITranslation("e.cannot_scrap_gadget", "Can't scrap the gadget because you don' have that gadget");
			TranslationPatcher.AddUITranslation("m.cycle_of_gadget", "Remaining Cycles: {0}");
			TranslationPatcher.AddPediaTranslation("m.upgrade.name.silo.silo_interface", "Silo Interface");
			TranslationPatcher.AddPediaTranslation("m.upgrade.desc.silo.silo_interface", "An interface has been added to the silo for the purpose of using a vacuum to remove items from the inside. This upgrade allows for more efficient and convenient cleaning of the silo.<color=#ff0000> Warning: Slimes are not compatible with this interface and will be not compatible</color>.");
			Enums.DECORIZER_LINK.GetTranslation()
				.SetNameTranslation("Decorizer Link")
				.SetDescriptionTranslation("A compact decorizer link that allows you to shoot and vacuum the decoration to decorizer.");
			TranslationPatcher.AddPediaTranslation("m.beacon.desc", "Add a beacon to the map and radar to more easily navigate.");
			TranslationPatcher.AddPediaTranslation("t.beacon", "Beacon");
			Enums.BEACON_GADGET.GetTranslation()
				.SetNameTranslation("Beacon")
				.SetDescriptionTranslation("Never get lost again, add beacons to the map and radar!");
			Enums.TELEPORTER_MULTI.GetTranslation()
				.SetNameTranslation("MultiTeleport")
				.SetDescriptionTranslation("The Advanced Teleporter is a central network hub that links all other teleporters together in the Far, Far, Range!");
			TranslationPatcher.AddUITranslation("b.sleep_six_hours", "Sleep Six Hours");
			TranslationPatcher.AddUITranslation("b.sleep_until_night", "Sleep Until Night");
			
		}

		public override void Load()
		{
			GameObject gameObject = new GameObject("");
			gameObject.Prefabitize();
			gameObject.name = "gadgetBeacon(Clone)";
			gameObject.AddComponent<BeaconGadget>().id = Enums.BEACON_GADGET;
			BeaconDisplayOnMap beaconDisplayOnMap = gameObject.AddComponent<BeaconDisplayOnMap>();
			beaconDisplayOnMap.markerPrefab = PrefabUtils.CopyPrefab(Gadget.Id.TELEPORTER_RED.GetGadgetDefinition().prefab.GetComponent<DisplayOnMap>().markerPrefab.gameObject).GetComponent<MapMarker>();
			beaconDisplayOnMap.markerPrefab.name = "BeaconGadget";
			LookupRegistry.RegisterGadget(CreateGadgetDefinition(gameObject, Enums.BEACON_GADGET, PediaDirector.Id.WARP_TECH, 1, 0, BeaconActivator.PersistentKeysWithSprites["iconBeaconBase"], new GadgetDefinition.CraftCost[]
			{
				new GadgetDefinition.CraftCost
				{
					id = Identifiable.Id.PINK_PLORT,
					amount = 5
				},
				new GadgetDefinition.CraftCost
				{
					id = Identifiable.Id.ROCK_PLORT,
					amount = 5
				},
				new GadgetDefinition.CraftCost
				{
					id = Identifiable.Id.PHOSPHOR_PLORT,
					amount = 5
				},
				new GadgetDefinition.CraftCost
				{
					id = Identifiable.Id.SPIRAL_STEAM_CRAFT,
					amount = 1
				}
			}));
			DataModelRegistry.RegisterCustomGadgetModel(Enums.BEACON_GADGET, (GadgetSiteModel site, GameObject obj) => new BeaconGadgetModel(obj.GetComponent<Gadget>().id, site.id, obj.transform));
			SaveRegistry.RegisterSerializableGadgetModel<BeaconGadgetModel>(0);
			GameObject decorizerLinkObj = PrefabUtils.CopyPrefab(Gadget.Id.MARKET_LINK.GetGadgetDefinition().prefab);
			decorizerLinkObj.GetComponent<Gadget>().id = Enums.DECORIZER_LINK;

			Transform triggerDepositTransform = decorizerLinkObj.transform.Find("triggerDeposit");
			triggerDepositTransform.gameObject.RemoveComponentImmediate<ScorePlort>();

			SiloCatcher siloCatcher = triggerDepositTransform.gameObject.AddComponent<SiloCatcher>();
			siloCatcher.CopyFields(Gadget.Id.REFINERY_LINK.GetGadgetDefinition().prefab.GetComponentInChildren<SiloCatcher>());

			LookupRegistry.RegisterGadget(CreateGadgetDefinition(decorizerLinkObj, Enums.DECORIZER_LINK, PediaDirector.Id.WARP_TECH, 1000, 0, CreateSpriteFromImage("iconGadgetDecorizerLink"), new GadgetDefinition.CraftCost[]
			{
				new GadgetDefinition.CraftCost { id = Identifiable.Id.QUANTUM_PLORT, amount = 15 },
				new GadgetDefinition.CraftCost { id = Identifiable.Id.MANIFOLD_CUBE_CRAFT, amount = 15 },
				new GadgetDefinition.CraftCost { id = Identifiable.Id.HEXACOMB_CRAFT, amount = 15 },
				new GadgetDefinition.CraftCost { id = Identifiable.Id.ROYAL_JELLY_CRAFT, amount = 1 }
			}));
			
			GadgetRegistry.RegisterBlueprintLock(Enums.BEACON_GADGET, director => new GadgetDirector.BlueprintLocker(director, Enums.TELEPORTER_MULTI, delegate
			{
				var flag2 = !Levels.IsLevel("worldGenerated");
				return !flag2 && 2 >= SRSingleton<SceneContext>.Instance.PlayerState.model.unlockedZoneMaps.Count;
			}, 0f));
			GameObject multiTeleporterObj = PrefabUtils.CopyPrefab(Gadget.Id.TELEPORTER_VIOLET.GetGadgetDefinition().prefab);
			multiTeleporterObj.name = "gadgetTeleportMulti";
			multiTeleporterObj.GetComponent<Gadget>().id = Enums.TELEPORTER_MULTI;

			Transform teleportColliderTransform = multiTeleporterObj.transform.Find("Teleport Collider");
			teleportColliderTransform.GetComponent<TeleportSource>().Destroy();
			teleportColliderTransform.GetComponent<TeleporterGadget>().Destroy();
			teleportColliderTransform.gameObject.AddComponent<MultiTeleportDestination>();
            
			multiTeleporterObj.transform.Find("Teleport FX (1)").gameObject.SetActive(true);
			multiTeleporterObj.AddComponent<Recolorizer>();
			multiTeleporterObj.transform.Find("decoGlassScreen01").gameObject.SetActive(false);

			LookupRegistry.RegisterGadget(CreateGadgetDefinition(multiTeleporterObj, Enums.TELEPORTER_MULTI, PediaDirector.Id.WARP_TECH, 100, 0, CreateSpriteFromImage("iconGadgetTeleportMulti"), new GadgetDefinition.CraftCost[]
			{
				new GadgetDefinition.CraftCost { id = Identifiable.Id.QUANTUM_PLORT, amount = 25 },
				new GadgetDefinition.CraftCost { id = Identifiable.Id.MANIFOLD_CUBE_CRAFT, amount = 10 },
				new GadgetDefinition.CraftCost { id = Identifiable.Id.STRANGE_DIAMOND_CRAFT, amount = 1 }
			}));
			CorporatePartnerRegistry.AddToRank(13, new CorporatePartnerRegistry.RewardEntry(CreateSpriteFromImage("iconGadgetDecorizerLink"), "m.gadget.decorizerlink"));
			CorporatePartnerRegistry.AddToRank(22, new CorporatePartnerRegistry.RewardEntry(CreateSpriteFromImage("iconGadgetScrap"), "t.scrap_gadget"));
			CorporatePartnerRegistry.AddToRank(28, new CorporatePartnerRegistry.RewardEntry(CreateSpriteFromImage("iconGadgetTeleportMulti"), "m.gadget.multiteleport"));
			LandPlotUpgradeRegistry.RegisterPlotUpgrader<SiloInterfaceUpgrader>(LandPlot.Id.SILO);
			LandPlotUpgradeRegistry.UpgradeShopEntry upgradeShopEntry = default(LandPlotUpgradeRegistry.UpgradeShopEntry);
			upgradeShopEntry.cost = 3500;
			upgradeShopEntry.holdtopurchase = false;
			upgradeShopEntry.icon = PediaDirector.Id.SILO.GetIcon();
			upgradeShopEntry.upgrade = Enums.SILO_INTERFACE;
			upgradeShopEntry.isAvailable = (LandPlot plot) => !plot.HasUpgrade(Enums.SILO_INTERFACE);
			upgradeShopEntry.isUnlocked = (LandPlot plot) => true;
			upgradeShopEntry.mainImg = PediaDirector.Id.SILO.GetIcon();
			upgradeShopEntry.landplotPediaId = PediaDirector.Id.SILO;
			upgradeShopEntry.LandPlotName = "silo";
			LandPlotUpgradeRegistry.RegisterPurchasableUpgrade<SiloUI>(upgradeShopEntry);
			foreach (Renderer renderer in multiTeleporterObj.GetComponentsInChildren<Renderer>())
			{
				bool flag = renderer.sharedMaterial.name.Equals("Telepad_Violet");
				if (flag)
				{
					renderer.sharedMaterial.name = "Telepad_Multi";
				}
			}
			SRCallbacks.PreSaveGameLoad += delegate(SceneContext context)
			{
				Renderer renderer = Enums.TELEPORTER_MULTI.GetGadgetDefinition().prefab.GetComponentsInChildren<Renderer>().FirstOrDefault((Renderer x) => x.sharedMaterial.name.Equals("Telepad_Multi"));
				context.RanchDirector.recolorMats[RanchDirector.PaletteType.RANCH_TECH] = SRSingleton<SceneContext>.Instance.RanchDirector.recolorMats[RanchDirector.PaletteType.RANCH_TECH].AddItem(renderer.sharedMaterial).ToArray();
				
				GameObject techFabricator = GameObject.Find("zoneRANCH/cellRanch_Lab/Sector/Ranch Features/Barn/Barn Interior/Shops/techFabricator");
				Vector3 position = techFabricator.transform.position;
				position.z = -312f;
				techFabricator.transform.position = position;
				GameObject.Find("zoneRANCH/cellRanch_Lab/Sector/Ranch Features/Barn/Barn Interior/Floor/prop_rail (3)").SetActive(false);
				GameObject.Find("zoneRANCH/cellRanch_Lab/Sector/Interactives/nodeJournal (3)").transform.position = new Vector3(189.6702f, 17.4259f, -316.0682f);
				
				GameObject techScrapper = GameObject.Find("zoneRANCH/cellRanch_Home/Sector").transform.Find("Upgrades/toyShop").gameObject.InstantiateInactive(new Vector3(189.1417f, 15.1879f, -302.092f), Quaternion.Euler(0f, 88.7653f, 0f), techFabricator.transform.parent, false);
				techScrapper.transform.Find("techSiloTop (1)").gameObject.Destroy();
				techScrapper.name = "techScrapper";
				techScrapper.transform.Find("Additional/techChute").gameObject.SetActive(false);
				techScrapper.GetComponent<ActivateOnProgressRange>().Initialize(delegate(ActivateOnProgressRange range)
				{
					range.maxProgress = int.MaxValue;
					range.minProgress = 22;
				});
				techScrapper.GetComponentInChildren<Image>().sprite = CreateSpriteFromImage("iconGadgetScrap");
				GameObject scienceUIActivator = techScrapper.GetComponentInChildren<ToyShopUIActivator>().gameObject;
				scienceUIActivator.RemoveComponent<ToyShopUIActivator>();
				bool flag3 = ScrapperUI == null;
				if (flag3)
				{
					ScrapperUI = new GameObject("ScrapperUI");
					ScrapperUI.Prefabitize();
					ScrapperUI.AddComponent<ScrapperUI>();
				}
				scienceUIActivator.AddComponent<ScienceUIActivator>().Initialize(delegate(ScienceUIActivator activator)
				{
					activator.uiPrefab = ScrapperUI;
				});
				techScrapper.SetActive(true);
				
				GadgetDefinition gadgetDefinition = Enums.DECORIZER_LINK.GetGadgetDefinition();
				if (gadgetDefinition.prefab.name.Equals("gadgetMarketLink(Clone)"))
				{
					gadgetDefinition.prefab.name = "gadgetDecorizerLink";
					gadgetDefinition.prefab.GetComponentInChildren<SiloCatcher>().type = SiloCatcher.Type.DECORIZER;
					Transform techDecorizer = GameObject.Find("zoneRANCH/cellRanch_Home/Sector/Upgrades/techDecorizer").transform;
					techDecorizer.Find("model_decorizer/techActivator").gameObject.Instantiate(gadgetDefinition.prefab.transform, true).transform.Find("triggerActivate");
					techDecorizer.Find("model_decorizer/button_base").gameObject.Instantiate(gadgetDefinition.prefab.transform, true).transform.localPosition = new Vector3(0f, 1.8462f, 0.209f);
					Transform decorizerSlotUI = techDecorizer.Find("DecorizerSlotUI").Instantiate(gadgetDefinition.prefab.transform, true);
					gadgetDefinition.prefab.transform.Find("techDisplay1x1/MarketIconUI").gameObject.SetActive(false);
					decorizerSlotUI.transform.localPosition = new Vector3(0f, 2.69f, 0f);
					if (SRMLConfig.CREATE_MARKET_BUTTON)
					{
						GadgetDefinition marketLink = Gadget.Id.MARKET_LINK.GetGadgetDefinition();
						marketLink.prefab = PrefabUtils.CopyPrefab(marketLink.prefab);
						marketLink.prefab.name = "gadgetMarketLink";
						GameObject techActivator = techDecorizer.Find("model_decorizer/techActivator").gameObject.Instantiate(marketLink.prefab.transform, true).transform.Find("triggerActivate").gameObject;
						techActivator.RemoveComponent<DecorizerStorageActivator>();
						techActivator.AddComponent<MarketLinkActivator>();
						techDecorizer.Find("model_decorizer/button_base").gameObject.Instantiate(marketLink.prefab.transform, true).transform.localPosition = new Vector3(0f, 1.8462f, 0.209f);
					}
				}
				if (Enums.BEACON_GADGET.GetGadgetDefinition().prefab.name.Equals("gadgetBeacon(Clone)"))
				{
					Enums.BEACON_GADGET.GetGadgetDefinition().prefab.name = "gadgetBeacon";
					GadgetDefinition beaconGadgetDef = Enums.BEACON_GADGET.GetGadgetDefinition();
					GameObject nodeMapEntry = UnityEngine.Object.Instantiate(GameObject.Find("zoneREEF/cellReef_Hub/Sector/Interactives/nodeMapEntry"), beaconGadgetDef.prefab.transform);
					UnityEngine.Object.DestroyImmediate(nodeMapEntry.transform.Find("model_journalDevice/MapHologram").gameObject);
					UnityEngine.Object.DestroyImmediate(nodeMapEntry.transform.Find("model_journalDevice/Active FX").gameObject);
					UnityEngine.Object.DestroyImmediate(nodeMapEntry.GetComponentInChildren<MapDataEntry>());
					nodeMapEntry.AddComponent<BeaconActivator>();
					nodeMapEntry.AddComponent<SphereCollider>();
					nodeMapEntry.transform.localPosition = Vector3.zero;
				}
			};
		}

		private static GadgetDefinition CreateGadgetDefinition(GameObject prefab, Gadget.Id id, PediaDirector.Id pediaLink, int blueprintCost, int buyCountLink, Sprite icon, GadgetDefinition.CraftCost[] craftCosts)
		{
			GadgetDefinition gadgetDefinition = ScriptableObject.CreateInstance<GadgetDefinition>();
			gadgetDefinition.prefab = prefab;
			gadgetDefinition.id = id;
			gadgetDefinition.pediaLink = pediaLink;
			gadgetDefinition.blueprintCost = blueprintCost;
			gadgetDefinition.buyCountLimit = buyCountLink;
			gadgetDefinition.buyInPairs = false;
			gadgetDefinition.icon = icon;
			gadgetDefinition.craftCosts = craftCosts;
			return gadgetDefinition;
		}

		private static Texture2D CreateTexture2DWithFullNameFromImage(string fileName)
		{
			Texture2D texture2D = new Texture2D(4, 4);
			Stream manifestResourceStream = execAssembly.GetManifestResourceStream(fileName);
			bool flag = manifestResourceStream != null;
			Texture2D texture2D2;
			if (flag)
			{
				byte[] array = new byte[manifestResourceStream.Length];
				_ = manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
				texture2D.LoadImage(array);
				texture2D.name = fileName.Replace("QoLMod.Images.Beacons.", string.Empty).Replace(".png", string.Empty);
				texture2D2 = texture2D;
			}
			else
			{
				ConsoleInstance.LogError("Missing Texture2D: " + fileName, true);
				texture2D2 = Resources.FindObjectsOfTypeAll<Sprite>().First((Sprite x) => x.name == "unknownLarge").texture;
			}
			return texture2D2;
		}

		private static Texture2D CreateTexture2DWithNonFullNameFromImage(string fileName)
		{
			string text = "QoLMod.Images." + fileName + ".png";
			Texture2D texture2D = new Texture2D(4, 4);
			Stream manifestResourceStream = execAssembly.GetManifestResourceStream(text);
			Texture2D texture2D2;
			if (manifestResourceStream != null)
			{
				byte[] array = new byte[manifestResourceStream.Length];
				_ = manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
				texture2D.LoadImage(array);
				texture2D.name = fileName;
				texture2D2 = texture2D;
			}
			else
			{
				ConsoleInstance.LogError("Missing Texture2D: " + fileName, true);
				texture2D2 = Resources.FindObjectsOfTypeAll<Sprite>().First((Sprite x) => x.name == "unknownLarge").texture;
			}
			return texture2D2;
		}

		public static Sprite CreateSpriteFromTexture2D(Texture2D texture2D)
		{
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			sprite.name = texture2D.name;
			return sprite;
		}

		public static Sprite CreateSpriteFromImage(string fileName)
		{
			Texture2D texture2D;
			try
			{
				texture2D = CreateTexture2DWithNonFullNameFromImage(fileName);
			}
			catch
			{
				ConsoleInstance.LogError("Missing Sprite: " + fileName, true);
				return Resources.FindObjectsOfTypeAll<Sprite>().First((Sprite x) => x.name == "unknownLarge");
			}
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			sprite.name = fileName;
			return sprite;
		}

		public new static Console.ConsoleInstance ConsoleInstance;

		public static GameObject ScrapperUI;

		public static Assembly execAssembly = Assembly.GetExecutingAssembly();
	}
}
