using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
	internal class ResourceTreeLoader_Patch
	{
		/// <summary>
		/// add research card to research screen
		/// </summary>
		[HarmonyPatch(typeof(ResourceTreeLoader<ResourceTreeNode>), MethodType.Constructor, typeof(TextAsset))]
		public class ResourceTreeLoader_Load_Patch
		{
			public static void Postfix(ResourceTreeLoader<ResourceTreeNode> __instance, TextAsset file)
			{
				if (Config.Instance.RocketDocking)
				{
					TechUtils.AddNode(__instance,
					ModAssets.Techs.DockingTechID,
					GameStrings.Technology.ColonyDevelopment.CelestialDetection,
					xDiff: 1,
					yDiff: 0
					);
				}
				if (Config.Instance.EnableFuelLoaders)
				{
					TechUtils.AddNode(__instance,
					ModAssets.Techs.FuelLoaderTechID,
					Config.Instance.RocketDocking ? ModAssets.Techs.DockingTechID : GameStrings.Technology.ColonyDevelopment.CelestialDetection,
					xDiff: 2,
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
				if (Config.SpaceStationsPossible)
				{
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
					xDiff: 2,
					yDiff: 0
					);

					TechUtils.AddNode(__instance,
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
						TechUtils.AddNode(__instance,
							ModAssets.Techs.HugeCargoBayTechID,
							ModAssets.Techs.SpaceStationTechID,
							xDiff: 2,
							yDiff: -1);
					}
					else
					{
						TechUtils.AddNode(__instance,
							ModAssets.Techs.HugeCargoBayTechID,
							GameStrings.Technology.SolidMaterial.SolidManagement,
							xDiff: 1,
							yDiff: 0);
					}
				}



			}
		}
	}
}
