using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopod
{
    class STRINGS
    {
        public class BUILDING
        {
            public class STATUSITEMS
            {
                public class CRY_DUPLICANTINTERNALTEMPERATURE
                {
                    public static LocString NAME = "The Dupe is at {InternalTemperature}.";
                    public static LocString TOOLTIP = "Cryogenic process cools down the body for preservation.";
                }
                public class CRY_DUPLICANTNAMESTATUS
                {
                    public static LocString NAME = "Duplicant in cryogenic sleep: {DupeName}";
                    public static LocString TOOLTIP = "{DupeName} takes a cool nap.";
                }
            }
        }
    }
}
