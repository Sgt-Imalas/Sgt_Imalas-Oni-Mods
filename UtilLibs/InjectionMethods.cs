using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using Database;
using static ModUtil;

namespace UtilLibs
{
	public static class InjectionMethods
	{
		public class BATCH_TAGS
		{
			public const int SWAPS = -77805842;
			public const int INTERACTS = -1371425853;
		}

		///Use the following patch to add any custom interact anims;
		//      [HarmonyPatch(typeof(KAnimGroupFile), "Load")]
		//public class KAnimGroupFile_Load_Patch
		//      {
		//          public static void Prefix(KAnimGroupFile __instance)
		//          {
		//              InjectionMethods.RegisterBatchTag(
		//                  __instance,
		//                  InjectionMethods.BATCH_TAGS.INTERACTS,
		//                  new HashSet<HashedString>()
		//                  {
		//                      "aete_interacts_espresso_short_kanim",
		//                      "aete_goop_vomit_kanim"
		//                  });
		//          }
		//      }

		public static void RegisterCustomSwapAnim(KAnimGroupFile kAnimGroupFile, HashedString swap) => RegisterCustomSwapAnims(kAnimGroupFile, new HashSet<HashedString>() { swap });
		public static void RegisterCustomSwapAnims(KAnimGroupFile kAnimGroupFile, HashSet<HashedString> swaps) =>
			MoveAnimGroups(kAnimGroupFile, BATCH_TAGS.SWAPS, swaps);
		public static void RegisterCustomInteractAnim(KAnimGroupFile kAnimGroupFile,HashedString swap) => RegisterCustomInteractAnims(kAnimGroupFile, new HashSet<HashedString>() { swap });
		public static void RegisterCustomInteractAnims(KAnimGroupFile kAnimGroupFile, HashSet<HashedString> swaps) =>
			MoveAnimGroups(kAnimGroupFile, BATCH_TAGS.INTERACTS, swaps);

		/// <summary>
		/// Required to register the correct anim group for custom made interact anims
		/// </summary>
		/// <param name="kAnimGroupFile"></param>
		/// <param name="taghash"></param>
		/// <param name="swaps"></param>
		public static void MoveAnimGroups(KAnimGroupFile kAnimGroupFile, int taghash, HashSet<HashedString> swaps)
		{
			var groups = kAnimGroupFile.GetData();
			var swapAnimsGroup = KAnimGroupFile.GetGroup(new HashedString(taghash));

			// remove the wrong group
			groups.RemoveAll(g => swaps.Contains(g.animNames[0]));

			foreach (var swap in swaps)
			{
				// readd to correct group
				var anim = global::Assets.GetAnim(swap);

				if (anim == null)

				{
					SgtLogger.warning("anim " + swap + " not found");
					continue;
				}
				if (swapAnimsGroup.animFiles.Contains(anim) || swapAnimsGroup.animNames.Contains(anim.name))
				{

					SgtLogger.warning("anim " + swap + " already in group");
					continue;
				}

				swapAnimsGroup.animFiles.Add(anim);
				swapAnimsGroup.animNames.Add(anim.name);
				SgtLogger.l(anim+"; "+anim.name+" added to group");
			}
		}
		public static Func<S, T> CreateGetter<S, T>(FieldInfo field)
		{
			string methodName = field.ReflectedType.FullName + ".get_" + field.Name;
			DynamicMethod setterMethod = new DynamicMethod(methodName, typeof(T), new Type[1] { typeof(S) }, true);
			ILGenerator gen = setterMethod.GetILGenerator();
			if (field.IsStatic)
			{
				gen.Emit(OpCodes.Ldsfld, field);
			}
			else
			{
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldfld, field);
			}
			gen.Emit(OpCodes.Ret);
			return (Func<S, T>)setterMethod.CreateDelegate(typeof(Func<S, T>));
		}

