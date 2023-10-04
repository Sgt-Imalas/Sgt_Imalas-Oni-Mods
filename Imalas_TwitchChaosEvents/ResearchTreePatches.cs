using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
    public class ResearchTreePatches
    {

        /// <summary>
        /// add research card to research screen
        /// </summary>
        [HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
        public class ResourceTreeLoader_Load_Patch
        {
            public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance, TextAsset file)
            {
                TechUtils.AddNode(__instance,
                ModAssets.Techs.TacoTechID,
                GameStrings.Technology.Food.GourmetMealPreparation,
                xDiff: 1,
                yDiff: 0
                );

            }
        }

        /// <summary>
        /// Add research node to tree
        /// </summary>
        [HarmonyPatch(typeof(Database.Techs), "Init")]
        public class Techs_TargetMethod_Patch
        {
            public static void Postfix(Database.Techs __instance)
            {

                ModAssets.Techs.TacoTech = new Tech(ModAssets.Techs.TacoTechID, new List<string>
                {
                },
            __instance,
            new Dictionary<string, float>()
                {{"basic", 1f }}
                );
            }
        }


        //Disable tech item and all associated connections
        [HarmonyPatch(typeof(ResearchScreen), nameof(ResearchScreen.OnSpawn))]
        public class TechScreen_Hide_TacoTech
        {
            public static void Postfix(ResearchScreen __instance)
            {
                SgtLogger.Assert("ModAssets.Techs.TacoTech", ModAssets.Techs.TacoTech);
                SgtLogger.Assert("__instance", __instance);

                var TacoTechItem = __instance.entryMap[ModAssets.Techs.TacoTech];
                if (TacoTechItem == null)
                {
                    SgtLogger.warning("TacoTech was null!");
                    return;
                }
                if (TacoTechItem.techLineMap != null)
                {
                    foreach (UILineRenderer item in TacoTechItem.techLineMap.Values)
                    {
                        item.enabled = false;
                    }
                }
                TacoTechItem.gameObject.SetActive(false);
            }
        }

        //manually disable tech after the enabling method has run so its always disabled in the sidebar
        [HarmonyPatch(typeof(ResearchScreenSideBar), nameof(ResearchScreenSideBar.RefreshProjectsActive))]
        public class TechScreen_Sidebar_Hide_TacoTech
        {
            public static void Postfix(ResearchScreenSideBar __instance)
            {
                var TacoTechItem = __instance.projectTechs[ModAssets.Techs.TacoTechID];
                if (TacoTechItem == null)
                {
                    SgtLogger.warning("TacoTechSidescreen was null!");
                    return;
                }
                TacoTechItem.gameObject.SetActive(false);
            }
        }

        //Skip disabled Items to prevent crash
        [HarmonyPatch(typeof(ResearchEntry), nameof(ResearchEntry.SetEverythingOff))]
        public class TechScreen_Hide_TacoTech_crashprevention2
        {
            public static bool Prefix(ResearchEntry __instance)
            {
                if (__instance.targetTech.Id == ModAssets.Techs.TacoTechID)
                {
                    return false;
                }

                return true;
            }
        }
        //Skip disabled Items to prevent crash
        [HarmonyPatch(typeof(ResearchEntry), nameof(ResearchEntry.SetEverythingOn))]
        public class TechScreen_Hide_TacoTech_crashprevention
        {
            public static bool Prefix(ResearchEntry __instance)
            {
                if(__instance.targetTech.Id == ModAssets.Techs.TacoTechID)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
