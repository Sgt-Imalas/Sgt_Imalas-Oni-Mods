using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopod
{
    class STRINGS
    {
        public class DUPLICANTS
        {
            public class STATUSITEMS 
            { 
                public class FORCETHAWED
                {
                    public static LocString NAME = "Cryo Sickness";
                    public static LocString TOOLTIP = "Being forcefully thawed, this Duplicant does not have a good time.";
                }
            }
        }

        public class BUILDINGS
        {
            public class PREFABS
            {
                public class CRY_BUILDABLECRYOTANK
                {
                    public static LocString NAME = "Cryotank 3001";
                    public static LocString DESC = (LocString)"A design found in an ancient facility, your Duplicants have managed to almost perfectly replicate it.";
                    public static LocString EFFECT = (LocString)"Can store 1 Duplicant.\n\nWill generate a lot of heat during cooling.\nWill draw a lot of heat from the surrounding area during thawing.";
                    public static LocString DEFROSTBUTTON = (LocString)"Defrost stored Friend";
                    public static LocString DEFROSTBUTTONCANCEL = (LocString)"Cancel Defrosting";
                    public static LocString DEFROSTBUTTONTOOLTIP = (LocString)"Unfreeze the duplicant stored here.";
                }
            }
        }
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
                public class CRY_DUPLICANTCRYODAMAGE
                {
                    public static LocString NAME = "Warning, Duplicant is thawing improperly";
                    public static LocString TOOLTIP = "When this duplicant thaws fully, it won't have a good time.";
                }
                public class CRY_DUPLICANTATTEMPERATURE
                {
                    public static LocString NAME = "Energy Saving Mode";
                    public static LocString TOOLTIP = "Fully cooled down, this buiding has entered energy saving mode.";
                }
            }
        }
        public class DISEASES
        {
            public class CRYOSICKNESS
            {
                public static LocString NAME = (LocString)UI.FormatAsLink("Cryo Sickness", nameof(CRYOSICKNESS));
                public static LocString DESCRIPTIVE_SYMPTOMS = (LocString)"A lot of cells got pierced by a non standard cryopod thawing.";
                public static LocString DESCRIPTION = (LocString)"After a botched thawing process, this dupe will take time to feel normal again.";
            }
        }
    }
}
