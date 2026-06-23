using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaticMinnowMinion
{
	internal class STRINGS
	{
		public class MISC
		{
			public class TAGS
			{
				public static LocString AM_OXYGENIZEDLIQUID = "Oxygen-enriched Liquid";
				public static LocString AM_OXYGENIZEDLIQUID_DESC = "Amphibious Duplicants can breathe this liquid";
				public static LocString AM_POLLUTEDLIQUID = "Polluted Liquid";
			}
		}

		public class DUPLICANTS
		{
			public class MODEL
			{
				public class AQUATIC
				{
					public static LocString NAME = "Amphibious Duplicant";
					public static LocString NAME_TOOLTIP = "This Duplicant is the result of genetic experiments that spliced the dna of aquatic critters with that of regular duplicants.";
				}
			}
			public class STATS
			{
				public class AQUATIC_GILLMOISTURE
				{
					public static LocString NAME = "Gill Moisture";
					public static LocString TOOLTIP = "Amphibious duplicants will get severly stressed out when their gills dry out.";
				}
			}
			public class STATUSITEMS
			{
				public class BREATHINGINAQUATIC
				{
					public static LocString NAME = "Inhaling {ConsumptionRate} {Element}";
					public static LocString TOOLTIP = $"Duplicants require {UI.FormatAsLink("Oxygen", "OXYGEN")} to live, amphibious duplicants can filter it from water.";
				}
			}
			public class MODIFIERS
			{
				public class AQ_ITCHYGILLS
				{
					public static LocString NAME = "Itchy Gills";
					public static LocString TOOLTIP ="This Duplicant got a big nasty gill-ful of contaminated liquids";
				}
			}
		}
	}
}