		public static Action<S, T> CreateSetter<S, T>(FieldInfo field)
		{
			string methodName = field.ReflectedType.FullName + ".set_" + field.Name;
			DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[2] { typeof(S), typeof(T) }, true);
			ILGenerator gen = setterMethod.GetILGenerator();
			if (field.IsStatic)
			{
				gen.Emit(OpCodes.Ldarg_1);
				gen.Emit(OpCodes.Stsfld, field);
			}
			else
			{
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldarg_1);
				gen.Emit(OpCodes.Stfld, field);
			}
			gen.Emit(OpCodes.Ret);
			return (Action<S, T>)setterMethod.CreateDelegate(typeof(Action<S, T>));
		}
		public static void AddStatusItem(string status_id, string category, string name, string desc)
		{
			status_id = status_id.ToUpperInvariant();
			category = category.ToUpperInvariant();
			Strings.Add("STRINGS." + category + ".STATUSITEMS." + status_id + ".NAME", name);
			Strings.Add("STRINGS." + category + ".STATUSITEMS." + status_id + ".TOOLTIP", desc);
		}

		static HashSet<string> ResearchablesFromMod = new();
		public static bool IsFromThisMod(string id) => ResearchablesFromMod.Contains(id);

		/// <summary>
		/// Add SpriteOnly Item to techs
		/// </summary>
		/// <param name="techId"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="spriteName"></param>
		/// <param name="awailableDLCs">DlcManager.</param>
		public static TechItem AddItemToTechnologySprite(string techItemId, string techId, string name, string description, string spriteName, string[] requiredDLcs = null, string[] forbiddenDlc = null, bool isPoiUnlock = false)
		{
			AddBuildingToTechnology(techId, techItemId);
			return Db.Get().TechItems.AddTechItem(techItemId, name, description, GetSpriteFnBuilder(spriteName), requiredDLcs, forbiddenDlc, isPoiUnlock);
		}

		public static TechItem AddItemToTechnologyKanim(string techItemId, string techId, string name, string description, string kanimName, string uiAnim = "ui", string[] requiredDLcs = null, string[] forbiddenDlc = null, bool isPoiUnlock = false)
		{
			var sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(kanimName), uiAnim);

			AddBuildingToTechnology(techId, techItemId);
			return Db.Get().TechItems.AddTechItem(techItemId, name, description, (anim, centered) => sprite, requiredDLcs, forbiddenDlc, isPoiUnlock);
		}

		public static void MoveExistingBuildingToNewPlanscreen(
			HashedString category,
			string building_id,
			string subcategoryID = "uncategorized",
			ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
			=> MoveExistingBuildingToNewCategory(category, building_id, string.Empty, subcategoryID, ordering);

		public static void MoveExistingBuildingToNewCategory(
			HashedString category,
			string building_id,
			string relativeBuildingId = "",
			string subcategoryID = "uncategorized",
			ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
		{
			if (subcategoryID == string.Empty || subcategoryID == null)
				subcategoryID = "uncategorized";

			bool foundInExisting = false;
			foreach (var EntryList in TUNING.BUILDINGS.PLANORDER)
			{
				int locationIndex = EntryList.buildingAndSubcategoryData.FindIndex(dat => dat.Key == building_id);
				if (locationIndex > -1)
				{
					SgtLogger.l($"Building {building_id} found in category {EntryList.category}, moving it to {category}");
					EntryList.buildingAndSubcategoryData.RemoveAt(locationIndex);
					foundInExisting = true;
					break;
				}
			}
			if (TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(building_id))
				TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Remove(building_id);

			if (!foundInExisting)
				SgtLogger.l($"Building {building_id} had no previous category defined, adding it to {category}");


			AddBuildingToPlanScreenBehindNext(category, building_id, relativeBuildingId, subcategoryID, ordering);
		}


		public static void AddBuildingToPlanScreen(
			HashedString category,
			string building_id,
			string subcategoryID = "uncategorized",
			ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
			=> AddBuildingToPlanScreenBehindNext(category, building_id, string.Empty, subcategoryID, ordering);

		public static void AddBuildingToPlanScreenBehindNext(
			HashedString category,
			string building_id,
			string relativeBuildingId = null,
			string subcategoryID = "uncategorized",
			ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After
			)
		{
			if (relativeBuildingId != null)
			{
				if (subcategoryID == "uncategorized" && TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(relativeBuildingId))
				{
					subcategoryID = TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[relativeBuildingId];
				}
				if (TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(building_id))
					TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[building_id] = subcategoryID;
				else
				{
					TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, subcategoryID);
				}

				ModUtil_AddBuildingToPlanScreen(category, building_id, subcategoryID, relativeBuildingId, ordering); //replace with ModUtil again when they fix it
			}
			else if (relativeBuildingId == string.Empty && subcategoryID != "uncategorized")
			{
				if (TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(building_id))
					TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[building_id] = subcategoryID;
				else
				{
					TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, subcategoryID);
				}
				ModUtil_AddBuildingToPlanScreen(category, building_id, subcategoryID, relativeBuildingId, ordering);//replace with ModUtil again when they fix it
			}
			else
			{
				TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[building_id] = subcategoryID;
				ModUtil_AddBuildingToPlanScreen(category, building_id, subcategoryID, relativeBuildingId,ordering);//replace with ModUtil again when they fix it
			}
		}

		/// <summary>
		/// this method is cloned from ModUtil.AddBuildingToPlanScreen, because that has an insertion index bug for BuildingOrdering.Before
		/// using this clone until they fix that
		/// </summary>
		/// <param name="category"></param>
		/// <param name="building_id"></param>
		/// <param name="subcategoryID"></param>
		/// <param name="relativeBuildingId"></param>
		/// <param name="ordering"></param>
		public static void ModUtil_AddBuildingToPlanScreen(HashedString category, string building_id, string subcategoryID, string relativeBuildingId, BuildingOrdering ordering = BuildingOrdering.After)
		{
			int num = TUNING.BUILDINGS.PLANORDER.FindIndex((PlanScreen.PlanInfo x) => x.category == category);
			if (num < 0)
			{
				Debug.LogWarning($"Mod: Unable to add '{building_id}' as category '{category}' does not exist");
				return;
			}

			List<KeyValuePair<string, string>> buildingAndSubcategoryData = TUNING.BUILDINGS.PLANORDER[num].buildingAndSubcategoryData;
			KeyValuePair<string, string> item = new KeyValuePair<string, string>(building_id, subcategoryID);
			if (relativeBuildingId == null)
			{
				buildingAndSubcategoryData.Add(item);
				return;
			}

			int foundInsertionIndex = buildingAndSubcategoryData.FindIndex((KeyValuePair<string, string> x) => x.Key == relativeBuildingId);
			if (foundInsertionIndex == -1)
			{
				buildingAndSubcategoryData.Add(item);
				Debug.LogWarning("Mod: Building '" + relativeBuildingId + "' doesn't exist, inserting '" + building_id + "' at the end of the list instead");
			}
			else
			{
				int index = (ordering == BuildingOrdering.After) ? (foundInsertionIndex + 1) : Math.Max(foundInsertionIndex, 0);
				buildingAndSubcategoryData.Insert(index, item);
			}
		}



		private static Func<string, bool, Sprite> GetSpriteFnBuilder(string spriteName) => (Func<string, bool, Sprite>)((anim, centered) => Assets.GetSprite((HashedString)spriteName));

		public static void MoveItemToNewTech(string buildingId, string oldTechId, string newTechId)
		{
			var techs = Db.Get().Techs;
			if (techs.Exists(oldTechId) && techs.Exists(newTechId) && techs.Get(oldTechId).unlockedItemIDs.Contains(buildingId))
			{
				techs.Get(oldTechId).unlockedItemIDs.Remove(buildingId);
				techs.Get(newTechId).unlockedItemIDs.Add(buildingId);
			}
		}

		public static void AddBuildingToTechnology(string techId, string buildingId)
		{
			ResearchablesFromMod.Add(buildingId);
			Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);
		}
		public static Sprite AddSpriteToAssets(Assets instance, string spriteid, bool overrideExisting = false)
		{
			return AssetUtils.AddSpriteToAssets(instance, spriteid, overrideExisting);
		}


		public static void AddBuildingStrings(string buildingId, string name, string description = "", string effect = "")
		{
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", STRINGS.UI.FormatAsLink(name, buildingId));
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
		}

        public static void AddLaserEffect(string ID, HashedString context, KBatchedAnimEventToggler kbatchedAnimEventToggler, KBatchedAnimController kbac, string animFile, string defaultAnimation = "loop")
        {
            var laserEffect = new BaseMinionConfig.LaserEffect
            {
                id = ID,
                animFile = animFile,
                anim = defaultAnimation,
                context = context
            };

            var laserGo = new GameObject(laserEffect.id);
            laserGo.transform.parent = kbatchedAnimEventToggler.transform;
            laserGo.AddOrGet<KPrefabID>().PrefabTag = new Tag(laserEffect.id);

            var tracker = laserGo.AddOrGet<KBatchedAnimTracker>();
            tracker.controller = kbac;
            tracker.symbol = new HashedString("snapTo_rgtHand");
            tracker.offset = new Vector3(195f, -35f, 0f);
            tracker.useTargetPoint = true;

            var kbatchedAnimController = laserGo.AddOrGet<KBatchedAnimController>();
            kbatchedAnimController.AnimFiles =
            [
                Assets.GetAnim(laserEffect.animFile)
            ];

            var item = new KBatchedAnimEventToggler.Entry
            {
                anim = laserEffect.anim,
                context = laserEffect.context,
                controller = kbatchedAnimController
            };

            kbatchedAnimEventToggler.entries.Add(item);

            laserGo.AddOrGet<LoopingSounds>();
        }
    


    #region obsoleteStringInjections

    public static void AddCreatureStrings(string creatureId, string name)
		{
			Strings.Add($"STRINGS.CREATURES.FAMILY.{creatureId.ToUpperInvariant()}", STRINGS.UI.FormatAsLink(name, creatureId));
			Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{creatureId.ToUpperInvariant()}", STRINGS.UI.FormatAsLink(name + "s", creatureId));
		}
		public static void AddPlantStrings(string plantId, string name, string description, string domesticatedDescription)
		{
			Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.NAME", STRINGS.UI.FormatAsLink(name, plantId));
			Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.DESC", description);
			Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.DOMESTICATEDDESC", domesticatedDescription);
		}

		public static void AddPlantSeedStrings(string plantId, string name, string description)
		{
			Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{plantId.ToUpperInvariant()}.NAME", STRINGS.UI.FormatAsLink(name, plantId));
			Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{plantId.ToUpperInvariant()}.DESC", description);
		}

		public static void AddFoodStrings(string foodId, string name, string description, string recipeDescription = null)
		{
			Strings.Add($"STRINGS.ITEMS.FOOD.{foodId.ToUpperInvariant()}.NAME", STRINGS.UI.FormatAsLink(name, foodId));
			Strings.Add($"STRINGS.ITEMS.FOOD.{foodId.ToUpperInvariant()}.DESC", description);

			if (recipeDescription != null)
				Strings.Add($"STRINGS.ITEMS.FOOD.{foodId.ToUpperInvariant()}.RECIPEDESC", recipeDescription);
		}
		public static void AddDiseaseStrings(string id, string name, string symptomps, string description)
		{
			Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.NAME", STRINGS.UI.FormatAsLink(name, id));
			Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.DESCRIPTIVE_SYMPTOMS", symptomps);
			Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.DESC", description);
			//Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.LEGEND_HOVERTEXT", hover);
		}

        #endregion

        public static void Action(Tag speciesTag, string name, Dictionary<string, CodexEntry> results)
		{
			List<GameObject> brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
			CodexEntry entry = new CodexEntry("CREATURES", new List<ContentContainer>()
			{
				new ContentContainer(new List<ICodexWidget>()
				{
					new CodexSpacer(),
					new CodexSpacer()
				}, ContentContainer.ContentLayout.Vertical)
			}, name);
			entry.parentId = "CREATURES";
			CodexCache.AddEntry(speciesTag.ToString(), entry, null);
			results.Add(speciesTag.ToString(), entry);
			foreach (GameObject gameObject in brains)
			{
				if (gameObject.GetDef<BabyMonitor.Def>() == null)
				{
					Sprite sprite = null;
					GameObject prefab = Assets.TryGetPrefab((gameObject.PrefabID().ToString() + "Baby"));
					if (prefab != null)
						sprite = Def.GetUISprite(prefab, "ui", false).first;
					CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
					if (component.species == speciesTag)
					{
						List<ContentContainer> contentContainerList = new List<ContentContainer>();
						string symbolPrefix = component.symbolPrefix;
						Sprite first = Def.GetUISprite(gameObject, symbolPrefix + "ui", false).first;
						if ((bool)((UnityEngine.Object)sprite))
						{
							Traverse.Create(typeof(CodexEntryGenerator)).Method("GenerateImageContainers", new[] { typeof(Sprite[]), typeof(List<ContentContainer>), typeof(ContentContainer.ContentLayout) })
								.GetValue(new Sprite[2]
								{
									first,
									sprite
								}, contentContainerList, ContentContainer.ContentLayout.Horizontal);
						}
						else
						{
							contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
							{
							  new CodexImage(128, 128, first)
							}, ContentContainer.ContentLayout.Vertical));
						}

						Traverse.Create(typeof(CodexEntryGenerator)).Method("GenerateCreatureDescriptionContainers", new[] { typeof(GameObject), typeof(List<ContentContainer>) }).GetValue(gameObject, contentContainerList);
						entry.subEntries.Add(new SubEntry(component.PrefabID().ToString(), speciesTag.ToString(), contentContainerList, component.GetProperName())
						{
							icon = first,
							iconColor = UnityEngine.Color.white
						});
					}
				}
			}
		}
	}
}
