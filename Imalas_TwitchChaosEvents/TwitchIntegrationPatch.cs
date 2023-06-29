using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imalas_TwitchChaosEvents
{
    internal class TwitchIntegrationPatch
    {
        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                Util_TwitchIntegrationLib.EventRegistration.InitializeTwitchEventsInNameSpace("Imalas_TwitchChaosEvents.Events");
            }
        }
    }
}
