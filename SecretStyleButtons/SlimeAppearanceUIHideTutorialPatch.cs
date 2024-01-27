
using HarmonyLib;
using UnityEngine;

namespace QoLMod.SecretStyleButtons
{
  [HarmonyPatch(typeof (SlimeAppearanceUI), "HideTutorial")]
  public static class SlimeAppearanceUIHideTutorialPatch
  {
    public static void Prefix()
    {
      if (SlimeAppearanceUIAwakePatch.AllDisableButton != null)
        SlimeAppearanceUIAwakePatch.AllDisableButton.gameObject.SetActive(true);
      if (!(SlimeAppearanceUIAwakePatch.AllEnableButton != null))
        return;
      SlimeAppearanceUIAwakePatch.AllEnableButton.gameObject.SetActive(true);
    }
  }
}
