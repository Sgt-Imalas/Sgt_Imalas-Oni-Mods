using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipPlantNotify
{
	internal class STRINGS
	{
		public class UI
		{
			public class PIPPLANTNOTIFICATION
			{
				public static LocString TITLE = "Pip Planting Notification";
				public static LocString LABEL = "Notify on Planting";
				public static LocString TOOLTIP = "Toggle if this Pip should trigger a notification on planting a seed.";
				public class NOTIFICATION
				{
					public static LocString TITLE = "{0} has planted a seed!";
					public static LocString TOOLTIP = "{0} has planted a seed!\nClick here to visit the location.";
				}
			}
		}
	}
}
