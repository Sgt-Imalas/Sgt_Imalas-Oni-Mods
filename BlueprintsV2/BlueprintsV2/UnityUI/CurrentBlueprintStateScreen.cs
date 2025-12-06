using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.UnityUI.Components;
using BlueprintsV2.Tools;
using BlueprintsV2.UnityUI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static BlueprintsV2.STRINGS.UI;
using static BlueprintsV2.STRINGS.UI.TOOLS;
using static BlueprintsV2.STRINGS.UI.USEBLUEPRINTSTATECONTAINER.INFOITEMSCONTAINER;

namespace BlueprintsV2.BlueprintsV2.UnityUI
{
	internal class CurrentBlueprintStateScreen : KScreen
	{
		public static CurrentBlueprintStateScreen Instance = null;

		LocText CurrentBPName;
		FButton SelectPrevBP, SelectNextBP;
		LocText FolderInfo;
		GameObject FolderInfoGO;

		GameObject ColorPreviewPrefab;

		FToggle ApplyBPSettings, ForceRebuildMismatchedBuildings, EnableSnapshotMaterialOverrides;
		//YesNoInfo CanRotate;
		FButton RotateL, RotateR, ChangeMaterialOverrides;
		//YesNoInfo CanFlipH, CanFlipV;
		FButton FlipH, FlipV;

		public static void DestroyInstance() { Instance = null; }

		public static void ShowScreen(bool show)
		{
			if (Instance == null)
			{
				GameObject baseContent = ToolMenu.Instance.toolParameterMenu.content;
				//GameObject baseWidgetContainer = ToolMenu.Instance.toolParameterMenu.widgetContainer;

				Instance = Util.KInstantiateUI<CurrentBlueprintStateScreen>(ModAssets.BlueprintInfoStateGO, baseContent.transform.parent.gameObject);
				Instance.gameObject.SetActive(true);
			}
			Instance.gameObject.SetActive(show);
		}


		public void SetSelectedBlueprint(Blueprint bp)
		{
			if (bp == null)
			{
				CurrentBPName.SetText("-");
				return;
			}
			CurrentBPName.SetText(bp.FriendlyName);
			EnableSnapshotMaterialOverrides.gameObject.SetActive(BlueprintState.IsPlacingSnapshot);

			if (BlueprintState.IsPlacingSnapshot)
			{
				FolderInfoGO.SetActive(true);
				string folderInfo = string.Format(FOLDERINFO.LABEL_SNAPSHOT, SnapshotTool.SnapshotIndex + 1, SnapshotTool.SnapshotCount);
				FolderInfo.SetText(folderInfo);
			}
			else
			{
				FolderInfoGO.SetActive(true);
				var folder = ModAssets.GetCurrentFolder();
				int bpIndex = folder.GetBlueprintIndex(bp) + 1;
				int folderCount = folder.BlueprintCount;
				string folderInfo = string.Format(FOLDERINFO.LABEL, folder.Name, bpIndex, folderCount);
				FolderInfo.SetText(folderInfo);
			}
			RefreshButtonStates();
		}

		public void RefreshButtonStates()
		{
			bool canRotate = BlueprintState.CanRotate;

			//CanRotate.SetInfoState(canRotate);
			RotateL.SetInteractable(canRotate);
			RotateR.SetInteractable(canRotate);

			bool canFlipH = BlueprintState.CanFlipH;
			//CanFlipH.SetInfoState(canFlipH);
			FlipH.SetInteractable(canFlipH);

			bool canFlipV = BlueprintState.CanFlipV;
			//CanFlipV.SetInfoState(canFlipV);
			FlipV.SetInteractable(canFlipV);
			RefreshStateChangeBPs();
		}
		void RefreshStateChangeBPs()
		{
			if (BlueprintState.IsPlacingSnapshot)
			{
				bool storedBPs = SnapshotTool.HasSnapshotsStored;
				bool canGoNext = SnapshotTool.Instance?.HasNextSnapshot ?? false;
				bool canGoPrev = SnapshotTool.Instance?.HasPrevSnapshot ?? false;
				SelectNextBP.SetInteractable(storedBPs && canGoNext);
				SelectPrevBP.SetInteractable(storedBPs && canGoPrev);
			}
			else
			{
				var folder = ModAssets.GetCurrentFolder();
				var bp = ModAssets.SelectedBlueprint;

				bool storedBPs = folder.HasBlueprints;
				bool canGoNext = folder.HasNextBlueprint(bp);
				bool canGoPrev = folder.HasPrevBlueprint(bp);
				SelectNextBP.SetInteractable(storedBPs && canGoNext);
				SelectPrevBP.SetInteractable(storedBPs && canGoPrev);
			}
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}
		void Init()
		{
			CurrentBPName = transform.Find("InfoItemsContainer/CurrentBP/Label").gameObject.GetComponent<LocText>();
			FolderInfo = transform.Find("InfoItemsContainer/FolderInfo/Label").gameObject.GetComponent<LocText>();
			FolderInfoGO = transform.Find("InfoItemsContainer/FolderInfo").gameObject;

			SelectPrevBP = transform.Find("InfoItemsContainer/CurrentBP/Prev").gameObject.AddOrGet<FButton>();
			SelectPrevBP.OnClick += HandlePrevBP;
			UIUtils.AddSimpleTooltipToObject(SelectPrevBP.gameObject, string.Format(USE_TOOL.SELECTPREV, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSelectPrevious.GetKAction()) + "]")));

