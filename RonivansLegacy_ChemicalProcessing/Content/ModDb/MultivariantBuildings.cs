using ElementUtilNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static HoverTextDrawer;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	public static class MultivariantBuildings
	{
		public static Dictionary<Tag, Dictionary<Tag, Tag>> BuildingVariantsByMaterial = new();
		public static Dictionary<Tag, Dictionary<string, Tag>> BuildingVariantsByFacade = new();

		public static Dictionary<Tag, Tag> ReverseMapParentBuildingByFacade = new();
		public static Dictionary<Tag, string> ReverseMapFacadeByChildBuilding = new();
		public static Dictionary<Tag, Tag> ReverseMapParentBuildingByMaterial = new();

		public static void RegisterSkinVariant(Tag parentBuildingId, Tag childBuildingID, string skinId)
		{
			SgtLogger.l("Registering Skin variant " + skinId + " for building " + parentBuildingId);

			if (!BuildingVariantsByFacade.ContainsKey(parentBuildingId))
				BuildingVariantsByFacade.Add(parentBuildingId, new Dictionary<string, Tag>());
			BuildingVariantsByFacade[parentBuildingId].Add(skinId, childBuildingID);

			ReverseMapParentBuildingByFacade[skinId] = parentBuildingId;
			ReverseMapFacadeByChildBuilding[childBuildingID] = skinId;
		}
		public static void RegisterMaterialVariant(Tag parentBuildingID, Tag childBuildingId, Tag material)
		{
			SgtLogger.l("Registering Material variant " + childBuildingId + " for building " + parentBuildingID+" for material "+material);
			if (!BuildingVariantsByMaterial.ContainsKey(parentBuildingID))
				BuildingVariantsByMaterial.Add(parentBuildingID, new Dictionary<Tag, Tag>());

			BuildingVariantsByMaterial[parentBuildingID].Add(material, childBuildingId);
			ReverseMapParentBuildingByMaterial[childBuildingId] = parentBuildingID;
		}
		public static bool HasMaterialVariant(Tag parent, Tag material, out Tag targetBuilding)
		{
			if (BuildingVariantsByMaterial.TryGetValue(parent, out var variants) && variants.TryGetValue(material, out targetBuilding))
				return true;
			targetBuilding = null;
			return false;
		}
		public static bool IsFacadeVariantParent(Tag parent, out string firstFacade)
		{
			firstFacade = null;
			bool isVariant = BuildingVariantsByFacade.TryGetValue(parent, out var items);
			if (isVariant && items.Any())
				firstFacade = items.First().Key;

			return !firstFacade.IsNullOrWhiteSpace();
		}

		public static bool HasFacadeVariant(Tag parent, string skinId, out Tag targetBuilding)
		{
			targetBuilding = null;
			if (skinId.IsNullOrWhiteSpace()) 
				return false;

			if (BuildingVariantsByFacade.TryGetValue(parent, out var variants) && variants.TryGetValue(skinId, out targetBuilding))
				return true;
			return false;
		}
		public static bool TryGetFacadeFromChild(Tag prefabID, out string skinId) => ReverseMapFacadeByChildBuilding.TryGetValue(prefabID, out skinId);
		public static bool IsFacadeVariant(Tag prefabID, out Tag targetBuilding) => ReverseMapParentBuildingByFacade.TryGetValue(prefabID, out targetBuilding);
		public static bool IsMaterialVariant(Tag prefabID, out Tag targetBuilding) => ReverseMapParentBuildingByMaterial.TryGetValue(prefabID, out targetBuilding);
	}
}
