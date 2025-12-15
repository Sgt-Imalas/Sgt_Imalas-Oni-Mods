using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace ComplexFabricatorRibbonController
{
	class STRINGS
	{
		public class BUILDING
		{
			public class STATUSITEMS
			{
				public class CFRC_NOTLINKEDTOHEAD
				{
					public static LocString NAME = "Not attached to a fabricator";
					public static LocString TOOLTIP = "This building is not attached to a fabricator!";
				}
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class CFRC_COMPLEXFABRICATORRECIPECONTROLATTACHMENT
				{
					public static LocString NAME = FormatAsLink("Fabricator Recipe Controller", nameof(CFRC_COMPLEXFABRICATORRECIPECONTROLATTACHMENT));
					public static LocString DESC = "Beep Boop, the time of recipe automation is now!";
					public static LocString EFFECT = "This Building needs to be placed on top of a fabricator to function.\n\nIt allows to control the production of up to 4 recipes in the attached fabricator building.\n\nFabricators support multiple attachment in parallel.";

					public static LocString LOGIC_PORTS_DESCRIPTION = "Fabricator Recipe Automation";
					public static LocString LOGIC_PORTS_ACTIVE = FormatAsAutomationState("Green Signal", AutomationState.Active) + ": Enables endless production of the recipe selected for the respective bit.";
					public static LocString LOGIC_PORTS_INACTIVE = FormatAsAutomationState("Red Signal", AutomationState.Standby) + ": Stops the production of the recipe selected for the respective bit.";
				}
			}
		}
		public class UI
		{
			public class RFRC_CONFIG
			{
				public class MICROCHIP_REQUIRED
				{
					public static LocString TEXT = "Building requires microchip";
					public static LocString TOOLTIP = "Whether or not the building requires a microchip to construct. If disabled, the metal cost will be slightly higher.";
				}
			}
			public static LocString RFRC_NO_RECIPE = "No Recipe";
			public class RIBBONSELECTIONSIDESCREEN
			{
				public static LocString TITLE = "Fabricator Recipe Control";
				public class WARNING
				{
					public static LocString LABEL = "No Fabricator attached!";
					public static LocString TOOLTIP = "This recipe controller is not attached to a fabricator building!";
				}
			}


			public class LISTSELECTION_SECONDARYSIDESCREEN
			{
				public static LocString TITLE = "Recipe Selection for Bit {0}:";
				public class SEARCHBAR
				{
					public static LocString CLEARTOOLTIP = "Clear search bar";
					public class INPUT
					{
						public class TEXTAREA
						{
							public static LocString PLACEHOLDER = "Enter text to filter recipes...";
						}
					}
				}
				public class USETOOLTIPS
				{
					public static LocString CURRENTRECIPE = "Currently selected for this Bit";
					public static LocString ALREADYINUSE = "This recipe is already controlled by Bit {0}";
					public static LocString ALREADYINUSEOTHER = "This recipe is already controlled by Bit {0}\non a different Recipe Controller that\nis attached to the same fabricator.";
				}
			}
		}
	}
}
