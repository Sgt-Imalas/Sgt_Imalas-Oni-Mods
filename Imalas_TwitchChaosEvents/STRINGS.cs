using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Imalas_TwitchChaosEvents.STRINGS.ELEMENTS;
using static STRINGS.UI ;

namespace Imalas_TwitchChaosEvents
{
    public class STRINGS
    {
        public class CREATURES
        {
            public class SPECIES
            {
                public class ITCE_CREEPEREATER
                {
                    public static LocString NAME = FormatAsLink("Green Friend", nameof(ITCE_CREEPEREATER));
                    public static LocString DESC = "A green friend that LOVES to consume the Creeper";
                }
            }
        }
        public class ITEMS
        {
            public class FOOD
            {
                public class ICT_TACO_NONSPOILING
                {
                    public static LocString NAME = FormatAsLink("Space Taco", nameof(ICT_TACO_NONSPOILING));
                    public static LocString DESC = "A staple meal that provides vital nutrients and energy to those who consume it.\nFallen from the sky, it does not spoil for some reason.";
                }
                public class ICT_TACO
                {
                    public static LocString NAME = FormatAsLink("Taco", nameof(ICT_TACO));
                    public static LocString DESC = "A staple meal that provides vital nutrients and energy to those who consume it.";
                }
            }
            public class ICT_GHOSTTACO
            {
                public static LocString NAME = FormatAsLink("Ghostly Taco", nameof(ICT_GHOSTTACO));
                public static LocString DESC = "A semitranslucent image of a delicious Taco.\nSoon it will fade away";
            }
            public class ICT_OMEGASAWBLADE
            {
                public static LocString NAME = FormatAsLink("Omega Sawblade", nameof(ICT_OMEGASAWBLADE));
                public static LocString DESC = "Bzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz.\nHomes onto living beings and the mouse cursor.";
            }
        }
        public class COMETS
        {
            public class ITC_TACOCOMET
            {
                public static LocString NAME = FormatAsLink("Flying Taco", nameof(ITC_TACOCOMET));
                public static LocString DESC = "A flying taco, it looks delicious!";
            }
            public class ITC_GHOSTTACOCOMET
            {
                public static LocString NAME = FormatAsLink("Ghostly Flying Taco", nameof(ITC_TACOCOMET));
                public static LocString DESC = "A semitranslucent image of flying taco, it still looks delicious!";
            }

        }

        public class ELEMENTS
        {
            public class ITCE_INVERSE_ICE
            {
                public static LocString NAME = FormatAsLink("<rotate=180>Ice</rotate>", nameof(ITCE_INVERSE_ICE));
                public static LocString DESC = "weird Ice...";
            }
            public class ITCE_INVERSE_WATER
            {
                public static LocString NAME = FormatAsLink("<rotate=180>Water</rotate>", nameof(ITCE_INVERSE_WATER));
                public static LocString DESC = "weird Water...";
            }
            public class ITCE_INVERSE_STEAM
            {
                public static LocString NAME = FormatAsLink("<rotate=180>Steam</rotate>", nameof(ITCE_INVERSE_STEAM));
                public static LocString DESC = "weird Steam...";
            }
            public class ITCE_CREEPYLIQUID
            {
                public static LocString NAME = FormatAsLink("Creeper", nameof(ITCE_CREEPYLIQUID));
                public static LocString DESC = "The Creeper.\nEverly expanding\nWeak to the cold.\nWeak to Pressure";
            }
            public class ITCE_CREEPYLIQUIDGAS
            {
                public static LocString NAME = FormatAsLink("Creeper Tendril", nameof(ITCE_CREEPYLIQUIDGAS));
                public static LocString DESC = "the ever extending tendril of the creeper";
            }
            
        }

