
using SRML.SR;
using SRML.Utils.Enum;

namespace QoLMod
{
  [EnumHolder]
  public static class Enums
  {
    [GadgetCategorization(GadgetCategorization.Rule.NONE)]
    public static Gadget.Id DECORIZER_LINK;
    [GadgetCategorization(GadgetCategorization.Rule.TELEPORTER)]
    public static Gadget.Id TELEPORTER_MULTI;
    [GadgetCategorization(GadgetCategorization.Rule.NONE)]
    public static Gadget.Id BEACON_GADGET;
    public static LandPlot.Upgrade SILO_INTERFACE;
  }
}
