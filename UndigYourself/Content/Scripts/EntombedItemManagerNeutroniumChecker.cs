using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace UndigYourself.Content.Scripts
{
	internal class EntombedItemManagerNeutroniumChecker : KMonoBehaviour
	{
		[MyCmpGet] EntombedItemManager eim;

		public override void OnSpawn()
		{
			base.OnSpawn();
			UnburyAllNeutroniumStuckChunks();
		}
		void UnburyAllNeutroniumStuckChunks()
		{
			var cells = eim.cells;
			List<int> neutroniumBuridItemIndices = [];
			for (int i = 0; i < cells.Count; i++)
			{
				int cell = cells[i];
				int worldIdx = Grid.WorldIdx[cell];


				if (NeutroniumMole.InvalidCell(cell, worldIdx))
				{
					neutroniumBuridItemIndices.Add(i);
				}
			}
			SgtLogger.l("EntombedItemManager.OnSpawn, total buried item count: " + cells.Count + ", neutronium items found: " + neutroniumBuridItemIndices.Count);
			if (!neutroniumBuridItemIndices.Any())
				return;

			neutroniumBuridItemIndices.Sort();
			neutroniumBuridItemIndices.Reverse();
			EntombedItemVisualizer eiv = Game.Instance.GetComponent<EntombedItemVisualizer>();

			foreach (var buriedItemIndex in neutroniumBuridItemIndices)
			{
				EntombedItemManager.Item obj = eim.GetItem(buriedItemIndex);

				int cell = obj.cell;
				int worldIdx = Grid.WorldIdx[cell];
				int newCell = NeutroniumMole.FindValidNewCell(cell, worldIdx);
				if(newCell != cell)
				{
					eiv.RemoveItem(obj.cell);
					eim.RemoveItem(buriedItemIndex); 
					ElementLoader.FindElementByHash((SimHashes)obj.elementId)?.substance.SpawnResource(Grid.CellToPosCCC(newCell, Grid.SceneLayer.Ore), obj.mass, obj.temperature, obj.diseaseIdx, obj.diseaseCount);
				}
			}
		}
	}
}
