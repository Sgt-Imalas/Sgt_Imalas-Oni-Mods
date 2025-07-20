using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace PedestalFilter
{
	internal class Patches
	{
		static KInputTextField SearchBar = null;
		static KButton ArtifactFilter;
		static bool TmpDisable = false;

		bool ClearPending = false;
		public static bool EditingSearch = false;
		public static bool FilterArtifacts = Config.Instance.DefaultToArtifactsOnly;
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

		[HarmonyPatch(typeof(ReceptacleSideScreen), nameof(ReceptacleSideScreen.Initialize))]
		public static class InitializeSearchbar
		{
			static void OnSearchTextChanged(ReceptacleSideScreen __instance)
			{
				__instance.UpdateAvailableAmounts(null);
			}
			public static void ClearSearchBar()
			{
				if (SearchBar != null)
				{
					SearchBar.text = string.Empty;
				}
			}
			public static void Postfix(ReceptacleSideScreen __instance)
			{
				//if (__instance.GetType().IsSubclassOf(typeof(ReceptacleSideScreen)))
				//	return;


				if (SearchBar != null)
				{
					FilterArtifacts = Config.Instance.DefaultToArtifactsOnly;
					if (ArtifactFilter != null)
					{
						if (ArtifactFilter.bgImage != null || ArtifactFilter.GetComponent<KImage>() != null)
						{
							ArtifactFilter?.UpdateColor(true, FilterArtifacts, false);
						}
					}

					ClearSearchBar();
					return;
				}
				var searchbarPreset = PlanScreen.Instance.recipeInfoScreenParent.transform.Find("BuildingGroups/Searchbar").gameObject;
				PlanScreen.Instance.recipeInfoScreenParent.transform.Find("BuildingGroups").TryGetComponent<BuildingGroupScreen>(out var screen);

				GameObject searchbar = Util.KInstantiateUI(searchbarPreset, __instance.gameObject, true);
				searchbar.transform.SetAsFirstSibling();
				SearchBar = searchbar.transform.Find("FilterInputField").GetComponent<KInputTextField>();
				SearchBar.text = string.Empty;
				SearchBar.onValueChanged.AddListener((text) => OnSearchTextChanged(__instance));
				SearchBar.onSelect.AddListener(StartEditing);
				SearchBar.onSelect.AddListener((a) => __instance.isEditing = true);
				SearchBar.onDeselect.AddListener(StopEditing);
				SearchBar.onDeselect.AddListener(a => __instance.isEditing = false);
				SearchBar.placeholder.TryGetComponent<TMPro.TextMeshProUGUI>(out var textMesh);
				textMesh.text = STRINGS.PEDESTALSEARCHBAR.FILTER_FILLERTEXT;
				__instance.ConsumeMouseScroll = true;

				UIUtils.AddActionToButton(searchbar.transform, "ClearSearchButton", () => ClearSearchBar());
				var secondaryButton = searchbar.transform.Find("ListViewButton"); //.gameObject.SetActive(false);
				secondaryButton.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("ic_artifacts");
				ArtifactFilter = secondaryButton.gameObject.GetComponent<KButton>();
				ArtifactFilter.onClick += () =>
				{
					FilterArtifacts = !FilterArtifacts;
					__instance.UpdateAvailableAmounts(null);
				};
				ArtifactFilter.onPointerExit += () =>
				{
					if (ArtifactFilter != null)
					{
						if (ArtifactFilter.bgImage != null || ArtifactFilter.GetComponent<KImage>() != null)
						{
							ArtifactFilter?.UpdateColor(true, FilterArtifacts, false);
						}
					}
				};
				secondaryButton.gameObject.GetComponentInChildren<ToolTip>().SetSimpleTooltip(STRINGS.PEDESTALSEARCHBAR.FILTER_ARTIFACTS);

				//var tmpBtn = searchbar.transform.Find("GridViewButton"); //.gameObject.SetActive(false);
				//secondaryButton.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("ic_artifacts");
				//var tmpBt = tmpBtn.gameObject.GetComponent<KButton>();
				//tmpBt.onClick += () =>
				//{
				//    TmpDisable = !TmpDisable;
				//    __instance.UpdateAvailableAmounts(null);
				//};
				//tmpBt.onPointerExit += () =>
				//{
				//    tmpBt.UpdateColor(true, TmpDisable, false);
				//}
				//;
				searchbar.transform.Find("GridViewButton").gameObject.SetActive(false);

				initializing = true;
			}
		}
		static Dictionary<Tag, Color> TagToElementColorMap = new();
		static Dictionary<Tag, string> TagToNameMap = new();
		static Dictionary<Tag, bool> TagIsArtifactMap = new();

		static bool FastTrackFound = false;
		static bool initializing;

		public delegate void FastTrackVSDelegate(object instance);
		[HarmonyPatch(typeof(ReceptacleSideScreen), nameof(ReceptacleSideScreen.ConfigureActiveEntity))]
		public static class TintSelected
		{
			public static void Postfix(ReceptacleSideScreen __instance, Tag tag)
			{
				if (TagToElementColorMap.ContainsKey(tag))
				{
					__instance.activeEntityContainer.transform.GetChild(0).gameObject.GetComponentInChildrenOnly<Image>().color = TagToElementColorMap[tag];
				}
			}
		}

		[HarmonyPatch(typeof(ReceptacleSideScreen), nameof(ReceptacleSideScreen.UpdateAvailableAmounts))]
		public static class ReceptacleSideScreen_UpdateAvailableAmounts
		{
			public static bool Prefix(ReceptacleSideScreen __instance)
			{
				//if (__instance.GetType().IsSubclassOf(typeof(ReceptacleSideScreen)))
				//	return true;

				return __instance.requestObjectList.GetComponent("VirtualScroll") != null;
			}

			[HarmonyPostfix]
			[HarmonyPriority(90)]
			public static void Postfix(ReceptacleSideScreen __instance, ref bool __result)
			{
				//if (__instance.GetType().IsSubclassOf(typeof(ReceptacleSideScreen)))
				//	return;


				Component fastTrackVirtualScroll = __instance.requestObjectList.GetComponent("VirtualScroll");
				FastTrackFound = (fastTrackVirtualScroll != null);

				ArtifactFilter?.UpdateColor(true, FilterArtifacts, false);

				string filterText = SearchBar != null ? SearchBar.text.ToLowerInvariant() : string.Empty;
				bool HasFilterText = !string.IsNullOrEmpty(filterText);

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
							TagToNameMap[tag] = tag.ToString();
					}


					shouldBeActive = FastTrackFound ? currentlyActive : forceActive || inst.IsDiscovered(tag);

					if (shouldBeActive && FilterArtifacts)
					{
						if (!TagIsArtifactMap.ContainsKey(tag))
						{
							string artifactId = tag.ToString().ToLowerInvariant();
							TagIsArtifactMap[tag] = (artifactId.Contains("artifact_") || artifactId.Contains("keepsake_"));
						}
						if (!TagIsArtifactMap[tag])
						{
							shouldBeActive = false;
						}
					}

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
							SetImageToggleState(tag, __instance, toggle, selected != key
								? ImageToggleState.State.Disabled
								: ImageToggleState.State.DisabledActive);
						else if (selected != key)
							SetImageToggleState(tag, __instance, toggle, ImageToggleState.State.Inactive);
						else
							SetImageToggleState(tag, __instance, toggle, ImageToggleState.State.Active);
					}
				}

				//if __result == true, fast track has already queued a rebuild, doubling that would cause issues
				if (hasChanged && !__result)
				{
					Traverse.Create(fastTrackVirtualScroll).Method("Rebuild").GetValue();
				}

				__result = result;


				initializing = false;
			}

			static Dictionary<Tag, ImageToggleState.State> PrevToggleStates = new();
			private static void SetImageToggleState(Tag targetTag, ReceptacleSideScreen instance,
					KToggle toggle, ImageToggleState.State state)
			{
				bool newlyAdded = !PrevToggleStates.ContainsKey(targetTag);

				if (newlyAdded)
				{
					PrevToggleStates[targetTag] = state;
				}

				if (toggle.TryGetComponent(out ImageToggleState its)
				   //&& (initializing ||state != its.currentState)
				   && (newlyAdded || PrevToggleStates[targetTag] != state)
				   )
				{
					// SetState provides no feedback on whether the state actually changed
					var targetImage = toggle.gameObject.GetComponentInChildrenOnly<Image>();
					switch (state)
					{
						case ImageToggleState.State.Disabled:
							its.SetDisabled();
							targetImage.material = instance.desaturatedMaterial;
							break;
						case ImageToggleState.State.Inactive:
							its.SetInactive();
							targetImage.material = instance.defaultMaterial;
							break;
						case ImageToggleState.State.Active:
							its.SetActive();
							targetImage.material = instance.defaultMaterial;
							break;
						case ImageToggleState.State.DisabledActive:
							its.SetDisabledActive();
							targetImage.material = instance.desaturatedMaterial;
							break;
					}
				}
			}
		}
	}
}
