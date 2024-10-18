
using Assets.Script.Util.Extensions;
using AssetsLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QoLMod
{
  public class MultiTeleportDestination : MonoBehaviour
  {
    public TeleportNetwork teleportNetwork;
    public List<TeleporterInventoryItem> inventoryItems = new List<TeleporterInventoryItem>();

    public void Awake() => teleportNetwork = SRSingleton<SceneContext>.Instance.TeleportNetwork;

    public void OnTriggerEnter(Collider collider)
    {
      if (!PhysicsUtil.IsPlayerMainCollider(collider))
        return;
      StartCoroutine(CreateUI());
    }

    private IEnumerator CreateUI()
    {
      yield return new WaitForEndOfFrame();
      inventoryItems.Clear();
      foreach (TeleportDestination teleportDestination1 in teleportNetwork.destinationLookup.SelectMany(x => x.Value.exits))
      {
        TeleportDestination teleportDestination = teleportDestination1;
        Gadget gadget = teleportDestination.transform.GetComponentInParent<Gadget>(true);
        if (gadget)
        {
          TeleporterInventoryItem teleporterInventoryItem = new TeleporterInventoryItem(gadget.id, gadget.id.GetGadgetDefinition().icon, teleportDestination.GetComponentInParent<ZoneDirector>(true).zone, () =>
          {
            teleportDestination.GetComponent<TeleportSource>().OnDepart();
            teleportDestination.OnDepart();
            Vector3 position = teleportDestination.GetPosition();
            Vector3? eulerAngles = teleportDestination.GetEulerAngles();
            SRSingleton<SceneContext>.Instance.Player.GetComponent<TeleportablePlayer>().TeleportTo(position, teleportDestination.regionSetId, eulerAngles);
            teleportDestination.OnArrive();
            return true;
          });
          inventoryItems.Add(teleporterInventoryItem);
        }
      }
      UIUtils.CreateInventoryUI("t.multiteleporter", PediaDirector.Id.WARP_TECH.GetIcon(), inventoryItems, true, null);
    }
  }
}
