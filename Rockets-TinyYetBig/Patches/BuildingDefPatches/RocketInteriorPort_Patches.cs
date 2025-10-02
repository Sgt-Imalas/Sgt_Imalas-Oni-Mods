using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	/// <summary>
	/// Make Rocket interior wall ports part of the rocket wall to allow conduit loaders/unloaders to connect to them
	/// </summary>
	public class RocketInteriorPort_Patches
	{

		[HarmonyPatch]
		public static class AddRocketWallTagToTilesThatShouldBeWall
		{
			[HarmonyPostfix]
			public static void Postfix(GameObject go)
			{
				KPrefabID component = go.GetComponent<KPrefabID>();
				if (Config.Instance.HabitatInteriorPortImprovements)
				{
					component.AddTag(GameTags.RocketEnvelopeTile);
					component.AddTag(GameTags.CorrosionProof);
				}
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(RocketInteriorGasInputPortConfig).GetMethod(name);
				yield return typeof(RocketInteriorGasOutputPortConfig).GetMethod(name);
				yield return typeof(RocketInteriorLiquidInputPortConfig).GetMethod(name);
				yield return typeof(RocketInteriorLiquidOutputPortConfig).GetMethod(name);
			}
		}



		/// <summary>
		/// Fixes bugged rad absorpion of habitat ports
		/// </summary>
		[HarmonyPatch]
		public static class MakeHabitatPortsFoundationTiles
		{
			[HarmonyPostfix]
			static void Postfix(ref BuildingDef __result)
			{
				if (Config.Instance.HabitatInteriorPortImprovements)
				{
					BuildingTemplates.CreateFoundationTileDef(__result);
				}
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.CreateBuildingDef);
				yield return typeof(RocketInteriorGasInputPortConfig).GetMethod(name);
				yield return typeof(RocketInteriorGasOutputPortConfig).GetMethod(name);
				yield return typeof(RocketInteriorLiquidInputPortConfig).GetMethod(name);
				yield return typeof(RocketInteriorLiquidOutputPortConfig).GetMethod(name);
			}
		}
	}
}
