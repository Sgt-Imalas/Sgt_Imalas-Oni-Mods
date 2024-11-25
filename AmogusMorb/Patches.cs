
using HarmonyLib;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;


namespace AmogusMorb
{
	internal class Patches
	{
		[HarmonyPatch(typeof(GlomConfig))]
		[HarmonyPatch(nameof(GlomConfig.CreatePrefab))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Postfix(GameObject __result)
			{
				string amogusAnimName = Config.Instance.SussyPlus ? "amorbus_sus_kanim" : "amorbus_sus_old_kanim";

				KBatchedAnimController kBatchedAnimController = __result.AddOrGet<KBatchedAnimController>();
				kBatchedAnimController.AnimFiles = new KAnimFile[1] { Assets.GetAnim(amogusAnimName) };
				SoundUtils.CopySoundsToAnim(amogusAnimName, "glom_kanim");
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
				CREATURES.FAMILY.GLOM = UI.FormatAsLink("Amorbus", "GLOMSPECIES");
				CREATURES.FAMILY_PLURAL.GLOMSPECIES = UI.FormatAsLink("Amorbi", nameof(CREATURES.FAMILY_PLURAL.GLOMSPECIES));
				CREATURES.SPECIES.GLOM.NAME = UI.FormatAsLink("Amorbus", nameof(CREATURES.SPECIES.GLOM));
				CREATURES.SPECIES.GLOM.DESC = "When the Imposter is Sus?!?\n\n" + CREATURES.SPECIES.GLOM.DESC;
			}
		}
		[HarmonyPatch(typeof(Db), "Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				if (DecorPackA_ModAPI.TryInitialize(true))
				{
					DecorPackA_ModAPI.AddMoodLamp(
						"AM_SittingMogus",
						STRINGS.MOODLAMPSKINS.SITTINGMOGUS,
						"customizable",
						"moodlamp_amogus__sitting_kanim",
						Color.white,
						KAnim.PlayMode.Paused,

						new HashSet<HashedString>() { "Tintable" });
					DecorPackA_ModAPI.AddMoodLamp(
						"AM_StickbuggedLoL",
						STRINGS.MOODLAMPSKINS.STICKBUG,
						"",
						"moodlamp_stickbug_kanim",
						UIUtils.rgb(243, 241, 196),
						KAnim.PlayMode.Loop,
						new HashSet<HashedString>() { });

					//DecorPackA_ModAPI.AddComponentToLampPrefab(typeof(TestMoodlampBehavior));
				}
			}
		}
	}
}
