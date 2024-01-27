
using AssetsLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QoLMod
{
  public class TeleporterInventoryItem : IInventoryItem
  {
    private Gadget.Id gadgetId;
    private Sprite sprite;
    private ZoneDirector.Zone zoneId;
    private Func<bool> onClick;

    public TeleporterInventoryItem(
      Gadget.Id gadgetId,
      Sprite sprite,
      ZoneDirector.Zone zoneId,
      Func<bool> onClick)
    {
      this.gadgetId = gadgetId;
      this.sprite = sprite;
      this.zoneId = zoneId;
      this.onClick = onClick;
    }

    bool IInventoryItem.OnClick()
    {
      Func<bool> onClick = this.onClick;
      return onClick != null && onClick();
    }

    void IInventoryItem.RefreshEntry(GameObject gO)
    {
      string str = string.Empty;
      if (ZoneDirector.zonePediaIdLookup.ContainsKey(zoneId))
        str = " (" + SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("pedia").Get("t." + ZoneDirector.zonePediaIdLookup[zoneId].ToString().ToLowerInvariant()) + ")";
      gO.transform.Find("Content/Name").GetComponent<TMP_Text>().text = SRSingleton<GameContext>.Instance.MessageDirector.Get("pedia", "m.gadget.name." + gadgetId.ToString().ToLowerInvariant()) + str;
      gO.transform.Find("Content/Icon").GetComponent<Image>().sprite = sprite;
      gO.transform.Find("CountsOuterPanel/CountsPanel/Counts").transform.parent.gameObject.SetActive(false);
    }
  }
}
