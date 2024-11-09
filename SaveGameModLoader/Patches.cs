using HarmonyLib;
using Klei;
using KMod;
using KSerialization;
using PeterHan.PLib.UI;
using SaveGameModLoader.ModFilter;
using SaveGameModLoader.ModsFilter;
using SaveGameModLoader.Patches;
using SaveGameModLoader.UIComponents;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.ModSyncing;
using UtilLibs.UIcmp;
using YamlDotNet;
using static ModsScreen;
using static SaveGameModLoader.ModAssets;
using static SaveGameModLoader.STRINGS.UI.FRONTEND.MODTAGS;

namespace SaveGameModLoader
{
	class AllPatches
	{
		[HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
		public static class OnASsetPrefabPatch
		{
			public static void Postfix()
			{
				LoadModConfigPatch.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
			}
		}

		[HarmonyPatch(typeof(LoadScreen), nameof(LoadScreen.ShowColony))]
		public static class AddModSyncButtonLogic
		{
			public static void Postfix(LoadScreen __instance, List<LoadScreen.SaveGameFileDetails> saves, int selectIndex = -1)
			{
                __instance.colonyViewRoot.TryGetComponent<HierarchyReferences>(out var hierarchyReferences);
                var container = hierarchyReferences.GetReference<RectTransform>("Content").transform; //save entry container

                int saveCount = saves.Count;
				int rectTransformCount = container.childCount;

				int childrenOffset = rectTransformCount - saveCount;
				SgtLogger.l("childrenOffset " + childrenOffset);
				SgtLogger.l("rectTransformCount " + rectTransformCount);
				SgtLogger.l("saveCount " + saveCount);

                for (int i = rectTransformCount - 1; i >= childrenOffset; i--)
				{
					var saveEntryTransform = container.GetChild(i);
					var saveEntry = saves[i - childrenOffset];

					InsertModButtonCode(saveEntryTransform.rectTransform(), saveEntry);
                }
            }



			public static void InsertModButtonCode(
				  RectTransform entry
				, LoadScreen.SaveGameFileDetails FileDetails
				)
			{

				string baseName = FileDetails.BaseName; 
				string fileName = FileDetails.FileName;

				var btn = entry.Find("SyncButton").GetComponent<KButton>();

				if (btn != null)
				{
					var colonyList = ModlistManager.Instance.TryGetColonyModlist(baseName);

					btn.isInteractable = colonyList != null && colonyList.TryGetModListEntry(fileName, out _) && App.GetCurrentSceneName() == "frontend";
					btn.onClick += (() =>
					{
						ModlistManager.Instance.InstantiateModViewForPathOnly(fileName);
					});
				}
			}
			//public static readonly MethodInfo ButtonLogic = AccessTools.Method(
			//   typeof(AddModSyncButtonLogic),
			//   nameof(InsertModButtonCode));

			//private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
			//		typeof(KButton),
			//		nameof(KButton.ClearOnClick));

			//private static readonly MethodInfo SaveGameFileIndexFinder = AccessTools.Method(
			//		typeof(System.IO.Path),
			//		nameof(System.IO.Path.GetFileNameWithoutExtension), [ typeof(string) ]);

			////private static MethodInfo TransformIndexFinder =
			////    AccessTools.Method(
			////    typeof(UnityEngine.Object),
			////    nameof(UnityEngine.Object.Instantiate),
			////    new System.Type[] { typeof(UnityEngine.Object), typeof(UnityEngine.Transform)}); 


			//static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			//{
			//	///
			//	var Methods = typeof(UnityEngine.Object).GetPublicStaticMethods();

			//	var GenericMethodInfo = Methods.FirstOrDefault(meth =>
			//	meth.Name.Contains("Instantiate")
			//	&& meth.GetParameters().Length == 2
			//	&& meth.GetParameters().Last().ParameterType == typeof(UnityEngine.Transform)
			//	&& meth.GetParameters().First().ParameterType.IsGenericParameter
			//	).MakeGenericMethod(typeof(RectTransform));


			//	//SgtLogger.l(GenericMethodInfo.Name + "::" + GenericMethodInfo, "postselect");
			//	///


			//	var code = instructions.ToList();
			//	var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SuitableMethodInfo);


			//	var SaveGameFileIndexFinderStart = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == SaveGameFileIndexFinder);
			//	var saveFileRootIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, SaveGameFileIndexFinderStart);

			//	var TransformIndexFinderStart = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == GenericMethodInfo);// code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand.ToString().Contains("Instantiate"));


			//	var TransformIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, TransformIndexFinderStart, false);
			//	//SgtLogger.l(TransformIndex + "", "TRANSFORMINDEX");

			//	//foreach (var v in code) { SgtLogger.log(v.opcode + " -> " + v.operand); };
			//	if (insertionIndex != -1)
			//	{
			//		insertionIndex += 1;
			//		code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, TransformIndex));//7
			//		code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, saveFileRootIndex));//6
			//		code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ButtonLogic));

			//		//TranspilerHelper.PrintInstructions(code,true);
			//	}
			//	//foreach (var v in code) { SgtLogger.log(v.opcode + " -> " + v.operand); };

			//	return code;
			//}
		}


		//[HarmonyPatch(typeof(ModsScreen), nameof(ModsScreen.BuildDisplay))]
		//[HarmonyPriority(Priority.HigherThanNormal)]
		public static class ModsScreen_BuildDisplay_Patch_Pin_Button
		{
			public static void ExecutePatch(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("ModsScreen, Assembly-CSharp:BuildDisplay");
				var m_Prefix = AccessTools.Method(typeof(ModsScreen_BuildDisplay_Patch_Pin_Button), "Prefix");
				var m_Postfix = AccessTools.Method(typeof(ModsScreen_BuildDisplay_Patch_Pin_Button), "Postfix");
				harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix, Priority.HigherThanNormal), new HarmonyMethod(m_Postfix, Priority.LowerThanNormal), null);
			}


