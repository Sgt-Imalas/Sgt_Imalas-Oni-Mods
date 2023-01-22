using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Buildings.Habitats;
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

namespace Rockets_TinyYetBig.Science
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

                if (Config.Instance.EnableExtendedHabs)
                {
                    TechUtils.AddNode(__instance,
                    ModAssets.Techs.LargerRocketLivingSpaceTechID,
                    GameStrings.Technology.ColonyDevelopment.DurableLifeSupport,
                    xDiff: 1,
                    yDiff: 0
                    );
                }

                TechUtils.AddNode(__instance,
                    ModAssets.Techs.SpaceStationTechID,
                    new[] {
                        Config.Instance.EnableExtendedHabs ? ModAssets.Techs.LargerRocketLivingSpaceTechID : GameStrings.Technology.ColonyDevelopment.DurableLifeSupport
                        , ModAssets.Techs.DockingTechID },
                    xDiff: Config.Instance.EnableExtendedHabs ? 1 : 2,
                    yDiff: 0
                    );

                TechUtils.AddNode(__instance,
                ModAssets.Techs.SpaceStationTechMediumID,
                ModAssets.Techs.SpaceStationTechID,
                xDiff: 1,
                yDiff: 0
                );

                TechUtils.AddNode(__instance,
                ModAssets.Techs.SpaceStationTechLargeID,
                ModAssets.Techs.SpaceStationTechMediumID,
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

                if (Config.Instance.EnableExtendedHabs)
                {
                    ModAssets.Techs.LargerRocketLivingSpaceTech = new Tech(ModAssets.Techs.LargerRocketLivingSpaceTechID, new List<string>
                    {
                        HabitatModuleMediumExpandedConfig.ID,
                    },
                    __instance
                    );
                }

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
                    SpaceStationDockingDoorConfig.ID,
                    SpaceStationBuilderModuleConfig.ID
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
                ModAssets.Techs.SpaceStationTechMedium = new Tech(ModAssets.Techs.SpaceStationTechMediumID, new List<string>
                {
                },
                __instance
                , new Dictionary<string, float>()
                {
                    {"basic", 0f },
                    {"advanced", 50f},
                    {"orbital", 50f},
                    {"nuclear", 50f},
                    {ModAssets.DeepSpaceScienceID,20f }
                }
                );

                ModAssets.Techs.SpaceStationTechLarge = new Tech(ModAssets.Techs.SpaceStationTechLargeID, new List<string>
                {
                },
                __instance
                , new Dictionary<string, float>()
                {
                    {"basic", 0f },
                    {"advanced", 50f},
                    {"orbital", 50f},
                    {"nuclear", 50f},
                    {ModAssets.DeepSpaceScienceID,30f }
                }
                );


                //Debug.Log("AAAAAAAA: DeepSpaceScience Added");
                InjectionMethods.AddItemToTechnologyKanim(ModAssets.SpaceStationTypes[0].ID, ModAssets.Techs.SpaceStationTechID, ModAssets.SpaceStationTypes[0].Name, ModAssets.SpaceStationTypes[0].Description, ModAssets.SpaceStationTypes[0].Kanim, DlcManager.AVAILABLE_EXPANSION1_ONLY);
                InjectionMethods.AddItemToTechnologyKanim(ModAssets.SpaceStationTypes[1].ID, ModAssets.Techs.SpaceStationTechMediumID, ModAssets.SpaceStationTypes[1].Name, ModAssets.SpaceStationTypes[1].Description, ModAssets.SpaceStationTypes[1].Kanim, DlcManager.AVAILABLE_EXPANSION1_ONLY);
                InjectionMethods.AddItemToTechnologyKanim(ModAssets.SpaceStationTypes[2].ID, ModAssets.Techs.SpaceStationTechLargeID, ModAssets.SpaceStationTypes[2].Name, ModAssets.SpaceStationTypes[2].Description, ModAssets.SpaceStationTypes[2].Kanim, DlcManager.AVAILABLE_EXPANSION1_ONLY);
                InjectionMethods.AddItemToTechnologySprite(ModAssets.DeepSpaceScienceID, ModAssets.Techs.SpaceStationTechID, STRINGS.DEEPSPACERESEARCH.UNLOCKNAME, STRINGS.DEEPSPACERESEARCH.UNLOCKDESC, "research_type_deep_space_icon_unlock", DlcManager.AVAILABLE_EXPANSION1_ONLY);
                Db.Get().Techs.Get(ModAssets.Techs.SpaceStationTechID).unlockedItemIDs.Reverse();
            }
        }

    }
}
