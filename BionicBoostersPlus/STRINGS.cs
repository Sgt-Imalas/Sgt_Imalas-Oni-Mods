using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace BionicBoostersPlus
{
	internal class STRINGS
	{
		public class MISC
		{
			public class STATUSITEMS
			{
				public class DREAMBOOSTERJOURNALSTORAGEFULL
				{
					public static LocString NAME = (LocString)"Dream Recorded";
					public static LocString TOOLTIP = (LocString)$"This {PRE_KEYWORD}Dreaming Booster{PST_KEYWORD} has sufficient dream data stored to create a new dream journal.\n\nIt must be installed in a Bionic Duplicant in order to function";
				}

				public class DREAMBOOSTERJOURNALSTORAGE
				{
					public static LocString NAME = (LocString)"Stored Dreamdata: {0}";
					public static LocString TOOLTIP = (LocString)$"{PRE_KEYWORD}Dreaming Boosters{PST_KEYWORD} retain dream data gathered by Bionic Duplicants\n\nWhen sufficient dream data is collected and this booster is installed in a Bionic Duplicant, a new dream journal will be generated";
				}
			}
		}
		public class DUPLICANTS
		{
			public class MODIFIERS
			{
				public class BB_WATERPROOFEDSTRESSREDUCTION
				{
					public static LocString NAME = "Hydrophobic Coating";
					public static LocString TOOLTIP = "This Bionic Duplicant got its internal components covered in an extra thick layer of gear oil,\nprotecting them from most adverse effects of water based problems.\nCauses increased gear oil consumption";
				}

			}
			public class STATUSITEMS
			{
				public class DREAMBOOSTER_IDLE
				{
					public static LocString NAME = FormatAsLink("Dreaming Booster", "BB_BOOSTER_DREAM") + " equipped";

					public static LocString TOOLTIP = "This Duplicant can now produce " + FormatAsLink("Dream Journals", "DREAMJOURNAL") + " when defragmenting";
				}
				public class DREAMBOOSTER_DREAMING
				{
					public static LocString NAME = "Dreaming (of electric sheep)";

					public static LocString TOOLTIP = "This Duplicant is adventuring through their own subconscious\n\nBionic Dreams are caused by equipping a " + FormatAsLink("Dreaming Booster", "BB_BOOSTER_DREAM") + "\n\n" + FormatAsLink("Dream Journal", "DREAMJOURNAL") + " will be ready in {time}";
				}
				public class MEDBOOSTER_REPAIRING
				{
					public static LocString NAME = "Repairing Damage";

					public static LocString TOOLTIP = "This Bionic Duplicant is currently reparing tissue damage.";
				}
				public class SOLARBOOSTER_CONSUMINGSUN
				{
					public static LocString NAME = "Current Wattage: {Wattage}";
					public static LocString TOOLTIP = $"This Bionic Duplicant is currently generating {FormatAsPositiveRate("{Wattage}")} of {PRE_KEYWORD}Power{PST_KEYWORD} from absorbed light.";
				}
				

			}
			public class ROLES
			{
				public class BIONICS_C3_WATTS
				{
					public static LocString NAME = FormatAsLink("Optimized Circuitry", nameof(BIONICS_C3_WATTS));
					public static LocString TOOLTIP = "Optimizes the design of the Bionic Duplicants circuits, allowing them to run at a higher efficiency and with less disturbances of the organic components.";
				}
				public class BIONICS_D3_RADS
				{
					public static LocString NAME = FormatAsLink("Internal Plating", nameof(BIONICS_D3_RADS));
					public static LocString TOOLTIP = "Allows Bionic Duplicants to work safely in areas with higher ambient radiation";
				}
			}
		}
		public class ITEMS
		{
			public class BIONIC_BOOSTERS
			{
				public class BB_OVERCLOCKED
				{
					public static LocString OC_PREFIX = "Fused {0}";
					public static LocString OC_SUFFIX ="This booster has been fabricated by combining multiple boosters and is considered unstable.";
				}
				public class BB_BOOSTER_DREAM
				{
					public static LocString NAME = FormatAsLink("Dreaming Booster", "BB_BOOSTER_DREAM");
					public static LocString DESC = "Grants a Bionic Duplicant the ability to dream of electric sheep.";
					//public static LocString EFFECT = "Dreaming while Defragmenting";
				}
				public class BB_BOOSTER_BATTERYSLOT
				{
					public static LocString NAME = FormatAsLink("Battery Slot Booster", "BB_BOOSTER_BATTERYSLOT");
					public static LocString DESC = "Allows the installation of another power bank using the space of booster's slot.\nThis booster is bending the specs of the bionic duplicant model, causing slight discomfort.";
					//public static LocString EFFECT = "Trading skills for power bank capacity";
				}
				public class BB_BOOSTER_WATERPROOFED
				{
					public static LocString NAME = FormatAsLink("Waterproofing Booster", "BB_BOOSTER_WATERPROOFED");
					public static LocString DESC = "Actively coats the internal electric components of the Bionic Duplicant in a thick layer of gear oil with persistent reapplication,\nprotecting them from most adverse effects of water induced stress.";
				}
				public class BB_BOOSTER_MEDIKIT
				{
					public static LocString NAME = FormatAsLink("Biomechanical Repair Booster", "BB_BOOSTER_MEDIKIT");
					public static LocString DESC = "Repairs both the organic tissue and mechanical components of Bionic Duplicants in case of direct or radiation induced damage.";
				}
				public class BB_BOOSTER_SOLAR
				{
					public static LocString NAME = FormatAsLink("Subdermal Solar Booster", "BB_BOOSTER_SOLAR");
					public static LocString DESC = "Installs small subdermal solar arrays in various locations, allowing the Bionic Duplicant to harvest the power of the sun directly\n\nSunscreen not included.";
				}
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class BBP_BOOSTERRECYCLER
				{
					public static LocString NAME = FormatAsLink("Booster 'Recycling' Station", "BBP_BOOSTERRECYCLER");
					public static LocString DESC = "Stepping up the booster 'Refinement'";
					public static LocString EFFECT = "Crushes down bionic boosters back into microchips.\n\nMicrochip recovery is not fully guaranteed!";
					public static LocString RECIPE_FORMAT = "Crush down {0} to recover some of its Microchips.\nMicrochip recovery rate will fluctuate.";
				}
				public class BBP_MK3BOOSTERMAKER
				{
					public static LocString NAME = FormatAsLink("Experimental Laser Soldering Station", "BBP_MK3BOOSTERMAKER");
					public static LocString DESC = "Lasers make everything more hightech right?";
					public static LocString EFFECT = "Allows the fabrication of experimental booster modules that can bring your bionics to new levels of performance, at a cost.";
				}
			}
		}
		public class UI
		{
			public class ROLES_SCREEN
			{
				public class PERKS
				{
					public class BB_BIONICDREAM
					{
						public static LocString DESCRIPTION = $"Ability to dream while defragmenting.";
					}
					public class BB_REDUCEDWATERSTRESS
					{
						public static LocString DESCRIPTION = $"Reduces Stress from liquid exposure.";
					}
					public class BB_WATTAGEREDUCTION
					{
						public static LocString DESCRIPTION = "Power Draw";
					}
				}
			}
		}
	}
}
