using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.TwitchEvents
{
    internal class TwitchIntegrationPatch
    {
        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                TwitchEventsInit.RegisterAll();
            }
        }
    }
}