        public class CHAOSEVENTS
        {
            public class HUNGRYROACHES
            {
                public static LocString NAME = "Roach Infestation";
                public static LocString TOAST = "Roach Infestation!";
                //public static LocString TOASTTEXT = "Hungry Roaches have taken a liking to your food supply.";
                public static LocString TOASTTEXT = 
                    "Your kitchen fridge, it holds some pastry\n" +
                    "the roaches come, it looks them tasty!" +
                    "\n\nHungry roaches ate 50% of your food supply on {0}.";
                public static LocString TOASTTEXTALTERNATIVE =
                    "Hungry roaches have taken a liking to your food supply on {0}.";
                public static LocString EVENTFAIL =
                    "Not enough food was available to attract any roaches.";
            }
            public class RAINBOWLIQUIDS
            {
                public static LocString NAME = "Rainbow Rave";
                public static LocString TOAST = "Rainbow Rave";
                public static LocString TOASTTEXT = "All Liquids are throwing a rainbow party!";
            }
            public class INVERSEELEMENT
            {
                public static LocString NAME = "Doolf";
                public static LocString TOAST = "Doolf!";
                public static LocString TOASTTEXT = "<rotate=180>Something is off with this...</rotate>";
            }
            public class FOG
            {
                public static LocString NAME = "Fog";
                public static LocString TOAST = "Fog";
                public static LocString TOASTTEXT = "Bloody hell, it's foggy innit";

                public static LocString TOASTTEXTENDING = "The fog is starting to clear.";
            }
            public class CREEPEREATINGBOI
            {
                public static LocString NAME = "Green Boi";
                public static LocString TOAST = "Green Boi";
                public static LocString TOASTTEXT = "Hi!";
            }
            public class BUZZSAW
            {
                public static LocString NAME = "Omega Sawblade";
                public static LocString TOAST = "Omega Sawblade";
                public static LocString TOASTTEXT = "Keep that cursor dancing :]";
            }
            public class BURNINGCURSOR
            {
                public static LocString NAME = "Burning Cursor";
                public static LocString TOAST = "Burning Cursor";
                public static LocString TOASTTEXT = "The mouse cursor has caught fire!";
                public static LocString TOASTTEXTENDING = "The mouse cursor has been extinguished";
            }
            public class CREEPERRAIN
            {
                public static LocString NAME = "Creeper Rain";
                public static LocString TOAST = "Creeper Rain";
                public static LocString TOASTTEXT = "ALARM!\nThe Creeper is about to arrive on {0}";
                public static LocString TOASTTEXT2 = "The Creeper has arrived on {0}!";
            }
            public class TACORAIN
            {
                public static LocString NAME = "Taco Rain";
                public static LocString TOAST = "Taco Rain!";
                public static LocString TOASTTEXT = "It's raining Tacos!";
                public static LocString NEWRECIPE = "\n\nThere is also a new recipe in the Gas Range.";

            }
        }
        public class HOTKEYACTIONS
        {
            public static LocString TRIGGER_FAKE_TACORAIN_NAME = "Trigger a ghostly Taco Rain";
            public static LocString UNLOCK_TACO_RECIPE = "Manually unlock Taco Recipe";
            public static LocString UNLOCK_TACO_RECIPE_TITLE = "Tacos!";
            public static LocString UNLOCK_TACO_RECIPE_BODY = "The Taco recipe has been unlocked in the Gas Range";

        }
        public class CHAOS_CONFIG
        {

            public static LocString TACORAIN_MUSIC_NAME = "Music on Taco Rain Event";
            public static LocString TACORAIN_MUSIC_TOOLTIP = "During the twitch event \"Taco Rain\", the song \"Raining Tacos - Parry Gripp & BooneBum\" gets played.\nDisable this option here to mute it.";

            public static LocString FAKE_TACORAIN_MUSIC_NAME = "Music on triggerable ghostly Taco Rain";
            public static LocString FAKE_TACORAIN_MUSIC_TOOLTIP = "During the triggerable Taco Rain the song \"Raining Tacos - Parry Gripp & BooneBum\" gets played.\nDisable this option here to mute it.";

            public static LocString FAKE_TACORAIN_DURATION_NAME = "triggerable ghostly Taco Rain duration in s";
            public static LocString FAKE_TACORAIN_DURATION_TOOLTIP = "How long should the triggerable Taco Rain last (in seconds).";
        }
    }
}
