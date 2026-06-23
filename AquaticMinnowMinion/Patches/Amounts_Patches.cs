using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaticMinnowMinion.Patches
{
    class Amounts_Patches
    {

        [HarmonyPatch(typeof(Database.Amounts), nameof(Database.Amounts.Load))]
        public class Amounts_Load_Patch
        {
            public static void Postfix(Database.Amounts __instance)
            {
				Aq_Amounts.RegisterAmounts(__instance);
			}
        }
    }
}   
