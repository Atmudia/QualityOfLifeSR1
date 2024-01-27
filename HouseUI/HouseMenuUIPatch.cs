using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using SRML.Console;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QoLMod.HouseUI
{
    [HarmonyPatch(typeof(RanchHouseUI))]
    public static class HouseMenuUIPatch
    {
        public static Button[] Buttons = new Button[2];
        [HarmonyPatch(nameof(RanchHouseUI.Awake)), HarmonyPostfix]
        public static void Awake(RanchHouseUI __instance)
        {
            var sleepButton = __instance.gameObject.FindChild("SleepButton", true);
            var sleepsixhours = Object.Instantiate(sleepButton.gameObject, sleepButton.transform.parent);
            sleepsixhours.GetComponentInChildren<XlateText>().key = "b.sleep_six_hours";
            sleepsixhours.transform.SetSiblingIndex(4);
            var button = sleepsixhours.GetComponent<Button>();
            Buttons[0] = button;
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
            }
            button.onClick.AddListener(() => SleepTo(__instance, @__instance.timeDir.HoursFromNow(6)));
            var sleeptothenight = Object.Instantiate(sleepButton.gameObject, sleepButton.transform.parent);
            sleeptothenight.transform.SetSiblingIndex(5);
            button = sleeptothenight.GetComponent<Button>();
            Buttons[1] = button;
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
            }
            button.onClick.AddListener(() => SleepTo(__instance, __instance.timeDir.GetHourAfter(0, 18)));
            sleeptothenight.GetComponentInChildren<XlateText>().key = "b.sleep_until_night";
        }

        [HarmonyPatch(nameof(RanchHouseUI.OnEndGame)), HarmonyPrefix]
        public static void OnEndGame()
        {
            foreach (var button in Buttons)
            {
                if (button != null)
                    button.interactable = true;
            }
        }

        public static void SleepTo(RanchHouseUI @this, double hours)
        {
            if (@this.sleeping)
            {
                Debug.Log((object) "Attempted to sleep while sleeping. Ignore.");
            }
            else
            {
                AnalyticsUtil.CustomEvent("PlayerSlept");
                @this.sleeping = true;
                @this.timeDir.Unpause(false);
                @this.beatrixImg.DOFade(0.0f, 0.5f).SetUpdate<TweenerCore<Color, Color, ColorOptions>>(true);
                SRSingleton<LockOnDeath>.Instance.LockUntil(hours, 0.0f, (UnityAction) (() =>
                {
                    @this.beatrixImg.DOFade(1f, 0.5f).SetUpdate<TweenerCore<Color, Color, ColorOptions>>(true);
                    @this.timeDir.Pause(false);
                    @this.sleeping = false;
                }));
            }
        }
    }
   
}