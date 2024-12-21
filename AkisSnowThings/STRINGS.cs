using AkisSnowThings.Content.Defs.Buildings;
using AkisSnowThings.Content.Defs.Entities;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static global::STRINGS.UI;

namespace AkisSnowThings
{
	public class STRINGS
	{
		public class ELEMENTS
		{
			public class EVERGREENTREESAP
			{
				public static LocString NAME = FormatAsLink("Tree Sap", nameof(EVERGREENTREESAP));
				public static LocString DESC = ("Sticky goo harvested from an evergreen tree.\n\nIt can be polymerized into " + FormatAsLink("Isoresin", "ISORESIN") + " by boiling away its excess moisture.");
			}

			public class EVERGREENTREESAPFROZEN
			{
				public static LocString NAME = FormatAsLink("Frozen Sap", nameof(EVERGREENTREESAPFROZEN));
				public static LocString DESC = "Solidified goo harvested from an evergreen tree.\n\nIt is used in the production of " + FormatAsLink("Isoresin", "ISORESIN") + ".";
			}
		}
		public class MISC
		{
			public class TAGS
			{
				public static LocString SNOWTHINGS_GLASSCASE_ATTACHMENTSLOT = "Meltable Statue";
				public static LocString SNOWTHINGS_PINETREE_ATTACHMENTSLOT = "Evergreen Tree Stem";
			}
		}
		public class SNOWMODCONFIG
		{
			public static LocString CATEGORY_SNOWMACHINE = "Snow Machine";
			public class SNOWMACHINE_RANGE
			{
				public static LocString NAME = "Decor Range";
				public static LocString TOOLTIP = "Decor range in of the snow machine in tiles";
			}
			public class SNOWMACHINE_DECOR
			{
				public static LocString NAME = "Decor Value";
				public static LocString TOOLTIP = "Decor value in of the snow machine";
			}
			public static LocString CATEGORY_SNOWSCULPTURE = "Snow Sculpture";

		}
		public class CREATURES
		{
			public class SPECIES
			{
				public class SNOWSCULPTURES_EVERGREEN_TREE
				{
					public static LocString NAME = FormatAsLink("Evergreen", nameof(SNOWSCULPTURES_EVERGREEN_TREE));
					public static LocString DESC = NAME+"s are an excellent source of " + global::STRINGS.ELEMENTS.WOODLOG.NAME + ".\n\nThey also are a great joy of the season";
					public static LocString DOMESTICATEDDESC = DESC;
					public class REMAINS
					{
						public static LocString NAME = FormatAsLink("Wood Pile", nameof(global::STRINGS.ELEMENTS.WOODLOG));
						public static LocString DESC = (LocString)("The wooden remains of a felled " + SNOWSCULPTURES_EVERGREEN_TREE.NAME + ". Turns into " + global::STRINGS.ELEMENTS.WOODLOG.NAME+".");
					}
				}
				public class SEEDS
				{
					public class SNOWSCULPTURES_EVERGREEN_TREE
					{
						public static LocString NAME = FormatAsLink("Pine Cone", nameof(SNOWSCULPTURES_EVERGREEN_TREE));
						public static LocString DESC = ("The " + FormatAsLink("Seed", "PLANTS") + " of a " + SPECIES.SNOWSCULPTURES_EVERGREEN_TREE.NAME) + ".";
					}
				}
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class SNOWSCULPTURES_SNOWSCULPTURE
				{
					public static LocString NAME = FormatAsLink("Snow Pile", "SNOWSCULPTURES_SNOWSCULPTURE");
					public static LocString DESC = "Duplicants who have learned art skills can produce more decorative sculptures.";
					public static LocString EFFECT = "Majorly increases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";
					public static LocString POORQUALITYNAME = "\"Abstract\" Snowman";
					public static LocString MEDIOCREQUALITYNAME = "Mediocre Snowman";
					public static LocString EXCELLENTQUALITYNAME = "Snowman";
					public static LocString SNOWDOG = "Snowdog";

					public class FACADES
					{
						public class SNOWSCULPTURES_SNOWSCULPTURE_CRAP_1
						{
							public static LocString NAME = FormatAsLink("Friendly Pile", nameof(SNOWSCULPTURES_SNOWSCULPTURE_CRAP_1));
							public static LocString DESC = "It's just happy to see you";
						}

						public class SNOWSCULPTURES_SNOWSCULPTURE_GOOD_1
						{
							public static LocString NAME = FormatAsLink("Snowy Muckroot", nameof(SNOWSCULPTURES_SNOWSCULPTURE_GOOD_1));
							public static LocString DESC = "It almost tastes the same as the original";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_GOOD_2
						{
							public static LocString NAME = FormatAsLink("Happy Giant", nameof(SNOWSCULPTURES_SNOWSCULPTURE_GOOD_2));
							public static LocString DESC = "The artist ran out of snow half way in.";
						}

						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_1
						{
							public static LocString NAME = FormatAsLink("Philosopher Cat", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_1));
							public static LocString DESC = "It ponders and wonders about things beyond our comprehension... (and about cat snacks)";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_2
						{
							public static LocString NAME = FormatAsLink("Winter Pip", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_2));
							public static LocString DESC = "Prepared and ready to bury that arbor acorn in the ice..";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_3
						{
							public static LocString NAME = FormatAsLink("Meep", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_3));
							public static LocString DESC = "Just your average Meep";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_4
						{
							public static LocString NAME = FormatAsLink("Mechatronics Engineer", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_4));
							public static LocString DESC = "It will start constructing conveyor rails aaaany moment now.";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_5
						{
							public static LocString NAME = FormatAsLink("Cat Loaf", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_5));
							public static LocString DESC = "Give a cat a loaf of bread and it will eat for a day. <b>Teach</b> a cat to loaf...";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_6
						{
							public static LocString NAME = FormatAsLink("Hassan", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_6));
							public static LocString DESC = "Such a fabulous hairstyle!";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_7
						{
							public static LocString NAME = FormatAsLink("Puft Puft Puft", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_7));
							public static LocString DESC = "A stack of pufts in all of their farting glory";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_8
						{
							public static LocString NAME = FormatAsLink("Gentle-(Snow)-man", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_8));
							public static LocString DESC = "The coolest gentleman on the asteroid";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_9
						{
							public static LocString NAME = FormatAsLink("Pei", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_9));
							public static LocString DESC = "The cold never bothered here any Pei";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_10
						{
							public static LocString NAME = FormatAsLink("Snow Golem", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_10));
							public static LocString DESC = "Will always defend you against undead mobs";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_11
						{
							public static LocString NAME = FormatAsLink("Slickster Swarm", nameof(SNOWSCULPTURES_SNOWSCULPTURE_AMAZING_11));
							public static LocString DESC = "Unlike the other slicksters, these like to stay frosty";
						}
						public class SNOWSCULPTURES_SNOWSCULPTURE_DOG
						{
							public static LocString NAME = FormatAsLink("Lesser (Snow) Dog", nameof(SNOWSCULPTURES_SNOWSCULPTURE_DOG));
							public static LocString DESC = "You can pet that dawg!";
						}
					}
				}

				public class SNOWSCULPTURES_SNOWMACHINE
				{
					public static LocString NAME = FormatAsLink("Snow Machine", SnowMachineConfig.ID);
					public static LocString DESC = "Delights your duplicants with pretty snow.";
					public static LocString EFFECT = "Increases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE") + ".\n\nMust be powered.";
				}

