using HarmonyLib;
using SRML.SR.Patches;
using UnityEngine;

namespace QoLMod.CycleExtractor
{
  [HarmonyPatch(typeof(TargetingUI), nameof(TargetingUI.GetIdentifiableTarget))]
  public static class TargetingUIGetIdentifiableTargetPatch
  {
    public static void Postfix(TargetingUI __instance, GameObject gameObject, ref bool __result)
    {
      if (__result)
          return;
      Extractor extractor = gameObject.GetComponent<Extractor>();
      if (extractor)
      {
        Gadget gadget = gameObject.GetComponent<Gadget>();
        __instance.nameText.text = Gadget.GetName(gadget.id);
        __instance.infoText.text = __instance.uiBundle.Get("m.cycle_of_gadget", extractor.model.cyclesRemaining);
        __result = true;      
      }
    }
  }

}
