using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.ELEMENTS;
using static STRINGS.UI;

namespace ShapeProjector
{
	internal class STRINGS
	{
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class SP_PLANSHAPEPROJECTOR
				{
					public static LocString NAME = FormatAsLink("Shape Plan Projector", nameof(SP_PLANSHAPEPROJECTOR));
					public static LocString DESC = "Just like a protractor, but more versatile.";
					public static LocString EFFECT = "Allows displaying configurable shapes for build planning.";
				}
			}
		}
		public class UI
		{
			public class SHAPEPROJECTIONSIDESCREEN
			{
				public class TITLE
				{
					public static LocString TITLETEXT = "Shape Projector Configuration";
				}
				public class CONTENTCONTAINER
				{
					public class SCROLLRECTCONTAINER
					{
						public class ITEMPREFAB
						{
							public class DIMENSIONS
							{
								public static LocString LABEL = "Dimensions (X , Y):";
							}
							public class PROJECTIONACTIVE
							{
								public static LocString LABEL = "Projection active:";
							}
							public class THICCWALLS
							{
								public static LocString LABEL = "Thick diagonal lines:";
							}							
						}

					}
				}
				public class ADDNEWBUTTON
				{
					public static LocString LABEL = "Add another shape projection";
				}
			}

		}
	}
}
