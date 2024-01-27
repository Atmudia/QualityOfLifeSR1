
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QoLMod
{
  public static class DecorizerLink
  {
    private static DecorizerStorage decorizerstorage;

    public static DecorizerStorage DecorizerStorage
    {
      get
      {
        if (decorizerstorage != null)
          return decorizerstorage;
        decorizerstorage = UnityEngine.Object.FindObjectsOfType<DecorizerStorage>().First<DecorizerStorage>();
        return decorizerstorage;
      }
    }

    [HarmonyPatch(typeof (SiloCatcherTypeExtensions), "HasInput")]
    public static class SiloCatcherTypeExtensionsHasInputPatch
    {
      public static void Postfix(SiloCatcher.Type type, ref bool __result)
      {
        if (type != Enums.DECORIZER_LINK_CATCHER)
          return;
        __result = true;
      }
    }

    [HarmonyPatch(typeof (SiloCatcherTypeExtensions), "HasOutput")]
    public static class SiloCatcherTypeExtensionsHasOutputPatch
    {
      public static void Postfix(SiloCatcher.Type type, ref bool __result)
      {
        if (type != Enums.DECORIZER_LINK_CATCHER)
          return;
        __result = true;
      }
    }

    [HarmonyPatch(typeof (SiloCatcher), "Awake")]
    public static class SiloCatcherAwakePatch
    {
      public static void Postfix(SiloCatcher __instance)
      {
        if (__instance.type != Enums.DECORIZER_LINK_CATCHER)
          return;
        __instance.storageDecorizer = DecorizerStorage;
      }
    }

    [HarmonyPatch(typeof (BaseUI), "Close")]
    public static class BaseUIClosePatch
    {
      public static void Prefix() => DecorizerStorageActivatorActivatePatch.Activator = null;
    }

    [HarmonyPatch(typeof (DecorizerStorage), "Cleanup")]
    public static class DecorizerStorageCleanup
    {
      public static bool Prefix(DecorizerStorage __instance, IEnumerable<Identifiable.Id> ids)
      {
        if (DecorizerStorageActivatorActivatePatch.Activator == null)
          return true;
        DecorizerStorageActivatorActivatePatch.Activator.GetRequiredComponentInParent<CellDirector>().Get(ids, DecorizerStorage.CLEANUP_RESULTS);
        foreach (GameObject actorObj in DecorizerStorage.CLEANUP_RESULTS.Where<GameObject>(gameObject => __instance.Add(Identifiable.GetId(gameObject))))
          Destroyer.DestroyActor(actorObj, "DecorizerStorage.Cleanup");
        DecorizerStorage.CLEANUP_RESULTS.Clear();
        return false;
      }
    }

    [HarmonyPatch(typeof (DecorizerStorageActivator), "Activate")]
    public static class DecorizerStorageActivatorActivatePatch
    {
      public static DecorizerStorageActivator Activator;

      public static void Prefix(DecorizerStorageActivator __instance)
      {
        if (__instance.GetComponentInParent<Gadget>() == null || __instance.GetComponentInParent<Gadget>().id != Enums.DECORIZER_LINK)
          return;
        Activator = __instance;
      }
    }

    [HarmonyPatch(typeof (SiloCatcher), "Remove")]
    public static class SiloCatcherRemovePatch
    {
      public static bool Prefix(SiloCatcher __instance, ref Identifiable.Id id, ref bool __result)
      {
        if (__instance.type != Enums.DECORIZER_LINK_CATCHER)
          return true;
        __result = __instance.storageDecorizer.Remove(out id);
        return false;
      }
    }

    [HarmonyPatch(typeof (SiloCatcher), "OnTriggerEnter")]
    public static class SiloCatcherOnTriggerEnterPatch
    {
      public static bool Prefix(SiloCatcher __instance, Collider collider)
      {
        if (__instance.type != Enums.DECORIZER_LINK_CATCHER)
          return true;
        Identifiable.Id id = Identifiable.GetId(collider.gameObject);
        if (id == Identifiable.Id.NONE)
          return false;
        Vacuumable component1 = collider.gameObject.GetComponent<Vacuumable>();
        if (component1 == null || component1.isCaptive() || !__instance.collectedThisFrame.Add(collider.gameObject) || !__instance.Insert(id))
          return false;
        DestroyOnTouching component2 = collider.gameObject.GetComponent<DestroyOnTouching>();
        if (component2 != null)
          component2.NoteDestroying();
        Destroyer.DestroyActor(collider.gameObject, "BaseCatcher.OnTriggerEnter");
        SRBehaviour.SpawnAndPlayFX(__instance.storeFX, collider.gameObject.transform.position, collider.gameObject.transform.rotation);
        __instance.audioSource.Play();
        __instance.accelerationInput.OnTriggered();
        return false;
      }
    }

    [HarmonyPatch(typeof (SiloCatcher), "Insert")]
    public static class SiloCatcherInsertPatch
    {
      public static bool Prefix(SiloCatcher __instance, Identifiable.Id id, ref bool __result)
      {
        if (__instance.type != Enums.DECORIZER_LINK_CATCHER)
          return true;
        __result = __instance.storageDecorizer.Add(id);
        return false;
      }
    }

    [HarmonyPatch(typeof (DecorizerStorageActivator), "Awake")]
    public static class DecorizerStorageActivatorAwake
    {
      public static void Postfix(DecorizerStorageActivator __instance)
      {
        Gadget componentInParent = __instance.GetComponentInParent<Gadget>();
        if (componentInParent == null || componentInParent.id != Enums.DECORIZER_LINK)
          return;
        __instance.storage = DecorizerStorage;
      }
    }

    [HarmonyPatch(typeof (DecorizerSlotUI), "Awake")]
    public static class DecorizerSlotUIAwake
    {
      public static void Postfix(DecorizerSlotUI __instance)
      {
        Gadget componentInParent = __instance.GetComponentInParent<Gadget>();
        if (componentInParent == null || componentInParent.id != Enums.DECORIZER_LINK)
          return;
        __instance.storage = DecorizerStorage;
      }
    }
  }
}
