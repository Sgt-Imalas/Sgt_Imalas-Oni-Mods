using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
    internal class ResearchTreePatches
    {/// <summary>
     /// add research card to research screen
     /// </summary>
        [HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
        public class ResourceTreeLoader_Load_Patch
        {
            public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance, TextAsset file)
            {
                TechUtils.AddNode(__instance,
                    ModAssets.Techs.DockingTech,
                    GameStrings.Technology.ColonyDevelopment.CelestialDetection,
                    GameStrings.Technology.ColonyDevelopment.DurableLifeSupport,
                    GameStrings.Technology.ColonyDevelopment.CelestialDetection
                    );

                if (Config.Instance.EnableFuelLoaders)
                {
                    TechUtils.AddNode(__instance,
                    ModAssets.Techs.FuelLoaderTech,
                    ModAssets.Techs.DockingTech,
                    GameStrings.Technology.RadiationTechnologies.RadiationRefinement,
                    GameStrings.Technology.ColonyDevelopment.CelestialDetection
                    );
                }

                TechUtils.AddNode(__instance,
                    ModAssets.Techs.SpaceStationTech,
                    new[] { ModAssets.Techs.DockingTech, GameStrings.Technology.ColonyDevelopment.DurableLifeSupport },
                    GameStrings.Technology.RadiationTechnologies.RadiationRefinement,
                    GameStrings.Technology.ColonyDevelopment.DurableLifeSupport
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
                new Tech(ModAssets.Techs.DockingTech, new List<string>
                {
                    DockingTubeDoorConfig.ID,
                    SpaceStationDockingDoorConfig.ID
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
                
                new Tech(ModAssets.Techs.FuelLoaderTech, new List<string>
                {
                    UniversalFuelLoaderConfig.ID,
                    UniversalOxidizerLoaderConfig.ID,
                    HEPFuelLoaderConfig.ID,
                   // SolidOxidizerLoaderConfig.ID,
                   // LiquidOxidizerLoaderConfig.ID,
                },
                __instance
                );

                new Tech(ModAssets.Techs.SpaceStationTech, new List<string>
                {
                    SpaceStationBuilderModuleConfig.ID,
                },
                __instance
                );
            }
        }

    }
}
