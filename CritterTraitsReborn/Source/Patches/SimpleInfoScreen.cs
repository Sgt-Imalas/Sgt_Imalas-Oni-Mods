using HarmonyLib;
using Klei.AI;
using UnityEngine;

namespace Heinermann.CritterTraits.Patches
{
  [HarmonyPatch(typeof(SimpleInfoScreen), "OnSelectTarget")]
  static class SimpleInfoScreen_OnSelectTarget
  {
    public static CollapsibleDetailContentPanel TraitsPanel = null;
    public static DetailsPanelDrawer TraitsDrawer = null;
    public static GameObject LastParent = null;

    private static void InitTraitsPanel(SimpleInfoScreen instance)
    {
      if (TraitsPanel == null || LastParent != instance.gameObject)
      {
        TraitsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, instance.gameObject);
        TraitsDrawer = new DetailsPanelDrawer(instance.attributesLabelTemplate, TraitsPanel.Content.gameObject);
        LastParent = instance.gameObject;
      }
    }

    static void Prefix(ref SimpleInfoScreen __instance, GameObject target)
    {
      if (target != null &&
        target.GetComponent<Klei.AI.Traits>() != null &&
        target.GetComponent<KPrefabID>() != null &&
        target.HasTag(GameTags.Creature))
      {
        InitTraitsPanel(__instance);

        TraitsPanel.gameObject.SetActive(true);
        TraitsPanel.HeaderLabel.text = "TRAITS";

        TraitsDrawer.BeginDrawing();
        foreach (Trait trait in target.GetComponent<Klei.AI.Traits>().TraitList)
        {
          if (!Traits.AllTraits.IsSupportedTrait(trait.Id)) continue;

          var color = trait.PositiveTrait ? Constants.POSITIVE_COLOR : Constants.NEGATIVE_COLOR;
          TraitsDrawer.NewLabel($"<color=#{color.ToHexString()}>{trait.Name}</color>").Tooltip(trait.GetTooltip());
        }
        TraitsDrawer.EndDrawing();
      }
      else
      {
        TraitsPanel?.gameObject?.SetActive(false);
      }
    }
  }
}
