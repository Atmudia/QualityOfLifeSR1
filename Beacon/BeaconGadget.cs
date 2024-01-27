
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace QoLMod.Beacon
{
  public class BeaconGadget : Gadget, GadgetModel.Participant
  {
    public BeaconGadgetModel model;
    public BeaconDisplayOnMap displayOnMap;
    public GameObject radarTrackedObject;

    public override void Awake()
    {
      displayOnMap = GetComponentInChildren<BeaconDisplayOnMap>();
      if (radarTrackedObject == null)
      {
        GameObject gameObject = new GameObject("BeaconRadar", new Type[1]
        {
          typeof (Image)
        });
        gameObject.transform.SetParent(transform.GetChild(0), false);
        gameObject.transform.localPosition += new Vector3(0.0f, 1f, 0.0f);
        gameObject.SetActive(false);
        radarTrackedObject = gameObject;
      }
      base.Awake();
    }

    public void OnDestroy()
    {
      if (radarTrackedObject == null || !radarTrackedObject.gameObject.name.StartsWith("BeaconRadar_"))
        return;
      SRSingleton<RadarPanelUI>.Instance.UnregisterTracked(radarTrackedObject.gameObject);
    }

    public void InitModel(GadgetModel model)
    {
    }

    public void SetModel(GadgetModel model)
    {
      this.model = (BeaconGadgetModel) model;
      displayOnMap.isModel = true;
      displayOnMap.Awake();
      SetBeacon(this.model.BeaconName);
    }

    public void SetBeacon(string beaconName)
    {
      model.BeaconName = beaconName;
      if (beaconName.Equals("none"))
        return;
      Sprite persistentKeysWithSprite = BeaconActivator.PersistentKeysWithSprites[beaconName];
      displayOnMap.marker.GetComponent<Image>().sprite = persistentKeysWithSprite;
      displayOnMap.ShowOnMap();
      if (!radarTrackedObject.gameObject.name.StartsWith("BeaconRadar_"))
      {
        radarTrackedObject.gameObject.name = "BeaconRadar_" + model.siteId;
        SRSingleton<RadarPanelUI>.Instance.RegisterTracked(radarTrackedObject.gameObject, GetComponentInParent<Region>().setId, radarTrackedObject.GetComponent<Image>(), true);
      }
      SRSingleton<RadarPanelUI>.Instance.registered.Find(x => x.obj.name.EndsWith(model.siteId)).img.sprite = persistentKeysWithSprite;
    }

    public bool IsSelectedBeacon(string beaconName) => model.BeaconName.Equals(beaconName);
  }
}
