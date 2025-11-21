using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static STRINGS.BUILDING.STATUSITEMS.ACCESS_CONTROL;

namespace PedestalFilter
{
	internal class Patches
	{
		static bool HasFilterText => SearchBar != null && !string.IsNullOrEmpty(SearchBar.text);
		static public KScreen CurrentScreen = null;
		static KInputTextField SearchBar => CurrentScreen != null && ScreenToState.TryGetValue(CurrentScreen, out var state) ? state.SearchBar : null;

		static Dictionary<Tag, Color> TagToElementColorMap = [];
		static Dictionary<Tag, string> TagToNameMap = [];

		class UIState
		{
			public KInputTextField SearchBar;
		}
		static Dictionary<KScreen, UIState> ScreenToState = [];



		bool ClearPending = false;
		public static bool EditingSearch = false;



		[HarmonyPatch(typeof(Game), nameof(Game.OnLoadLevel))]
		public class Game_OnLoadLevel_Patch
		{
			public static void Postfix()
			{
				ScreenToState.Clear();
			}
		}

		public static void StartEditing(string ac)
		{
			EditingSearch = true;
		}
		public static void StopEditing(string ac)
		{
			EditingSearch = false;
		}

		[HarmonyPatch(typeof(KScreenManager), nameof(KScreenManager.OnKeyDown))]
		public static class ConsumeInputs
		{
			public static void Prefix(KScreen __instance, KButtonEvent e)
			{
				if (EditingSearch)
					e.Consumed = true;
			}
		}



		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}

		static bool BlockTMPEvents = false;
		static void OnSearchTextChanged(ReceptacleSideScreen __instance)
		{
			if (__instance == CurrentScreen && !BlockTMPEvents)
			{
				__instance.UpdateAvailableAmounts(null);
			}
		}
		public static void ClearSearchBar()
		{
			if (SearchBar != null)
			{
				BlockTMPEvents = true;
				SearchBar.text = string.Empty;
				BlockTMPEvents = false;
			}
		}
		static KInputTextField AddOrGetSearchBar(ReceptacleSideScreen __instance)
		{
			CurrentScreen = __instance;
			if (SearchBar != null && !SearchBar.gameObject.IsNullOrDestroyed())
			{
				//FilterArtifacts = Config.Instance.DefaultToArtifactsOnly && __instance.GetType() == typeof(ReceptacleSideScreen);
				//if (ArtifactFilter != null)
				//{
				//	if (ArtifactFilter.bgImage != null || ArtifactFilter.GetComponent<KImage>() != null)
				//	{
				//		ArtifactFilter?.UpdateColor(true, FilterArtifacts, false);
				//	}
				//}

				ClearSearchBar();
				return SearchBar;
			}
			else
				ScreenToState.Remove(__instance);


			var searchbarPreset = PlanScreen.Instance.recipeInfoScreenParent.transform.Find("BuildingGroups/Searchbar").gameObject;
			PlanScreen.Instance.recipeInfoScreenParent.transform.Find("BuildingGroups").TryGetComponent<BuildingGroupScreen>(out var screen);

			GameObject searchbar = Util.KInstantiateUI(searchbarPreset, __instance.gameObject, true);
			searchbar.transform.SetAsFirstSibling();



			ScreenToState[__instance] = new() { SearchBar = searchbar.transform.Find("FilterInputField").GetComponent<KInputTextField>() };
			SearchBar.text = string.Empty;
			SearchBar.onValueChanged.AddListener((text) => OnSearchTextChanged(__instance));
			SearchBar.onSelect.AddListener(StartEditing);
			SearchBar.onSelect.AddListener((a) => __instance.isEditing = true);
			SearchBar.onDeselect.AddListener(StopEditing);
			SearchBar.onDeselect.AddListener(a => __instance.isEditing = false);
			SearchBar.placeholder.TryGetComponent<TMPro.TextMeshProUGUI>(out var textMesh);
			textMesh.text = STRINGS.PEDESTALSEARCHBAR.FILTER_FILLERTEXT;
			__instance.ConsumeMouseScroll = true;

			UIUtils.AddActionToButton(searchbar.transform, "ClearSearchButton", () => { ClearSearchBar(); __instance.RefreshToggleStates(); });
			searchbar.transform.Find("ListViewButton").gameObject.SetActive(false);
			//secondaryButton.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("ic_artifacts");
			//ArtifactFilter = secondaryButton.gameObject.GetComponent<KButton>();
			//ArtifactFilter.onClick += () =>
			//{
			//	FilterArtifacts = !FilterArtifacts;
			//	__instance.UpdateAvailableAmounts(null);
			//};
			//ArtifactFilter.onPointerExit += () =>
			//{
			//	if (ArtifactFilter != null)
			//	{
			//		if (ArtifactFilter.bgImage != null || ArtifactFilter.GetComponent<KImage>() != null)
			//		{
			//			ArtifactFilter?.UpdateColor(true, FilterArtifacts, false);
			//		}
			//	}
			//};
			//secondaryButton.gameObject.GetComponentInChildren<ToolTip>().SetSimpleTooltip(STRINGS.PEDESTALSEARCHBAR.FILTER_ARTIFACTS);

			searchbar.transform.Find("GridViewButton").gameObject.SetActive(false);
			return SearchBar;
		}

