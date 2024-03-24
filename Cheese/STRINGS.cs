using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                public static LocString CHEESEMOD_BRACKENEPRODUCT_DESC = "This contains "+ MILK.NAME+".";

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
                    public static LocString NAME = (LocString)UI.FormatAsLink("Cheese Block", nameof(CHEESE_CHEESESCULPTURE));
                    public static LocString DESC = (LocString)"Duplicants who have learned art skills can produce more decorative sculptures.";
                    public static LocString EFFECT = (LocString)("Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.");
                    public static LocString POORQUALITYNAME = (LocString)"\"Abstract\" Cheese Sculpture";
                    public static LocString AVERAGEQUALITYNAME = (LocString)"Mediocre Cheese Sculpture";
                    public static LocString EXCELLENTQUALITYNAME = (LocString)"Genius Cheese Sculpture";

                    public class FACADES
                    {
                        public class SCULPTURE_CHEESE_CRAP_1
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("TODO", nameof(SCULPTURE_CHEESE_CRAP_1));
                            public static LocString DESC = (LocString)"TODO DESC";
                        }

                        public class SCULPTURE_CHEESE_GOOD_1
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("TODO", nameof(SCULPTURE_CHEESE_GOOD_1));
                            public static LocString DESC = (LocString)"TODO DESC";
                        }

                        public class SCULPTURE_CHEESE_AMAZING_1
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Ratatouille", nameof(SCULPTURE_CHEESE_AMAZING_1));
                            public static LocString DESC = (LocString)"The best head-chef the world has seen.";
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
                public static LocString DESC = "The salty remains of evaporated cheese. Will dissolve into "+ MILKFAT.NAME+" and "+SALT.NAME+".";
            }
        }
        public class DUPLICANTS
        {
            public class TRAITS
            {
                public class CHEESETHROWER
                {
                    public static LocString NAME = (LocString)"Cheese Connoisseur";
                    public static LocString DESC = (LocString)"This Duplicant shares their immovable love for cheese with everyone around when they are " + UI.PRE_KEYWORD + "Overjoyed" + UI.PST_KEYWORD;
                }
                public class BRACKTOSEINTOLERANT
                {
                    public static LocString NAME = (LocString)"Bracktose Intolerant";
                    public static LocString DESC = (LocString)"This duplicants tummy hurts after consuming tasty brackene products.";
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
        }
    }
}
