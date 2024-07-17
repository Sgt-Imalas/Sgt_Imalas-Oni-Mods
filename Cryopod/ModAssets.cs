using Cryopod.Buildings;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public static class Thawing
        {
            //public static void TransferToFrozen(CryopodReusable from, ref frozenDupe to)
            //{
            //    if(from.GetDupeName()!= "No duplicant stored.") { 
            //        Debug.Log(from + " and " + to);
            //        to.SetMinionInStorage(from.GetStoredDupe());
            //        to.SetStoredSicknesses(from.StoredSicknessIDs);
            //        to.SetDamageValue(from.storedDupeDamage);
            //        to.SetCryoDamageValue(from.GetDamage());
            //        from.DeleteDupeFromStorage(from.GetStoredDupe().First().id);
            //    }
            //}
            public static void HandleDupeThawing(ref GameObject dupe, ref List<string> storedSicknesses, ref float storedDamage, ref float cryoDamage)
            {
                //SicknessExposureInfo cold = new SicknessExposureInfo(ColdBrain.ID, "Frozen within self made cryopod.");
                //dupeModifiers.sicknesses.Infect(cold);
                //var coldMonitor = dupe.GetSMI<ColdImmunityMonitor.Instance>();
                //if (coldMonitor != null)
                //{
                //    coldMonitor.sm.coldCountdown.Set(0, coldMonitor);
                //    coldMonitor.GoTo(coldMonitor.sm.cold);
                //}


                var dupeModifiers = dupe.GetComponent<MinionModifiers>();

                if (storedSicknesses.Count > 0)
                {
                    foreach (var sickness in storedSicknesses)
                    {
                        if(Db.Get().Sicknesses.Get(sickness)!=null)
                            dupe.GetComponent<MinionModifiers>().sicknesses.Infect(new SicknessExposureInfo(sickness, "Got frozen with the disease"));
                    }
                    storedSicknesses.Clear();
                }
                if (storedDamage != -1f)
                {
                    dupe.GetComponent<Health>().Damage(storedDamage);
                    storedDamage = -1f;
                }
                if (cryoDamage > 0)
                {
                    HandleCryoDamage(dupe, cryoDamage);
                    cryoDamage = 0;
                }
            }

            private static void HandleCryoDamage(GameObject dupe, float dmgVal)
            {
                var helf = dupe.GetComponent<Health>();
                var doDamage = dmgVal / 2;

                Effect cryoSickness = new Effect(
                     ModAssets.ForcedCryoThawedID,
                     STRINGS.DUPLICANTS.STATUSITEMS.FORCETHAWED.NAME,
                     STRINGS.DUPLICANTS.STATUSITEMS.FORCETHAWED.TOOLTIP,
                     120f,
                     true,
                     true,
                     true);

                var debuffs = new List<AttributeModifier>();
                var debuffStrength = -(dmgVal / 16) <= -1f ? -(dmgVal / 16) : -1;

                debuffs.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, debuffStrength));
                debuffs.Add(new AttributeModifier(Db.Get().Attributes.Strength.Id, debuffStrength));
                debuffs.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, debuffStrength));
                debuffs.Add(new AttributeModifier(Db.Get().Attributes.Construction.Id, debuffStrength));
                debuffs.Add(new AttributeModifier(Db.Get().Attributes.Machinery.Id, debuffStrength));
                debuffs.Add(new AttributeModifier(Db.Get().Attributes.Caring.Id, debuffStrength));
                debuffs.Add(new AttributeModifier(Db.Get().Attributes.Ranching.Id, debuffStrength));
                //debuffs.Add(new AttributeModifier("AirConsumptionRate", 7.5f));

                if (dmgVal > 80)
                {
                    debuffs.Add(new AttributeModifier(Db.Get().Attributes.Art.Id, debuffStrength));
                    debuffs.Add(new AttributeModifier("SPACENAVIGATION", debuffStrength));
                    debuffs.Add(new AttributeModifier(Db.Get().Attributes.Learning.Id, debuffStrength));
                    debuffs.Add(new AttributeModifier(Db.Get().Attributes.Cooking.Id, debuffStrength));
                    debuffs.Add(new AttributeModifier(Db.Get().Attributes.Botanist.Id, debuffStrength));
                }
                debuffs.Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, (5f + (-debuffStrength)) / 600, STRINGS.DISEASES.CRYOSICKNESS.NAME));
                cryoSickness.duration = (60 * dmgVal);
                cryoSickness.SelfModifiers = debuffs;
                //helf.Damage(doDamage);
                GameScheduler.Instance.Schedule("applyDamageToDupe", 1, (obj) => helf.Damage(doDamage), null);
                //helf.StartCoroutine(KillOnEndEditRoutine(helf, doDamage));
                dupe.GetComponent<Effects>().Add(cryoSickness, true);
            }
            private static IEnumerator KillOnEndEditRoutine(Health helf, float dmg)
            {
                yield return (object)new WaitForEndOfFrame();
                yield return (object)new WaitForEndOfFrame();
                helf.Damage(dmg);
            }

        }

        public static bool SuccessPerChance(int chanceOfSuccess)
        {
            var randGen = new System.Random();
            return randGen.Next(100) < chanceOfSuccess;
        }

        public class Techs
        {
            public static string FrostedDupeResearchID = "FrostedDupeResearch";
        }
    }
}
