using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig
{
    class LocalisationPatch
    {
        /// <summary>
        /// Initializes Localisation for modded strings
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
                LocalisationUtil.FixRoomConstrains();
            }
        }
    }
}
