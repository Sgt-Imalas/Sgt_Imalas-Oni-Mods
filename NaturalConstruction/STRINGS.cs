using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace NaturalConstruction
{
	internal class STRINGS
	{
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class NC_NATURALTILE
				{
					public static LocString NAME = FormatAsLink("Natural Tile", nameof(NC_NATURALTILE));
					public static LocString DESC = "Very natural, much wow."; 
					public static LocString EFFECT = $"Creates a natural tile on finishing construction.\nMass can be configured during construction.";
				}
				public class NC_NATURALBACKWALL
				{
					public static LocString NAME = FormatAsLink("Natural Backwall", nameof(NC_NATURALBACKWALL));
					public static LocString DESC = "Very natural, much wow.";
					public static LocString EFFECT = $"Creates a natural backwal on finishing construction.\nMass can be configured during construction.";
				}
			}
		}
	}
}
