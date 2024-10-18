
using HarmonyLib;
using SRML.Console;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QoLMod.GadgetScrapper
{
  [HarmonyPatch(typeof (PurchaseUI), "Select")]
  public static class PurchaseUISelectPatch
  {
    public static void Postfix(PurchaseUI __instance, PurchaseUI.Purchasable purchasable)
    {
      if (!__instance.titleImg.sprite.name.Equals("iconGadgetScrap"))
        return;
      __instance.costsPanel.SetActive(true);
      __instance.costsPanel.FindChild("RequiresLabel", true).GetComponent<XlateText>().SetKey("l.returns");
      __instance.ClearCostListPanel(true);
      foreach (GadgetDefinition.CraftCost cost in ScrapperUI.cachedCraftCost[purchasable])
        CreateCost(__instance, cost).transform.SetParent(__instance.costListPanel.transform);
    }

    public static GameObject CreateCost(PurchaseUI instance, GadgetDefinition.CraftCost cost)
    {
      GameObject cost1 = Object.Instantiate(instance.costListItemPrefab);
      TMP_Text component1 = cost1.transform.Find("Content/Name").gameObject.GetComponent<TMP_Text>();
      Image component2 = cost1.transform.Find("Content/Icon").gameObject.GetComponent<Image>();
      TMP_Text component3 = cost1.transform.Find("CountsOuterPanel/CountsPanel/Counts").gameObject.GetComponent<TMP_Text>();
      Sprite icon = SRSingleton<GameContext>.Instance.LookupDirector.GetIcon(cost.id);
      component1.text = instance.actorBundle.Xlate("l." + cost.id.ToString().ToLowerInvariant());
      component2.sprite = icon;
      component3.text = instance.uiBundle.Xlate(MessageUtil.Tcompose("m.count_of_scrap", cost.amount, SRSingleton<SceneContext>.Instance.GadgetDirector.GetRefineryCount(cost.id)));
      return cost1;
    }
  }
}
