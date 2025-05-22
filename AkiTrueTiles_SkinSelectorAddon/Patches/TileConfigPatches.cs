using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
    class TileConfigPatches
	{
		[HarmonyPatch]
		public static class Add_Override_Component
		{
			[HarmonyPrefix]
			public static void Prefix(GameObject go)
			{
				go.AddOrGet<TrueTiles_OverrideStorage>();
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(TileConfig).GetMethod(name);
				yield return typeof(MeshTileConfig).GetMethod(name);
				yield return typeof(CarpetTileConfig).GetMethod(name);
				yield return typeof(MetalTileConfig).GetMethod(name);
				yield return typeof(InsulationTileConfig).GetMethod(name);
				yield return typeof(GasPermeableMembraneConfig).GetMethod(name); //airflow tile
			}
		}
	}
}
