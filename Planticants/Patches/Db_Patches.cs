using HarmonyLib;
using Planticants.Content.ModDb;

namespace Planticants.Patches
{
    class Db_Patches
    {

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
                PlantDb.Init(__instance);

			}
        }
    }
}
