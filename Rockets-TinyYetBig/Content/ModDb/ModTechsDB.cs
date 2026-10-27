using Database;
using Rockets_TinyYetBig.Buildings;
using Rockets_TinyYetBig.Buildings.CargoBays;
using Rockets_TinyYetBig.Buildings.Habitats;
using Rockets_TinyYetBig.Content.Defs.Buildings.Research;
using Rockets_TinyYetBig.RocketFueling;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb
{
	internal class ModTechsDB
	{
		internal static void RegisterCustomTechTypes(ResearchTypes instance)
		{
			instance.Types.Add(new ResearchType(
					ModAssets.DeepSpaceScienceID,
					STRINGS.DEEPSPACERESEARCH.NAME,
					STRINGS.DEEPSPACERESEARCH.DESC,
					Assets.GetSprite((HashedString)"research_type_deep_space_icon"),
					new Color32(100, 100, 100, byte.MaxValue),
					null,
					2400f,
					(HashedString)"research_center_kanim",
					[],
					STRINGS.DEEPSPACERESEARCH.RECIPEDESC
				));
			instance.Types.Add(new ResearchType(
					ModAssets.DerelictUnlock1,
					STRINGS.RTB_DERELICT_UNLOCK_1.NAME,
					STRINGS.RTB_DERELICT_UNLOCK_1.DESC,
					Assets.GetSprite((HashedString)"starmap_telescope_target"),
					UIUtils.rgb(3, 33, 102),
					null,
					2400f,
					(HashedString)"research_center_kanim",
					[],
					STRINGS.RTB_DERELICT_UNLOCK_1.DESC
				));
			instance.Types.Add(new ResearchType(
					ModAssets.DerelictUnlock2,
					STRINGS.RTB_DERELICT_UNLOCK_2.NAME,
					STRINGS.RTB_DERELICT_UNLOCK_2.DESC,
					Assets.GetSprite((HashedString)"ghosts_of_gravitas"),
					UIUtils.rgb(3, 33, 102),
					null,
					2400f,
					(HashedString)"research_center_kanim",
					[],
					STRINGS.RTB_DERELICT_UNLOCK_2.DESC
				));
		}

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


			bool bigCargoBays = Config.Instance.EnableLargeCargoBays;
			if (bigCargoBays)
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
			//if (Config.Instance.EnableRocketModuleUpgrades)
			//{
			//	if (Config.SpaceStationsPossible)
			//	{
			//		TechUtils.AddNode(instance,
			//			ModAssets.Techs.CargoBayFilterTechID,
			//			ModAssets.Techs.SpaceStationTechID,
			//			xDiff: 1,
			//			yDiff: 1);
			//	}
			//	else
			//	{
			//		TechUtils.AddNode(instance,
			//			ModAssets.Techs.CargoBayFilterTechID,
			//			bigCargoBays ? ModAssets.Techs.HugeCargoBayTechID  : GameStrings.Technology.SolidMaterial.HighVelocityTransport,
			//			xDiff: 1,
			//			yDiff: bigCargoBays  ? 0 : - 1.15f);
			//	}
			//}
		}



		internal static void RegisterTechs(Techs instance)
		{
			if (Config.Instance.RocketDocking)
			{
				ModAssets.Techs.DockingTech = new Tech(ModAssets.Techs.DockingTechID, [],
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
				ModAssets.Techs.LargerRocketLivingSpaceTech = new Tech(ModAssets.Techs.LargerRocketLivingSpaceTechID, [],
				instance
				);
			}

			ModAssets.Techs.FuelLoaderTech = new Tech(ModAssets.Techs.FuelLoaderTechID, [],
			instance
			);

			if (Config.SpaceStationsPossible)
			{
				ModAssets.Techs.SpaceScienceTech = new Tech(ModAssets.Techs.SpaceScienceTechID, [], instance,
					new Dictionary<string, float>()
				{
						{"basic", 0f },
						{"advanced", 0f},
						{"orbital", 25f},
						{"nuclear", 0f},
						{ModAssets.DeepSpaceScienceID, 1f },
					//	{ModAssets.DerelictUnlock1,1f }
				});
				//tech items are reversed
				ModAssets.Techs.SpaceStationTech = new Tech(ModAssets.Techs.SpaceStationTechID, [],
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
				ModAssets.Techs.SpaceStationTechMedium = new Tech(ModAssets.Techs.SpaceStationTechMediumID, [],
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

				ModAssets.Techs.SpaceStationTechLarge = new Tech(ModAssets.Techs.SpaceStationTechLargeID, [],
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
				ModAssets.Techs.HugeCargoBayTech = new Tech(ModAssets.Techs.HugeCargoBayTechID, [],
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
			//if (Config.Instance.EnableRocketModuleUpgrades)
			//{
			//	ModAssets.Techs.CargoBayFilterTech = new Tech(ModAssets.Techs.CargoBayFilterTechID, [ModuleUpgradeDatabase.CargoBayFilter.ID],
			//	instance
			//	, new Dictionary<string, float>()
			//	{
			//		//TODO: refine these:
			//			{TechUtils.ID.BASIC, 75f },
			//			{TechUtils.ID.ADVANCED, 75f},
			//			{TechUtils.ID.ORBITAL, 75f},
			//			{TechUtils.ID.NUCLEAR, 75f},
			//	}
			//	);
			//}

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
