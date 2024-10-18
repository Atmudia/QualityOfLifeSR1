
using AssetsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QoLMod
{
  public class SiloUIActivator : UIActivator
  {
    public SiloStorage storage;
    public Dictionary<Identifiable.Id, int> allSlots;

    private void Awake() => storage = GetComponentInParent<SiloStorage>();

    public override bool CanActivate()
    {
      allSlots = new Dictionary<Identifiable.Id, int>();
      foreach (Ammo.Slot slot in storage.model.siloAmmo.SelectMany(x => x.Value.slots))
      {
        if (slot != null && !Identifiable.IsSlime(slot.id) && !Identifiable.IsLargo(slot.id) && !Identifiable.IsTarr(slot.id))
        {
          if (!allSlots.ContainsKey(slot.id))
            allSlots.Add(slot.id, slot.count);
          else
            allSlots[slot.id] += slot.count;
        }
      }
      return allSlots.Count != 0;
    }

    public override GameObject Activate()
    {
      BaseUI baseUI = null;
      List<IdentInventoryItem> list = allSlots.Select(keyValue => new IdentInventoryItem(keyValue.Key, keyValue.Value, () =>
      {
        if (!SRSingleton<SceneContext>.Instance.PlayerState.Ammo.MaybeAddToSlot(keyValue.Key, keyValue.Key.GetPrefab().GetComponent<Identifiable>()))
          return false;
        storage.GetRelevantAmmo().Decrement(keyValue.Key);
        baseUI.Close();
        if (CanActivate())
          Activate();
        return true;
      })).ToList();
      baseUI = UIUtils.CreateInventoryUI("t.silo", PediaDirector.Id.SILO.GetIcon(), list, false, null).GetComponent<BaseUI>();
      return baseUI.gameObject;
    }
  }
}
