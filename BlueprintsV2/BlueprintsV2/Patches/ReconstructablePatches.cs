using BlueprintsV2.BlueprintData;
using BlueprintsV2.ModAPI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Patches
{
    class ReconstructablePatches
    {

        [HarmonyPatch(typeof(Reconstructable), nameof(Reconstructable.TryCommenceReconstruct))]
        public class Reconstructable_TryCommenceReconstruct_Patch
        {
            public static void Prefix(Reconstructable __instance)
            {
                //storing all the extra data
                var building = __instance.building;
                BuildingConfig dataCarrier = new BuildingConfig();
                dataCarrier.BuildingDef = building.Def;
                int cell = Grid.PosToCell(__instance);
				API_Methods.StoreAdditionalBuildingData(__instance.gameObject, dataCarrier);

                //next frame it places the building under construction, so we make sure the data is applied after that so the placed building exists
                GameScheduler.Instance.Schedule("Reconstructable Reapply Data", 0.1f, _ => 
				{
					var newBuildingUnderConstruction = Grid.Objects[cell, (int)building.Def.ObjectLayer];
                    if (newBuildingUnderConstruction == null)
                        return;

                    API_Methods.ApplyAdditionalBuildingData(newBuildingUnderConstruction, dataCarrier);
				});

			}
        }
    }
}
