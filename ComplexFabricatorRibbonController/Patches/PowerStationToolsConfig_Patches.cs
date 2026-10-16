using HarmonyLib;
using UnityEngine;

namespace ComplexFabricatorRibbonController.Patches
{
    class PowerStationToolsConfig_Patches
    {

        [HarmonyPatch(typeof(PowerStationToolsConfig), nameof(PowerStationToolsConfig.CreatePrefab))]
        public class PowerStationToolsConfig_CreatePrefab_Patch
		{
			public static void Postfix(GameObject __result)
			{
				KPrefabID component = __result.GetComponent<KPrefabID>();
				component.AddTag(ModAssets.Microchip_Buildable);
			}
        }
    }
}
