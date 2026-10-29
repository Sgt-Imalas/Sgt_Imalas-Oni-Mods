using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Patches
{
	internal class MaterialSelectorPanel_Patches
	{

		[HarmonyPatch(typeof(MaterialSelectionPanel), nameof(MaterialSelectionPanel.RefreshSelectors))]
		public class MaterialSelectionPanel_RefreshSelectors_Patch
		{
			static readonly Dictionary<string, bool> _isRocketModuleCache = new();

			public static void Postfix(MaterialSelectionPanel __instance)
			{
				if (__instance == null || __instance.gameObject.activeInHierarchy == false || __instance.activeRecipe == null)
					return;

				BuildingDef buildingDef = __instance.activeRecipe.GetBuildingDef();
				if (buildingDef == null || buildingDef.BuildingComplete == null)
					return;

				string prefabId = buildingDef.PrefabID;

				if (!_isRocketModuleCache.TryGetValue(prefabId, out bool isRocketModule))
				{ 
					isRocketModule = buildingDef.BuildingComplete.TryGetComponent<RocketModuleCluster>(out _);
					_isRocketModuleCache[prefabId] = isRocketModule;
				}

				if (!isRocketModule)
					return;
				__instance.ResearchRequired.AddOrGet<GoToBlockingLocationHandler>();
			}
		}
	}
}
