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
				public class NC_NATURALPLANTER
				{
					public static LocString NAME = FormatAsLink("Wild Planter", nameof(NC_NATURALPLANTER));
					public static LocString DESC = "Very natural, much wow.";
					public static LocString EFFECT = $"Allows selecting a single seed to be wild planted.\nWill also create a natural tile on seed planting.\nTile mass can be configured during construction.";
				}
				public class NC_NATURALPLANTERWIDE
				{
					public static LocString NAME = FormatAsLink("Wide Wild Planter", nameof(NC_NATURALPLANTERWIDE));
					public static LocString DESC = "Very natural, much wow.";
					public static LocString EFFECT = $"Allows selecting a single large seed to be wild planted.\nWill also create three natural tiles on seed planting.\nTotal tile mass can be configured during construction.";
				}
				public class NC_NATURALPLANTERBACKWALL
				{
					public static LocString NAME = FormatAsLink("Wild Backwall Planter", nameof(NC_NATURALPLANTERBACKWALL));
					public static LocString DESC = "Very natural, much wow.";
					public static LocString EFFECT = $"Allows selecting a single large backwall-planted seed to be wild planted.\nWill also create 4 natural backwalls on seed planting.\ntotal backwall mass can be configured during construction.";
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
				public class ENABLE_PLANTERS
				{
					public static LocString TITLE = "Enable Wild Planters";
					public static LocString TOOLTIP = "Enables wild planter buildings (wide planter and backwall planter are only available with aquatic planet pack like their regular planters)";
				}
			}
		}
	}
}
