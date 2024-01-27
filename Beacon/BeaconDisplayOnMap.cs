
namespace QoLMod.Beacon
{
  public class BeaconDisplayOnMap : DisplayOnMap
  {
    public BeaconGadget beaconGadget;
    public bool isModel;

    public override void Awake()
    {
      if (!isModel)
        return;
      beaconGadget = GetComponentInParent<BeaconGadget>();
      base.Awake();
      marker.gameObject.name = "Beacon_" + beaconGadget.model.siteId;
    }

    public override bool ShowOnMap() => !beaconGadget.model.BeaconName.Equals("none") && base.ShowOnMap();
  }
}
