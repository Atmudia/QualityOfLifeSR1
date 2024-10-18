
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using SRML.SR.SaveSystem.Patches;
using UnityEngine;

namespace QoLMod
{
  public static class HarmonyPatches
  {
    private static DecorizerStorage decorizerstorage;

    public static DecorizerStorage DecorizerStorage
    {
      get
      {
        if (decorizerstorage)
          return decorizerstorage;
        decorizerstorage = UnityEngine.Object.FindObjectsOfType<DecorizerStorage>().First();
        return decorizerstorage;
      }
    }

    [HarmonyPatch(typeof(SiloCatcher))]
    public static class SiloCatcherPatch
    {
      [HarmonyPatch(nameof(Awake)), HarmonyPostfix]
      public static void Awake(SiloCatcher __instance)
      {
        Gadget componentInParent = __instance.GetComponentInParent<Gadget>();
        if (componentInParent == null || componentInParent.id != Enums.DECORIZER_LINK)
          return;
        __instance.storageDecorizer = DecorizerStorage;
      }
    }

    [HarmonyPatch(typeof(BaseUI), "Close")]
    public static class BaseUIClosePatch
    {
      public static void Prefix() => DecorizerStorageActivatorPatch.Activator = null;
    }

    [HarmonyPatch(typeof(DecorizerStorage), "Cleanup")]
    public static class DecorizerStorageCleanup
    { 
      public static bool Prefix(DecorizerStorage __instance, IEnumerable<Identifiable.Id> ids)
      {
        if (!DecorizerStorageActivatorPatch.Activator)
            return true;
        DecorizerStorageActivatorPatch.Activator.GetRequiredComponentInParent<CellDirector>().Get(ids, DecorizerStorage.CLEANUP_RESULTS);
        foreach (GameObject actorObj in DecorizerStorage.CLEANUP_RESULTS.Where(gameObject => __instance.Add(Identifiable.GetId(gameObject))))
          Destroyer.DestroyActor(actorObj, "DecorizerStorage.Cleanup");
        DecorizerStorage.CLEANUP_RESULTS.Clear();
        return false;
      }
    }

    [HarmonyPatch(typeof(DecorizerStorageActivator))]
    public static class DecorizerStorageActivatorPatch
    {
      public static DecorizerStorageActivator Activator;

      [HarmonyPatch("Activate"), HarmonyPrefix]
      public static void Activate(DecorizerStorageActivator __instance)
      {
        var gadget = __instance.GetComponentInParent<Gadget>();
        if (!gadget || gadget.id != Enums.DECORIZER_LINK)
          return;
        Activator = __instance;
      }
      [HarmonyPatch("Awake"), HarmonyPostfix]
      public static void Postfix(DecorizerStorageActivator __instance)
      {
        var gadget = __instance.GetComponentInParent<Gadget>();
        if (!gadget || gadget.id != Enums.DECORIZER_LINK)
          return;
        __instance.storage = DecorizerStorage;
      }
    } 
    [HarmonyPatch(typeof(DecorizerSlotUI), "Awake")]
    public static class DecorizerSlotUIAwake
    {
      public static void Postfix(DecorizerSlotUI __instance)
      {
        var gadget = __instance.GetComponentInParent<Gadget>();
        if (!gadget || gadget.id != Enums.DECORIZER_LINK)
          return;
        __instance.storage = DecorizerStorage;
      }
    }
    
    
    
  }
}
