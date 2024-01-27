
using HarmonyLib;

namespace QoLMod
{
  [HarmonyPatch(typeof (ProgressDirector), "CheckBlueprintUpgrades")]
  internal static class ProgressDirectorCheckBlueprintUpgradesPatch
  {
    private static void AddWithPopup(Gadget.Id gadgetId)
    {
      if (SRSingleton<SceneContext>.Instance.GadgetDirector.model.availBlueprints.Contains(gadgetId))
        return;
      SRSingleton<SceneContext>.Instance.GadgetDirector.model.availBlueprints.Add(gadgetId);
      SRSingleton<SceneContext>.Instance.PopupDirector.QueueForPopup(new GadgetDirector.AvailBlueprintPopupCreator(gadgetId));
      SRSingleton<SceneContext>.Instance.PopupDirector.MaybePopupNext();
    }

    public static void Prefix(int progress)
    {
      if (13 <= progress)
        AddWithPopup(Enums.DECORIZER_LINK);
      if (28 > progress)
        return;
      AddWithPopup(Enums.TELEPORTER_MULTI);
    }
  }
}
