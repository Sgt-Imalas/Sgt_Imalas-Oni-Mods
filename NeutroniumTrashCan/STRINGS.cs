namespace NeutroniumTrashCan
{
	internal class STRINGS
	{

		public class MISC
		{
			public class TAGS
			{
				public static LocString ARTIFACT = "Artifact";
			}
		}

		public class BUILDINGS
		{
			public class PREFABS
			{
				public class NTC_ARTIFACTTRASHCAN
				{
					public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Artifact Trash Can", nameof(NTC_ARTIFACTTRASHCAN));
					public static LocString DESC = "Its bigger on the inside!";
					public static LocString EFFECT = "Eats and deletes artifacts.";

				}
				public class NTC_NEUTRONIUMTRASHCAN
				{
					public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Neutronium Trash Can", nameof(NTC_NEUTRONIUMTRASHCAN));
					public static LocString DESC = "Its bigger on the inside!";
					public static LocString EFFECT = "Eats and deletes Neutronium - for those cleanly shaved walls.";

				}
				public class NTC_GASTRASHCAN
				{
					public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Gas Trash Can", nameof(NTC_GASTRASHCAN));
					public static LocString DESC = "Its bigger on the inside!";
					public static LocString EFFECT = "Deletes Gasses pumped into it";

				}
				public class NTC_SOLIDTRASHCAN
				{
					public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Solid Trash Can", nameof(NTC_SOLIDTRASHCAN));
					public static LocString DESC = "Its bigger on the inside!";
					public static LocString EFFECT = "Deletes Solids pumped into it";

				}
				public class NTC_LIQUIDTRASHCAN
				{
					public static LocString NAME = (LocString)global::STRINGS.UI.FormatAsLink("Liquid Trash Can", nameof(NTC_LIQUIDTRASHCAN));
					public static LocString DESC = "Its bigger on the inside!";
					public static LocString EFFECT = "Deletes Liquids pumped into it";

				}
			}
		}
	}
}
