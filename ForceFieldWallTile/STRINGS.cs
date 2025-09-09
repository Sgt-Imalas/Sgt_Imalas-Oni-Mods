using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceFieldWallTile
{
	internal class STRINGS
	{
		public class BUILDING
		{
			public class STATUSITEMS
			{
				public class FFT_SHIELDFULLYCHARGED
				{
					public static LocString NAME = "Forcefield fully charged!";
					public static LocString TOOLTIP = "This forcefield projector is at maximum strength";
				}
				public class FFT_SHIELDOVERLOADED
				{
					public static LocString NAME = "Forcefield Overloaded! Remaining Cooldown time: {0}";
					public static LocString TOOLTIP = "This forcefield projector got exerted beyond its limits and has shut down temporarily to cool down.";
				}
				public class FFT_SHIELDCHARGING
				{
					public static LocString NAME = "Forcefield Charging: {0}";
					public static LocString TOOLTIP = "This forcefield projector is currently building up its barrier strength.";
				}
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class FFT_FORCEFIELDPROJECTOR
				{
					public static LocString NAME = UI.FormatAsLink("Forcefield Projector",nameof(FFT_FORCEFIELDPROJECTOR));
					public static LocString DESC = "Does not protect against the fury of the Pips!";
					public static LocString EFFECT = "Prevents gases, liquids and meteors from passing through when active. Does not block duplicant or debris movement.";
				}
			}
		}
	}
}