		[HarmonyPatch(typeof(ReceptacleSideScreen), nameof(ReceptacleSideScreen.SetTarget))]
		public static class InitializeSearchbar
		{
			public static void Prefix(ReceptacleSideScreen __instance)
			{
				AddOrGetSearchBar(__instance);
			}
		}
		static bool FastTrackFound = false;

		public delegate void FastTrackVSDelegate(object instance);
		[HarmonyPatch(typeof(ReceptacleSideScreen), nameof(ReceptacleSideScreen.ConfigureActiveEntity))]
		public static class TintSelected
		{
			public static void Postfix(ReceptacleSideScreen __instance, Tag tag)
			{
				CurrentScreen = __instance;
				if (TagToElementColorMap.ContainsKey(tag))
				{
					__instance.activeEntityContainer.transform.GetChild(0).gameObject.GetComponentInChildrenOnly<Image>().color = TagToElementColorMap[tag];
				}
			}
		}


		[HarmonyPatch(typeof(SingleEntityReceptacle), nameof(SingleEntityReceptacle.IsValidEntity))]
		public class SingleEntityReceptacle_IsValidEntity_Patch
		{
			[HarmonyPriority(Priority.Low)]
			public static void Postfix(SingleEntityReceptacle __instance, ref bool __result, GameObject candidate)
			{
				if (__result)
					return;

				if (!Game.IsCorrectDlcActiveForCurrentSave(candidate.GetComponent<KPrefabID>()))
				{
					return;
				}

				if (__instance.additionalCriteria.Any())
					return;

				///Allow flipped plant seeds in pedestals
				if (candidate.TryGetComponent<IReceptacleDirection>(out var direction) && (__instance.rotatable == null || __instance.rotatable.permittedRotations != PermittedRotations.FlipV))
				{
					__result = true;
				}
			}
		}


		[HarmonyPatch(typeof(ReceptacleSideScreen), nameof(ReceptacleSideScreen.UpdateAvailableAmounts))]
		public static class ReceptacleSideScreen_UpdateAvailableAmounts
		{
			public static bool Prefix(ReceptacleSideScreen __instance)
			{
				CurrentScreen = __instance;
				//if (__instance.GetType().IsSubclassOf(typeof(ReceptacleSideScreen)))
				//	return true;
				return __instance.requestObjectListContainer.GetComponent("VirtualScroll") != null;

			}

