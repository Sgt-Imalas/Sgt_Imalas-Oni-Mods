using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGTIM_NotificationManager
{
    internal class STRINGS
    {
        public class NOTIFICATION_CONFIG
        {

            public static LocString MUTE_PING = "Mute notification ping";
            public static LocString MUTE_PING_TOOLTIP = "Enable this option to remove the ping-sound from the warning.";
            public static LocString PAUSE_ON_NOTIFICATION = "Pause game on trigger";
            public static LocString PAUSE_ON_NOTIFICATION_TOOLTIP = "Pauses the game when the notification triggers.";
            public static LocString ZOOM_ON_NOTIFICATION = "Pan to origin";
            public static LocString ZOOM_ON_NOTIFICATION_TOOLTIP = "Moves the camera to the origin of the notification when it triggers.";


            public class STARVATION
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.STARVING.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
                public static LocString THRESHOLD = "Starvation Calorie Threshold";
                public static LocString THRESHOLD_TOOLTIP = "Calorie threshold for the starvation notification.\nDefault is 1000";
            }
            public class ATTACK
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.FIGHTING.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
            public class PEE
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
            public class FLEE
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.FLEEING.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
            public class SUFFOCATION
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.SUFFOCATING.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
                public static LocString THRESHOLD = "Suffocation Time Threshold";
                public static LocString THRESHOLD_TOOLTIP = "Time from notification start to death by suffocation in seconds.\nDefault is 50 seconds, higher values warn earlier";
            }
            public class STRESS
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.STRESSED.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
                public static LocString THRESHOLD = "Stress Threshold";
                public static LocString THRESHOLD_TOOLTIP = "Stress level where the notification triggers in percent.\nDefault is 80%";
            }
            public class RADIATIONVOMITING
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
                //public static LocString THRESHOLD = "Ra Threshold";
                //public static LocString THRESHOLD_TOOLTIP = "Stress level where the notification triggers in percent.\nDefault is 80%";
            }
            public class SCALDING
            {
                public static LocString CATEGORY = global::STRINGS.CREATURES.STATUSITEMS.SCALDING.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
            public class ENTOMBED
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.ENTOMBEDCHORE.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
            public class INCAPACITATED
            {
                public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.INCAPACITATED.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
            public class CEILINGCOLLAPSE
            {
                //public static LocString CATEGORY = global::STRINGS.DUPLICANTS.STATUSITEMS.INCAPACITATED.NOTIFICATION_NAME + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
            public class PLANTDEATH
            {
                public static LocString CATEGORY = global::STRINGS.CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION + "-" + global::STRINGS.BUILDINGS.PREFABS.LOGICALARM.LOGIC_PORT;
            }
        }
    }
}