				public class SNOWSCULPTURES_GLASSCASE
				{
					public static LocString NAME = FormatAsLink("Glass Case", GlassCaseConfig.ID);
					public static LocString DESC = "Protects Ice and Snow Sculptures.";
					public static LocString EFFECT = "Thermally insulates Snow and Ice Sculptures, so they cannot melt or exchange temperature with their environment.";
				}
				public class SNOWSCULPTURES_CHRISTMASTREE
				{
					public static LocString NAME = FormatAsLink("Festive Tree", ChristmasTreeAttachmentConfig.ID);
					public static LocString DESC = "It's the most wonderful time of the year";
					public static LocString EFFECT = "Attaches to a fully grown "+ CREATURES.SPECIES.SNOWSCULPTURES_EVERGREEN_TREE.NAME+".\n\nMajorly increases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE");
				}
			}

			public class STATUSITEMS
			{
				public class SNOWSCULPTURES_SEALEDSTATUSITEM
				{
					public static LocString NAME = "{0}";
					public static LocString SEALED = "Vacuum sealed";
					public static LocString SEALED2 = "Somehow still sealed";
					public static LocString TOOLTIP = "This building is thermally insulated, and cannot melt or exchange heat with it's surroundings.";
				}

			}
		}

        public class ENTITIES
        {
            public class PREFABS
            {
                public class SNOWSCULPTURES_GROWINGSNOWLAYER
                {
                    public static LocString NAME = FormatAsLink("Snow Drift", GrowingSnowLayerConfig.ID);
                    public static LocString DESC = "Snow that has accumulated from a snow machine.";
                }
            }

            public class STATUSITEMS
            {
                public class SNOWSCULPTURES_SEALEDSTATUSITEM
                {
                    public static LocString NAME = "{0}";
                    public static LocString SEALED = "Vacuum sealed";
                    public static LocString SEALED2 = "Somehow still sealed";
                    public static LocString TOOLTIP = "This building is thermally insulated, and cannot melt or exchange heat with it's surroundings.";
                }

            }
        }
        public class UI
		{
			public static LocString DECOROVERLAYTITLE = "Snow:";
			public static LocString PETTHATDAWG = "Pet";
			public static LocString PETTED = "Dog petted!";
			public class SNOWMACHINESIDESCREEN
			{
				public class CONTENTS
				{
					public class DENSITY
					{
						public static LocString LABEL = "Density";
						public static LocString TOOLTIP = "How snowy it should snow.";
					}

					public class SPEED
					{
						public static LocString LABEL = "Speed";
						public static LocString TOOLTIP = "How fast the snow should fall.";
					}

					public class LIFETIME
					{
						public static LocString LABEL = "Lifetime";
						public static LocString TOOLTIP = "Life length of a single particle.";
					}

					public class TURBULENCE
					{
						public static LocString LABEL = "Turbulence";
						public static LocString TOOLTIP = "The higher the value, the more random and chaotic the snowfall is.";
					}
				}
			}
		}
	}
}
