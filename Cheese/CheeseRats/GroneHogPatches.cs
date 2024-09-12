using HarmonyLib;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cheese.CheeseRats
{
	internal class GroneHogPatches
	{
		internal delegate System.Action CreateDietaryModifier(string id, Tag eggTag,
			HashSet<Tag> foodTags, float modifierPerCal);

		internal delegate void GenerateCreatureDescriptionContainers(GameObject creature,
			List<ContentContainer> containers);

		internal delegate void GenerateImageContainers(Sprite[] sprites,
			List<ContentContainer> containers, ContentContainer.ContentLayout layout);

		internal static readonly CreateDietaryModifier CREATE_DIETARY_MODIFIER =
			typeof(TUNING.CREATURES.EGG_CHANCE_MODIFIERS).
			CreateStaticDelegate<CreateDietaryModifier>(nameof(CreateDietaryModifier),
			typeof(string), typeof(Tag), typeof(HashSet<Tag>), typeof(float));

		internal static GenerateCreatureDescriptionContainers GENERATE_DESC;

		internal static readonly GenerateImageContainers GENERATE_IMAGE_CONTAINERS =
			typeof(CodexEntryGenerator).CreateStaticDelegate<GenerateImageContainers>(
			nameof(GenerateImageContainers), typeof(Sprite[]), typeof(List<ContentContainer>),
			typeof(ContentContainer.ContentLayout));
		private static void AddToCodex(Tag speciesTag, string name,
		IDictionary<string, CodexEntry> results)
		{
			string tagStr = speciesTag.ToString();
			var brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
			var entry = new CodexEntry(nameof(global::STRINGS.CREATURES), new List<ContentContainer> {
				new ContentContainer(new List<ICodexWidget> {
					new CodexSpacer(),
					new CodexSpacer()
				}, ContentContainer.ContentLayout.Vertical)
			}, name)
			{
				parentId = nameof(global::STRINGS.CREATURES)
			};
			CodexCache.AddEntry(tagStr, entry);
			results.Add(tagStr, entry);
			// Find all critters with this tag
			foreach (var prefab in brains)
				if (prefab.GetDef<BabyMonitor.Def>() == null && prefab.TryGetComponent(
						out CreatureBrain brain) && brain.species == speciesTag)
				{
					Sprite babySprite = null;
					string prefabID = prefab.PrefabID().Name;
					var baby = Assets.TryGetPrefab(prefabID + "Baby");
					var contentContainerList = new List<ContentContainer>(4);
					var first = Def.GetUISprite(prefab, brain.symbolPrefix + "ui").first;
					if (baby != null)
						babySprite = Def.GetUISprite(baby).first;
					if (babySprite != null)
						GENERATE_IMAGE_CONTAINERS.Invoke(new[] { first, babySprite },
							contentContainerList, ContentContainer.ContentLayout.Horizontal);
					else
						contentContainerList.Add(new ContentContainer(new List<ICodexWidget> {
							new CodexImage(128, 128, first)
						}, ContentContainer.ContentLayout.Vertical));
					GENERATE_DESC.Invoke(prefab, contentContainerList);
					entry.subEntries.Add(new SubEntry(prefabID, tagStr, contentContainerList,
							prefab.GetProperName())
					{
						icon = first,
						iconColor = Color.white
					});
				}
		}
		[HarmonyPatch]
		public class CodexEntryGenerator_GenerateCreatureEntries_Patch
		{
			internal static MethodBase TargetMethod()
			{
				// TODO Remove when versions prior to U49-574642 no longer need to be supported
				var method = typeof(CodexEntryGenerator).GetMethodSafe(
					"GenerateCreatureEntries", true, PPatchTools.AnyArguments);
				System.Type targetType;
				if (method == null)
				{
					targetType = PPatchTools.GetTypeSafe("CodexEntryGenerator_Creatures");
					method = targetType?.GetMethodSafe("GenerateEntries", true,
						PPatchTools.AnyArguments);
				}
				else
					targetType = typeof(CodexEntryGenerator);
				GENERATE_DESC = targetType?.
					CreateStaticDelegate<GenerateCreatureDescriptionContainers>(
					nameof(GenerateCreatureDescriptionContainers), typeof(GameObject),
					typeof(List<ContentContainer>));
				return method;
			}
			public static void Postfix(Dictionary<string, CodexEntry> __result)
			{
				Strings.Add($"STRINGS.CREATURES.FAMILY.{BaseGroneHogConfig.SpeciesId.ToUpperInvariant()}", GroneHogConfig.Name);
				Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{BaseGroneHogConfig.SpeciesId.ToUpperInvariant()}", GroneHogConfig.PluralName);

				AddToCodex(BaseGroneHogConfig.SpeciesId, GroneHogConfig.PluralName, __result);
			}
		}
	}
}
