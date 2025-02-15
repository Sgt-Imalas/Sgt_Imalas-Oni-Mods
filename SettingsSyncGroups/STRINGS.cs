using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsSyncGroups
{
	internal class STRINGS
	{
		public class UI
		{
			public class BUILDINGSETTINGSGROUP_SIDESCREEN
			{
				public static LocString TITLE = "Building Settings Group";
			}
			public class GROUPASSIGNMENT_SECONDARYSIDESCREEN
			{
				public static LocString TITLE = "Change settings group";
				public static LocString NO_GROUP_ASSIGNED = "[no group assigned]";

				public class SEARCHBAR
				{
					internal class INPUT
					{
						public class TEXTAREA
						{
							public static LocString PLACEHOLDER = NO_GROUP_ASSIGNED;
							public static LocString TEXT = "";
						}
					}
				}
			}
		}
	}
}
