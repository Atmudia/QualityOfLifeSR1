
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QoLMod.SecretStyleButtons
{
  [HarmonyPatch(typeof (SlimeAppearanceUI), "Awake")]
  public static class SlimeAppearanceUIAwakePatch
  {
    public static Button AllDisableButton;
    public static Button AllEnableButton;

    public static void Prefix(SlimeAppearanceUI __instance)
    {
      GameObject gameObject1 = __instance.showTutorialButton.gameObject.InstantiateInactive(__instance.showTutorialButton.transform.parent, false);
      gameObject1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000f, -715f);
      Button component1 = gameObject1.GetComponent<Button>();
      AllDisableButton = component1;
      for (int index = 0; index < component1.onClick.GetPersistentEventCount(); ++index)
        component1.onClick.SetPersistentListenerState(index, UnityEventCallState.Off);
      component1.onClick.AddListener(() =>
      {
        foreach (SlimeDefinition definition in SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes.Where(x => x.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC) != null))
        {
          SlimeAppearance appearanceForSet = definition.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC);
          SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.UpdateChosenSlimeAppearance(definition, appearanceForSet);
        }
        __instance.Select(__instance.currentSlime);
      });
      gameObject1.SetActive(true);
      gameObject1.GetComponentInChildren<XlateText>().SetKey("b.disable_all_ss");
      GameObject gameObject2 = __instance.showTutorialButton.gameObject.InstantiateInactive(__instance.showTutorialButton.transform.parent, false);
      gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300f, -715f);
      Button component2 = gameObject2.GetComponent<Button>();
      AllEnableButton = component2;
      for (int index = 0; index < component1.onClick.GetPersistentEventCount(); ++index)
        component2.onClick.SetPersistentListenerState(index, UnityEventCallState.Off);
      component2.onClick.AddListener(() =>
      {
        foreach (SlimeDefinition slimeDefinition in SRSingleton<GameContext>.Instance.SlimeDefinitions.Slimes.Where(x => x.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.SECRET_STYLE) != null))
        {
          SlimeAppearance appearanceForSet = slimeDefinition.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.SECRET_STYLE);
          if (SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.IsAppearanceUnlocked(slimeDefinition, appearanceForSet))
            SRSingleton<SceneContext>.Instance.SlimeAppearanceDirector.UpdateChosenSlimeAppearance(slimeDefinition, appearanceForSet);
        }
        __instance.Select(__instance.currentSlime);
      });
      gameObject2.SetActive(true);
      gameObject2.GetComponentInChildren<XlateText>().SetKey("b.active_all_ss");
    }
  }
}
