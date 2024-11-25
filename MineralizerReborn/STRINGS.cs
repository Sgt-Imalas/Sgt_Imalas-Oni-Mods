using static STRINGS.UI;

namespace MineralizerReborn
{
	internal class STRINGS
	{
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class MINERALIZER
				{
					public static LocString NAME = FormatAsLink("Mineralizer", nameof(MINERALIZER));
					public static LocString DESC = $"Washing {FormatAsLink("Salt", "SALT")} with {FormatAsLink("Water", "WATER")} dissolves it and creates {FormatAsLink("Salt Water", "SALTWATER")}.";
					public static LocString EFFECT = $"Adds {FormatAsLink("Salt", "SALT")} to {FormatAsLink("Water", "WATER")}, producing {FormatAsLink("Salt Water", "SALTWATER")}.";
				}
			}
		}
	}
}