			static ColorStyleSetting blue = null;
			static ColorStyleSetting yellow = null;
			/// <summary>
			/// Applied after BuildDisplay runs.
			/// </summary>
			/// 

			internal static void Prefix(ModsScreen __instance)
			{
				//__instance.canBackoutWithRightClick = false;
				var transf = __instance.entryPrefab.transform;
				if (blue == null)
				{
					SgtLogger.l("preparing color");
					transf.Find("ManageButton").TryGetComponent<KImage>(out var mngButtonImage);
					var defaultStyle = mngButtonImage.colorStyleSetting;
					blue = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
					float hsvShift = 70f;
					blue.inactiveColor = UIUtils.HSVShift(defaultStyle.inactiveColor, hsvShift);
					blue.activeColor = UIUtils.HSVShift(defaultStyle.activeColor, hsvShift);
					blue.disabledColor = UIUtils.HSVShift(defaultStyle.disabledColor, hsvShift);
					blue.hoverColor = UIUtils.HSVShift(defaultStyle.hoverColor, hsvShift);

					hsvShift = 20f;
					yellow = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
					yellow.inactiveColor = UIUtils.HSVShift(defaultStyle.inactiveColor, hsvShift);
					yellow.activeColor = UIUtils.HSVShift(defaultStyle.activeColor, hsvShift);
					yellow.disabledColor = UIUtils.HSVShift(defaultStyle.disabledColor, hsvShift);
					yellow.hoverColor = UIUtils.HSVShift(defaultStyle.hoverColor, hsvShift);
				}
				if (!__instance.entryPrefab.transform.Find("PinBtn") && __instance.entryPrefab.TryGetComponent<HierarchyReferences>(out var hr))
				{
					var dragIndicator = __instance.entryPrefab.transform.Find("DragReorderIndicator").gameObject;
					if (dragIndicator.TryGetComponent<ToolTip>(out var toolTip))
					{
						toolTip.UseFixedStringKey = false;
						toolTip.SetSimpleTooltip(global::STRINGS.UI.FRONTEND.MODS.DRAG_TO_REORDER + "\n\n" + STRINGS.UI.FRONTEND.MODORDER.BUTTONTOOLTIPADDON);
					}

					var rightclickButton = dragIndicator.AddComponent<FButton>();
					rightclickButton.normalColor = PUITuning.Colors.ButtonPinkStyle.activeColor;
					rightclickButton.hoverColor = UIUtils.Lighten(PUITuning.Colors.ButtonPinkStyle.activeColor, 20);
					SgtLogger.l("preparing prefab");


					var tagSprite = Assets.GetSprite(SpritePatch.tagSymbol);
					var tagButtonGO = Util.KInstantiateUI(FilterPatches._buttonPrefab, __instance.entryPrefab, true);
					tagButtonGO.name = "tagBtn";
					var flatTransform = tagButtonGO.transform;

					if (!flatTransform.Find("GameObject").TryGetComponent<Image>(out var tagImg))
						SgtLogger.warning("button image failed!");
					tagImg.sprite = tagSprite;
					tagImg.color = HasNoTags;

					if (!tagButtonGO.TryGetComponent<KButton>(out var tagBg))
						SgtLogger.warning("button failed!");
					tagBg.ClearOnClick();
					var cmp_tagBgnTT = UIUtils.AddSimpleTooltipToObject(flatTransform, TAGEDITWINDOW.NOTAGS);

					ElementReference er_rightclickBt = new() { behaviour = rightclickButton, Name = rightClickBtn };
					ElementReference er_tagBg = new() { behaviour = tagBg, Name = tagBgn };
					ElementReference er_flagtt = new() { behaviour = cmp_tagBgnTT, Name = tagBgnText };
					ElementReference er_flagimg = new() { behaviour = tagImg, Name = tagBgnImg };
					ElementReference er_tagBgTransf = new() { behaviour = flatTransform.transform, Name = tagBgnTransform };


					var btn = Util.KInstantiateUI(FilterPatches._buttonPrefab, __instance.entryPrefab, true);
					btn.name = "PinBtn";
					var tr = btn.transform;
					//tr.SetSiblingIndex(2);

					if (!tr.Find("GameObject").TryGetComponent<Image>(out var img))
						SgtLogger.warning("button image failed!");
					ElementReference buttonImageBg = new() { behaviour = img, Name = PinButtonImageBg };
					if (!transf.Find("BG").TryGetComponent<Image>(out var bgImg))
						SgtLogger.warning("Bg image failed!");
					ElementReference backgroundImage = new() { behaviour = bgImg, Name = BgImage };
					if (!btn.TryGetComponent<KButton>(out var button))
						SgtLogger.warning("button failed!");

					if (!hr.GetReference<KButton>("ManageButton").TryGetComponent<KImage>(out var mngButtonImg))
						SgtLogger.warning("manage button failed!");

					button.ClearOnClick();

					ElementReference managebtnImage = new() { behaviour = mngButtonImg, Name = MngBtImage };

					ElementReference pinButton = new() { behaviour = button, Name = PinButton };
					ElementReference pinButtonTransform = new() { behaviour = tr, Name = PinTransform };

					ElementReference[] refs = new ElementReference[]
					{
						er_rightclickBt,
						buttonImageBg,
						backgroundImage,
						pinButton,
						managebtnImage,
						pinButtonTransform,
						er_tagBgTransf,
						er_tagBg,
						er_flagimg,
						er_flagtt
					};

					hr.references = hr.references.AddRangeToArray(refs);
				}
			}
			const string
				PinButtonImageBg = "MPM_PinButtonBg",
				PinButton = "MPM_PinButton",
				BgImage = "MPM_BackgroundImage",
				MngBtImage = "MPM_ManageBtnImage",
				PinTransform = "MPM_PinTransform",
				tagBgnTransform = "MPM_tagBgnTransform",
				tagBgn = "MPM_tagBgn",
				rightClickBtn = "MPM_rightclickbutton",
				tagBgnImg = "MPM_tagBgnImage",
				tagBgnText = "MPM_FlagText"
				;

