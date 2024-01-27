using HarmonyLib;
using UnityEngine;

namespace QoLMod.CycleExtractor
{
  [HarmonyPatch(typeof (TargetingUI)), HarmonyPatch("GetIdentifiableTarget")]
  public static class TargetingUIPatch
  {
    public static int remainingCycle;
    [HarmonyPrefix]
    public static void GetIdentifiableTarget(TargetingUI __instance, GameObject gameObject, ref bool __result)
    {
      if (__result)
        return;
      Extractor component1 = __instance.GetComponent<Extractor>();
      if (component1 == null)
      {
        remainingCycle = 0;
      }
      else
      {
        Gadget component2 = gameObject.GetComponent<Gadget>();
        remainingCycle = component1.model.cyclesRemaining;
        __instance.nameText.text = __instance.pediaBundle.Get("m.gadget.name." + component2.id.ToString().ToLowerInvariant());
        __instance.infoText.text = __instance.uiBundle.Get("m.cycle_of_gadget", component1.model.cyclesRemaining);
        __result = true;
      }
    }
  }
}
