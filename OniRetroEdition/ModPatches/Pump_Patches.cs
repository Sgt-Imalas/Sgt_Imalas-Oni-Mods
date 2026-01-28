using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OniRetroEdition.ModPatches
{
	internal class Pump_Patches
	{

		[HarmonyPatch(typeof(Pump), nameof(Pump.IsPumpable))]
		public class Pump_IsPumpable_Patch
		{
			static Element.State state;
			static bool BlockingCallback(int cell)
			{
				return Grid.IsSolidCell(cell);
			}
			public static bool Prefix(Pump __instance, Element.State expected_state, ref bool __result)
			{
				state = expected_state;
				__result = false;
				var consumer = __instance.consumer;
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


						if (Grid.Element[cellToTest].IsState(expected_state) && Grid.TestLineOfSight(sourceXY.X, sourceXY.Y, XY.X, XY.Y, BlockingCallback))
						{
							__result = true;
							return false;
						}
					}
				}
				return false;
			}
		}
	}
}
