
using HarmonyLib;

namespace QoLMod.OffTarrMusicOption
{
  [HarmonyPatch(typeof (MusicDirector), "SetTarrMode")]
  public static class Patch_MusicDirector_SetTarrMode
  {
    public static void Prefix(ref bool enabled) => enabled = enabled && Configs.TARR_MUSIC;
  }
}
