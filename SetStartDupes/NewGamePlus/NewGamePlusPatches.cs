using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SetStartDupes.NewGamePlus
{
    internal class NewGamePlusPatches
    {
        /// <summary>
        /// collect Spaced Out Temporal Tear dupes
        /// </summary>
        [HarmonyPatch(typeof(TemporalTear), nameof(TemporalTear.ConsumeCraft))]
        public class RegisterTearDupeDLC
        {
            public static void Prefix(Clustercraft craft, TemporalTear __instance)
            {
                if (!__instance.m_open || craft.Location != __instance.Location || craft.IsFlightInProgress())
                    return;


                int rocketWorldId = craft.ModuleInterface.GetInteriorWorld().id;
                for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
                {
                    MinionIdentity minionIdentity = Components.LiveMinionIdentities[idx];
                    if (minionIdentity.GetMyWorldId() == rocketWorldId)
                    {
                        SgtLogger.l("Tear Dupe detected, creating an image...");
                        MinionStatConfig.RegisterTearDuplicant(minionIdentity);
                    }
                }
            }
        }
        
        /// <summary>
        /// collect base game Temporal Tear dupe
        /// </summary>
        [HarmonyPatch(typeof(Spacecraft), nameof(Spacecraft.TemporallyTear))]
        public class RegisterTearDupe
        {
            public static void Prefix(Spacecraft __instance)
            {
                LaunchConditionManager launchConditions = __instance.launchConditions;
                for (int num = launchConditions.rocketModules.Count - 1; num >= 0; num--)
                {
                    if (launchConditions.rocketModules[num].TryGetComponent<MinionStorage>(out var minionstorage))
                    {
                        List<MinionStorage.Info> storedMinionInfo = minionstorage.GetStoredMinionInfo();
                        for (int num2 = storedMinionInfo.Count - 1; num2 >= 0; num2--)
                        {
                            SgtLogger.l("Tear Dupe detected, creating an image...");
                            storedMinionInfo[num2].serializedMinion.Get().TryGetComponent<StoredMinionIdentity>(out var identity);
                            MinionStatConfig.RegisterTearDuplicant(identity);
                        }
                    }
                }
            }
        }

    }
}
