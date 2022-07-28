using System;
using System.Collections.Generic;
using HarmonyLib;
using STRINGS;
using TUNING;
using UnityEngine;

namespace UtilLibs
{
    public static class InjectionMethods
	{
        public static void PrintInstructions(List<HarmonyLib.CodeInstruction> codes)
        {
            Debug.Log("\n");
            for (int i = 0; i < codes.Count; i++)
            {
                Debug.Log(i + ": " + codes[i]);
            }
        }
        public static void AddStatusItem(string status_id, string category , string name, string desc)
        {
            status_id = status_id.ToUpperInvariant();
             category = category.ToUpperInvariant();
            Strings.Add("STRINGS." + category + ".STATUSITEMS." + status_id + ".NAME", name);
            Strings.Add("STRINGS." + category + ".STATUSITEMS." + status_id + ".TOOLTIP", desc);
        }

        public static void AddBuildingToTechnology(string techId, string buildingId)
		{
			Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);
		}
        public static void AddBuildingStrings(string buildingId, string name, string description = "", string effect= "")
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
			Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{creatureId.ToUpperInvariant()}", UI.FormatAsLink(name+"s", creatureId));
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
