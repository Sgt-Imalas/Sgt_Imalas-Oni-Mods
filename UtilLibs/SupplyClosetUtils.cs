using Database;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IceCooledFan.States;
using static InventoryOrganization;
using static LogicGate.LogicGateDescriptions;
using static ResearchTypes;

namespace UtilLibs
{
	public static class SupplyClosetUtils
	{
		public class SkinCollection
		{
			public static List<SkinCollection> SkinSets = new();
			protected class SkinEntry
			{
				public string ID, Name, Description, KanimFile;
				public Dictionary<string, string> Workables;
				public SkinEntry(string id, string name, string desc, string kanim, Dictionary<string, string> workables = null)
				{
					ID = id;
					Name = name;
					Description = desc;
					KanimFile = kanim;
					Workables = workables;
				}
			}

			private bool isMainCategory = false;
			private string subcategoryID, mainCategoryID, buildingId;
			Sprite newCategoryIcon = null;
			int sortkey = -1;
			private List<SkinEntry> skins;

			public static SkinCollection Create(string buildingID) => new SkinCollection(buildingID);
			public SkinCollection(string buildingID)
			{
				buildingId = buildingID; 
				skins = []; 
				SkinSets.Add(this); }
			public SkinCollection AddAsNew(string _mainCategory, string _subcategoryID, Sprite icon, int _sortkey)
			{
				mainCategoryID = _mainCategory;
				subcategoryID = _subcategoryID;
				newCategoryIcon = icon;
				sortkey = _sortkey;
				isMainCategory = true;
				return this;
			}
			public SkinCollection AddToExisting(string _subcategoryID)
			{
				subcategoryID = _subcategoryID;
				return this;
			}
			public SkinCollection Skin(string Id, string name, string description, string kanimFile, Dictionary<string, string> workables = null)
			{
				skins.Add(new SkinEntry(Id, name, description, kanimFile, workables));
				return this;
			}
			public void RegisterCategory()
			{
				if (!skins.Any())
					return;

				string[] skinIDs = [.. skins.Select(entry => entry.ID)];
				if (isMainCategory)
				{
					SgtLogger.Assert(mainCategoryID, "mainCategoryID");
					SgtLogger.Assert(subcategoryID, "subcategoryID");
					SgtLogger.Assert(newCategoryIcon, "newCategoryIcon");
					AddSubcategory(mainCategoryID, subcategoryID, newCategoryIcon, sortkey, skinIDs);
				}
				else
				{
					SgtLogger.Assert(subcategoryID, "subcategoryID");
					AddItemsToSubcategory(subcategoryID, skinIDs);
				}
			}
			public void RegisterSkins(ResourceSet<BuildingFacadeResource> set)
			{
				if (!skins.Any())
					return;

				foreach (var skin in skins)
				{
					set.resources.Add(new BuildingFacadeResource(skin.ID, skin.Name, skin.Description, PermitRarity.Universal, buildingId, skin.KanimFile, skin.Workables, null, null));
				}
			}
			public static void RegisterAllSkins()
			{
				var harmony = new Harmony("SgtImalas_SupplyClosetUtils");
				{
					var targetType = AccessTools.TypeByName("Database.BuildingFacades");
					var target = AccessTools.Constructor(targetType, [typeof(ResourceSet)]);
					var postfix = AccessTools.Method(typeof(SkinCollection), nameof(BuildingFacades_Postfix));
					harmony.Patch(target, postfix: new HarmonyMethod(postfix));
				}
				{
					var targetType = AccessTools.TypeByName("InventoryOrganization");
					var target = AccessTools.Method(targetType, "GenerateSubcategories");
					var postfix = AccessTools.Method(typeof(SkinCollection), nameof(Subcategories_Postfix));
					harmony.Patch(target, postfix: new HarmonyMethod(postfix));
				}
			}
			public static void BuildingFacades_Postfix(object __instance)
			{
				var resource = (ResourceSet<BuildingFacadeResource>)__instance;
				foreach (var collection in SkinSets)
				{
					collection.RegisterSkins(resource);
				}
			}
			public static void Subcategories_Postfix()
			{
				foreach (var collection in SkinSets)
				{
					collection.RegisterCategory();
				}
			}
		}

		public static void AddItemsToSubcategory(string subcategoryID, string[] permitIDs)
		{
			if (!subcategoryIdToPermitIdsMap.ContainsKey(subcategoryID))
			{
				SgtLogger.error("Supply Closet Item subcategory not found! Use AddSubcategory instead");
			}
			else
			{
				for (int i = 0; i < permitIDs.Length; i++)
				{
					subcategoryIdToPermitIdsMap[subcategoryID].Add(permitIDs[i]);
				}
			}
		}

		public static void AddSubcategory(string mainCategory, string subcategoryID, Sprite icon, int sortkey, string[] permitIDs)
		{

			if (categoryIdToSubcategoryIdsMap.ContainsKey(mainCategory))
			{
				if (!subcategoryIdToPermitIdsMap.ContainsKey(subcategoryID))
				{
					subcategoryIdToPresentationDataMap.Add(subcategoryID, new SubcategoryPresentationData(subcategoryID, icon, sortkey));
					subcategoryIdToPermitIdsMap.Add(subcategoryID, new HashSet<string>());

					if (!categoryIdToSubcategoryIdsMap[mainCategory].Contains(subcategoryID))
						categoryIdToSubcategoryIdsMap[mainCategory].Add(subcategoryID);
					else
						SgtLogger.warning("How did this happen? subcategory is registered to this main category but didnt exist?!");

				}
				else
				{
					SgtLogger.warning("Supply Closet Item subcategory already existing! Use AddItemsToSubcategory instead");
				}
				for (int i = 0; i < permitIDs.Length; i++)
				{
					subcategoryIdToPermitIdsMap[subcategoryID].Add(permitIDs[i]);
				}
			}
			else
				SgtLogger.error("Supply Closet Main Category not found!");
		}
	}
}