			[HarmonyPostfix]
			[HarmonyPriority(90)]
			public static void Postfix(ReceptacleSideScreen __instance, ref bool __result)
			{



				//if (__instance.GetType().IsSubclassOf(typeof(ReceptacleSideScreen)))
				//	return;
				CurrentScreen = __instance;

				if (true)
				{
					foreach (Tag possibleTag in __instance.targetReceptacle.possibleDepositObjectTags)
					{
						if (__instance.contentContainers.TryGetValue(possibleTag, out var container) && container.TryGetComponent<HierarchyReferences>(out var hr))
						{
							bool active = HasFilterText ? true : __instance.categoryExpandedStatus[possibleTag];
							hr.GetReference<MultiToggle>("HeaderToggle").ChangeState(active ? 0 : 1);
							hr.GetReference<GridLayoutGroup>("GridLayout").gameObject.SetActive(active);
						}
					}
				}

				Component fastTrackVirtualScroll = __instance.requestObjectListContainer.GetComponent("VirtualScroll");
				FastTrackFound = (fastTrackVirtualScroll != null);

				//ArtifactFilter?.UpdateColor(true, FilterArtifacts, false);

				string filterText = SearchBar != null ? SearchBar.text.ToLowerInvariant() : string.Empty;

				//if (!FilterArtifacts && FastTrackFound && !HasFilterText)
				//    return true;

				Tag tag;
				bool result = false;
				bool hasChanged = false;
				bool debugActive = DebugHandler.InstantBuildMode;
				//SgtLogger.l("running filter");
				bool forceActive =
					debugActive ||
					!__instance.hideUndiscoveredEntities;
				var inst = DiscoveredResources.Instance;
				ReceptacleToggle selected = __instance.selectedEntityToggle;

				if (__instance == null || __instance.targetReceptacle == null || __instance.targetReceptacle.GetMyWorld() == null)
				{
					return;
				}

				foreach (var pair in __instance.depositObjectMap)
				{
					var key = pair.Key;
					var display = pair.Value;
					tag = display.tag;
					var go = key.gameObject;
					bool currentlyActive = go.activeSelf;
					bool shouldBeActive = true;

					//Cache item name
					if (!TagToNameMap.ContainsKey(tag))
					{
						Element element = ElementLoader.GetElement(tag);
						var item = Assets.GetPrefab(tag);

						if (element != null)
						{
							TagToNameMap[tag] = element.name.ToLowerInvariant();
							if (element.state != Element.State.Solid)
							{
								Color col = element.substance.colour;
								col.a = 1;

								TagToElementColorMap[tag] = col;
							}
						}
						else if (item != null)
							TagToNameMap[tag] = item.GetProperName().ToLowerInvariant();
						else
							TagToNameMap[tag] = tag.ToString().ToLowerInvariant();
					}


					shouldBeActive = FastTrackFound ? currentlyActive : forceActive || inst.IsDiscovered(tag);


					if (shouldBeActive && HasFilterText)
					{
						if (!TagToNameMap[tag].Contains(filterText))
						{
							shouldBeActive = false;
						}
					}

					if (currentlyActive != shouldBeActive)
					{
						//SgtLogger.l($"changing {tag} from {currentlyActive} to {shouldBeActive}");
						if (!hasChanged && FastTrackFound)
						{
							Traverse.Create(fastTrackVirtualScroll).Method("OnBuild").GetValue();
							hasChanged = true;
						}

						go.SetActive(shouldBeActive);
					}
					if (shouldBeActive && TagToElementColorMap.ContainsKey(tag) && TagToElementColorMap[tag] != Color.white)
						key.image.color = TagToElementColorMap[tag];


					// only update the state of active items and only if fast track is not active
					if (shouldBeActive && !FastTrackFound)
					{
						var toggle = key.toggle;
						float availableAmount = __instance.GetAvailableAmount(tag);
						if (!Mathf.Approximately(display.lastAmount, availableAmount))
						{
							result = true;
							// Update display only if it actually changed
							display.lastAmount = availableAmount;
							key.amount.text = availableAmount.ToString();
						}

						if (!__instance.ValidRotationForDeposit(display.direction) ||
								availableAmount <= 0.0f)
							// Disable items which cannot fit in this orientation or are
							// unavailable
							__instance.SetToggleState(toggle, selected != key ? ImageToggleState.State.Disabled : ImageToggleState.State.DisabledActive);
						else if (selected != key)
							__instance.SetToggleState(toggle, ImageToggleState.State.Inactive);
						else
							__instance.SetToggleState(toggle, ImageToggleState.State.Active);
					}
				}

				//if __result == true, fast track has already queued a rebuild, doubling that would cause issues
				if (hasChanged && !__result)
				{
					Traverse.Create(fastTrackVirtualScroll).Method("Rebuild").GetValue();
				}

				__result = result;
			}
		}
	}
}
