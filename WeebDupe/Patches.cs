using HarmonyLib;
using UtilLibs;
using static WeebDupe.ModAssets;

namespace WeebDupe
{
	internal class Patches
	{

		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "hat_role_weeb1");
			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}


		static class AccessoryPatch
		{
			[HarmonyPatch(typeof(Db), "Initialize")]
			public class Db_Initialize_Patch
			{
				public static void Postfix(Db __instance)
				{
					WSkillPerks.Register(__instance.SkillPerks);
					WSkills.Register(__instance.Skills);
					WAccessories.Register(__instance.AccessorySlots, __instance.Accessories);
				}

			}
		}
	}
}
