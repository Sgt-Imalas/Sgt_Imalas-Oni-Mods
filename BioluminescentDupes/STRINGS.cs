using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioluminescentDupes
{
	internal class STRINGS
	{
		public class ITEMS
		{
			public class PILLS
			{
				public class BD_TRAITESSENCE
				{
					public static LocString NAME = "Biolumin Essence";
					public static LocString DESC = "Permanently gives the consumer the Bioluminescent trait";
					public static LocString RECIPEDESC = ("A highly experimental medicine that gives the duplicant a permanent faint glow. No side effects were recorded so far!");

				}
			}

		}
		public class DUPLICANTS
		{
			public class TRAITS
			{
				public class BD_BIOLUMINESCENSE
				{
					public static LocString NAME = "Bioluminescent";
					public static LocString DESC = "This Duplicant is has a faintly glow to it";
					public static LocString SHORT_DESC = "Emits low amounts light";
					public static LocString SHORT_DESC_TOOLTIP = "Emits low amounts of light";
				}
			}
		}
	}
}
