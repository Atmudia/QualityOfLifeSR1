
using HarmonyLib;

namespace QoLMod.Beacon
{
  [HarmonyPatch(typeof (MapDataEntry), "IsZoneLocked")]
  public static class MapDataEntryIsZoneLocked
  {
    public static void Postfix(bool __result)
    {
      if (__result)
        return;
      SRSingleton<SceneContext>.Instance.GadgetDirector.CheckAllBlueprintLockers();
    }
  }
}
