using STRINGS;
using UtilLibs;
using static STRINGS.ELEMENTS;

namespace Cheese
{
	internal class STRINGS
	{
		public class MISC
		{
			public class TAGS
			{


				public static LocString CHEESEMOD_BRACKENEPRODUCT = "Brackene Product";
				public static LocString CHEESEMOD_BRACKENEPRODUCT_DESC = "This contains " + MILK.NAME + ".";

				public static LocString CHEESEMOD_CHEESEMATERIAL = "Cheese";
				public static LocString CHEESEMOD_CHEESEMATERIAL_DESC = "Cheese is a very malleable and appealing material";
			}

		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class CHEESE_CHEESESCULPTURE
				{
					public static LocString NAME = UI.FormatAsLink("Cheese Block", nameof(CHEESE_CHEESESCULPTURE));
					public static LocString DESC = "Duplicants who have learned art skills can produce more decorative sculptures.";
					public static LocString EFFECT = ("Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.");
					public static LocString POORQUALITYNAME = "\"Abstract\" Cheese Sculpture";
					public static LocString AVERAGEQUALITYNAME = "Mediocre Cheese Sculpture";
					public static LocString EXCELLENTQUALITYNAME = "Genius Cheese Sculpture";

					public class FACADES
					{
						public class SCULPTURE_CHEESE_CRAP_1
						{
							public static LocString NAME = UI.FormatAsLink("The Bite", nameof(SCULPTURE_CHEESE_CRAP_1));
							public static LocString DESC = "With all that cheese, the artist got hungry...";
						}

						public class SCULPTURE_CHEESE_GOOD_1
						{
							public static LocString NAME = UI.FormatAsLink("Cheeseception", nameof(SCULPTURE_CHEESE_GOOD_1));
							public static LocString DESC = "A cheese born from cheese.";
						}

						public class SCULPTURE_CHEESE_AMAZING_1
						{
							public static LocString NAME = UI.FormatAsLink("Ratatouille", nameof(SCULPTURE_CHEESE_AMAZING_1));
							public static LocString DESC = "The best head-chef the world has ever seen.";
						}
						public class SCULPTURE_CHEESE_AMAZING_2
						{
							public static LocString NAME = UI.FormatAsLink("Wallace", nameof(SCULPTURE_CHEESE_AMAZING_2));
							public static LocString DESC = "No cheese, Gromit! We've forgot the cheese!";
						}
						public class SCULPTURE_CHEESE_AMAZING_3
						{
							public static LocString NAME = UI.FormatAsLink("The Moon", nameof(SCULPTURE_CHEESE_AMAZING_3));
							public static LocString DESC = "Everyone knows the moon is made out of cheese!";
						}
						public class SCULPTURE_CHEESE_AMAZING_4
						{
							public static LocString NAME = UI.FormatAsLink("Sergal", nameof(SCULPTURE_CHEESE_AMAZING_4));
							public static LocString DESC = "The fluffy cheese";
						}
						public class SCULPTURE_CHEESE_AMAZING_5
						{
							public static LocString NAME = UI.FormatAsLink("Spongebob Squarecheese", nameof(SCULPTURE_CHEESE_AMAZING_5));
							public static LocString DESC = "He lives in a pineapple under the sea.";
						}
						public class SCULPTURE_CHEESE_AMAZING_6
						{
							public static LocString NAME = UI.FormatAsLink("Hungry Kitty", nameof(SCULPTURE_CHEESE_AMAZING_6));
							public static LocString DESC = "Hey Kitty, you can has CheezeBurger - Mreaow.";
						}
						public class SCULPTURE_CHEESE_AMAZING_7
						{
							public static LocString NAME = UI.FormatAsLink("Pile of Cheese Wheels", nameof(SCULPTURE_CHEESE_AMAZING_7));
							public static LocString DESC = "Consuming these would probably heal you to full heath.";
						}
					}
				}

			}
		}
		public class ITEMS
		{
			public class FOOD
			{
				public class CHEESE
				{
					public static LocString NAME = UI.FormatAsLink("Cheese", nameof(CHEESE));
					public static LocString DESC = "So Tasty";
					public static LocString EFFECT = "So Tasty";
				}
				public class CHEESEBURGER
				{
					public static LocString NAME = UI.FormatAsLink("Cheeseburger", nameof(CHEESEBURGER));
					public static LocString DESC = (UI.FormatAsLink("Meat", "MEAT") + " and " + UI.FormatAsLink("Lettuce", "LETTUCE") + " and " + UI.FormatAsLink("Cheese", "CHEESE") + " on a chilled " + UI.FormatAsLink("Frost Bun", "COLDWHEATBREAD") + ".\n\nIt's the only burger best served cold.");
					public static LocString RECIPEDESC = (UI.FormatAsLink("Meat", "MEAT") + " and " + UI.FormatAsLink("Lettuce", "LETTUCE") + " and " + UI.FormatAsLink("Cheese", "CHEESE") + " on a chilled " + UI.FormatAsLink("Frost Bun", "COLDWHEATBREAD") + ".");
				}
				public class CHEESESANDWICH
				{
					public static LocString NAME = UI.FormatAsLink("Cheese Sandwich", nameof(CHEESESANDWICH));
					public static LocString DESC = "Frostbun with cheese";
					public static LocString RECIPEDESC = "Frostbun with cheese";
				}
				public class GRILLEDCHEESE
				{
					public static LocString NAME = UI.FormatAsLink("Grilled Cheese", nameof(GRILLEDCHEESE));
					public static LocString DESC = "Pepper bread with grilled cheese";
					public static LocString RECIPEDESC = "Pepper bread with grilled cheese";
				}
			}
			public class STATUSITEMS
			{
				public class CHEESE_ENCRUSTEDFOOD
				{
					public static LocString NAME = UIUtils.ColorText("Cheese Encrusted", ModAssets.CheeseColor);
					public static LocString TOOLTIP = "This food item has been encrusted in delicious cheese!";
				}
			}
		}
		public class ELEMENTS
		{
			public class CHEESE
			{
				public static LocString NAME = UI.FormatAsLink("Cheese", nameof(CHEESE));
				public static LocString DESC = "So Tasty";
				public static LocString EFFECT = "So Tasty";
			}
			public class CHEESEMOLTEN
			{
				public static LocString NAME = UI.FormatAsLink("Molten Cheese", nameof(CHEESEMOLTEN));
				public static LocString DESC = "So Tasty";
			}
			public class SALTYMILKFAT
			{
				public static LocString NAME = UI.FormatAsLink("Salty Brackwax", nameof(SALTYMILKFAT));
				public static LocString DESC = "The salty remains of evaporated cheese. Will dissolve into " + MILKFAT.NAME + " and " + SALT.NAME + ".";
			}
		}
		public class DUPLICANTS
		{
			public class TRAITS
			{
				public class CHEESETHROWER
				{
					public static LocString NAME = "Cheese Connoisseur";
					public static LocString DESC = "This Duplicant shares their immovable love for cheese with everyone around when they are " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD;
				}
				public class BRACKTOSEINTOLERANT
				{
					public static LocString NAME = "Bracktose Intolerant";
					public static LocString DESC = "This duplicants tummy hurts after consuming tasty brackene products.";
				}
			}
			public class PERSONALITIES
			{
				public class JAMESCHEESE
				{
					public static LocString NAME = "James May";
					public static LocString DESC = "Cheese!";
				}
			}
			public class DISEASES
			{
				public class CHEESEMAKINGBACTERIA
				{
					public static LocString NAME = "Cheese Microbes";
					public static LocString DESCRIPTION = "These tiny friends turn brackene into delicious cheese.";
					public static LocString LEGEND_HOVERTEXT = "Tiny cheese makers hard at work.";
				}
			}
			public class STATS
			{
				public class CHEESYNESS
				{
					public static LocString NAME = "Cheesyness";
					public static LocString TOOLTIP = "how cheesy is this dupe :D\nincreases when consuming cheese, decreases when not consuming cheese";
				}
			}
		}
	}
}
