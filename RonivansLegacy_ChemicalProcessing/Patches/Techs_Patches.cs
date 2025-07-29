using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Techs_Patches
    {
        [HarmonyPatch(typeof(Database.Techs), nameof(Database.Techs.Init))]
        public class Database_Techs_Init_Patch
        {
            public static void Postfix(Database.Techs __instance)
            {
				ModTechs.RegisterTechs(__instance);
            }
        }
    }
}
