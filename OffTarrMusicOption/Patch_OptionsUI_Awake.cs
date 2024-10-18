
using System.IO;
using HarmonyLib;
using SRML;
using SRML.Config;
using SRML.Console;
using SRML.SR.Options;
using SRML.SR.SaveSystem;
using UnityEngine;
using UnityEngine.Events;

namespace QoLMod.OffTarrMusicOption
{
  [HarmonyPatch(typeof (OptionsUI), "Awake")]
  public static class Patch_OptionsUI_Awake
  {
    public static SECTR_AudioCue CachedTarrMusic = null;

    public static void Save()
    {
      SRMod mod = SRModLoader.GetModForAssembly(EntryPoint.execAssembly);
      SRMod.ForceModContext(mod);
      mod.Configs.Find((x) => x.FileName.ToLowerInvariant().Equals("config")).SaveToFile();
      SRMod.ClearModContext();

    }
    public static void Postfix(OptionsUI __instance)
    {
      Transform transform1 = Object.Instantiate(__instance.videoPanel.FindChild("FullscreenToggle", true), __instance.audioPanel.transform).transform;
      transform1.name = "TarrMusic";
      transform1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 170f);
      Transform transform2 = transform1.Find("Label");
      transform2.name = "TarrMusicLabel";
      transform2.GetComponent<XlateText>().key = "l.tarr_music";
      SRToggle component = transform1.GetComponent<SRToggle>();
      component.isOn = Configs.TARR_MUSIC;
      component.onValueChanged.AddListener(arg0 =>
      {
        MusicDirector musicDirector = SRSingleton<GameContext>.Instance.MusicDirector;
        Configs.TARR_MUSIC = arg0;
        if (Configs.TARR_MUSIC)
        {
          if (CachedTarrMusic == null)
          {
            CachedTarrMusic = musicDirector.tarrMusic;
          }
          SRSingleton<GameContext>.Instance.MusicDirector.SetTarrMode(false);
        }
        else
        {
          if (musicDirector.tarrMusic == null)
          {
            musicDirector.tarrMusic = CachedTarrMusic;
          }
        }
        Save();
      });
    }
  }
}
