using HarmonyLib;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class Techs_Patches
	{

		/// <summary>
		/// Add research node to tree
		/// </summary>
		[HarmonyPatch(typeof(Database.Techs), "Init")]
		public class Techs_TargetMethod_Patch
		{
			public static void Postfix(Database.Techs __instance)
			{
				if (Config.Instance.RocketDocking)
				{
					ModAssets.Techs.DockingTech = new Tech(ModAssets.Techs.DockingTechID, new List<string>
				{
					DockingTubeDoorConfig.ID,
                    //AI_DockingPortConfig.ID,
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
				}

				if (Config.Instance.EnableExtendedHabs)
				{
					ModAssets.Techs.LargerRocketLivingSpaceTech = new Tech(ModAssets.Techs.LargerRocketLivingSpaceTechID, new List<string>
					{
						HabitatModuleMediumExpandedConfig.ID,
						HabitatModulePlatedNoseconeLargeConfig.ID,
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

				if (Config.SpaceStationsPossible)
				{
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
						{"advanced", 150f},
						{"orbital", 150f},
						{"nuclear", 100f},
						{ModAssets.DeepSpaceScienceID,50f }
					}
					);

					ModAssets.Techs.SpaceStationTechLarge = new Tech(ModAssets.Techs.SpaceStationTechLargeID, new List<string>
					{
					},
					__instance
					, new Dictionary<string, float>()
					{
						{"basic", 0f },
						{"advanced", 150f},
						{"orbital", 150f},
						{"nuclear", 100f},
						{ModAssets.DeepSpaceScienceID,100f
						}
					}
					);
				}
				if (Config.Instance.EnableLargeCargoBays)
				{
					ModAssets.Techs.HugeCargoBayTech = new Tech(ModAssets.Techs.HugeCargoBayTechID, new List<string>
					{
						SolidCargoBayClusterLargeConfig.ID,
						LiquidCargoBayClusterLargeConfig.ID,
						GasCargoBayClusterLargeConfig.ID
					},
					__instance
					, new Dictionary<string, float>()
					{
						{"basic", 100f },
						{"advanced", 150f},
						{"orbital", 250f},
						{"nuclear", 150f},
					}
					);
				}

				if (Config.SpaceStationsPossible)
				{

					InjectionMethods.AddItemToTechnologyKanim(ModAssets.SpaceStationTypes[0].ID, ModAssets.Techs.SpaceStationTechID, ModAssets.SpaceStationTypes[0].Name, ModAssets.SpaceStationTypes[0].Description, ModAssets.SpaceStationTypes[0].Kanim, requiredDLcs: DlcManager.EXPANSION1);
					InjectionMethods.AddItemToTechnologyKanim(ModAssets.SpaceStationTypes[1].ID, ModAssets.Techs.SpaceStationTechMediumID, ModAssets.SpaceStationTypes[1].Name, ModAssets.SpaceStationTypes[1].Description, ModAssets.SpaceStationTypes[1].Kanim, requiredDLcs: DlcManager.EXPANSION1);
					InjectionMethods.AddItemToTechnologyKanim(ModAssets.SpaceStationTypes[2].ID, ModAssets.Techs.SpaceStationTechLargeID, ModAssets.SpaceStationTypes[2].Name, ModAssets.SpaceStationTypes[2].Description, ModAssets.SpaceStationTypes[2].Kanim, requiredDLcs: DlcManager.EXPANSION1);
					InjectionMethods.AddItemToTechnologySprite(ModAssets.DeepSpaceScienceID, ModAssets.Techs.SpaceStationTechID, STRINGS.DEEPSPACERESEARCH.UNLOCKNAME, STRINGS.DEEPSPACERESEARCH.UNLOCKDESC, "research_type_deep_space_icon_unlock", DlcManager.EXPANSION1);
					Db.Get().Techs.Get(ModAssets.Techs.SpaceStationTechID).unlockedItemIDs.Reverse();
				}
			}
		}
	}
}
