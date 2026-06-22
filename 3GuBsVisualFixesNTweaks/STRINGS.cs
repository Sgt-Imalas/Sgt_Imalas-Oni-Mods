using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks
{
    class STRINGS
	{
		public class MISC
		{
			public class TAGS
			{
				public static LocString VFNT_PLACEMENTVISUALIZEREXCLUDED = "Hide Floor Visualizer";
			}

		}
		public class VFNT_MODCONFIG
		{
			public class ROCKETPLATFORM_FRONT
			{
				public static LocString NAME = "Rocket Platform in Front of rockets";
				public static LocString TOOLTIP = "Render the rocket platform in front of rocket engines and their exhaust";
			}
			public class HIDDENTILEBITS
			{
				public static LocString NAME = "Hide Tile Bits";
				public static LocString TOOLTIP = "Hide parts of the decor bits of tiles";
				public class TILEBITCHANGE
				{
					public static LocString NONE = "None";
					public static LocString BITSONLY = "Small bits only";
					public static LocString BITSANDTOPS = "Small Bits and Tops"; 
					public static LocString EVERYTHING = "All Bits and Tops";
				}
			}
			public class CATEGORIES
			{
				public static LocString LIQUID = "Liquid Tweaks";
			}
			public class MAGMA_BUBBLES
			{
				public static LocString NAME = "Bubbling Magma";
				public static LocString TOOLTIP = "Magma open to air/vacuum is now bubbling (visual fx only).";
			}
			public class OLD_MAGMA
			{
				public static LocString NAME = "Old Magma Visuals";
				public static LocString TOOLTIP = "Reverts Magma back to the Red Soup it was before the liquid shader update (U59/Aquatic Dlc Release)";
			}
			public class OLD_METALS
			{
				public static LocString NAME = "Old Molten Metal Visuals";
				public static LocString TOOLTIP = "Reverts molten metals back to the colorful Soup they were before the liquid shader update (U59/Aquatic Dlc Release)";
			}
			public class MORETOOLFX
			{
				public static LocString NAME = "More Tool FX";
				public static LocString TOOLTIP = "Makes the Dig tool display different tool FX based on the material dug out.\nAlso reintroduces an unused deconstruct tool fx beam.";
			}
		}
	}
}
