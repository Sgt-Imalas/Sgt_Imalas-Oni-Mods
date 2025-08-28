using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class BuildingFacade_Patches
    {

        [HarmonyPatch(typeof(BuildingFacade), nameof(BuildingFacade.ChangeBuilding))]
        public class BuildingFacade_ChangeBuilding_Patch
        {
            public static void Postfix(BuildingFacade __instance)
            {
                __instance.gameObject.Trigger(ModAssets.OnBuildingFacadeChanged);
            }
        }
    }
}
