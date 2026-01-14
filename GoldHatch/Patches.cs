using GoldHatch.Creatures;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace GoldHatch
{
	internal class Patches
	{
		///Use the following patch to add any custom interact anims;
		[HarmonyPatch(typeof(KAnimGroupFile), "Load")]
		public class KAnimGroupFile_Load_Patch
		{
			public static void Prefix(KAnimGroupFile __instance)
			{
				//var converterSettings = new JsonSerializerSettings()
				//{
				//	ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				//	TypeNameHandling = TypeNameHandling.Objects,
				//	Formatting = Formatting.Indented,
				//	ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
				//	ContractResolver = new InjectionMethods.IncludePrivateContractResolver()
				//};
				InjectionMethods.MoveKanimsToBatchGroupOf(
					__instance,
					new HashSet<HashedString>()
					{
						"hatch_gold_build_kanim"
					},
					"hatch_kanim");
			}
		}
	}

	/// <summary>
	/// add modifier increase for hatches when eating lead
	/// </summary>
	[HarmonyPatch(typeof(ModifierSet), nameof(ModifierSet.LoadFertilityModifiers))]
	public static class ModifierSet_LoadFertilityModifiers
	{
		public static void Prefix()
		{
			TUNING.CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS
				.Add(TUNING.CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier(HatchGoldConfig.ID, HatchGoldConfig.EGG_ID, SimHashes.Lead.CreateTag(), 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE));
		}
	}
	/// <summary>
	/// add lead to stone and metal hatch diet
	/// </summary>
	[HarmonyPatch(typeof(BaseHatchConfig), nameof(BaseHatchConfig.MetalDiet))]
	public static class BaseHatchConfig_DietPatches
	{

		[HarmonyPostfix]
		public static void Postfix(List<Diet.Info> __result,
			Tag poopTag,
			float caloriesPerKg,
			float producedConversionRate,
			string diseaseId,
			float diseasePerKgProduced)
		{

			if (!__result.Any(entry => entry.consumedTags.Contains(SimHashes.Lead.CreateTag())))
			{
				__result.Add(new Diet.Info(new HashSet<Tag>()
				{SimHashes.Lead.CreateTag()},
				SimHashes.Lead.CreateTag(),
				caloriesPerKg,
				producedConversionRate,
				diseaseId,
				diseasePerKgProduced));
			}
			else
			{
				SgtLogger.l("lead already in list");
			}
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
}

