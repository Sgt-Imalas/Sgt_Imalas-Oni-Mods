using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NaturalConstruction.STRINGS.BUILDINGS.PREFABS;
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
					public static LocString EFFECT = $"Creates a natural backwall on finishing construction.\nMass can be configured during construction.";
				}
			}
		}
		public class UI
		{
			public class NC_MODCONFIG
			{
				public class DEFAULT_MASS_TILE
				{
					public static LocString TITLE = "Natural Tile default mass";
					public static LocString TOOLTIP = "Configure the default value for natural tile mass.\nThis value can be changed on the building plan.";
				}
				public class DEFAULT_MASS_BACKWALL
				{
					public static LocString TITLE = "Natural Backwall default mass";
					public static LocString TOOLTIP = "Configure the default value for natural backwall mass.\nThis value can be changed on the building plan.";
				}
				public class CONSTRUCTIONTIME_MASS_SCALING
				{
					public static LocString TITLE = "Construction time mass scaling";
					public static LocString TOOLTIP = "When active, lets the construction time scale with the mass picked for the individual natural building.\nOtherwise defaults to 30 s.";
				}
				public class CONSTRUCTION_MASS_MULTIPLIER
				{
					public static LocString TITLE = "Spawn Mass multiplier";
					public static LocString TOOLTIP = "Multiplies the building material mass when converting into a spawned natural tile/backwall.\nThis allows compensating for the 50% mass loss on digging the natural tile.\nExample with x1.5: 100kg construction mass -> 150kg natural tile/backwall -> digging result 75kg";
				}
			}
		}
	}
}
