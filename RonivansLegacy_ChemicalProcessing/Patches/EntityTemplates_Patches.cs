using HarmonyLib;
using PeterHan.PLib.Core;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.CodexInfoDummies;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class EntityTemplates_Patches
	{
		//attempt at generic recipe generation for expeller press...

		//[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendPlantToFertilizable), [typeof(GameObject), typeof(PlantElementAbsorber.ConsumeInfo[])])]
		//public class EntityTemplates_ExtendPlantToFertilizable_Patch
		//{
		//	public static void Postfix(GameObject template, PlantElementAbsorber.ConsumeInfo[] fertilizers) => CollectAbsorbtionInfo(template, fertilizers);
		//}

		//[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendPlantToIrrigated), [typeof(GameObject), typeof(PlantElementAbsorber.ConsumeInfo[])])]
		//public class EntityTemplates_ExtendPlantToIrrigated_Patch
		//{
		//	public static void Postfix(GameObject template, PlantElementAbsorber.ConsumeInfo[] consume_info) => CollectAbsorbtionInfo(template, consume_info);
		//}
		//static void CollectAbsorbtionInfo(GameObject go, PlantElementAbsorber.ConsumeInfo[] consumeInfos)
		//{
		//	if (go == null || !go.TryGetComponent<Crop>(out var crop))
		//		return;
		//	Tag CropId = crop.cropId;

		//	foreach (var info in consumeInfos)
		//	{
		//		AdditionalRecipes.AddOrGetPlantConsumptionInfo(CropId, info.tag, info.massConsumptionRate);
		//	}
		//}

		/// <summary>
		/// allow all "atmosphere" plants to also grow in nitrogen
		/// </summary>
		[HarmonyPatch]
		public class EntityTemplates_ExtendEntityToBasicPlant_Patch
		{
			[HarmonyTargetMethod]
			static MethodBase GetTarget()
			{
				return typeof(EntityTemplates).GetOverloadWithMostArguments(nameof(EntityTemplates.ExtendEntityToBasicPlant), true);
			}

			public static void Prefix(GameObject template, ref SimHashes[] safe_elements, bool can_tinker)
			{
				if (safe_elements == null)
					return;

				if (safe_elements.Contains(SimHashes.Oxygen) && safe_elements.Contains(SimHashes.CarbonDioxide))
				{
					safe_elements = safe_elements.Append(ModElements.Nitrogen_Gas);
				}
				if (!can_tinker)
					return;

				var prefabId = template.PrefabID();
				ManualCodexConversionRegistry.AddConversion(ModElements.Nitrogen_Gas.Tag, PlantNitrogenConsumer.NitrogenConsumedPerSecond, NitrogenFertilizationInfo.ID, 0, prefabId, 0, STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.NAME
					, inputCustomFormating: (tag, amount, continuous) => GameUtil.GetFormattedByTag(tag, amount, GameUtil.TimeSlice.PerSecond)
					, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, PlantNitrogenConsumer.GrowthBoost * 100.0f));

				var butteflyId = ButterflyConfig.ID;
				if (prefabId != ButterflyPlantConfig.ID && DlcManager.IsContentSubscribed(DlcManager.DLC4_ID))
				{
					ManualCodexConversionRegistry.AddConversion( butteflyId, 0,  PollinationInfo.ID, 0, prefabId, 0, global::STRINGS.CODEX.POLLINATORS.TITLE
						, inputCustomFormating: (tag, amount, continuous) => global::STRINGS.CODEX.POLLINATORS.TITLE
						, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, ButterflyTuning.CROP_TENDED_MULTIPLIER_EFFECT * 100.0f));
				}
				if (DlcManager.IsExpansion1Active())
				{
					ManualCodexConversionRegistry.AddConversion( DivergentBeetleConfig.ID, 0, PollinationInfo.ID, 0,prefabId, 0, global::STRINGS.CODEX.POLLINATORS.TITLE
						, inputCustomFormating: (tag, amount, continuous) => global::STRINGS.CODEX.POLLINATORS.TITLE
						, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, BaseDivergentConfig.CROP_TENDED_MULTIPLIER_EFFECT * 100.0f));

					ManualCodexConversionRegistry.AddConversion( DivergentWormConfig.ID, 0, PollinationInfo.ID, 0, prefabId, 0, global::STRINGS.CODEX.POLLINATORS.TITLE
						, inputCustomFormating: (tag, amount, continuous) => global::STRINGS.CODEX.POLLINATORS.TITLE
						, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, DivergentWormConfig.CROP_TENDED_MULTIPLIER_EFFECT * 100.0f));
				}
				ManualCodexConversionRegistry.AddConversion(FarmStationToolsConfig.ID, 1, FertilizationInfo.ID, 0, prefabId, 0, global::STRINGS.CREATURES.STATS.FERTILIZATION.NAME
					, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, 100)); //micronutrient fertilizer gives 100% growth boost, but the const for that sits in the minionmodifiers file, which is not loaded yet at this point
			}
		}
	}
}
