using HarmonyLib;
using Planticants.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planticants.Patches
{
    class Amounts_Patches
    {

        [HarmonyPatch(typeof(Database.Amounts), nameof(Database.Amounts.Load))]
        public class Amounts_Load_Patch
        {
            public static void Postfix(Database.Amounts __instance)
            {
				PlantAmounts.RegisterAmounts(__instance);
			}
        }
    }
}   
