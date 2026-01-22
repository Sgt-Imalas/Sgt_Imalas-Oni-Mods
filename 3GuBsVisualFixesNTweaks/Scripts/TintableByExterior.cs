using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
		HashSet<string> ExistingTintSymbols = [];
		public override void OnSpawn()
		{
			base.OnSpawn();
			var extends = building.GetExtents();
			List<int> cells = new List<int>();
			//SgtLogger.l($"Extends: X:{extends.x}, Y:{extends.y},width:{extends.width},height:{extends.height}");
			for (int x = 0; x < extends.width; x++)
			{
				for (int y = 0; y < extends.height; y++)
				{
					//SgtLogger.l("Cell" + Grid.XYToCell(x, y) + ", x:" + x + ",y:" + y);
					cells.Add(Grid.XYToCell(extends.x+x, extends.y+y));
				}
			}

			monitorCells = cells.ToArray();
			AssignTintables();
			UpdateTint();
		}

		void AssignTintables()
		{
			ExistingTintSymbols.Clear();

			bool HasSymbol(KBatchedAnimController kbac, string symbol_name)
			{
				KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(kbac.GetBatchGroupID()).GetSymbol(symbol_name);
				return symbol != null;
			}

			foreach (var symbol in ModAssets.PossibleTintSymbols)
			{
				if (HasSymbol(kbac, symbol))
				{
					ExistingTintSymbols.Add(symbol);
				}
			}
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
					Tint(ModAssets.GetElementColor(element.id));
					return;
				}
			}

			Tint(Color.clear);
		}
		void Tint(Color color)
		{
			foreach (var symbol in ExistingTintSymbols)
			{
				kbac.SetSymbolTint(symbol, color);
			}
		}
	}
}