			private static readonly RectOffset BUTTON_MARGIN = new RectOffset(3, 3, 3, 3);
			private static readonly Vector2 ICON_SIZE = new Vector2(20.0f, 20.0f);

			internal static void Postfix(ModsScreen __instance, List<DisplayedMod> ___displayedMods)
			{
				var allMods = Global.Instance.modManager.mods;
				var modStateConfig = MPM_Config.Instance;
				//new Dialog_EditFilterTags(mod.label.defaultStaticID, () => __instance.RebuildDisplay("pinned mod changed")).CreateAndShow(null);
				foreach (var displayedMod in ___displayedMods)
				{
					var transf = displayedMod.rect_transform;
					var go = transf.gameObject;

					var mod = allMods[displayedMod.mod_index];
					string staticModId = mod.label.defaultStaticID;

					int currentIndex = displayedMod.mod_index;
					if (transf.TryGetComponent<HierarchyReferences>(out var hier))
					{
						if (mod.IsLocal)
						{
							var modButtonBg = hier.GetReference<KImage>(MngBtImage);
							modButtonBg.colorStyleSetting = mod.label.distribution_platform == KMod.Label.DistributionPlatform.Dev ? yellow : blue;
							modButtonBg.ApplyColorStyleSetting();
						}
						var contextButton = hier.GetReference<FButton>(rightClickBtn);
						contextButton.OnClick += () =>
						{
							Dialog_ModOrder.ShowIndexDialog(currentIndex, mod.title, contextButton.gameObject);
						};

						var pinButton = hier.GetReference<KButton>(PinButton);
						pinButton.onClick += () =>
						{
							modStateConfig.TogglePinnedMod(staticModId);
							__instance.RebuildDisplay("pinned mod changed");
						};

						if (modStateConfig.ModPinned(staticModId) && mod.contentCompatability == ModContentCompatability.OK)
						{
							var bgImage = hier.GetReference<Image>(PinButtonImageBg);
							bgImage.color = pinnedActive;
							transf.SetAsFirstSibling();

							var pinButtonBg = hier.GetReference<Image>(BgImage);
							pinButtonBg.color = pinnedBg;
						}
						if (modStateConfig.ModHasAnyTags(staticModId))
						{
							string tooltip = modStateConfig.GetModTagConfigUIString(staticModId);
							if (tooltip.Length > 0)
							{
								hier.GetReference<ToolTip>(tagBgnText).SetSimpleTooltip(tooltip);
							}

							var img = hier.GetReference<Image>(tagBgnImg);
							img.color = HasTags;
						}

						hier.GetReference<KButton>(tagBgn).onClick += () => Dialog_EditFilterTags.ShowFilterDialog(mod.label.defaultStaticID, () => __instance.RebuildDisplay("pinned mod changed"));
						hier.GetReference<Transform>(PinTransform).SetSiblingIndex(2);
						hier.GetReference<Transform>(tagBgnTransform).SetSiblingIndex(3);
					}
				}
				if (FilterButtons.Instance != null)
				{
					FilterButtons.Instance.RefreshUIState(false);
				}
				if (FilterToggleButtons.Instance != null)
				{
					FilterToggleButtons.Instance.RefreshUIState(false);
				}
				//ModAssets.ReorderVisualModState(___displayedMods, allMods);

			}
			static Color
				normal = UIUtils.rgb(62, 67, 87),
				pinnedBg = UIUtils.Darken(normal, 15),
				incompatibleBg = UIUtils.rgb(26, 28, 33),
				pinnedActive = UIUtils.Lighten(Color.red, 50),
				pinnedInactive = Color.white,
				HasTags = UIUtils.Lighten(Color.blue, 50),
				HasNoTags = Color.white;
		}

