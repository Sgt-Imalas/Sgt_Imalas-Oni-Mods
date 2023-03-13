using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTraitGenerationManager
{
    internal class STRINGS
    {
        public class UI
        {
            public class CUSTOMCLUSTERUI
            {
                public static LocString NAMECATEGORIES = (LocString)"Starmap Item Category";
                public static LocString NAMEITEMS = (LocString)"Starmap Items of this category";

               
                
                public static class CUSTOMCLUSTERCONFIG
                {
                    public static LocString SHOW = (LocString)"Show Cluster Config";
                    public static LocString HIDE = (LocString)"Hide Cluster Config";
                }
                public static class ENABLED
                {
                    public static LocString NAME = (LocString)"Enabled: ";
                    public static LocString DESC = (LocString)"Should this planet/POI be spawned at all";
                }
                public static class NUMBERS
                {
                    public static LocString NAME = (LocString)"Amount: ";
                    public static LocString DESC = (LocString)"How many of these should be spawned.\nValues that arent full numbers represent a chance to spawn for POIs.\n(0.8 = 80% chance to spawn this POI)";
                }
                public static class MINRINGS
                {
                    public static LocString NAME = (LocString)"Minimum Distance: ";
                    public static LocString DESC = (LocString)"The minimum distance this planet has to the center of the starmap.";
                }
                public static class MAXRINGS
                {
                    public static LocString NAME = (LocString)"Maximum Distance: ";
                    public static LocString DESC = (LocString)"The maximum distance this planet has to the center of the starmap.";
                }
                public static class BUFFER
                {
                    public static LocString NAME = (LocString)"Buffer Distance: ";
                    public static LocString DESC = (LocString)"The minimum distance this planet has to other planets.";
                }
                public static class MAPSIZE
                {
                    public static LocString NAME = (LocString)"Starmap Size: ";
                    public static LocString DESC = (LocString)"The radius of the starmap.";
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
                    public static LocString DESCRIPTION = (LocString)"Choose an amount of random outer asteroids.\n\nEach asteroid can only spawn once";
                }
                public class CGM_RANDOM_POI
                {
                    public static LocString NAME = "Random POI";
                    public static LocString DESCRIPTION = "Choose an amount of POIs at random.";
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
