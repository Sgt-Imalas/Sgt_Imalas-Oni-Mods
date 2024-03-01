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
        [HarmonyPatch(typeof(TemporalTear))]
        [HarmonyPatch(nameof(TemporalTear.ConsumeCraft))]
        public class RegisterTearDupe
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
    }
}