		[HarmonyPatch(typeof(KMod.Manager), nameof(KMod.Manager.NotifyDialog))]
		public static class OnLoad_BetterModDifferenceScreen_Patch
		{
			public static bool Prefix(KMod.Manager __instance, string title, string message_format, GameObject parent)
			{
				if (title == global::STRINGS.UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.TITLE && __instance.events.Count > 0)
				{

					var eventList = __instance.events.OrderBy(entry => entry.mod.title).Distinct().ToList();

					HashSet<string> changedModIDs = new();
					foreach (var entry in eventList)
					{
						if (entry.event_type == EventType.ExpectedInactive || entry.event_type == EventType.ExpectedActive)
						{
							if (!changedModIDs.Contains(entry.mod.id))
								changedModIDs.Add(entry.mod.id);
							else
								changedModIDs.Remove(entry.mod.id);

						}
					}

					var newlyEnabled = new StringBuilder();
					var newlyDisabled = new StringBuilder();


					Event.GetUIStrings(EventType.ExpectedInactive, out var expectedInactive, out _);
					Event.GetUIStrings(EventType.ExpectedActive, out var expectedActive, out _);

					bool hadNewlyEnabled = false, hadNewlyDisabled = false;

					newlyEnabled.AppendLine();
					newlyDisabled.AppendLine();

					newlyEnabled.AppendLine(UIUtils.ColorText("<b>" + expectedInactive + ":</b>", GlobalAssets.Instance.colorSet.logicOnSidescreen));
					newlyDisabled.AppendLine(UIUtils.ColorText("<b>" + expectedActive + ":</b>", GlobalAssets.Instance.colorSet.logicOffSidescreen));

					int changesOverLimit = 30;
					int changesOverLimitExAc = 0;
					int changesOverLimitExIn = 0;

					foreach (Event @event in eventList)
					{
						if (!changedModIDs.Contains(@event.mod.id))
						{
							continue;
						}

						if (@event.event_type == EventType.ExpectedInactive)
						{
							hadNewlyEnabled = true;
							if (changesOverLimit >= 0)
							{
								changesOverLimit--;
								newlyEnabled.AppendLine("• " + @event.mod.title);
							}
							else
							{
								changesOverLimitExIn++;
							}
						}
						else if (@event.event_type == EventType.ExpectedActive)
						{
							hadNewlyDisabled = true;
							if (changesOverLimit >= 0)
							{
								changesOverLimit--;
								newlyDisabled.AppendLine("• " + @event.mod.title);
							}
							else
							{
								changesOverLimitExAc++;
							}
						}
					}

					__instance.events.Clear();

					if (!hadNewlyDisabled && !hadNewlyEnabled)
						return false;

					string allMods =
						(hadNewlyEnabled ? newlyEnabled.ToString() : string.Empty)
						+ (changesOverLimitExIn > 0 ? global::STRINGS.UI.FRONTEND.MOD_DIALOGS.ADDITIONAL_MOD_EVENTS.Replace("(", "(" + changesOverLimitExIn + " ").Replace("...", string.Empty) : string.Empty)
						+ (hadNewlyDisabled ? newlyDisabled.ToString() : string.Empty)
						+ (changesOverLimitExAc > 0 ? global::STRINGS.UI.FRONTEND.MOD_DIALOGS.ADDITIONAL_MOD_EVENTS.Replace("(", "(" + changesOverLimitExAc + " ").Replace("...", string.Empty) : string.Empty);
					;

					string text = string.Format(global::STRINGS.UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.MESSAGE, allMods);


					ConfirmDialogScreen popUpGO = ((ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, Global.Instance.globalCanvas));
					popUpGO.PopupConfirmDialog(text, null, null, null, null, global::STRINGS.UI.FRONTEND.MOD_DIALOGS.SAVE_GAME_MODS_DIFFER.TITLE, null, null, null);
					popUpGO.popupMessage.alignment = TMPro.TextAlignmentOptions.TopLeft;
					return false;
				}
				return true;
			}
		}

