using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadingPip
{
	internal class STRINGS
	{
		public class MODOPTIONS
		{
			public class LOADINGICON
			{
				public static LocString NAME = "Loading icon Id/Name";
				public static LocString TOOLTIP = "The id or name of the entity you want to see during the loading screen.";
			}
			public class RANDOMIZEDLOADINGICON
			{
				public static LocString NAME = "Randomize loading icon";
				public static LocString TOOLTIP = "Randomize the loading icon from the selected List.";


				public static LocString OPTION_NONE = "No Randomization";
				public static LocString OPTION_CRITTERS = "Pick random critter";
				public static LocString OPTION_CUSTOM = "Pick random custom icon";
				public static LocString OPTION_CHAOS = "Pick completely random icon";
			}
			public class PRIMALASPID
			{
				public static LocString NAME = "Primal Aspid";
				public static LocString TOOLTIP = "The bane of the knight, now in your loading screen!";
			}
			public class OPEN_ICON_FOLDER
			{
				public static LocString NAME = "Open Custom Icon folder";
				public static LocString TOOLTIP = "Open the folder for custom icons";
			}
		}
	}
}
