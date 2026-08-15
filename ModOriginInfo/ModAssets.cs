using Klei.AI;
using KMod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static GeyserConfigurator;

namespace ModOriginInfo
{
	internal class ModAssets
	{
		/// <summary>
		/// Mod ids that get skipped in the research screen (they add their own origin tooltips)
		/// </summary>
		public static HashSet<string> SkippableResearchModIds = ["Rocketry Expanded", "RonivansLegacy_ChemicalProcessing"];
		/// <summary>
		/// if you have a mod that does this as well, msg me or reflect for this method below to add your own id to the list
		/// </summary>
		/// <param name="modID"></param>
		public static void AddSkippableResearchModId(string modID) => SkippableResearchModIds.Add(modID);

		public static bool IsModdedResearchItem(string id) => IsModdedInternal(id, out var modId) && !SkippableResearchModIds.Contains(modId);

		public static bool IsModded(KPrefabID id) => id != null && IsModdedInternal(id.PrefabTag, out _);
		public static bool IsModded(string id, out string modId) => IsModdedInternal(id, out modId);
		public static bool IsModded(Tag id, out string modId) => IsModdedInternal(id, out modId);
		static bool IsModdedInternal(Tag tag, out string modId) => ModdedContentPrefabIdToModId.TryGetValue(tag, out modId);
		static Dictionary<Tag, string> ModdedContentPrefabIdToModId = [];
		static HashSet<Tag> ModdedBuildings = [];
		static HashSet<Tag> ModdedRecipes = [];
		static Dictionary<Assembly, string> AssemblyToModIdMap = [];
		public static bool IsBuilding(Tag tag) => ModdedBuildings.Contains(tag);

		internal static void RegisterBuildingDef(IBuildingConfig cfg, BuildingDef def)
		{
			var defType = cfg.GetType();

			if (AssemblyToModIdMap.TryGetValue(defType.Assembly, out string modID))
			{
				ModdedContentPrefabIdToModId[def.PrefabID] = modID;
				ModdedBuildings.Add(def.PrefabID);
				def.AddSearchTerms(modID);
				def.AddSearchTerms(GetModName(modID));
				//SgtLogger.l("Modded Building: " + def.PrefabID + " from " + modID);
			}
			//else
			//	SgtLogger.l("Vanilla Building: "+def.PrefabID);
		}
		internal static void RegisterElementId(Assembly assembly, string elementTag)
		{
			if (AssemblyToModIdMap.TryGetValue(assembly, out string modID))
			{
				if (ModdedContentPrefabIdToModId.ContainsKey(elementTag))
					return;
				ModdedContentPrefabIdToModId[elementTag] = modID;
			}
		}
		internal static void RegisterSpice(Assembly assembly, Database.Spice spice)
		{
			if (AssemblyToModIdMap.TryGetValue(assembly, out string modID))
			{
				string spiceId = spice.Id;
				if (ModdedContentPrefabIdToModId.ContainsKey(spiceId))
					return;
				SgtLogger.l("Modded Spice: " + spiceId + " from " + modID);

				ModdedContentPrefabIdToModId[spiceId] = modID;
			}
		}
		internal static void RegisterGeyser(Assembly assembly, GeyserType geyser)
		{
			if (AssemblyToModIdMap.TryGetValue(assembly, out string modID))
			{
				string geyserId = "GeyserGeneric_" + geyser.id;
				if (ModdedContentPrefabIdToModId.ContainsKey(geyserId))
					return;

				ModdedContentPrefabIdToModId[geyserId] = modID;
			}
		}
		internal static void RegisterRecipe(Assembly assembly, ComplexRecipe recipe)
		{
			if (AssemblyToModIdMap.TryGetValue(assembly, out string modID))
			{
				//SgtLogger.l(modID + " mod recipe: " + recipe.id);
				ModdedContentPrefabIdToModId[recipe.id] = modID;
				ModdedRecipes.Add(recipe.id);
				if (recipe.recipeCategoryID != null)
				{
					ModdedContentPrefabIdToModId[recipe.recipeCategoryID] = modID;
					ModdedRecipes.Add(recipe.recipeCategoryID);
				}
			}
		}
		internal static void RegisterEntity(IEntityConfig def, KPrefabID prefabId)
		{
			RegisterEntity(def.GetType(), prefabId);
		}
		internal static void RegisterMultiEntity(IMultiEntityConfig def, KPrefabID prefabId)
		{
			RegisterEntity(def.GetType(), prefabId);
		}
		static void RegisterEntity(Type defType, KPrefabID prefabId)
		{
			if (AssemblyToModIdMap.TryGetValue(defType.Assembly, out string modID))
			{
				ModdedContentPrefabIdToModId[prefabId.PrefabTag] = modID;
			}
		}

