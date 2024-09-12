namespace GoodByeFrostByte
{
	internal class STRINGS
	{
		public class MODCONFIG_GOODBYEFROSTBITE
		{
			public class DISABLECOLDSLEEP
			{
				public static LocString NAME = "Disable cold sleep interrupt";
				public static LocString TOOLTIP = "Dupes no longer wake up from being too cold.\nAlso included in \"Disable cold debuffs\"\nUse this option if you still want the cold debuff without the waking up part.";
			}
			public class DISABLECOLDDEBUFF
			{
				public static LocString NAME = "Disable cold debuffs";
				public static LocString TOOLTIP = "Dupes don't get a athletics and stamina debuff in cold areas\nAlso includes the feature of \"Disable cold sleep interupt\"";
			}
			public class DISABLECOLDDAMAGE
			{
				public static LocString NAME = "Disable frostbite damage";
				public static LocString TOOLTIP = "Dupes no longer get any frostbite damage";
			}
			public class FROSTBITETHRESHOLD
			{
				public static LocString NAME = "Frostbite temperature threshold (in °C)";
				public static LocString TOOLTIP = "Adjust the temperature at which dupes receive frostbite damage";
			}
			public class DISABLEHEATDEBUFF
			{
				public static LocString NAME = "Disable heat debuffs";
				public static LocString TOOLTIP = "Dupes don't get a athletics and stamina debuff in hot areas";
			}
			public class OLDCRITTERTEMPDETECTION
			{
				public static LocString NAME = "Old critter temperature detection";
				public static LocString TOOLTIP = "Critters will only check their own temperature for any heat or cold related effects";
			}
			public class OLDCRITTERTEMPHAPPINESS
			{
				public static LocString NAME = "Disable critter temperature glum effects";
				public static LocString TOOLTIP = "Critters will no longer receive happiness debuffs from temperatures";
			}
			public class LOGARITHMICSPEEDDEBUFF
			{
				public static LocString NAME = "Logarithmic Athletics debuff scaling";
				public static LocString TOOLTIP = "Negative Athletics will now be less severe by switching the speed scaling from a linear to a (approx.) logarithmic curve.\n" +
					"the new formula used for these values is \"Total dupe speed = 1/(1 - (StatValue * 10%))\"";
			}
		}
	}
}
