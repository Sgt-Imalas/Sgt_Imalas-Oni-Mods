using Database;
using HarmonyLib;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
		public static bool TryGetCollectionFor(string buildingID, out List<string> collection) => SkinCollection.SkinIds.TryGetValue(buildingID, out collection);

		public class SkinCollection
		{
			public static List<SkinCollection> SkinSets = new();
			public static Dictionary<string,List<string>> SkinIds = new();
			public class SkinEntry
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
			public List<SkinEntry> Skins=>skins;

			public static SkinCollection Create(string buildingID, string _subcategoryId) => new SkinCollection(buildingID, _subcategoryId);
			public static SkinCollection CategoryInit(string _mainCategory, string _subcategoryID, Sprite icon, int _sortkey) => new SkinCollection("", _subcategoryID).NewCategory(_mainCategory, icon, _sortkey);
			public SkinCollection(string buildingID, string _subcategoryID)
			{
				buildingId = buildingID;
				subcategoryID = _subcategoryID;
				skins = [];
				SkinSets.Add(this);
				if(buildingId.Any())
					SkinIds.Add(buildingId, []);
			}
			public SkinCollection NewCategory(string _mainCategory, Sprite icon, int _sortkey)
			{
				mainCategoryID = _mainCategory;
				newCategoryIcon = icon;
				sortkey = _sortkey;
				isMainCategory = true;
				return this;
			}
			public SkinCollection Skin(string Id, string name, string description, string kanimFile, Dictionary<string, string> workables = null)
			{
				skins.Add(new SkinEntry(Id, name, description, kanimFile, workables));
				SkinIds[buildingId].Add(Id);
				return this;
			}
			public void RegisterCategory()
			{
				string[] skinIDs = [.. skins.Select(entry => entry.ID)];
				if (isMainCategory)
				{
					SgtLogger.Assert("mainCategoryID",mainCategoryID);
					SgtLogger.Assert("subcategoryID",subcategoryID);
					SgtLogger.Assert("newCategoryIcon",newCategoryIcon);
					AddSubcategory(mainCategoryID, subcategoryID, newCategoryIcon, sortkey, skinIDs);
				}
				else
				{
					SgtLogger.Assert("subcategoryID", subcategoryID);
					AddItemsToSubcategory(subcategoryID, skinIDs);
				}
			}
			public void RegisterSkins(ResourceSet<BuildingFacadeResource> set)
			{
				foreach (var skin in skins)
				{
					set.resources.Add(new BuildingFacadeResource(skin.ID, skin.Name, skin.Description, PermitRarity.Universal, buildingId, skin.KanimFile, skin.Workables, null, null));
				}
			}
			public static void RegisterAllSkins()
			{
				SgtLogger.l("Patching Skin Injection..");
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
				SgtLogger.l("Registering " + SkinSets.Count + " skin collections");
				foreach (var collection in SkinSets)
				{
					SgtLogger.l("Registering skins for " + collection.buildingId);
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
		private static HashSet<string> AddOrGetSubCategory(string subCategory, string mainCategory = null, Sprite icon = null, int sortkey = 850)
		{
			if (!InventoryOrganization.subcategoryIdToPermitIdsMap.ContainsKey(subCategory))
			{
				if (mainCategory == null)
					mainCategory = "UNCATEGORIZED";
				if (icon == null)
					icon = Assets.GetSprite("unknown");


				InventoryOrganization.AddSubcategory(
					subCategory,
					icon,
					sortkey,
					[]);

				InventoryOrganization.categoryIdToSubcategoryIdsMap[mainCategory].Add(subCategory);
			}
			return InventoryOrganization.subcategoryIdToPermitIdsMap[subCategory];
		}
		public static void AddItemsToSubcategory(string subcategoryID, string[] permitIDs)
		{
			var categoryItems = AddOrGetSubCategory(subcategoryID);
			for (int i = 0; i < permitIDs.Length; i++)
			{
				categoryItems.Add(permitIDs[i]);
			}
		}

		public static void AddSubcategory(string mainCategory, string subcategoryID, Sprite icon, int sortkey, string[] permitIDs)
		{
			var categoryItems = AddOrGetSubCategory(subcategoryID, mainCategory, icon,sortkey);
			for (int i = 0; i < permitIDs.Length; i++)
			{
				categoryItems.Add(permitIDs[i]);
			}
		}
	}
}