		internal static void MapAssembliesToMods()
		{
			foreach (var mod in Global.Instance.modManager.mods)
			{
				if (!mod.IsEnabledForActiveDlc() || mod.loaded_mod_data == null || mod.loaded_mod_data.dlls == null)
					continue;

				foreach (var dll in mod.loaded_mod_data.dlls)
				{
					if (!AssemblyToModIdMap.ContainsKey(dll))
					{
						AssemblyToModIdMap.Add(dll, mod.staticID);
					}
					if (dll.FullName.Contains("PLib"))
					{
						SgtLogger.warning("the mod " + mod.title + " did not implement pLib properly as instructed and left the dll dangling in the mod folder instead of il-merging it.");
					}
				}
			}
		}


		static Color modLabelColor = UIUtils.rgb(52, 79, 103),
					 modLabelResearchColor = UIUtils.rgb(123, 177, 255);
		static Dictionary<string, string> modIdToNamesMap = [];

		internal static string GetModNameIfValid(KPrefabID def, int newLineCount) => def != null ? GetModNameIfValid(def.PrefabTag, newLineCount) : string.Empty;
		internal static string GetModNameIfValid(BuildingDef def, int newLineCount) => def != null ? GetModNameIfValid(def.PrefabID, newLineCount) : string.Empty;
		internal static string GetModNameIfValid(Tag tag, int newLineCount) => GetModNameIfValid(tag.ToString(), newLineCount);
		internal static string GetResearchModNameIfValid(string tag, int newLineCount) => GetModNameIfValid(tag, newLineCount, modLabelResearchColor);

		internal static string GetModName(string modId)
		{
			if (!modIdToNamesMap.TryGetValue(modId, out string modName))
			{
				var mod = Global.Instance.modManager.mods.FirstOrDefault(mod => mod.IsEnabledForActiveDlc() && mod.staticID == modId);
				//var randomizedColor = UIUtils.HSVShift(modLabel, new System.Random(modId.GetHashCode()).Next(100));
				if (mod != null)
				{
					modName = UIUtils.StripAllFormatting(mod.label.title);
				}
				else
					modName = modId;
				modName = UIUtils.ItalicText(modName);

				modIdToNamesMap[modId] = modName;
			}
			return modIdToNamesMap[modId];
		}

		internal static string GetModNameIfValid(string tag, int newLineCount, Color? colorOverride = null)
		{
			if (!colorOverride.HasValue)
				colorOverride = modLabelColor;
			var result = string.Empty;

			if (!IsModded(tag, out string modId))
				return result;

			string modName = GetModName(modId);

			string selectedInfoPrefix = TextEntity;
			if (ModdedBuildings.Contains(tag))
				selectedInfoPrefix = TextBuilding;
			else if (ModdedRecipes.Contains(tag))
				selectedInfoPrefix = TextRecipe;
			return new string('\n', newLineCount) + UIUtils.EmboldenText(string.Format("{0}\n{1}", selectedInfoPrefix, UIUtils.ColorText(modName, colorOverride.Value)));
		}

		static readonly string TextBuilding = new TranslatableTextBuilder("Modded Building, added by:")
				.Add("zh", "模组建筑，添加自：")
				.Add("de", "Mod-Gebäude, hinzugefügt von:")
				.Add("fr", "Équipement Moddé, ajouté par:")
				.Add("kr", "MOD 건물, 추가자:")
				.Add("ru", "Здание добавлено:")
				.Translate();

		static readonly string TextEntity = new TranslatableTextBuilder("Modded Content, added by:")
				.Add("zh", "模组内容，添加自：")
				.Add("de", "Mod-Content, hinzugefügt von:")
				.Add("fr", "Contenu Moddé, ajouté par:")
				.Add("kr", "MOD 엔티티, 추가자: ")
				.Add("ru", "Контент добавлен:")
				.Translate();

		static readonly string TextRecipe = new TranslatableTextBuilder("Modded Recipe, added by:")
				.Add("zh", "模组食谱容，添加自：")
				.Add("de", "Mod-Rezept, hinzugefügt von:")
				.Add("fr", "Recette Moddée, ajouté par:")
				.Add("kr", "MOD 레시피, 추가자:")
				.Add("ru", "Рецепт добавлен:")
				.Translate();

	}
}
