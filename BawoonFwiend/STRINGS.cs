using static STRINGS.UI;

namespace BawoonFwiend
{
	internal class STRINGS
	{
		public class MODCONFIG
		{
			public class GASMASS
			{
				public static LocString NAME = "Gas Mass";
				public static LocString TOOLTIP = "Amount of gas (in kg) it takes to make one balloon.";
			}
			public class STATBOOST
			{
				public static LocString NAME = "Balloon Machine Stat Boost";
				public static LocString TOOLTIP = "Set the attribute height Duplicants recieve when getting a balloon from the Balloon Dispenser.\nBalloon Buddy gives +8 by default";
			}
		}
		public class EFFECTS
		{
			public class NOTATRUEFRIEND
			{
				public static LocString NAME = "Machine made Balloon";
				public static LocString DESC = "It's just not the same coming from a machine";
			}
		}

		public class MISC
		{
			public class TAGS
			{
				public static LocString BALLOONGAS = "Balloon Gas";
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class BF_BALLOONSTATION
				{
					public static LocString NAME = FormatAsLink("Balloon Dispenser", nameof(BF_BALLOONSTATION));
					public static LocString DESC = "You get a balloon,\n you get a balloon,\neverybody gets a balloon!";
					public static LocString EFFECT = ("This building gives out balloons to duplicants during downtime.\n\nConsumes 5kg of either " + FormatAsLink("Hydrogen", "HYDROGEN") + " or " + FormatAsLink("Helium", "HELIUM") + " for each balloon created.");
				}
			}
		}
		public class UI
		{
			public class UISIDESCREENS
			{
				public class BF_BALLOONSTAND
				{
					public static LocString TITLE = "Balloon Skins";
					public static LocString TOGGLEALLON = "Activate All";
					public static LocString TOGGLEALLOFF = "Deactivate All";

					public static LocString ENABLEMANUALDELIVERY = "Enable Manual Delivery";
					public static LocString ENABLEMANUALDELIVERYTOOLTIP = "Enables manual delivery of gasses, disables pipe input";
					public static LocString DISABLEMANUALDELIVERY = "Disable Manual Delivery";
					public static LocString DISABLEMANUALDELIVERYTOOLTIP = "Disables manual delivery of gasses, enables pipe input";


					public static LocString ALLRANDOMYES = "Each balloon skin is chosen fully random.\nResults in random amounts of each chosen skin to appear.\nToggle to change it to a queue based system";
					public static LocString ALLRANDOMNO = "Each active balloon skin is picked atleast\nonce before duplicates can appear.\nResults in an even distribution of each chosen skin.\nToggle to change it to a fully random system";
				}
			}
		}
	}
}
