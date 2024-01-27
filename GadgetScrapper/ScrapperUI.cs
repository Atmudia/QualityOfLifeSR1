
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Console = SRML.Console.Console;

namespace QoLMod.GadgetScrapper
{
  public class ScrapperUI : BaseUI
  {
    public Dictionary<string, string> categoryMap = new Dictionary<string, string>();
    public static Dictionary<PurchaseUI.Purchasable, GadgetDefinition.CraftCost[]> cachedCraftCost = new Dictionary<PurchaseUI.Purchasable, GadgetDefinition.CraftCost[]>();
    public PurchaseUI purchaseUI;

    public override void Awake()
    {
      base.Awake();
      cachedCraftCost.Clear();
      BuildUI();
    }

    public void BuildUI()
    {
      for (int index = 0; index < transform.childCount; ++index)
        Destroyer.Destroy(transform.GetChild(index).gameObject, "ScrapperUI.RebuildUI");
      GameObject purchaseUi = CreatePurchaseUI();
      purchaseUi.transform.SetParent(transform, false);
      statusArea = purchaseUi.GetComponent<PurchaseUI>().statusArea;
    }

    private GameObject CreatePurchaseUI()
    {
      categoryMap.Clear();
      GadgetDirector gadgetDir = SRSingleton<SceneContext>.Instance.GadgetDirector;
      List<PurchaseUI.Purchasable> purchasableList1 = new List<PurchaseUI.Purchasable>();
      Dictionary<PediaDirector.Id, List<PurchaseUI.Purchasable>> dict = new Dictionary<PediaDirector.Id, List<PurchaseUI.Purchasable>>();
      foreach (KeyValuePair<Gadget.Id, int> gadget in gadgetDir.model.gadgets)
      {
        if (gadget.Value != 0)
        {
          GadgetDefinition entry;
          try
          {
            entry = gadget.Key.GetGadgetDefinition();

          }
          catch
          {
            EntryPoint.ConsoleInstance.LogWarning(gadget.Key.ToString());
            continue;
          }
          string lowerInvariant = gadget.Key.ToString().ToLowerInvariant();
          string descKey = "m.gadget.desc." + lowerInvariant;
          string str = "m.gadget.name." + lowerInvariant;
          GadgetDefinition.CraftCost[] entryCraftCosts = entry.craftCosts;
          PurchaseUI.Purchasable key = new PurchaseUI.Purchasable(str, entry.icon, entry.icon, descKey, 0, new PediaDirector.Id?(entry.pediaLink), () => ScrapBlueprint(entry.id, entryCraftCosts), () => true, () => true, currCount: () => gadgetDir.GetGadgetCount(entry.id));
          purchasableList1.Add(key);
          cachedCraftCost.Add(key, entryCraftCosts);
          categoryMap[str] = entry.pediaLink.ToString().ToLowerInvariant();
          List<PurchaseUI.Purchasable> purchasableList2 = dict.Get<PediaDirector.Id, List<PurchaseUI.Purchasable>>(entry.pediaLink) ?? new List<PurchaseUI.Purchasable>();
          purchasableList2.Add(key);
          dict[entry.pediaLink] = purchasableList2;
        }
      }
      GameObject purchaseUi = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(EntryPoint.CreateSpriteFromImage("iconGadgetScrap"), MessageUtil.Qualify("ui", "t.scrap_gadget"), purchasableList1.ToArray(), true, new PurchaseUI.OnClose(Close));
      List<PurchaseUI.Category> list = PediaUI.SCIENCE_ENTRIES.Where(key => dict.ContainsKey(key)).Select<PediaDirector.Id, PurchaseUI.Category>(key => new PurchaseUI.Category(key.ToString().ToLowerInvariant(), dict[key].ToArray())).ToList<PurchaseUI.Category>();
      purchaseUI = purchaseUi.GetComponent<PurchaseUI>();
      purchaseUI.SetCategories(list);
      purchaseUI.SetPurchaseMsgs("b.scrap_gadget", "b.sold_out");
      return purchaseUi;
    }

    public void ScrapBlueprint(Gadget.Id id, GadgetDefinition.CraftCost[] craftCosts)
    {
      if (SRSingleton<SceneContext>.Instance.GadgetDirector.GetGadgetCount(id) == 0)
      {
        Error("e.cannot_scrap_gadget");
      }
      else
      {
        foreach (GadgetDefinition.CraftCost craftCost in craftCosts)
          SRSingleton<SceneContext>.Instance.GadgetDirector.AddToRefinery(craftCost.id, craftCost.amount, true);
        SRSingleton<SceneContext>.Instance.GadgetDirector.RemoveGadget(id);
        purchaseUI.PlayPurchaseFX();
        BuildUI();
        if (purchaseUI.selected != null)
          purchaseUI.Rebuild(true);
      }
    }

    public void PlayPurchaseCue() => Play(SRSingleton<GameContext>.Instance.UITemplates.purchaseBlueprintCue);
  }
}
