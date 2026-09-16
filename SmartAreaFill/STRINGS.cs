namespace SmartAreaFill
{
	internal class STRINGS
	{
		public class FLOODFILLTOOLS_OPTIONS
		{
			public class HOLD_DELAY
			{
				public static LocString NAME = "Flood fill hold delay";
				public static LocString TOOLTIP = "Time delay from holding down leftclick to flood fill starting to propagate, value in seconds.";
			}
			public class SOLID_DOORS
			{
				public static LocString NAME = "Doors count as solids";
				public static LocString TOOLTIP = "Affects tools that propagate in non-solid tiles;\nMakes these tools stop their floodfill at doors, even if the door is not solid (like a pneumatic door).";
			}
			public class DIAGONAL_PROPAGATION
			{
				public static LocString NAME = "Diagonal Fill Checks";
				public static LocString TOOLTIP = "Affects mop tool and dig tool currently;\nMakes these tools also check cells diagonally for propagation";
			}
		}
	}
}
