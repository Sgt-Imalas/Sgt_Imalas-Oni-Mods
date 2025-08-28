using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.BuildingConfigInterfaces
{
	public interface IHasConfigurableRange
	{
		int GetTileRange();
		void SetTileRange(int tiles);
#nullable enable
		string? GetDescriptorText();
		public Tuple<int, int>? GetTileValueRange();
#nullable disable
	}
}
