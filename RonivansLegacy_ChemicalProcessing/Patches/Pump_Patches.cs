using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
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
			public static void Postfix(Pump __instance, Element.State expected_state, ref bool __result)
			{
				if (__instance is not RotatablePump pump)
					return;

				int originCell = __instance.consumer.GetSampleCell();
				var sourceXY = Grid.CellToXY(originCell);
				var worldIdx = Grid.WorldIdx[originCell];

				int radius = __instance.consumer.consumptionRadius;

				for (int i = -radius; i < radius; i++)
				{
					for (int j = -radius; j < radius; j++)
					{
						if ((Math.Abs(i) + Math.Abs(j)) >= radius)
							continue;

						int cellToTest = originCell + j + Grid.WidthInCells * i;
						if (!Grid.IsValidCellInWorld(cellToTest, worldIdx))
							continue;

						var XY = Grid.CellToXY(cellToTest);

						if (Grid.Element[cellToTest].IsState(expected_state) && Grid.TestLineOfSight(sourceXY.X, sourceXY.Y, XY.X, XY.Y, Grid.IsSolidCell))
						{
							__result = true;
							return;
						}
					}
				}
				__result = false;
			}
		}
	}
}
