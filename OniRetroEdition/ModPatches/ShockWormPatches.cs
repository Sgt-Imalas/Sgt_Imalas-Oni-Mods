using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using System.Reflection;
using TUNING;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
	internal class ShockWormPatches
	{

		public static Tag ShockwormConfigSpeciesID = TagManager.Create("ShockWormSpecies");

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
				AddToCodex(ShockwormConfigSpeciesID, STRINGS.CREATURES.FAMILY_PLURAL.SHOCKWORMSPECIES, __result);
			}
		}



		[HarmonyPatch(typeof(ShockwormConfig), "CreatePrefab")]
		public static class Make_Creature
		{
			public static void Postfix(ref GameObject __result)
			{
				__result = EntityTemplates.CreatePlacedEntity("ShockWorm", global::STRINGS.CREATURES.SPECIES.SHOCKWORM.NAME, global::STRINGS.CREATURES.SPECIES.SHOCKWORM.DESC, 50f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim("shockworm_kanim"), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 2);
				EntityTemplates.ExtendEntityToBasicCreature(__result, FactionManager.FactionID.Hostile, null, "FlyerNavGrid1x2", NavType.Hover, 32, 2f, "Meat", 3, drownVulnerable: true, entombVulnerable: true, lethalLowTemperature: TUNING.CREATURES.TEMPERATURE.FREEZING_2, warningLowTemperature: TUNING.CREATURES.TEMPERATURE.FREEZING_1, warningHighTemperature: TUNING.CREATURES.TEMPERATURE.HOT_1, lethalHighTemperature: TUNING.CREATURES.TEMPERATURE.HOT_2);
				__result.AddOrGet<LoopingSounds>();
				__result.AddWeapon(3f, 6f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 10, 4f).AddEffect();
				SoundEventVolumeCache.instance.AddVolume("shockworm_kanim", "Shockworm_attack_arc", NOISE_POLLUTION.CREATURES.TIER6);
				__result.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = CREATURES.SPACE_REQUIREMENTS.TIER1;
				ChoreTable.Builder chore_table =
					new ChoreTable.Builder()
					.Add((StateMachine.BaseDef)new DeathStates.Def())
					.Add((StateMachine.BaseDef)new AnimInterruptStates.Def())
					.Add((StateMachine.BaseDef)new TrappedStates.Def())
					.Add((StateMachine.BaseDef)new BaggedStates.Def())
					.Add((StateMachine.BaseDef)new StunnedStates.Def())
					.Add((StateMachine.BaseDef)new FallStates.Def())
					.Add((StateMachine.BaseDef)new DebugGoToStates.Def())
					.Add((StateMachine.BaseDef)new DrowningStates.Def())
					.Add((StateMachine.BaseDef)new FleeStates.Def())
					.Add((StateMachine.BaseDef)new AttackStates.Def("attack_loop", "attack_pst"))
					.Add((StateMachine.BaseDef)new IdleStates.Def())
					;
				EntityTemplates.AddCreatureBrain(__result, chore_table, ShockwormConfigSpeciesID, "");
				KPrefabID component = __result.GetComponent<KPrefabID>();
				component.AddTag(GameTags.Creatures.Flyer);

				__result.AddOrGetDef<ThreatMonitor.Def>();
				__result.AddOrGetDef<AgeMonitor.Def>();

				Trait trait = Db.Get().CreateTrait("ShockWormBaseBaseTrait", "Shock Worm", "Shock Worm", (string)null, false, (ChoreGroup[])null, true, true);
				trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 15f, "Shock Worm"));
				trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, "Shock Worm"));

				Modifiers modifiers = __result.AddOrGet<Modifiers>();
				modifiers.initialTraits.Add("ShockWormBaseBaseTrait");
				SoundEventVolumeCache.instance.AddVolume("shockworm_kanim", "attack_loop", NOISE_POLLUTION.CREATURES.TIER6);

			}
		}
	}
}

