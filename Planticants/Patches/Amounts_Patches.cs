using HarmonyLib;
using Planticants.Content.ModDb;

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
