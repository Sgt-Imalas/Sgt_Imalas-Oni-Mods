using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Pump_Patches
	{

        [HarmonyPatch(typeof(Pump), nameof(Pump.IsPumpable))]
        public class Pump_IsPumpable_Patch
        {
            public static void Postfix(Pump __instance, Element.State expected_state,  ref bool __result)
            {
                if (!__result)
                    return;

				int originCell = Grid.PosToCell(__instance.transform.GetPosition());
				var sourceXY = Grid.CellToXY(originCell);

				for (int i = 0; i < __instance.consumer.consumptionRadius; i++)
				{
					for (int j = 0; j < __instance.consumer.consumptionRadius; j++)
					{
						int cellToTest = originCell + j + Grid.WidthInCells * i;

						var XY = Grid.CellToXY(cellToTest);

						if (Grid.Element[cellToTest].IsState(expected_state) && Grid.TestLineOfSight(sourceXY.X,sourceXY.Y, XY.X,XY.Y, Grid.IsSolidCell))
						{
							return;
						}
					}
				}
				__result = false;
			}
        }
	}
}
