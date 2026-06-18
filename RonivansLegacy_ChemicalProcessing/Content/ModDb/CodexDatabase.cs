using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.CodexInfoDummies;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class CodexDatabase
	{
		static bool entriesGenerated = false;
		internal static void GenerateGuidanceDeviceEntries()
		{
			if (entriesGenerated || !Config.Instance.MineralProcessing_Mining_Enabled)
				return;
			entriesGenerated = true;

			string parentID = CodexCache.FormatLinkID(ModAssets.Tags.MineralProcessing_GuidanceUnit.ToString());

			var entry = CodexCache.FindEntry(parentID);

			foreach (GameObject go1 in Assets.GetPrefabsWithComponent<ProgrammableGuidanceModule>())
			{
				var prefabTag = go1.GetComponent<KPrefabID>().PrefabTag;
				SgtLogger.l("Creating sub entry for " + prefabTag);

				List<ContentContainer> contentContainerList = new List<ContentContainer>();
				CodexEntryGenerator.GenerateTitleContainers(go1.GetProperName(), contentContainerList);
				Sprite first = Def.GetUISprite(go1).first;
				CodexEntryGenerator.GenerateImageContainers(first, contentContainerList);
				List<ICodexWidget> content = new List<ICodexWidget>();
				content.Add((ICodexWidget)new CodexText(go1.GetComponent<InfoDescription>().description));
				content.Add((ICodexWidget)new CodexSpacer());


				//content.Add(new CodexEntry_MadeAndUsed() { tag = prefabTag.ToString() });
				contentContainerList.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
				CodexEntryGenerator_Elements.GenerateMadeAndUsedContainers(prefabTag, contentContainerList);
				string id = prefabTag.ToString();
				if (entry.subEntries.Find((Predicate<SubEntry>)(x => x.id == id)) == null)
					CodexCache.FindEntry(parentID).subEntries.Add(new SubEntry(id, parentID, contentContainerList, go1.GetProperName())
					{
						icon = first
					});
			}

		}


		public static void GenerateFertilizationInfo(GameObject template, ref SimHashes[] safe_elements, string crop_id, bool can_tinker)
		{
			if (template == null)
				return;

			//no duplicate entries for thee
			if (template.HasTag(GameTags.PlantBranch))
				return;

			bool hasCrop = crop_id != null || template.PrefabID() == "SpaceTree"; //spacetree gets a non-default implementation..

			if (!hasCrop)
				return;

			bool requiresOtherAtmosphere = safe_elements != null;

			if (safe_elements != null && (safe_elements.Contains(SimHashes.Oxygen) || safe_elements.Contains(SimHashes.CarbonDioxide)))
			{
				safe_elements = safe_elements.Append(ModElements.Nitrogen_Gas);
				requiresOtherAtmosphere = false;
			}

			bool requiresLiquidAtmosphere = safe_elements != null && safe_elements.All(e => ElementLoader.FindElementByHash(e).IsLiquid);

			var prefabId = template.PrefabID();
			if (!requiresLiquidAtmosphere && !requiresOtherAtmosphere)
				ManualCodexConversionRegistry.AddConversion(ModElements.Nitrogen_Gas.Tag, PlantNitrogenConsumer.NitrogenConsumedPerSecond, NitrogenFertilizationInfo.ID, 0, prefabId, 0, STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.NAME
					, inputCustomFormating: (tag, amount, continuous) => GameUtil.GetFormattedByTag(tag, amount, GameUtil.TimeSlice.PerSecond)
					, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, PlantNitrogenConsumer.GrowthBoost * 100.0f));


			if (hasCrop && !requiresLiquidAtmosphere) //these two target plants with crop component
			{
				if (DlcManager.IsContentSubscribed(DlcManager.DLC4_ID))
				{
					var butterflyId = ButterflyConfig.ID;
					if (prefabId != ButterflyPlantConfig.ID)
					{
						ManualCodexConversionRegistry.AddConversion(butterflyId, 0, PollinationInfo.ID, 0, prefabId, 0, global::STRINGS.CODEX.POLLINATORS.TITLE
							, inputCustomFormating: (tag, amount, continuous) => global::STRINGS.CODEX.POLLINATORS.TITLE
							, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, ButterflyTuning.CROP_TENDED_MULTIPLIER_EFFECT * 100.0f));
					}
				}
				if (DlcManager.IsExpansion1Active() && !template.HasTag(GameTags.Hanging))
				{
					ManualCodexConversionRegistry.AddConversion(DivergentBeetleConfig.ID, 0, PollinationInfo.ID, 0, prefabId, 0, global::STRINGS.CODEX.POLLINATORS.TITLE
						, inputCustomFormating: (tag, amount, continuous) => global::STRINGS.CODEX.POLLINATORS.TITLE
						, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, BaseDivergentConfig.CROP_TENDED_MULTIPLIER_EFFECT * 100.0f));

					ManualCodexConversionRegistry.AddConversion(DivergentWormConfig.ID, 0, PollinationInfo.ID, 0, prefabId, 0, global::STRINGS.CODEX.POLLINATORS.TITLE
						, inputCustomFormating: (tag, amount, continuous) => global::STRINGS.CODEX.POLLINATORS.TITLE
						, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, DivergentWormConfig.CROP_TENDED_MULTIPLIER_EFFECT * 100.0f));
				}
			}
			if (can_tinker)
				ManualCodexConversionRegistry.AddConversion(FarmStationToolsConfig.ID, 1, FertilizationInfo.ID, 0, prefabId, 0, global::STRINGS.CREATURES.STATS.FERTILIZATION.NAME
					, outputCustomFormating: (tag, amount, continuous) => string.Format(STRINGS.CREATURES.MODIFIERS.AIO_NITROGENIZED.CODEX_FORMAT, 100)); //micronutrient fertilizer gives 100% growth boost, but the const for that sits in the minionmodifiers file, which is not loaded yet at this point

		}
	}
}
