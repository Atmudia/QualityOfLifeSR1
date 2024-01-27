
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace QoLMod.Beacon
{
  public class BeaconActivator : UIActivator
  {
    public BeaconGadget BeaconGadget;
    public static Dictionary<string, Sprite> PersistentKeysWithSprites = new Dictionary<string, Sprite>();

    public void Awake() => BeaconGadget = GetComponentInParent<BeaconGadget>();

    public override bool CanActivate() => true;

    public override GameObject Activate()
    {
      GameObject purchaseUI = null;
      purchaseUI = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(PersistentKeysWithSprites["iconBeaconBase"], "t.beacon", PersistentKeysWithSprites.Values.Select<Sprite, PurchaseUI.Purchasable>(sprite => new PurchaseUI.Purchasable("m.beacon.name." + sprite.name.Replace("iconBeacon", string.Empty).ToLowerInvariant(), sprite, sprite, "m.beacon.desc", 0, new PediaDirector.Id?(), () =>
      {
        BeaconGadget?.SetBeacon(sprite.name);
        purchaseUI.GetComponent<PurchaseUI>().Rebuild(true);
      }, () => true, () => !BeaconGadget.IsSelectedBeacon(sprite.name))).ToArray<PurchaseUI.Purchasable>(), false, () => { }, true);
      purchaseUI.GetComponent<PurchaseUI>().SetPurchaseMsgs("b.select", "b.already_selected");
      return purchaseUI;
    }
  }
}
