using HarmonyLib;

namespace KleiPaintings
{
	internal class Patches
	{
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Prefix()
			{
				SkinDatabase.AddSkins();
			}
		}
	}
}
