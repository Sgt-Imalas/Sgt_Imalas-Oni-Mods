namespace _WorldGenStateCapture
{
	internal class STRINGS
	{
		public static LocString STARTPARSING = "Start Collecting Worlds";
		public class WORLDPARSERMODCONFIG
		{
			public class TARGETCLUSTERBASE
			{
				public static LocString NAME = "Target World (Basegame)";
				public static LocString DESC = "Coordinate prefix of the world to parse seeds from.";
			}
			public class TARGETCLUSTERDLC
			{
				public static LocString NAME = "Target Cluster (DLC)";
				public static LocString DESC = "Coordinate prefix of the cluster to parse seeds from.";
            }
            public class RANDOMIZEDGEN
            {
				public static LocString NAME = "generate random Clusters";
                public static LocString DESC = "selects the cluster to generate at random.";
            }
            public class TARGETNUMBER
			{
				public static LocString NAME = "Number of runs";
				public static LocString DESC = "total number of runs the program should parse before stopping.\nInfinite parsing overrides this!";
			}
			public class TARGETINFINITE
			{
				public static LocString NAME = "Infinite parsing";
				public static LocString DESC = "the tool will run continuously until the mod is removed by deleting the mods.json file.\nOnly do this if you want fully autonomous seed parsing!";
			}
			public class RANDOMMIXING
			{
				public static LocString NAME = "Enable Random Mixing";
				public static LocString DESC = "Mixing Options will be enabled at random";
			}
		}
	}
}
