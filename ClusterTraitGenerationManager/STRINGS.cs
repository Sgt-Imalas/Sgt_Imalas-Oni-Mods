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
