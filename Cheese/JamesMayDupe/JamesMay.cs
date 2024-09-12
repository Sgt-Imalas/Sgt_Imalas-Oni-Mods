using Cheese.Traits;
using HarmonyLib;
using UtilLibs;

namespace Cheese.JamesMayDupe
{
	internal class JamesMay
	{
		public const string JAMESMAY = "JAMESMAY";
		public static readonly int JAMESMAY_HASH = Hash.SDBMLower(JAMESMAY);

		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		static class Db_Init_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(Db __instance)
			{
				var jams = new Personality(
				JAMESMAY,
				STRINGS.DUPLICANTS.PERSONALITIES.JAMESCHEESE.NAME,
				"Male",
				null,
				"Aggressive",
				CheeseThrower.ID,
				"",
				null,
				3,
				3,
				2,
				3,
				3,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				STRINGS.DUPLICANTS.PERSONALITIES.JAMESCHEESE.DESC,
				false,
				"jams");
				//jams.Disabled = true; // do not show up in menus and such

				__instance.Personalities.Add(jams);
			}
		}
		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			[HarmonyPriority(Priority.LowerThanNormal)]
			public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "dreamIcon_JamesMay");
			}
		}
	}
}
