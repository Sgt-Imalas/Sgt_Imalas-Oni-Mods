using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planticants
{
    class STRINGS
    {
		public class DUPLICANTS
		{
			public class PERSONALITIES
			{
				public class FLORAL_SYLVIA
				{
					public static LocString NAME = "Sylvia";
					public static LocString DESCRIPTION = "tba";
				}
				public class FLORAL_CAITHE
				{
					public static LocString NAME = "Caithe";
					public static LocString DESCRIPTION = "tba";
				}
				public class FLORAL_CANACH
				{
					public static LocString NAME = "Canach";
					public static LocString DESCRIPTION = "tba";
				}
				public class FLORAL_WYNNE
				{
					public static LocString NAME = "Wynne";
					public static LocString DESCRIPTION = "tba";
				}
			}
			public class MODEL
			{
				public class PLANT
				{
					public static LocString NAME = "Floral Duplicant";
					public static LocString NAME_TOOLTIP = "This Duplicant is a former gravitas decor plant scan that got its data mixed up with a regular duplicant";
				}
			}
			public class STATS
			{
				public class PLANT_GLUCOSE
				{
					public static LocString NAME = "Glucose";
					public static LocString TOOLTIP = "Floral duplicants will pass out from fatigue when the" + UI.PRE_KEYWORD + "Glucose Level" + UI.PST_KEYWORD + " reaches zero";
				}
				public class PLANT_CO2_TANK
				{
					public static LocString NAME = "Carbon Dioxide";
					public static LocString TOOLTIP = "Floral duplicants breathe in and store" + UI.PRE_KEYWORD + "Carbon Dioxide" + UI.PST_KEYWORD + " to use it for photosynthesis.";
				}
				public class PLANT_WATER
				{
					public static LocString NAME = "Water";
					public static LocString TOOLTIP = "Floral duplicants require" + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD + " for photosynthesis.\nIf they fully dry out, they die";
				}
				public class PLANT_LIGHTLEVELAVERAGE
				{
					public static LocString NAME = "Light";
					public static LocString TOOLTIP = "Floral duplicants require light for photosynthesis.";
				}
			}
		}
    }
}
