using HarmonyLib;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Patches
{
	class CharacterContainer_Patches
	{


		[HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.GetIdleAnim))]
		public class CharacterContainer_GetIdleAnim_Patch
		{
			public static void Postfix(CharacterContainer __instance, ref HashedString __result)
			{
				if (__instance.Stats.personality.model == ModAssets.Tags.AquaticMinion)
					__result = "character_select_swim_kanim";
			}
		}

		[HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.OnSpawn))]
		public class CharacterContainer_OnSpawn_Patch
		{
			public static void Prefix(CharacterContainer __instance)
			{
				if (!__instance.allMinionModels.Contains(Tags.AquaticMinion))
					__instance.allMinionModels.Add(Tags.AquaticMinion);

				///default select also only has bionics + normals
				if (!__instance.permittedModels.Contains(Tags.AquaticMinion) && __instance.permittedModels.Count > 1)
					__instance.permittedModels.Add(Tags.AquaticMinion);

				CharacterContainer.portraitBGAnimsByModel[Tags.AquaticMinion] = new()
				{
					animFileName = "crewselect_backdrop_aquatic_swim_kanim",
					//hasPreAnim = true,
					foregroundAnimFileName = "crewselect_backdrop_swim_fg_kanim",
				};
			}

		}

		[HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.SetReshufflingState))]
		public class CharacterContainer_SetReshufflingState_Patch
		{
			[HarmonyPriority(Priority.Low)]
			public static void Postfix(CharacterContainer __instance, bool enable)
			{
				if (!enable || Game.IsDlcActiveForCurrentSave(DlcManager.DLC3_ID) || !Game.IsDlcActiveForCurrentSave(DlcManager.DLC5_ID))
					return;

				__instance.modelDropDown.transform.parent.gameObject.SetActive(true);
			}
		}

		[HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.IsCharacterInvalid))]
		public class CharacterContainer_IsCharacterInvalid_Patch
		{
			public static void Postfix(CharacterContainer __instance, ref bool __result)
			{
				if (__result || Game.IsDlcActiveForCurrentSave(DlcManager.DLC5_ID))
					return;

				if (__instance.Stats.personality.model == ModAssets.Tags.AquaticMinion)
					__result = true;
			}
		}
	}
}
