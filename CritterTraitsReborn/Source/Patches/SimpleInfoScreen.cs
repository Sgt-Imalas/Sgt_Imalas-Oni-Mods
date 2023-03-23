using HarmonyLib;
using Klei.AI;
using UnityEngine;

namespace CritterTraitsReborn.Patches
{
    [HarmonyPatch(typeof(SimpleInfoScreen))]
    [HarmonyPatch(nameof(SimpleInfoScreen.OnSelectTarget))]
    static class SimpleInfoScreen_OnSelectTarget
    {
        public static CollapsibleDetailContentPanel TraitsPanel = null;
        public static DetailsPanelDrawer TraitsDrawer = null;
        public static GameObject LastParent = null;

        private static void InitTraitsPanel(SimpleInfoScreen instance)
        {
            if (TraitsPanel == null || LastParent != instance.gameObject)
            {
                if (TraitsPanel != null)
                {
                    UnityEngine.Object.Destroy(TraitsPanel.gameObject);
                }
                TraitsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, instance.gameObject);
                TraitsDrawer = new DetailsPanelDrawer(instance.attributesLabelTemplate, TraitsPanel.Content.gameObject);
                LastParent = instance.gameObject;
            }
        }

        static void Prefix(ref SimpleInfoScreen __instance, GameObject target)
        {
            if (target != null 
                && target.TryGetComponent<Klei.AI.Traits>(out var traits) 
                && target.TryGetComponent<KPrefabID>(out var prefab) 
                && traits.TraitList != null 
                && traits.TraitList.Count > 0
                && target.HasTag(GameTags.Creature))
            {
                InitTraitsPanel(__instance);
                bool shouldShow=false;
                TraitsPanel.gameObject.SetActive(true);
                TraitsPanel.HeaderLabel.text = global::STRINGS.UI.CHARACTERCONTAINER_TRAITS_TITLE;

                TraitsDrawer.BeginDrawing();
                foreach (Trait trait in traits.TraitList)
                {
                    if (!Traits.AllTraits.IsTraitVisibleInUI(trait.Id)) continue;

                    var color = trait.PositiveTrait ? Constants.POSITIVE_COLOR : Constants.NEGATIVE_COLOR;
                    TraitsDrawer.NewLabel($"<color=#{color.ToHexString()}>{(trait.Name)}</color>").Tooltip(trait.GetTooltip());
                    shouldShow = true;
                }
                TraitsDrawer.EndDrawing();
                if(!shouldShow)
                {
                    TraitsPanel.gameObject.SetActive(false);
                }
            }
            else
            {
                if(TraitsPanel!=null)
                {
                    TraitsPanel.gameObject.SetActive(false);
                }
            }
        }
    }
}
