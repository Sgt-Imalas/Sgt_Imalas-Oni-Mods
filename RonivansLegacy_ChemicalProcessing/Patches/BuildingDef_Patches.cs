using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class BuildingDef_Patches
	{

        [HarmonyPatch(typeof(ExteriorWallConfig), nameof(ExteriorWallConfig.CreateBuildingDef))]
        public class ExteriorWallConfig_CreateBuildingDef_Patch
        {
			[HarmonyPrepare] static bool Prepare() => Config.Instance.Drywall_Hides_Pipes;
			public static void Postfix(BuildingDef __result) => ModAssets.MakeWallHidePipesIfEnabled(__result);
        }

        [HarmonyPatch(typeof(ThermalBlockConfig), nameof(ThermalBlockConfig.CreateBuildingDef))]
        public class ThermalBlockConfig_CreateBuildingDef_Patch
		{
			[HarmonyPrepare] static bool Prepare() => Config.Instance.Drywall_Hides_Pipes;
			public static void Postfix(BuildingDef __result) => ModAssets.MakeWallHidePipesIfEnabled(__result);
		}

        [HarmonyPatch(typeof(BuildingDef), nameof(BuildingDef.DefaultElements))]
        public class BuildingDef_DefaultElements_Patch
        {
            ///Replace rayon fiber as default material with regular reed fiber.
            public static void Postfix(BuildingDef __instance, ref List<Tag> __result)
            {
                var rayonIndex = __result.FindIndex(tag => tag == RayonFabricConfig.TAG);
				if(rayonIndex>=0 && rayonIndex < __result.Count())
                    __result[rayonIndex] = BasicFabricConfig.ID;
			}
        }
	}
}
