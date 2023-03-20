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
                            public class INPUT
                            {
                                public static LocString TEXT = (LocString)"";

                            }
                        }
                    }
                    public class ASTEROIDSIZEINFO
                    {
                        public static LocString LABEL = (LocString)"Asteroid Size:";
                        public static LocString INFO = (LocString)"{0}x{1}";
                        public class INPUT
                        {
                            public static LocString TEXT = (LocString)"";

                        }
                    }
                    public class CLUSTERSIZE
                    {
                        public class DESCRIPTOR
                        {
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
                                public static LocString LABEL = (LocString)"Select Traits";
                                public class DROPDOWN
                                {
                                    public class VIEWPORT
                                    {
                                        public class CONTENT
                                        {
                                            public class ITEM
                                            {
                                                public static LocString LABEL = (LocString)"Option N";

                                            }

                                        }
                                    }
                                }
                            }
                            public class LISTVIEWENTRYPREFAB
                            {
                                public static LocString LABEL = (LocString)"Trait";

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
                        }
                        public class RESETSELECTIONBUTTON
                        {
                            public static LocString TEXT = (LocString)"Reset Current Selection";
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


                public static class CUSTOMCLUSTERCONFIG
                {
                    public static LocString SHOW = (LocString)"Show Cluster Config";
                    public static LocString HIDE = (LocString)"Hide Cluster Config";
                }
                public static class RESET
                {
                    public static LocString NAME = (LocString)"Reset all";
                    public static LocString DESC = (LocString)"Undo all changes you have made by reloading the cluster preset.";
                }


                public static class ENABLED
                {
                    public static LocString NAME = (LocString)"Enabled: ";
                    public static LocString DESC = (LocString)"Should this asteroid/POI be generated at all?";
                }
                public static class NUMBERS
                {
                    public static LocString NAME = (LocString)"Amount: ";
                    public static LocString DESC = (LocString)"How many instances of these should be generated.\nValues that arent full numbers represent a chance to generate for POIs.\n(f.e. 0.8 = 80% chance to generate this POI)";
                }
                public static class MINRINGS
                {
                    public static LocString NAME = (LocString)"Minimum Distance: ";
                    public static LocString DESC = (LocString)"The minimum distance this asteroid has to the center of the starmap.";
                }
                public static class MAXRINGS
                {
                    public static LocString NAME = (LocString)"Maximum Distance: ";
                    public static LocString DESC = (LocString)"The maximum distance this asteroid has to the center of the starmap.";
                }
                public static class BUFFER
                {
                    public static LocString NAME = (LocString)"Buffer Distance: ";
                    public static LocString DESC = (LocString)"The minimum distance this asteroid has to other asteroids.";
                }
                public static class MAPSIZE
                {
                    public static LocString NAME = (LocString)"Starmap Radius: ";
                    public static LocString DESC = (LocString)"The radius of the starmap.";
                }
                public static class PLANETSIZE
                {
                    public static LocString NAME = (LocString)"Asteroid Size: {0} x {1}";
                    public static LocString DESC = (LocString)"The dimensions of this asteroid.";
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