			SelectNextBP = transform.Find("InfoItemsContainer/CurrentBP/Next").gameObject.AddOrGet<FButton>();
			SelectNextBP.OnClick += HandleNextBP;
			UIUtils.AddSimpleTooltipToObject(SelectNextBP.gameObject, string.Format(USE_TOOL.SELECTNEXT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSelectNext.GetKAction()) + "]")));


			ApplyBPSettings = transform.Find("InfoItemsContainer/ApplyStoredSettings").gameObject.AddOrGet<FToggle>();
			ApplyBPSettings.SetCheckmark("Checkbox/Checkmark");
			ApplyBPSettings.SetOnFromCode(BlueprintState.ApplyBlueprintSettings);
			ApplyBPSettings.OnChange += (on) => BlueprintState.ApplyBlueprintSettings = on;


			ForceRebuildMismatchedBuildings = transform.Find("InfoItemsContainer/ForceRebuild").gameObject.AddOrGet<FToggle>();
			ForceRebuildMismatchedBuildings.SetCheckmark("Checkbox/Checkmark");
			ForceRebuildMismatchedBuildings.SetOnFromCode(BlueprintState.ForceMaterialChange);
			ForceRebuildMismatchedBuildings.OnChange += (on) => BlueprintState.ForceMaterialChange = on;
			UIUtils.AddSimpleTooltipToObject(ForceRebuildMismatchedBuildings.gameObject, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsToggleForce.GetKAction()) + "]"));

			EnableSnapshotMaterialOverrides = transform.Find("InfoItemsContainer/MaterialReplacement").gameObject.AddOrGet<FToggle>();
			EnableSnapshotMaterialOverrides.SetCheckmark("Checkbox/Checkmark");
			EnableSnapshotMaterialOverrides.SetOnFromCode(BlueprintState.MaterialReplacementInSnapshots);
			EnableSnapshotMaterialOverrides.OnChange += (on) => BlueprintState.MaterialReplacementInSnapshots = on;
			UIUtils.AddSimpleTooltipToObject(EnableSnapshotMaterialOverrides.gameObject, MATERIALREPLACEMENT.TOOLTIP);


			ChangeMaterialOverrides = transform.Find("InfoItemsContainer/MaterialOverrides/Button").gameObject.AddOrGet<FButton>();
			ChangeMaterialOverrides.OnClick += ShowMaterialReplacementList;

			//CanRotate = transform.Find("InfoItemsContainer/CanRotateYesNo").gameObject.AddOrGet<YesNoInfo>();
			RotateL = transform.Find("InfoItemsContainer/RotateActions/RotateL").gameObject.AddOrGet<FButton>();
			RotateL.OnClick += HandleRotationL;
			UIUtils.AddSimpleTooltipToObject(RotateL.gameObject, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsRotateInverse.GetKAction()) + "]"));

			RotateR = transform.Find("InfoItemsContainer/RotateActions/RotateR").gameObject.AddOrGet<FButton>();
			RotateR.OnClick += HandleRotationR;
			UIUtils.AddSimpleTooltipToObject(RotateR.gameObject, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsRotate.GetKAction()) + "]"));

			//CanFlipH = transform.Find("InfoItemsContainer/CanFlipHYesNo").gameObject.AddOrGet<YesNoInfo>();
			//CanFlipV = transform.Find("InfoItemsContainer/CanFlipVYesNo").gameObject.AddOrGet<YesNoInfo>();
			FlipH = transform.Find("InfoItemsContainer/FlipActions/FlipH").gameObject.AddOrGet<FButton>();
			FlipH.OnClick += HandleFlipH;
			UIUtils.AddSimpleTooltipToObject(FlipH.gameObject, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsFlipHorizontal.GetKAction()) + "]"));
			FlipV = transform.Find("InfoItemsContainer/FlipActions/FlipV").gameObject.AddOrGet<FButton>();
			FlipV.OnClick += HandleFlipV;
			UIUtils.AddSimpleTooltipToObject(FlipV.gameObject, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsFlipVertical.GetKAction()) + "]"));

			ColorPreviewPrefab = transform.Find("InfoItemsContainer/PreviewColorPrefab").gameObject;
			ColorPreviewPrefab.AddOrGet<ColorLegendEntry>();
			ColorPreviewPrefab.SetActive(false);

			BuildColorLegend();
		}

		void BuildColorLegend()
		{
			AddLegend(COLOR_LEGEND.BLUEPRINTS_COLOR_VALIDPLACEMENT, ModAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT);
			AddLegend(COLOR_LEGEND.BLUEPRINTS_COLOR_CAN_APPLY_SETTINGS, ModAssets.BLUEPRINTS_COLOR_CAN_APPLY_SETTINGS);
			if (Config.Instance.RequireConstructable)
			{
				AddLegend(COLOR_LEGEND.BLUEPRINTS_COLOR_NOTECH, ModAssets.BLUEPRINTS_COLOR_NOTECH);
				AddLegend(COLOR_LEGEND.BLUEPRINTS_COLOR_NOMATERIALS, ModAssets.BLUEPRINTS_COLOR_NOMATERIALS);
			}
			AddLegend(COLOR_LEGEND.BLUEPRINTS_COLOR_NOTALLOWEDINWORLD, ModAssets.BLUEPRINTS_COLOR_NOTALLOWEDINWORLD);
			AddLegend(COLOR_LEGEND.BLUEPRINTS_COLOR_INVALIDPLACEMENT, ModAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT);
		}

		void AddLegend(string label, Color color)
		{
			var item = Util.KInstantiateUI<ColorLegendEntry>(ColorPreviewPrefab, ColorPreviewPrefab.transform.parent.gameObject);
			item.SetContent(label, color);
			item.gameObject.SetActive(true);
		}


		void HandleFlipH()
		{
			BlueprintState.FlipHorizontal();
			BlueprintState.RefreshBlueprintVisualizers();
		}
		void HandleFlipV()
		{

			BlueprintState.FlipVertical();
			BlueprintState.RefreshBlueprintVisualizers();
		}
		void HandleRotationR()
		{
			BlueprintState.TryRotateBlueprint();
			BlueprintState.RefreshBlueprintVisualizers();
		}
		void HandleRotationL()
		{
			BlueprintState.TryRotateBlueprint(true);
			BlueprintState.RefreshBlueprintVisualizers();
		}

		void ShowMaterialReplacementList()
		{

		}

		void HandleNextBP()
		{
			if (BlueprintState.IsPlacingSnapshot)
			{
				SnapshotTool.Instance.VisualizeNextSnapshot();
				SetSelectedBlueprint(SnapshotTool.CurrentSnapshot);
			}
			else
			{
				UseBlueprintTool.Instance.SelectNextBlueprint();
				SetSelectedBlueprint(ModAssets.SelectedBlueprint);
			}
		}
		void HandlePrevBP()
		{
			if (BlueprintState.IsPlacingSnapshot)
			{
				SnapshotTool.Instance.VisualizePreviousSnapshot();
				SetSelectedBlueprint(SnapshotTool.CurrentSnapshot);
			}
			else
			{
				UseBlueprintTool.Instance.SelectPrevBlueprint();
				SetSelectedBlueprint(ModAssets.SelectedBlueprint);
			}
		}

		internal void SetForceMaterialChange(bool enabled)
		{
			ForceRebuildMismatchedBuildings.SetOnFromCode(enabled);
		}
	}
}
