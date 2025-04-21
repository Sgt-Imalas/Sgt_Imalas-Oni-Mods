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
		}
	}
}
