using Database;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.Content.Defs.Buildings.Research;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class ModTechsDB
	{
		internal static void RegisterTechCards(ResourceTreeLoader<ResourceTreeNode> instance)
		{
			if (Config.Instance.RocketDocking)
			{
				TechUtils.AddNode(instance,
				ModAssets.Techs.DockingTechID,
				GameStrings.Technology.ColonyDevelopment.Missiles,
				xDiff: 1,
				yDiff: 0
				);
			}
			if (Config.Instance.EnableFuelLoaders)
			{
				TechUtils.AddNode(instance,
				ModAssets.Techs.FuelLoaderTechID,
				GameStrings.Technology.ColonyDevelopment.RoboticTools,
				xDiff: 1,
				yDiff: 0
				);
			}

			if (Config.Instance.EnableExtendedHabs)
			{
				TechUtils.AddNode(instance,
				ModAssets.Techs.LargerRocketLivingSpaceTechID,
				GameStrings.Technology.ColonyDevelopment.DurableLifeSupport,
				xDiff: 1,
				yDiff: 0
				);
			}
			if (Config.SpaceStationsPossible)
			{
				TechUtils.AddNode(instance,
				ModAssets.Techs.SpaceScienceTechID,
				new[] {
						Config.Instance.EnableExtendedHabs ? ModAssets.Techs.LargerRocketLivingSpaceTechID : GameStrings.Technology.ColonyDevelopment.DurableLifeSupport
						, ModAssets.Techs.DockingTechID },
				xDiff: Config.Instance.EnableExtendedHabs ? 1 : 2,
				yDiff: 0
				); 
				
				TechUtils.AddNode(instance,
				ModAssets.Techs.SpaceStationTechID,
				[ModAssets.Techs.SpaceScienceTechID],
				xDiff: 1,
				yDiff: 0
				);

				TechUtils.AddNode(instance,
				ModAssets.Techs.SpaceStationTechMediumID,
				ModAssets.Techs.SpaceStationTechID,
				xDiff: 1,
				yDiff: 0
				);

				TechUtils.AddNode(instance,
				ModAssets.Techs.SpaceStationTechLargeID,
				ModAssets.Techs.SpaceStationTechMediumID,
				xDiff: 1,
				yDiff: 0
				);
			}



			if (Config.Instance.EnableLargeCargoBays)
			{
				if (Config.SpaceStationsPossible)
				{
					TechUtils.AddNode(instance,
						ModAssets.Techs.HugeCargoBayTechID,
						ModAssets.Techs.SpaceStationTechID,
						xDiff: 1,
						yDiff: -1);
				}
				else
				{
					TechUtils.AddNode(instance,
						ModAssets.Techs.HugeCargoBayTechID,
						GameStrings.Technology.SolidMaterial.HighVelocityTransport,
						xDiff: 1,
						yDiff: -1.15f);
				}
			}
		}

		internal static void RegisterTechs(Techs instance)
		{
			if (Config.Instance.RocketDocking)
			{
				ModAssets.Techs.DockingTech = new Tech(ModAssets.Techs.DockingTechID, new List<string>
				{
					DockingTubeDoorConfig.ID,
                    //AI_DockingPortConfig.ID,
                    //SpaceStationDockingDoorConfig.ID
                },
			instance
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
				instance
				);
			}

			ModAssets.Techs.FuelLoaderTech = new Tech(ModAssets.Techs.FuelLoaderTechID, new List<string>
				{
					UniversalFuelLoaderConfig.ID,
					UniversalOxidizerLoaderConfig.ID,
					HEPFuelLoaderConfig.ID,
				},
			instance
			);

			if (Config.SpaceStationsPossible)
			{
				ModAssets.Techs.SpaceScienceTech = new Tech(ModAssets.Techs.SpaceScienceTechID, new List<string>(), instance,
					new Dictionary<string, float>()
				{
						{"basic", 0f },
						{"advanced", 0f},
						{"orbital", 0f},
						{"nuclear", 0f},
						{ModAssets.DeepSpaceScienceID,1f }
				});
				//tech items are reversed
				ModAssets.Techs.SpaceStationTech = new Tech(ModAssets.Techs.SpaceStationTechID, new List<string>
					{
						SpaceStationDockingDoorConfig.ID,
						SpaceStationBuilderModuleConfig.ID,
						DeepSpaceResearchCenterConfig.ID,
						DeepSpaceResearchTelescopeConfig.ID,
					},
				instance
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
				instance
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
				instance
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
				instance
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
				var deepSpace = InjectionMethods.AddItemToTechnologySprite(ModAssets.DeepSpaceScienceID, ModAssets.Techs.SpaceScienceTechID, STRINGS.DEEPSPACERESEARCH.UNLOCKNAME, STRINGS.DEEPSPACERESEARCH.UNLOCKDESC, "research_type_deep_space_icon_unlock", DlcManager.EXPANSION1);
				deepSpace.isPOIUnlock = true;
				Db.Get().Techs.Get(ModAssets.Techs.SpaceStationTechID).unlockedItemIDs.Reverse();
			}
		}
	}
}
