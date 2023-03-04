using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrywallPatternColours
{
    internal class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class EXTERIORWALL
                {
                    public class FACADES
                    {
                        public class RED_DEEP
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Magma", nameof(EXTERIORWALL));
                            public static LocString DESC = (LocString)"A red wallpaper.";
                        }

                        public class ORANGE_SATSUMA
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Bright", nameof(EXTERIORWALL));
                            public static LocString DESC = (LocString)"An orange wallpaper.";
                        }

                        public class YELLOW_LEMON
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Yellowcake", nameof(EXTERIORWALL));
                            public static LocString DESC = (LocString)"A radiation-free wallpaper.";
                        }

                        public class GREEN_KELLY
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Algae", nameof(EXTERIORWALL));
                            public static LocString DESC = (LocString)"A slippery wallpaper.";
                        }

                        public class BLUE_COBALT
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("H2O", nameof(EXTERIORWALL));
                            public static LocString DESC = (LocString)"A damp wallpaper.";
                        }

                        public class PINK_FLAMINGO
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Petal", nameof(EXTERIORWALL));
                            public static LocString DESC = (LocString)"A pink wallpaper.";
                        }

                        public class GREY_CHARCOAL
                        {
                            public static LocString NAME = (LocString)UI.FormatAsLink("Charcoal", nameof(EXTERIORWALL));
                            public static LocString DESC = (LocString)"A sleek wallpaper.";
                        }
                    }
                }

            }
        }
    }
}
