using ComplexFabricatorRibbonController.Scripts.Buildings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexFabricatorRibbonController.Patches
{
    class ComplexFabricator_Patches
    {

        [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.OnCleanUp))]
        public class ComplexFabricator_OnCleanUp_Patch
        {
            public static void Postfix(ComplexFabricator __instance) => RefreshAttachments(__instance, true);
		}

        [HarmonyPatch(typeof(ComplexFabricator), nameof(ComplexFabricator.OnSpawn))]
        public class ComplexFabricator_OnSpawn_Patch
        {
            public static void Postfix(ComplexFabricator __instance) => RefreshAttachments(__instance, false);
		}

        static void RefreshAttachments(ComplexFabricator __instance, bool isCleanup)
        {
            if(__instance.gameObject.TryGetComponent<Building>(out var building))
            {
                HashSet<int> cells  = building.PlacementCells.ToHashSet();

				foreach (var attachment in ComplexFabricatorRecipeControlAttachment.AllAttachments)
                {
                    if (cells.Contains(attachment.Cell))
                    {
                        if (isCleanup)
                            attachment.Detatch();
                        else
                            attachment.TryReattach();
					}
				}
            }
        }
    }
}
