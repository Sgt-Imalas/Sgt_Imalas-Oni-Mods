using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDINGS.PREFABS;

namespace ClusterTraitGenerationManager
{
    internal class STRINGS
    {
        public class UI
        {
            public class CGM
            {
                public class INDIVIDUALSETTINGS
                {
                    public class STARMAPITEMENABLED
                    {
                        public static LocString LABEL = (LocString)"Generate Item:";
                        public static LocString TOOLTIP = (LocString)"Should this asteroid/POI be generated at all?";
                    }
                    public class AMOUNTSLIDER
                    {
                        public class DESCRIPTOR
                        {
                            public static LocString LABEL = (LocString)"Number to generate:";
                            public static LocString TOOLTIP = (LocString)"How many instances of these should be generated.\nValues that aren't full numbers represent a chance to generate for POIs.\n(f.e. 0.8 = 80% chance to generate this POI)";
                            public static LocString OUTPUT = (LocString)"REPLAC";
                            public class INPUT
                            {
                                public static LocString TEXT = (LocString)"";

                            }
                        }
                    }
                    public class MINDISTANCESLIDER
                    {
                        public class DESCRIPTOR
                        {
                            public static LocString LABEL = (LocString)"Minimum Distance:";
                            public static LocString TOOLTIP = (LocString)"The minimum distance this asteroid has to the center of the starmap.";
                            public class INPUT
                            {
                                public static LocString TEXT = (LocString)"";
                            }
                        }
                    }
                    public class MAXDISTANCESLIDER
                    {
                        public class DESCRIPTOR
                        {
                            public static LocString LABEL = (LocString)"Maximum Distance:";
                            public static LocString TOOLTIP = (LocString)"The maximum distance this asteroid has to the center of the starmap.";
                            public class INPUT
                            {
                                public static LocString TEXT = (LocString)"";

                            }
                        }
                    }
                    public class BUFFERSLIDER
                    {
                        public class DESCRIPTOR
                        {
                            public static LocString LABEL = (LocString)"Buffer Distance:";
                            public static LocString TOOLTIP = (LocString)"The minimum distance this asteroid has to other asteroids.";
                            public class INPUT
                            {
                                public static LocString TEXT = (LocString)"";

                            }
                        }
                    }
                    public class ASTEROIDSIZEINFO
                    {
                        public static LocString LABEL = (LocString)"Asteroid Size:";
                        public static LocString WIDTH = (LocString)"Width:";
                        public static LocString HEIGHT = (LocString)"Height:";
                        public static LocString TOOLTIP = (LocString)"The dimensions of this asteroid.";
                        public class SIZESELECTOR
                        {

                            //public static LocString NEGSIZE0 = (LocString)"Tiny";
                            //public static LocString NEGSIZE0TOOLTIP = (LocString)"The asteroid is at 25% of its usual size.";
                            //public static LocString NEGSIZE1 = (LocString)"Smaller";
                            //public static LocString NEGSIZE1TOOLTIP = (LocString)"The asteroid is at 40% of its usual size.";
                            //public static LocString NEGSIZE2 = (LocString)"Small";
                            //public static LocString NEGSIZE2TOOLTIP = (LocString)"The asteroid is at 55% of its usual size.";
                            //public static LocString NEGSIZE3 = (LocString)"Slightly Smaller";
                            //public static LocString NEGSIZE3TOOLTIP = (LocString)"The asteroid is at 75% of its usual size.";

                            public static LocString SIZE0 = (LocString)"Normal";
                            public static LocString SIZE0TOOLTIP = (LocString)"The asteroid is at its usual size.";
                            public static LocString SIZE1 = (LocString)"Slightly Larger";
                            public static LocString SIZE1TOOLTIP = (LocString)"The asteroid is 25% larger than normal.";
                            public static LocString SIZE2 = (LocString)"Large";
                            public static LocString SIZE2TOOLTIP = (LocString)"The asteroid is 50% larger than normal.";
                            public static LocString SIZE3 = (LocString)"Huge";
                            public static LocString SIZE3TOOLTIP = (LocString)"The asteroid has twice its usual size.";
                            public static LocString SIZE4 = (LocString)"Massive";
                            public static LocString SIZE4TOOLTIP = (LocString)"The asteroid has three times its usual size.";
                            public static LocString SIZE5 = (LocString)"Enormous";
                            public static LocString SIZE5TOOLTIP = (LocString)"The asteroid has four times its usual size.";
                        }
                        public class RATIOSELECTOR
                        {

                            public static LocString NORMAL = (LocString)"Default Ratio";
                            public static LocString NORMALTOOLTIP = (LocString)"Width and height of this asteroid are at their normal size ratio.";
                            public static LocString WIDE1 = (LocString)"Slightly Wider";
                            public static LocString WIDE1TOOLTIP = (LocString)"The asteroid is a bit wider than normal";
                            public static LocString WIDE2 = (LocString)"Wider";
                            public static LocString WIDE2TOOLTIP = (LocString)"The asteroid is wider than normal";
                            public static LocString WIDE3 = (LocString)"Much Wider";
                            public static LocString WIDE3TOOLTIP = (LocString)"The asteroid is a lot wider than normal";

