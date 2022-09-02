using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SgtImalasUtilityLib;

namespace Rockets_TinyYetBig
{
    class LocalisationPatch
    {
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                ImalasLocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
    }
}
