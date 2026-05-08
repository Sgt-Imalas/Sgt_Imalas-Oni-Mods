using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class BuildingTemplates_Patches
	{

		[HarmonyPatch(typeof(BuildingTemplates),
			nameof(BuildingTemplates.ExtendBuildingToClusterCargoBay),
			[typeof(GameObject), typeof(float), typeof(List<Tag>), typeof(List<Tag>), typeof(CargoBay.CargoType)])]
		public class BuildingTemplates_ExtendBuildingToClusterCargoBay_Patch
		{
			/// <summary>
			/// cargo bays should automatically filter out non-fitting elements.
			/// </summary>
			/// <param name="forbiddenTags"></param>
			/// <param name="cargoType"></param>
			public static void Prefix(ref  List<Tag> forbiddenTags, CargoBay.CargoType cargoType)
			{
				if(forbiddenTags == null && cargoType != CargoBay.CargoType.Entities)
					forbiddenTags = new List<Tag>();

				switch (cargoType)
				{
					case CargoBay.CargoType.Solids:
						if (!forbiddenTags.Contains(GameTags.Gas)) forbiddenTags.Add(GameTags.Gas);
						if (!forbiddenTags.Contains(GameTags.Liquid)) forbiddenTags.Add(GameTags.Liquid);
						break;

					case CargoBay.CargoType.Liquids:
						if (!forbiddenTags.Contains(GameTags.Solid)) forbiddenTags.Add(GameTags.Solid);
						if (!forbiddenTags.Contains(GameTags.Gas)) forbiddenTags.Add(GameTags.Gas);
						break;
					case CargoBay.CargoType.Gasses:
						if (!forbiddenTags.Contains(GameTags.Solid)) forbiddenTags.Add(GameTags.Solid);
						if (!forbiddenTags.Contains(GameTags.Liquid)) forbiddenTags.Add(GameTags.Liquid);
						break;

				}
			}
		}


		[HarmonyPatch(typeof(BuildingTemplates), nameof(BuildingTemplates.ExtendBuildingToRocketModuleCluster))]
		public class BuildingTemplates_ExtendBuildingToRocketModuleCluster_Patch
		{
			public static void Postfix(GameObject template)
			{
				template.AddOrGet<RocketModuleUpgradeStorage>();
			}
		}
	}
}
