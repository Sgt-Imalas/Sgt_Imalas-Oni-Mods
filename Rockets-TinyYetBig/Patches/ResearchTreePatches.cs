using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    internal class ResearchTreePatches
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
                    ModAssets.Techs.DockingTechID,
                    GameStrings.Technology.ColonyDevelopment.CelestialDetection,
                    xDiff: 2,
                    yDiff: 0
                    );

                if (Config.Instance.EnableFuelLoaders)
                {
                    TechUtils.AddNode(__instance,
                    ModAssets.Techs.FuelLoaderTechID,
                    ModAssets.Techs.DockingTechID,
                    xDiff: 1,
                    yDiff: 0
                    );
                }
                



                TechUtils.AddNode(__instance,
                    ModAssets.Techs.SpaceStationTechID,
                    new[] {GameStrings.Technology.ColonyDevelopment.DurableLifeSupport, ModAssets.Techs.DockingTechID },
                    xDiff: 2,
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
                ModAssets.Techs.DockingTech = new Tech(ModAssets.Techs.DockingTechID, new List<string>
                {
                    DockingTubeDoorConfig.ID,
                    //SpaceStationDockingDoorConfig.ID
                },
                __instance
                //,new Dictionary<string, float>()
                //{
                //    {"basic", 50f },
                //    {"advanced", 50f},
                //    {"orbital", 400f},
                //    {"nuclear", 50f}
                //}
                );

                ModAssets.Techs.FuelLoaderTech = new Tech(ModAssets.Techs.FuelLoaderTechID, new List<string>
                {
                    UniversalFuelLoaderConfig.ID,
                    UniversalOxidizerLoaderConfig.ID,
                    HEPFuelLoaderConfig.ID,
                },
                __instance
                );

                ModAssets.Techs.SpaceStationTech = new Tech(ModAssets.Techs.SpaceStationTechID, new List<string>
                {
                    SpaceStationBuilderModuleConfig.ID,
                },
                __instance
                , new Dictionary<string, float>()
                {
                    {"basic", 0f },
                    {"advanced", 50f},
                    {"orbital", 50f},
                    {"nuclear", 50f},
                    {ModAssets.DeepSpaceScienceID,3f }
                }
                );


                //Debug.Log("AAAAAAAA: DeepSpaceScience Added");
                InjectionMethods.AddItemToTechnology(ModAssets.DeepSpaceScienceID, ModAssets.Techs.SpaceStationTechID, STRINGS.DEEPSPACERESEARCH.NAME, STRINGS.DEEPSPACERESEARCH.DESC, "research_type_deep_space_icon");
                Db.Get().Techs.Get(ModAssets.Techs.SpaceStationTechID).unlockedItemIDs.Reverse();
            }
        }

    }
}
