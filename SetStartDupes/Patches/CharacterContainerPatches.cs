using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace SetStartDupes.Patches
{
	internal class CharacterContainerPatches
	{
		/// <summary>
		/// Reorder the aptitude entries so entries with the same skillgroup end up on top of each other.
		/// also remove preferred height so it stretches with more aptitude entries
		/// </summary>
		[HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.SetInfoText))]
		public class CharacterContainer_SetInfoText_Patch
		{
			static MinionStartingStats instanceStats;
			public static void Prefix(CharacterContainer __instance)
			{
				instanceStats = __instance.stats;
				instanceStats.Traits = instanceStats.Traits
					.OrderByDescending(trait => ModAssets.IsMinionBaseTrait(trait.Id))
					.ThenBy(trait => ModAssets.GetTraitListOfTrait(trait))
					.ThenBy(t => t.Name)
					.ToList();

				//SgtLogger.l("Sorting traits:");
				//foreach(var trait in instanceStats.Traits)
				//{
				//	SgtLogger.l("Trait: " + trait.Name + " Type: " + ModAssets.GetTraitListOfTrait(trait));
				//}
			}
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				var m_set_color = AccessTools.Method(typeof(UnityEngine.UI.Graphic), "set_color");
				//var index = codes.FindIndex(c => c.Calls(m_set_color));
				var goodTraitColorIndex = codes.FindIndex(ci => ci.LoadsField(AccessTools.Field(typeof(Constants), nameof(Constants.POSITIVE_COLOR))));
				var badTraitColorIndex = codes.FindIndex(ci => ci.LoadsField(AccessTools.Field(typeof(Constants), nameof(Constants.NEGATIVE_COLOR))));


				if (goodTraitColorIndex == -1 || badTraitColorIndex == -1)
				{
					SgtLogger.error("TRANSPILER FAILED: Could not find index for set_color in CharacterContainer_SetInfoText_Patch");
					return codes;
				}
				int currentTraitLocIndex = TranspilerHelper.FindIndexOfNextLocalIndex(codes, goodTraitColorIndex);
				if (currentTraitLocIndex == -1 )
				{
					SgtLogger.error("TRANSPILER FAILED: Could not find loc index for current trait");
					return codes;
				}



				var m_ReplaceColorForTrait = AccessTools.Method(typeof(CharacterContainer_SetInfoText_Patch), "ReplaceColorForTrait", [typeof(Color),typeof(Trait)]);

				codes.InsertRange(++goodTraitColorIndex,
					[new (OpCodes.Ldloc_S,currentTraitLocIndex),new (OpCodes.Call, m_ReplaceColorForTrait)]);
				codes.InsertRange(++badTraitColorIndex,
					[new(OpCodes.Ldloc_S, currentTraitLocIndex), new(OpCodes.Call, m_ReplaceColorForTrait)]);

				//codes.InsertRange(index, new[]
				//{
				//	new CodeInstruction(OpCodes.Call, m_ReplaceColorForTrait)
				//});

				TranspilerHelper.PrintInstructions(codes);
				return codes;
			}

			private static Color ReplaceColorForTrait(Color existing, Trait currentTrait)
			{
				if (instanceStats == null)
					return existing;


				var traitType = ModAssets.GetTraitListOfTrait(currentTrait);
				//SgtLogger.l("Traittype of " + currentTrait.Name + " is " + traitType);

				if (traitType == DupeTraitManager.NextType.needTrait || traitType == DupeTraitManager.NextType.geneShufflerTrait)
				{
					return ModAssets.GetColourFromType(traitType);
				}
				return existing;
			}

			public static void Postfix(CharacterContainer __instance)
			{
				if (__instance.aptitudeEntry?.transform?.parent?.parent?.gameObject?.TryGetComponent<LayoutElement>(out LayoutElement layoutElement) ?? false)
				{
					/// Remove prev height so additional traits extend the box indstead of going hidden
					layoutElement.preferredHeight = -1;
				}

				if (__instance.stats.personality.model == GameTags.Minions.Models.Bionic) //no aptitude entries for bionic dupes
					return;

				var skillgroups = Db.Get().SkillGroups;
				Dictionary<SkillGroup, GameObject> AptitudeEntries = new(16);

				int index = 0;
				foreach (KeyValuePair<SkillGroup, float> skillAptitude in __instance.stats.skillAptitudes)
				{
					if (skillAptitude.Value == 0f)
					{
						continue;
					}

					SkillGroup skillGroup = skillgroups.Get(skillAptitude.Key.IdHash);
					if (skillGroup == null)
					{
						Debug.LogWarningFormat("Role group not found for aptitude: {0}", skillAptitude.Key);
						continue;
					}
					//store the aptitude entries associated gameobjects
					AptitudeEntries[skillGroup] = __instance.aptitudeEntries[index++];
				}

				var sortedEntries = AptitudeEntries.OrderBy(entry => entry.Key.relevantAttributes.First().Id).ToList();
				sortedEntries.ForEach(entry => entry.Value.transform.SetAsLastSibling());
			}
		}
	}
}