		// [HarmonyPatch(typeof(Steam), nameof(Steam.MakeMod))]
		public static class Steam_MakeMod
		{
			public static void TryPatchingSteam(Harmony harmony)
			{
				var type = AccessTools.TypeByName("Steam");
				if (type == null)
				{
					SgtLogger.warning("steam.makemod was null");
					return;
				}
				SgtLogger.l("patching steam.makemod");
				var m_TargetMethod = AccessTools.Method(type, "MakeMod");
				//var m_Transpiler = AccessTools.Method(typeof(LoadModConfigPatch), "Transpiler");
				// var m_Prefix = AccessTools.Method(typeof(Steam_MakeMod), "Prefix");
				var m_Postfix = AccessTools.Method(typeof(Steam_MakeMod), "Postfix");

				harmony.Patch(m_TargetMethod,
					null, //new HarmonyMethod(m_Prefix),
					new HarmonyMethod(m_Postfix)
					);
			}

			public static void Postfix(KMod.Mod __result)
			{
				if (__result == null || __result.staticID == null || __result.label.id == null)
					return;

				__result.on_managed = () =>
				{
					if (UseSteamOverlay && SteamUtils.IsOverlayEnabled())
						SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/sharedfiles/filedetails/?id=" + __result.label.id);
					else
						App.OpenWebURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + __result.label.id);
				};
			}
		}
		[HarmonyPatch(typeof(KMod.Manager), nameof(KMod.Manager.Install))]
		public static class ModManager_Subscribe
		{
			public static void Postfix(KMod.Mod mod)
			{
				if (ulong.TryParse(mod.label.id, out var steamID))
					SteamInfoQuery.FindMissingModsQuery(new List<ulong>() { steamID });
			}
		}


		[HarmonyPatch(typeof(ModsScreen), "OnDeactivate")]
		public static class ModsScreen_SyncModeOff
		{
			public static void Postfix()
			{
				ModlistManager.Instance.IsSyncing = false;
				Dialog_ModOrder.Close();
			}
		}


		[HarmonyPatch(typeof(ModsScreen), "OnActivate")]
		public static class ModsScreen_AddModListButton
		{
			public static void Postfix(ModsScreen __instance)
			{

				if (ModAssets.ModsFilterActive)
				{
					var modsFilterGO = __instance.transform.Find("Panel/Search/LocTextInputField");
					if (modsFilterGO != null)
					{
						modsFilterGO.gameObject.TryGetComponent(out FilterManager.ModFilterTextCmp);
					}
				}


				__instance.workshopButton.ClearOnClick();
				__instance.workshopButton.onClick += () =>
				{

					if (UseSteamOverlay && SteamUtils.IsOverlayEnabled())
						SteamFriends.ActivateGameOverlayToWebPage("http://steamcommunity.com/workshop/browse/?appid=457140");
					else
						App.OpenWebURL("http://steamcommunity.com/workshop/browse/?appid=457140");
				};
				///Add Modlist Button
				var workShopButton = __instance.transform.Find("Panel/DetailsView/WorkshopButton");
				var DetailsView = __instance.transform.Find("Panel/DetailsView").gameObject;



				var panel = __instance.transform.Find("Panel").rectTransform();

				var totalWindowHeight = __instance.rectTransform().rect.height;
				float goldenHeigh = totalWindowHeight / 1.309f;
				float paddingSize = (totalWindowHeight - goldenHeigh) / 2f;
				panel.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, paddingSize, goldenHeigh);


				if (__instance.gameObject.name == "SYNCSCREEN")
					return;

				__instance.closeButton.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
				AddCopyButton(__instance.workshopButton.transform.parent.gameObject, () => ModAssets.PutCurrentToClipboard(false), () => PutCurrentToClipboard(true), __instance.workshopButton.bgImage.colorStyleSetting);


				var modlistButtonGO = Util.KInstantiateUI<RectTransform>(workShopButton.gameObject, DetailsView, true);
				modlistButtonGO.name = "ModListsButton";
				var modlistButtonText = modlistButtonGO.Find("Text").GetComponent<LocText>();
				modlistButtonText.text = STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSBUTTON;
				modlistButtonGO.FindOrAddUnityComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.MODLISTSBUTTONINFO);
				var modlistButton = modlistButtonGO.GetComponent<KButton>();
				modlistButton.ClearOnClick();

