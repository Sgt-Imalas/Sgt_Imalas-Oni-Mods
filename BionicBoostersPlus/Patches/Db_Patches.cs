using BionicBoostersPlus.Content.ModDb;
using HarmonyLib;

namespace BionicBoostersPlus.Patches
{
	class Db_Patches
	{

		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				BB_Db.Init(__instance);
			}
		}
	}
}
