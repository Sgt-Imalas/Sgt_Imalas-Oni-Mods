using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class FilterablePump : Pump
	{
		[SerializeField] public SimHashes FilterElement = SimHashes.Vacuum;

		public bool CanPump()
		{
			int originCell = consumer.GetSampleCell();
			var sourceXY = Grid.CellToXY(originCell);
			var worldIdx = Grid.WorldIdx[originCell];

			Element.State expected_state = Element.State.Vacuum;
			switch (dispenser.conduitType)
			{
				case ConduitType.Gas:
					expected_state = Element.State.Gas;
					break;
				case ConduitType.Liquid:
					expected_state = Element.State.Liquid;
					break;
			}

			int radius = consumer.consumptionRadius;
			bool elementFilterActive = FilterElement != SimHashes.Vacuum;

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

					bool correctElement = !elementFilterActive || Grid.Element[cellToTest].id == FilterElement;

					if (correctElement && Grid.Element[cellToTest].IsState(expected_state) && Grid.TestLineOfSight(sourceXY.X, sourceXY.Y, XY.X, XY.Y, BlockingCallback))
					{
						return true;
					}
				}
			}
			return false;
		}
		bool BlockingCallback(int cell)
		{
			//liquids can block gases, gases cannot block liquids, solids block always
			if (conduitType == ConduitType.Gas)
			{
				return Grid.IsSolidCell(cell) || Grid.IsLiquid(cell);

			}
			else
				return Grid.IsSolidCell(cell);

		}
	}
}
