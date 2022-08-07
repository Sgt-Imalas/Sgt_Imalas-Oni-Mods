using Cryopod.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopod
{
    internal class ModAssets
    {
        public static Components.Cmps<CryopodReusable> CryoPods = new Components.Cmps<CryopodReusable>();
        public const string ForcedCryoThawedID = "CRY_ForcedCryoThawed";
        public class StatusItems
        {
            public static StatusItem DupeName;
            public static StatusItem DupeHealth;
            public static StatusItem CurrentDupeTemperature;
            public static StatusItem CryoDamage;
            public static StatusItem EnergySaverModeCryopod;
            
            public static void Register()
            {
                EnergySaverModeCryopod = new StatusItem(
                      "CRY_DUPLICANTATTEMPERATURE",
                      "BUILDING",
                      "",
                      StatusItem.IconType.Info,
                      NotificationType.Neutral,
                      false,
                      OverlayModes.None.ID
                      );
                DupeName = new StatusItem(
                   "CRY_DuplicantNameStatus",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Info,
                   NotificationType.Neutral,
                   false,
                   OverlayModes.None.ID);

                DupeHealth = new StatusItem(
                   "CRY_DuplicantHealthStatus",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Info,
                   NotificationType.Neutral,
                   false,
                   OverlayModes.None.ID);

                CurrentDupeTemperature = new StatusItem(
                   "CRY_DuplicantInternalTemperature",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Info,
                   NotificationType.Neutral,
                   false,
                   OverlayModes.None.ID
                   );
                CryoDamage = new StatusItem(
                   "CRY_DuplicantCryoDamage",
                   "BUILDING",
                   "",
                   StatusItem.IconType.Info,
                   NotificationType.BadMinor,
                   false,
                   OverlayModes.None.ID
                   );

                DupeName.SetResolveStringCallback((str, obj) =>
                {
                    if (obj is CryopodReusable cryopod)
                    {
                        string dupeName = cryopod.GetDupeName();
                        return str.Replace("{DupeName}", dupeName);
                    }
                    return str;
                });

                DupeHealth.SetResolveStringCallback((str, obj) =>
                {
                    if (obj is CryopodReusable cryopod)
                    {
                        string name = "", tooltip = "";
                        var potentialDamage = cryopod.GetDamage() / 2;
                        //Debug.Log("DAMAGE: " + potentialDamage);
                        if (potentialDamage <= 1f)
                        {
                            name = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHGOODNAME;
                            tooltip = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHGOOD;
                        }
                        if (potentialDamage > 1 && potentialDamage <= 50)
                        {
                            name = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHSOMEDAMAGENAME;
                            tooltip = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHSOMEDAMAGE;
                        }
                        if (potentialDamage > 50 && potentialDamage <= 99)
                        {
                            name = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHMAJORDAMAGENAME;
                            tooltip = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHMAJORDAMAGE;
                        }
                        if (potentialDamage > 99)
                        {
                            name = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHINCAPACITATENAME;
                            tooltip = STRINGS.BUILDING.STATUSITEMS.CRY_DUPLICANTHEALTHSTATUS.HEALTHINCAPACITATE;
                        }

                        if (str.Contains("{DupeHealthState}"))
                            return str.Replace("{DupeHealthState}", name);
                        if (str.Contains("{DupeHealthStateTooltip}"))
                            return str.Replace("{DupeHealthStateTooltip}", tooltip);
                    }
                    return str;
                });
                CurrentDupeTemperature.SetResolveStringCallback((str, obj) =>
                {
                    if (obj is CryopodReusable cryopod)
                    {
                        float dupeTemp = cryopod.InternalTemperatureKelvin;
                        return str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(dupeTemp));
                    }
                    return str;
                });

            }
        }

        public static void UnlockCryopod(int chanceInPercent = 100)
        {
            var frostedResearch = Research.Instance.GetTechInstance(ModAssets.Techs.FrostedDupeResearchID);
            if (!frostedResearch.IsComplete() && SuccessPerChance(chanceInPercent)) { 
                frostedResearch.Purchased();
                Game.Instance.Trigger((int)GameHashes.ResearchComplete, frostedResearch.tech);
            }
        }

        public static bool SuccessPerChance(int chanceOfSuccess)
        {
            var randGen = new Random();
            return randGen.Next(100) < chanceOfSuccess;
        }

        public class Techs
        {
            public static string FrostedDupeResearchID = "FrostedDupeResearch";
        }
    }
}
