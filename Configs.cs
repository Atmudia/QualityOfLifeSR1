
using SRML.Config.Attributes;

namespace QoLMod
{
  [ConfigFile("CONFIG", "CONFIG")]
  public static class Configs
  {
    [ConfigComment("It's only used for saving")]
    public static bool TARR_MUSIC = true;
  }
}
