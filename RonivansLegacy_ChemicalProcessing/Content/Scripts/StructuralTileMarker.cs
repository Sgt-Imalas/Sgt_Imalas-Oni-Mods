using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class StructuralTileMarker : KMonoBehaviour
	{
		public static void ClearEverything() => Markers.Clear();
		static Dictionary<int, StructuralTileMarker> Markers = [];

		public override void OnSpawn()
		{
			base.OnSpawn();
			Markers[Grid.PosToCell(this)] = this;
			SgtLogger.l("StructuralTileMarker placed at cell " + Grid.PosToCell(this));
		}
		public override void OnCleanUp()
		{
			Markers.Remove(Grid.PosToCell(this));
			base.OnCleanUp();
		}

		internal static bool TileAtCell(int cell) => Markers.ContainsKey(cell);
	}
}
