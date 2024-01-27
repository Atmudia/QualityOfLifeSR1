
using System.IO;
using HarmonyLib;
using SRML.Config;
using SRML.Console;
using SRML.SR.Options;
using SRML.SR.SaveSystem;
using UnityEngine;
using UnityEngine.Events;

namespace QoLMod.OffTarrMusicOption
{
  [HarmonyPatch(typeof (OptionsUI), "Awake")]
  public static class OptionsUIAwakePatch
  {
    public static bool Enabled;
    public static SECTR_AudioCue CachedTarrMusic = null;

    public static void Save()
    {
      var fileInfo = new FileInfo(Path.Combine(Application.persistentDataPath, "qolconfig.json"));
      File.WriteAllText(fileInfo.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(Enabled));

    }
    public static void Load()
    {
      var fileInfo = new FileInfo(Path.Combine(Application.persistentDataPath, "qolconfig.json"));
      if (fileInfo.Exists)
      {
        Enabled = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(File.ReadAllText(fileInfo.FullName));
      }
      else
      {
        fileInfo.Create().Close();
        Enabled = false;
      }
    }
    public static void Postfix(OptionsUI __instance)
    {
      Transform transform1 = Object.Instantiate<GameObject>(__instance.videoPanel.FindChild("FullscreenToggle", true), __instance.audioPanel.transform).transform;
      transform1.name = "TarrMusic";
      transform1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 170f);
      Transform transform2 = transform1.Find("Label");
      transform2.name = "TarrMusicLabel";
      transform2.GetComponent<XlateText>().key = "l.tarr_music";
      SRToggle component = transform1.GetComponent<SRToggle>();
      component.isOn = Enabled;
      component.onValueChanged.AddListener(arg0 =>
      {
        MusicDirector musicDirector = SRSingleton<GameContext>.Instance.MusicDirector;
        Enabled = arg0;
        if (Enabled)
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
        //
        // if (musicDirector == null || !(musicDirector.current.cue == musicDirector.tarrMusic) || Configs.TARR_MUSIC)
        //   return;
        // SRSingleton<GameContext>.Instance.MusicDirector.SetTarrMode(false);
      });
    }
  }
}
