using HarmonyLib;
using NaturalConstruction.Content.Scripts;
using UnityEngine;

namespace NaturalConstruction.Patches
{
	internal class CopyBuildingSettings_Patches
	{

        [HarmonyPatch(typeof(CopyBuildingSettings), nameof(CopyBuildingSettings.ResolveLayer))]
        public class CopyBuildingSettings_ResolveLayer_Patch
        {
            public static void Postfix(CopyBuildingSettings __instance, GameObject sourceGameObject, ref ObjectLayer __result)
            {
                if(sourceGameObject.TryGetComponent<ConstructableNaturalSpawner>(out var spawner) && spawner.Backwall)
                    __result = spawner.building.Def.ObjectLayer;
			}
        }
	}
}