#if DEBUG
                // UIUtils.ListAllChildren(__instance.transform);
#endif

				modlistButton.onClick += () =>
				{
					///Util.KInstantiateUI(ScreenPrefabs.Instance.RailModUploadMenu.gameObject, modScreen.gameObject, true); ///HMMM; great if modified for modpack creation
					GameObject window = Util.KInstantiateUI(ScreenPrefabs.Instance.languageOptionsScreen.gameObject);
					window.SetActive(false);
					var copy = window.transform;
					UnityEngine.Object.Destroy(window);
					var newScreen = Util.KInstantiateUI(copy.gameObject, __instance.gameObject, true);
					newScreen.name = "ModListView";
					newScreen.AddComponent(typeof(ModListScreen));

				};
				var closeButton = __instance.transform.Find("Panel/DetailsView/CloseButton");
				//UIUtils.ListAllChildrenWithComponents(closeButton.transform);
				closeButton.SetAsLastSibling();
			}
		}

		[HarmonyPatch(typeof(LoadScreen), "OnActivate")]
		public static class AddModSyncButtonToLoadscreen
		{
			public static bool Prefix(LoadScreen __instance)
			{
				if (__instance.name == "NODONTDOTHAT") return false;

				ModlistManager.Instance.ParentObjectRef = __instance.transform.parent.gameObject;
				ModlistManager.Instance.GetAllStoredModlists();


				var ViewRootFinder = typeof(LoadScreen).GetField("colonyViewRoot", BindingFlags.NonPublic | BindingFlags.Instance);

				GameObject viewRoot = (GameObject)ViewRootFinder.GetValue(__instance);

				HierarchyReferences references = viewRoot.GetComponent<HierarchyReferences>();
				///get ListEntryTemplate 
				RectTransform template = references.GetReference<RectTransform>("SaveTemplate");

				///Get LoadButton and move it to the left 
				HierarchyReferences TemplateRefs = template.GetComponent<HierarchyReferences>();
				var SyncTemplate = TemplateRefs.GetReference<RectTransform>("LoadButton");
				SyncTemplate.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 65f, SyncTemplate.rect.width);
				///Move Time&Date text to the left
				var date = TemplateRefs.GetReference<RectTransform>("DateText");
				date.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, SyncTemplate.rect.width + 85f, date.rect.width);


				///Instantiate SyncButton
				RectTransform kbutton = Util.KInstantiateUI<RectTransform>(SyncTemplate.gameObject, template.gameObject, true);
				kbutton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 50);

				///Add SyncButton to template and set Params
				kbutton.name = "SyncButton";
				var syncText = kbutton.GetComponentInChildren<LocText>(true);
				var btn = kbutton.GetComponentInChildren<KButton>(true);
				syncText.key = "STRINGS.UI.FRONTEND.MODSYNCING.SYNCMODSBUTTONBG";
				btn.bgImage.sprite = Assets.GetSprite("icon_thermal_conductivity");
				return true;
			}
		}

		/// <summary>
		/// Add a "Sync and Continue"-Button to the main menu prefab
		/// </summary>
		[HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
		public static class MainMenu_OnPrefabInit_Patch
		{
			public static void Prefix(MainMenu __instance)
			{
				string path;
				if (KPlayerPrefs.HasKey("AutoResumeSaveFile"))
				{
					path = KPlayerPrefs.GetString("AutoResumeSaveFile");
				}
				else
					path = string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveForCurrentDLC() : GenericGameSettings.instance.performanceCapture.saveGame;
#if DEBUG
                //UIUtils.ListAllChildren(__instance.transform);
#endif
				if (path == null || path == string.Empty)
				{
					return;
				}

				Transform parentBar;
				Transform contButton;

				if (DlcManager.IsExpansion1Active())
				{
					parentBar = __instance.transform.Find("MainMenuMenubar/MainMenuButtons");
					contButton = __instance.transform.Find("MainMenuMenubar/MainMenuButtons/Button_ResumeGame");
				}
				else
				{
					parentBar = __instance.transform.Find("UI Group/TopRight/MainMenuButtons");
					contButton = __instance.transform.Find("UI Group/TopRight/MainMenuButtons/Button_ResumeGame");
				}

				var bt = Util.KInstantiateUI(contButton.gameObject, parentBar.gameObject, true);

				string colonyName = SaveGameModList.GetModListFileName(path);

				var colony = ModlistManager.Instance.TryGetColonyModlist(colonyName);
				bool interactable = colony != null && colony.TryGetModListEntry(path, out _);

				var button = bt.GetComponent<KButton>();
				bt.name = "SyncAndContinue";
				var internalText = bt.transform.Find("ResumeText").GetComponent<LocText>();

				internalText.text = STRINGS.UI.FRONTEND.MODSYNCING.CONTINUEANDSYNC;

				button.isInteractable = interactable;
				button.ClearOnClick();
				var autoResumeOnSync = () => __instance.ResumeGame();
				button.onClick +=
				() =>
				{
					ModlistManager.Instance.InstantiateModViewForPathOnly(path, autoResumeOnSync);
				};
				ModlistManager.Instance.ParentObjectRef = __instance.gameObject;
				var SaveGameName = button.transform.Find("SaveNameText").gameObject;
				UnityEngine.Object.Destroy(SaveGameName);


				///SteamAuthorInfoFetching:
				///

				if (SteamManager.Initialized)
				{
					var steamMods = Global.Instance.modManager.mods
						.Where(mod => mod.label.distribution_platform == KMod.Label.DistributionPlatform.Steam)
						.Select(mod => mod.label.id)
						.ToList();
					if (steamMods.Count > 0)
						SteamInfoQuery.InitModAuthorQuery(steamMods);
				}
			}
		}


		/// <summary>
		/// On loading a savegame, store the mod config in the modlist.
		/// </summary>
		//[HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.Load), new System.Type[] { typeof(IReader) })]
		public static class LoadModConfigPatch
		{
			//Manual patch required here, otherwise it will break translations of game settings as those get their assets initialized earlier than translation
			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("SaveLoader, Assembly-CSharp:Load", new System.Type[] { typeof(IReader) });
				var m_Transpiler = AccessTools.Method(typeof(LoadModConfigPatch), "Transpiler");
				var m_Prefix = AccessTools.Method(typeof(LoadModConfigPatch), "Prefix");
				//var m_Postfix = AccessTools.Method(typeof(MainMenuSearchBarInit), "Postfix");

				harmony.Patch(m_TargetMethod,
					new HarmonyMethod(m_Prefix),
					null,//new HarmonyMethod(m_Postfix),
					new HarmonyMethod(m_Transpiler)
					);
			}



			internal class SaveFileRoot
			{
				public int WidthInCells;
				public int HeightInCells;
				public Dictionary<string, byte[]> streamed;
				public string clusterID;
				public List<ModInfo> requiredMods;
				public List<KMod.Label> active_mods;
				public SaveFileRoot() => this.streamed = new Dictionary<string, byte[]>();
			}
			public static SaveLoader instance;
			public static void Prefix(SaveLoader __instance)
			{
				instance = __instance;
			}
			public static void WriteModlistPatch(SaveFileRoot saveFileRoot)
			{
				var savedButNotEnabledMods = saveFileRoot.active_mods;
				savedButNotEnabledMods.Remove(savedButNotEnabledMods.Find(mod => mod.title == "SaveGameModLoader"));


				bool init = ModlistManager.Instance.CreateOrAddToModLists(
					SaveLoader.GetActiveSaveFilePath(),
					saveFileRoot.active_mods
					);

				KPlayerPrefs.DeleteKey("AutoResumeSaveFile");
			}

			public static readonly MethodInfo ScreenCreator = AccessTools.Method(
			   typeof(LoadModConfigPatch),
			   nameof(WriteModlistPatch));


			public static readonly MethodInfo SaveFileRootCallLocation = AccessTools.Method(
			   typeof(Deserializer),
			   nameof(Deserializer.Deserialize),
				new System.Type[] { typeof(object) });

			private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
					typeof(KMod.Manager),
					"MatchFootprint");

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();
				var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SuitableMethodInfo);
				var deserializerSearchIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == SaveFileRootCallLocation);

				var saveFileRootIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, deserializerSearchIndex);

				//foreach (var v in code) { SgtLogger.log(v.opcode + " -> " + v.operand); };
				if (insertionIndex != -1 && saveFileRootIndex != -1)
				{
					code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, saveFileRootIndex));
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ScreenCreator));
				}

				return code;
			}
		}

		/// <summary>
		/// On Saving, store the mod config to the modlist
		/// </summary>
		[HarmonyPatch(typeof(SaveLoader))]
		[HarmonyPatch(nameof(SaveLoader.Save))]
		[HarmonyPatch(new System.Type[] { typeof(string), typeof(bool), typeof(bool) })]
		public static class SaveModConfigOnSave
		{
			public static void Postfix(string filename, bool isAutoSave = false, bool updateSavePointer = true)
			{
				//SgtLogger.log(filename + isAutoSave + updateSavePointer);
				KMod.Manager modManager = Global.Instance.modManager;

				var enabledModLabels = modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();

				bool init = ModlistManager.Instance.CreateOrAddToModLists(
					filename,
					enabledModLabels
					);
			}
		}

		///// <summary>
		///// Since the Mod patches the load method, it will recieve blame on ANY load crash.
		///// This Patch keeps it enabled on a crash, so you dont need to reenable it for syncing,
		///// </summary>
		[HarmonyPatch(typeof(KMod.Mod))]
		[HarmonyPatch(nameof(KMod.Mod.SetCrashed))]
		public static class DontDisableModOnCrash
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.NeverDisable;
			public static bool Prefix(KMod.Mod __instance)
			{
				return !ModSyncUtils.IsModSyncMod(__instance);
			}
		}
	}
}
