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
					public static LocString TOOLTIP = "This forcefield projector is at maximum strength.\nPower draw is reduced.";
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
		public class DUPLICANTS
		{
			public class MODIFIERS
			{
				public class FORCEFIELDTILE_STUCKINBARRIER
				{
					public static LocString NAME = "Slowed by Forcefield";
					public static LocString TOOLTIP = "The barrier makes it hard to move around.";
				}
			}
		}
		public class FFT_MODCONFIG
		{
			public class FFT_WATTAGE
			{
				public static LocString NAME = "Power Consumption (in W)";
				public static LocString TOOLTIP = "Power consumption of the forcefield projector while recharging.";
			}
			public class FFT_WATTAGE_STEADY
			{
				public static LocString NAME = "Energy Saver Consumption Multiplier";
				public static LocString TOOLTIP = "Power consumption of the forcefield projector when it is fully charged, Multiplier is applied to regular power consumption.";
			}
			public class FFT_DUPEEFFECT
			{
				public static LocString NAME = "Barrier affects passing duplicants";
				public static LocString TOOLTIP = "Duplicants inside of an active forcefield are slowed down by that barrier.";
			}
			public class FFT_PRESSUREDAMAGE
			{
				public static LocString NAME = "Barrier Pressure Damage";
				public static LocString TOOLTIP = "Liquids over the overpressure threshold deal forcefield damage, scaling with the overpressure amounts.";
			}
			public class FFT_METEORYIELD
			{
				public static LocString NAME = "Meteor Mass Percentage";
				public static LocString TOOLTIP = "Percentage of meteor mass dropped when a meteor explodes on the forcefield.";
			}
		}
	}
}
