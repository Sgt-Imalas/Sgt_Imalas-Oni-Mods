using AkisSnowThings.Content.Scripts.Buildings;
using HarmonyLib;
using UnityEngine;

namespace AkisSnowThings.Patches
{
	public class BuildingLoaderPatch
	{
		[HarmonyPatch(typeof(BuildingLoader), "CreateBuildingComplete")]
		public class BuildingLoader_CreateBuildingComplete_Patch
		{
			public static void Postfix(GameObject go, BuildingDef def)
			{
				if (ModAssets.GlassCaseSealables.Contains(def.PrefabID))
				{
					go.AddComponent<GlassCaseSealable>();
				}
			}
		}
	}
}
