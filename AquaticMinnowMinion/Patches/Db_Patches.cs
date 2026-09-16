using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;

namespace AquaticMinnowMinion.Patches
{
	class Db_Patches
	{

		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				Aq_Db.Init(__instance);
			}
		}
	}
}
