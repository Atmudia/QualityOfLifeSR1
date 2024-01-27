
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
      StartCoroutine(CreateAUI());
    }

    private IEnumerator CreateAUI()
    {
      yield return new WaitForEndOfFrame();
      inventoryItems.Clear();
      foreach (TeleportDestination teleportDestination1 in teleportNetwork.destinationLookup.SelectMany<KeyValuePair<string, TeleportNetwork.Destination>, TeleportDestination>(x => x.Value.exits))
      {
        TeleportDestination teleportDestination = teleportDestination1;
        Gadget gadget = teleportDestination.transform.GetComponentInParent<Gadget>(true);
        if (gadget != null)
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
          gadget = null;
          teleporterInventoryItem = null;
        }
      }
      GameObject inventoryUI = UIUtils.CreateInventoryUI("t.multiteleporter", PediaDirector.Id.WARP_TECH.GetIcon(), inventoryItems, true, null);
    }

    public void OnTriggerExit(Collider collider)
    {
    }
  }
}
