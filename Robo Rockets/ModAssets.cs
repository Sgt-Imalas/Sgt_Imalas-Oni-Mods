using KnastoronOniMods;
using RoboRockets.LearningBrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static RoboRockets.STRINGS.BUILDINGS.PREFABS;
using static RoboRockets.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;

namespace RoboRockets
{
    public class ModAssets
    {
        public class Techs
        {
            public static string AiBrainsTech = "RR_BrainModuleTech";
        }

        public static class Tags
        {
            public static Tag SpaceBrain = TagManager.Create("RR_SpaceBrainFlyer");
        }
        public static Dictionary<int, AIPassengerModule> ForbiddenInteriorIDs = new Dictionary<int, AIPassengerModule>();

        public static StatusItem ExperienceLevel;
        public static StatusItem NoBrain;
        public static StatusItem ExperienceLevelInsidePod;
        public static void RegisterStatusItems()
        {
            ExperienceLevel = new StatusItem(
                   "RR_BrainExperience",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Info,
                   NotificationType.Neutral,
                   false,
                   OverlayModes.None.ID);
            NoBrain = new StatusItem(
                   "RR_NOBRAIN",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Exclamation,
                   NotificationType.Bad,
                   false,
                   OverlayModes.None.ID);


            ExperienceLevel.SetResolveStringCallback((str, obj) =>
            {
                if (obj is FlyingBrain brain)
                {
                    float learnedSpeed = brain.GetCurrentSpeed();
                    string ExpDesc;
                    string brainName = brain.nameable.savedName == RR_BRAINFLYER.NAME ? (string)STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.UNNAMEDBRAIN : brain.nameable.savedName;

                    if (learnedSpeed < 1.0f)
                    {
                        ExpDesc = STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.LVL1;
                    }
                    else if (learnedSpeed < 1.25f)
                    {
                        ExpDesc = STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.LVL2;
                    }
                    else if (learnedSpeed < 1.5f)
                    {
                        ExpDesc = STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.LVL3;
                    }
                    else if (learnedSpeed < 1.75f)
                    {
                        ExpDesc = STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.LVL4;
                    }
                    else if (learnedSpeed < 2.0f)
                    {
                        ExpDesc = STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.LVL5;
                    }
                    else
                    {
                        ExpDesc = STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.LVL6;
                    }
                    var speedString = (learnedSpeed - 1f) * 100f;

                    str = str.Replace("{BRAINNAME}", brainName);
                    str = str.Replace("{BRAINBOOST}", speedString.ToString());
                    return str.Replace("{BRAINXPSTATE}", ExpDesc);
                }
                else if( obj is float defaultSpeedModifier )
                {
                    str = str.Replace("{BRAINNAME}", STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.NOBRAIN);
                    var speedString = (defaultSpeedModifier - 1f) * 100f;
                    str = str.Replace("{BRAINBOOST}", speedString.ToString());
                    return str.Replace("{BRAINXPSTATE}", STRINGS.BUILDING.STATUSITEMS.RR_BRAINEXPERIENCE.LVLNONE);

                }
                return str;
            });

        }
    }
}
