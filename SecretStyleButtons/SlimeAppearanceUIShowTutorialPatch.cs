
using HarmonyLib;
using UnityEngine;

namespace QoLMod.SecretStyleButtons
{
  [HarmonyPatch(typeof (SlimeAppearanceUI), "ShowTutorial")]
  public static class SlimeAppearanceUIShowTutorialPatch
  {
    public static void Prefix()
    {
      if (SlimeAppearanceUIAwakePatch.AllDisableButton != null)
        SlimeAppearanceUIAwakePatch.AllDisableButton.gameObject.SetActive(false);
      if (!(SlimeAppearanceUIAwakePatch.AllEnableButton != null))
        return;
      SlimeAppearanceUIAwakePatch.AllEnableButton.gameObject.SetActive(false);
    }
  }
}
