using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using STRINGS;
using TUNING;
using UnityEngine;

namespace UtilLibs
{
    public static class InjectionMethods
    {

        public static void AddStatusItem(string status_id, string category, string name, string desc)
        {
            status_id = status_id.ToUpperInvariant();
            category = category.ToUpperInvariant();
            Strings.Add("STRINGS." + category + ".STATUSITEMS." + status_id + ".NAME", name);
            Strings.Add("STRINGS." + category + ".STATUSITEMS." + status_id + ".TOOLTIP", desc);
        }
        /// <summary>
        /// Add SpriteOnly Item to techs
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="spriteName"></param>
        /// <param name="awailableDLCs">DlcManager.</param>
        public static TechItem AddItemToTechnologySprite(string techItemId, string techId, string name, string description, string spriteName, string[] availableDLCs = null)
        {
            if (availableDLCs == null)
            {
                availableDLCs = DlcManager.AVAILABLE_ALL_VERSIONS;
            }
            AddBuildingToTechnology(techId, techItemId);
            return Db.Get().TechItems.AddTechItem(techItemId, name, description, GetSpriteFnBuilder(spriteName), availableDLCs);
        }

        public static TechItem AddItemToTechnologyKanim(string techItemId, string techId, string name, string description, string kanimName, string[] availableDLCs = null, string uiAnim = "ui")
        {
            var sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(kanimName), uiAnim);
            if (availableDLCs == null)
            {
                availableDLCs = DlcManager.AVAILABLE_ALL_VERSIONS;
            }
            AddBuildingToTechnology(techId, techItemId);
            return Db.Get().TechItems.AddTechItem(techItemId, name, description, (anim, centered) => sprite, availableDLCs);
        }

        public static void AddBuildingToPlanScreen(
            HashedString category,
            string building_id,
            string subcategoryID = "uncategorized",
            ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After) 
            => AddBuildingToPlanScreenBehindNext(category,building_id,string.Empty,subcategoryID,ordering);

        public static void AddBuildingToPlanScreenBehindNext(
            HashedString category,
            string building_id,
            string relativeBuildingId = "",
            string subcategoryID = "uncategorized",
            ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After
            )
        {
            if (relativeBuildingId != string.Empty)
            {
                if (subcategoryID == "uncategorized" && TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(relativeBuildingId))
                {
                    subcategoryID = TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[relativeBuildingId];
                }
                if (TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(building_id))
                    TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[building_id] = subcategoryID;
                else
                    TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, subcategoryID);

                ModUtil.AddBuildingToPlanScreen(category, building_id, subcategoryID, relativeBuildingId, ordering);
            }
            else if (relativeBuildingId == string.Empty && subcategoryID != "uncategorized")
            {
                if (TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(building_id))
                    TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[building_id] = subcategoryID;
                else
                    TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.Add(building_id, subcategoryID);

                ModUtil.AddBuildingToPlanScreen(category, building_id, subcategoryID);
            }
            else
            {
                TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[building_id] = subcategoryID;
                ModUtil.AddBuildingToPlanScreen(category, building_id, subcategoryID);
            }
        }



        private static Func<string, bool, Sprite> GetSpriteFnBuilder(string spriteName) => (Func<string, bool, Sprite>)((anim, centered) => Assets.GetSprite((HashedString)spriteName));

        public static void MoveItemToNewTech(string buildingId, string oldTechId, string newTechId)
        {
            var techs = Db.Get().Techs;
            if(techs.Exists(oldTechId) && techs.Exists(newTechId) && techs.Get(oldTechId).unlockedItemIDs.Contains(buildingId))
            {
                techs.Get(oldTechId).unlockedItemIDs.Remove(buildingId);
                techs.Get(newTechId).unlockedItemIDs.Add(buildingId);
            }
        }

        public static void AddBuildingToTechnology(string techId, string buildingId)
        {
            Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);
        }
        public static void AddSpriteToAssets(Assets instance, string spriteid)
        {
            var path = Path.Combine(UtilMethods.ModPath, "assets");
            var texture = LoadTexture(spriteid, path);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
            sprite.name = spriteid;
            instance.SpriteAssets.Add(sprite);

        }


        public static Texture2D LoadTexture(string name, string directory)
        {
            if (directory == null)
            {
                directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets");
            }

            string path = Path.Combine(directory, name + ".png");

            return LoadTexture(path);
        }
        public static Texture2D LoadTexture(string path, bool warnIfFailed = true)
        {
            Texture2D texture = null;

            if (File.Exists(path))
            {
                byte[] data = TryReadFile(path);
                texture = new Texture2D(1, 1);
                texture.LoadImage(data);
            }
            else if (warnIfFailed)
            {
                SgtLogger.logwarning($"Could not load texture at path {path}.", "SgtImalasUtils");
            }

            return texture;
        }
        public static byte[] TryReadFile(string texFile)
        {
            try
            {
                return File.ReadAllBytes(texFile);
            }
            catch (Exception e)
            {
                SgtLogger.logwarning("Could not read file: " + e, "SgtImalasUtils");
                return null;
            }
        }


        public static void AddBuildingStrings(string buildingId, string name, string description = "", string effect = "")
        {
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, buildingId));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
        }

        //[HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        //CodexEntryGenerator_GenerateCreatureEntries_Patch
        public static void AddCreatureStrings(string creatureId, string name)
        {
            Strings.Add($"STRINGS.CREATURES.FAMILY.{creatureId.ToUpperInvariant()}", UI.FormatAsLink(name, creatureId));
            Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{creatureId.ToUpperInvariant()}", UI.FormatAsLink(name + "s", creatureId));
        }
        public static void AddPlantStrings(string plantId, string name, string description, string domesticatedDescription)
        {
            Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, plantId));
            Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.DESC", description);
            Strings.Add($"STRINGS.CREATURES.SPECIES.{plantId.ToUpperInvariant()}.DOMESTICATEDDESC", domesticatedDescription);
        }

        public static void AddPlantSeedStrings(string plantId, string name, string description)
        {
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{plantId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, plantId));
            Strings.Add($"STRINGS.CREATURES.SPECIES.SEEDS.{plantId.ToUpperInvariant()}.DESC", description);
        }

        public static void AddFoodStrings(string foodId, string name, string description, string recipeDescription = null)
        {
            Strings.Add($"STRINGS.ITEMS.FOOD.{foodId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, foodId));
            Strings.Add($"STRINGS.ITEMS.FOOD.{foodId.ToUpperInvariant()}.DESC", description);

            if (recipeDescription != null)
                Strings.Add($"STRINGS.ITEMS.FOOD.{foodId.ToUpperInvariant()}.RECIPEDESC", recipeDescription);
        }
        public static void AddDiseaseStrings(string id, string name, string symptomps, string description)
        {
            Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, id));
            Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.DESCRIPTIVE_SYMPTOMS", symptomps);
            Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.DESC", description);
            //Strings.Add($"STRINGS.DUPLICANTS.DISEASES.{id.ToUpperInvariant()}.LEGEND_HOVERTEXT", hover);
        }
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
