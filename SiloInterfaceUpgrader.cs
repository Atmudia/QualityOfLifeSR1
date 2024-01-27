
using SRML.Utils;
using UnityEngine;

namespace QoLMod
{
  public class SiloInterfaceUpgrader : PlotUpgrader
  {
    public static GameObject siloUI;

    public override void Apply(LandPlot.Upgrade upgrade)
    {
      if (upgrade != Enums.SILO_INTERFACE)
        return;
      if (siloUI == null)
      {
        siloUI = PrefabUtils.CopyPrefab(LandPlot.Id.EMPTY.GetPlotPrefab().transform.Find("techActivator").gameObject);
        siloUI.transform.position = new Vector3(-2.921f, -0.05599976f, -2.888f);
        UIActivator componentInChildren = siloUI.GetComponentInChildren<UIActivator>();
        componentInChildren.gameObject.AddComponent<SiloUIActivator>();
        componentInChildren.DestroyImmediate<UIActivator>();
      }
      siloUI.Instantiate<GameObject>(transform, true);
    }
  }
}