                            public static LocString HEIGHT1 = (LocString)"Slightly Taller";
                            public static LocString HEIGHT1TOOLTIP = (LocString)"The asteroid is a bit taller than normal";

                            public static LocString HEIGHT2 = (LocString)"Taller";
                            public static LocString HEIGHT2TOOLTIP = (LocString)"The asteroid is taller than normal";

                            public static LocString HEIGHT3 = (LocString)"Much Taller";
                            public static LocString HEIGHT3TOOLTIP = (LocString)"The asteroid is a lot taller than normal";

                        }


                        public class INPUT
                        {
                            public static LocString TEXT = (LocString)"";

                        }
                    }
                    public class CLUSTERSIZE
                    {
                        public class DESCRIPTOR
                        {
                            public static LocString TOOLTIP = (LocString)"The radius of the starmap.";
                            public static LocString LABEL = (LocString)"Cluster Size:";
                            public class INPUT
                            {
                                public static LocString TEXT = (LocString)"";

                            }
                        }
                    }
                    public class ASTEROIDTRAITS
                    {
                        public static LocString LABEL = (LocString)"Asteroid Traits:";
                        public class LISTVIEW
                        {
                            public class NOTRAITSELECTEDINFO
                            {
                                public static LocString LABEL = (LocString)"  No Traits";                                
                            }
                        }
                        public class ADDTRAITBUTTON
                        {
                            public static LocString TEXT = (LocString)"Add Trait";
                        }
                    }
                    public class BUTTONS
                    {
                        public class RESETCLUSTERBUTTON
                        {
                            public static LocString TEXT = (LocString)"Reset Cluster";
                            public static LocString TOOLTIP = (LocString)"Undo all changes you have made by reloading the cluster preset.";
                        }
                        public class RESETSELECTIONBUTTON
                        {
                            public static LocString TEXT = (LocString)"Reset Current Selection";
                            public static LocString TOOLTIP = (LocString)"Undo all changes you have made to the currently selected item.";
                        }
                        public class RETURNBUTTON
                        {
                            public static LocString TEXT = (LocString)"Return";
                        }
                        public class GENERATECLUSTERBUTTON
                        {
                            public static LocString TEXT = (LocString)"Generate Cluster";
                        }
                    }
                }

                public class TRAITPOPUP
                {
                    public static LocString TEXT = (LocString)"available Traits:";
                    public class SCROLLAREA
                    {
                        public class CONTENT
                        {
                            public class NOTRAITAVAILABLE
                            {
                                public static LocString LABEL = (LocString)"No Traits available";

                            }
                            public class LISTVIEWENTRYPREFAB
                            {
                                public static LocString LABEL = (LocString)"trait label";
                                public class ADDTHISTRAITBUTTON
                                {
                                    public static LocString TEXT = (LocString)"Add this trait";

                                }
                            }
                        }
                    }
                    public class CANCELBUTTON
                    {
                        public static LocString TEXT = (LocString)"Cancel";
                    }
                }
            }


            public class CGMBUTTON
            {
                public static LocString DESC = (LocString)"Start customizing the currently selected cluster.";
            }
            public class CUSTOMCLUSTERUI
            {
                public static LocString NAMECATEGORIES = (LocString)"Starmap Item Category";
                public static LocString NAMEITEMS = (LocString)"Starmap Items of this category";
                public static class CATEGORYENUM
                {
                    public static LocString START = (LocString)"Start Asteroid";
                    public static LocString WARP = (LocString)"Teleport Asteroid";
                    public static LocString OUTER = (LocString)"Outer Asteroids";
                    public static LocString POI = (LocString)"Points of Interest";
                }

            }

            public class SPACEDESTINATIONS
            {
                public static class CGM_RANDOM_STARTER
                {
                    public static LocString NAME = (LocString)"Random Start Asteroid";
                    public static LocString DESCRIPTION = (LocString)"The starting asteroid will be picked at random";
                }
                public static class CGM_RANDOM_WARP
                {
                    public static LocString NAME = (LocString)"Random Teleporter Asteroid";
                    public static LocString DESCRIPTION = (LocString)"The teleporter asteroid will be picked at random";
                }
                public static class CGM_RANDOM_OUTER
                {
                    public static LocString NAME = (LocString)"Random Outer Asteroid(s)";
                    public static LocString DESCRIPTION = (LocString)"Choose an amount of random outer asteroids.\n\nEach asteroid can only generate once";
                }
                public class CGM_RANDOM_POI
                {
                    public static LocString NAME = "Random POI";
                    public static LocString DESCRIPTION = "Choose an amount of POIs at random.\n\nDoes not roll unique POIs\n(Temporal Tear, Russel's Teapot)";
                }
            }
        }
        public class CLUSTER_NAMES
        {
            public class CGSM
            {
                public static LocString NAME = "CGSM Cluster";
                public static LocString DESCRIPTION = "CGSM Cluster";
            }
        }
    }
}

