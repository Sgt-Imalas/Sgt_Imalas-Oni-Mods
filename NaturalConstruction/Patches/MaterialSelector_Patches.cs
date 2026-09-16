using HarmonyLib;
using NaturalConstruction.Content.Scripts;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UtilLibs;

namespace NaturalConstruction.Patches
{
	internal class MaterialSelector_Patches
	{
        static ConditionalWeakTable<MaterialSelector, MaterialFilterBar> SearchBars = [];

        /// <summary>
        /// happens after all the toggles have been created and put into the correct order
        /// </summary>
        [HarmonyPatch(typeof(MaterialSelector), nameof(MaterialSelector.UpdateScrollBar))]
        public class MaterialSelector_UpdateScrollBar_Patch
        {
            public static void Prefix(MaterialSelector __instance)
			{
				if (!SearchBars.TryGetValue(__instance, out MaterialFilterBar searchbar))
					return;

				if (!searchbar.HasFilter())
					return;


				foreach (KeyValuePair<Tag, KToggle> elementToggle in __instance.ElementToggles)
				{
					if (elementToggle.Value.gameObject.activeSelf && !searchbar.IsTagWithinFilters(elementToggle.Key))
					{
						elementToggle.Value.gameObject.SetActive(false);
					}
				}
			}
        }

        [HarmonyPatch(typeof(MaterialSelector), nameof(MaterialSelector.ConfigureScreen))]
        public class MaterialSelector_ConfigureScreen_Patch
        {
            public static void Postfix(MaterialSelector __instance)
            {
                bool isSearchableTag = __instance.ElementToggles.Count > 20;

				if (!SearchBars.TryGetValue(__instance, out var searchbar))
				{
					searchbar = AddSearchBarToSelector(__instance);
					SearchBars.Add(__instance, searchbar);
				}
				searchbar.Clear();
				searchbar.gameObject.SetActive(isSearchableTag);

			}

			private static MaterialFilterBar AddSearchBarToSelector(MaterialSelector instance)
			{
                if(SearchBarPrefab == null)
				{
					var prefabGo = ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject;
                    if(prefabGo == null)
                    {
                        SgtLogger.error("RetiredColonyInfoScreen was null!");
                        return null;
					}
					var searchBar = prefabGo.transform.Find("Content/ColonyData/Colonies and Achievements/Colonies/Search");
					if (searchBar == null)
					{
						SgtLogger.error("RetiredColonyInfoScreen.Searchbar was not found!");
						return null;
					}
                    SearchBarPrefab = Util.KInstantiateUI(searchBar.gameObject);
				}

                var searchbarGO = Util.KInstantiateUI(SearchBarPrefab, instance.gameObject, true);
                var search = searchbarGO.AddOrGet<MaterialFilterBar>();
                search.Init(instance);


				searchbarGO.transform.SetSiblingIndex(instance.transform.Find("Container").transform.GetSiblingIndex());

				return search;

			}
		}
        static GameObject SearchBarPrefab = null;
	}
}
