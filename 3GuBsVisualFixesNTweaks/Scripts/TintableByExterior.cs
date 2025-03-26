using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	class TintableByExterior : KMonoBehaviour, ISim1000ms
	{
		[MyCmpReq] Building building;
		[MyCmpReq] KBatchedAnimController kbac;

		int[] monitorCells;
		public override void OnSpawn()
		{
			base.OnSpawn();
			var extends = building.GetExtents();
			List<int> cells = new List<int>();
			SgtLogger.l($"Extends: X:{extends.x}, Y:{extends.y},width:{extends.width},height:{extends.height}");
			for (int x = 0; x < extends.width; x++)
			{
				for (int y = 0; y < extends.height; y++)
				{
					SgtLogger.l("Cell" + Grid.XYToCell(x, y) + ", x:" + x + ",y:" + y);
					cells.Add(Grid.XYToCell(extends.x+x, extends.y+y));
				}
			}

			monitorCells = cells.ToArray();
			UpdateTint();
		}

		public void Sim1000ms(float dt)
		{
			UpdateTint();
		}

		private void UpdateTint()
		{
			if (!monitorCells.Any())
			{
				SgtLogger.warning("no monitor cells!");
				return;
			}

			for (int i = 0; i < monitorCells.Length; ++i)
			{
				int inputCell = monitorCells[i];
				var element = Grid.Element[inputCell];
				if (element.IsLiquid)
				{
					kbac.SetSymbolTint("tint", ModAssets.GetElementColor(element.id));
					return;
				}
			}

			kbac.SetSymbolTint("tint", Color.clear);
		}
	}
}
