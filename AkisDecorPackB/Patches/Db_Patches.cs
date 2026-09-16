using AkisDecorPackB.Content.ModDb;
using HarmonyLib;

namespace AkisDecorPackB.Patches
{
	internal class Db_Patches
	{

		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix(Db __instance)
			{
				Mod_Db.PostDbInit(__instance);
				ModAssets.PostDbInit();
			}
		}
	}
}
