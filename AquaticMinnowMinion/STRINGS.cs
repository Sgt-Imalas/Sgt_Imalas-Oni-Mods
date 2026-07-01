using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

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
			public class ATTRIBUTES
			{
				public class AQUATIC_GILLMOISTUREDELTA
				{
					public static LocString NAME = (LocString)"Gill Moisturization";
					public static LocString DESC = (LocString)$"Gill Moisturization determines how much fast the gills of an Amphibious Duplicant dry out.";
				}
			}
			public class CHOREGROUPS
			{
				public class AQ_ADAPTATIONSKILLS
				{
					public static LocString NAME = "Adaptation";
				}
			}
			public class MODEL
			{
				public class AQUATIC
				{
					public static LocString NAME = FormatAsLink("Amphibious Duplicant", nameof(DUPLICANTS));
					public static LocString NAME_TOOLTIP = "This Duplicant is the result of genetic experiments that spliced the dna of aquatic critters with that of regular duplicants.";
					public static LocString DESC = (LocString)$"Amphibious Duplicants are adapted to a life underwater, giving them an edge in flooded, oxygen-poor environments.\n\nThey will complete errands in order of {FormatAsLink("Priority", "PRIORITY")}.";
					public static LocString NAME_ADJECTIVE = (LocString) FormatAsLink("Amphibious", nameof (DUPLICANTS));
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
					public static LocString TOOLTIP = $"Duplicants require {FormatAsLink("Oxygen", "OXYGEN")} to live, amphibious duplicants can filter it from water.";
				}
			}
			public class CHORES
			{				
				public class MOISTURIZEME
				{
					public static LocString NAME = (LocString)"Moistuize Me!";
					public static LocString STATUS = (LocString)"Looking for moisturization";
					public static LocString TOOLTIP = (LocString)$"Amphibious Duplicants need {PRE_KEYWORD}Moisturization{PST_KEYWORD} of their gills.";
				}
			}
			public class MODIFIERS
			{
				public class AQ_ITCHYGILLS
				{
					public static LocString NAME = "Itchy Gills";
					public static LocString TOOLTIP ="This Duplicant got a big nasty gill-ful of contaminated liquids";
				}
				public class AQ_BREATHINGGILLS
				{
					public static LocString NAME = "Filtering oxygen from water";
					public static LocString TOOLTIP = "This Duplicant is currently filtering oxygen from the surrounding water.\nThis is not as efficient as breathing directly.";
					public static LocString TOOLTIP_SKILLED = "This Duplicant is currently filtering oxygen from the surrounding water with increased efficiency.";
				}
				public class AQ_DRYGILLS_MINOR
				{
					public static LocString NAME = "Slightly Dry Gills";
					public static LocString TOOLTIP = "This Duplicant's gills have become slightly dry.";
				}
				public class AQ_DRYGILLS_MAJOR
				{
					public static LocString NAME = "Very Dry Gills";
					public static LocString TOOLTIP = "This Duplicant's gills have become very dry!";
				}
				public class AQ_DRYGILLS_EXTREME
				{
					public static LocString NAME = "Completely Dry Gills";
					public static LocString TOOLTIP = "This Duplicant's gills are completely dry!!";
				}
				public class AQ_DRYSUITAIR
				{
					public static LocString NAME = "Dry Suit Air";
					public static LocString TOOLTIP = "The bottled air in this suit is so dry!";
				}
				public class AQ_REFRESHINGDRINK
				{
					public static LocString NAME = "Refreshing Drink";
					public static LocString TOOLTIP = "This amphibious duplicant just had a really moisturizing drink.";
				}
				
			}
			public class ROLES
			{
				public class ADAPTATION_EYEPROTECTION
				{
					public static LocString NAME = FormatAsLink("Nictitating Membrane", nameof(ADAPTATION_EYEPROTECTION));
					public static LocString TOOLTIP = "Develops a retractable secondary eye membrane, protecting them against irritants.";
				}
				public class ADAPTATION_GILLPROTECTION
				{
					public static LocString NAME = FormatAsLink("Mucus Glands", nameof(ADAPTATION_GILLPROTECTION));
					public static LocString TOOLTIP = "Develops glands that produce a thin mucus layer covering the skin and gills of the duplicant, protecting them from irritants and providing some heat protection.";
				}
				public class ADAPTATION_INSULATION
				{
					public static LocString NAME = FormatAsLink("Hypodermal Blubber", nameof(ADAPTATION_INSULATION));
					public static LocString TOOLTIP = "Develops a specialized fat layer that protects the duplicants in cold environments and increases its insulation";
				}
				public class ADAPTATION_WATERBREATHINGRATEREDUCTION
				{
					public static LocString NAME = FormatAsLink("Enlarged Gills", nameof(ADAPTATION_WATERBREATHINGRATEREDUCTION));
					public static LocString TOOLTIP = "Develops gills that are more effective at filtering oxygen from water.";
				}
			}
		}
		public class UI
		{
			public class ROLES_SCREEN
			{
				public class PERKS
				{
					public class ADAPT_WATERBREATHINGEFFICIENCY
					{
						public static LocString DESCRIPTION = (LocString)$"Decreased liquid consumption when breathing liquids.";
					}
				}
			}
		}
	}
}
