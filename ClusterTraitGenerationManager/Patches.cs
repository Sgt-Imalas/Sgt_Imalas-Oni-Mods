using Database;
using HarmonyLib;
using Klei.AI;
using KMod;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static ClusterTraitGenerationManager.ModAssets;
using static KAnim;

namespace ClusterTraitGenerationManager
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
            }
        }


        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
        
        //[HarmonyPatch(typeof(ColonyDestinationSelectScreen))]

        [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
        [HarmonyPatch(nameof(ColonyDestinationSelectScreen.OnSpawn))]
        public static class InsertCustomClusterOption
        {
            public static void Postfix(ColonyDestinationSelectScreen __instance)
            {
                var InsertLocation = __instance.shuffleButton.transform.parent; //__instance.transform.Find("Layout/DestinationInfo/Content/InfoColumn/Horiz/Section - Destination/DestinationDetailsHeader/");
                var copyButton = Util.KInstantiateUI(__instance.shuffleButton.gameObject, InsertLocation.gameObject, true); //UIUtils.GetShellWithoutFunction(InsertLocation, "CoordinateContainer", "cgsm");

                UIUtils.ListAllChildrenWithComponents(copyButton.transform); 
                UIUtils.TryFindComponent<Image>(copyButton.transform, "FG").sprite = Assets.GetSprite("icon_gear");
                UIUtils.TryFindComponent<ToolTip>(copyButton.transform, "").toolTip = "Customize Cluster";
                UIUtils.TryFindComponent<KButton>(copyButton.transform, "").onClick += () => CGSMClusterManager.InstantiateClusterSelectionView();
                //ColonyDestinationSelectScreen s;
                //ProcGen.ClusterLayouts;
                //ProcGen.ClusterLayout;
                //  UIUtils.ListAllChildren(copyButton);

                //SelectionFrame
                //BG
                //Header
                //Label
                //Anim
                //Border
                //- BG(1)
                //- Image
                //- Anim

                // UIUtils.FindAndDisable(copyButton, "Anim");
                //  UIUtils.FindAndDisable(copyButton, "Border/Anim");

                //vanillaButtonHeader = component.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
                //vanillalButtonSelectionFrame = component.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
                //MultiToggle multiToggle = vanillaButton;
                //multiToggle.onEnter = (System.Action)Delegate.Combine(multiToggle.onEnter, new System.Action(OnHoverEnterVanilla));
                //MultiToggle multiToggle2 = vanillaButton;
                //multiToggle2.onExit = (System.Action)Delegate.Combine(multiToggle2.onExit, new System.Action(OnHoverExitVanilla));
                //MultiToggle multiToggle3 = vanillaButton;
                //multiToggle3.onClick = (System.Action)Delegate.Combine(multiToggle3.onClick, new System.Action(OnClickVanilla));
            }
        }
        
    }
}
