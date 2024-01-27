
using UnityEngine;

namespace QoLMod
{
  public class MarketLinkActivator : UIActivator
  {
    public static GameObject basePrefab;

    public override GameObject Activate()
    {
      if (basePrefab == null)
        basePrefab = GameObject.Find("zoneRANCH/cellRanch_Home/Sector/Ranch Features/techMarket/techActivator(Clone)/triggerActivate").GetComponent<UIActivator>().uiPrefab;
      if (uiPrefab == null)
        uiPrefab = basePrefab;
      return base.Activate();
    }
  }
}
