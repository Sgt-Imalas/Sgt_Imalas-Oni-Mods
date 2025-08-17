
using BlueprintsV2.BlueprintData;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.Tools
{
	public sealed class SnapshotToolHoverCard : HoverTextConfiguration
	{
		public bool ShowHotkeys { get; set; } = true;
		public bool UsingSnapshot { get; set; }

		public SnapshotToolHoverCard()
		{
			ToolName = STRINGS.UI.TOOLS.SNAPSHOT_TOOL.TOOLTIP_TITLE;
		}
		
		public void ToggleHotkeyTooltips() => ShowHotkeys = !ShowHotkeys;

		public override void UpdateHoverElements(List<KSelectable> hoveredObjects)
		{

			HoverTextScreen screenInstance = HoverTextScreen.Instance;
			HoverTextDrawer drawer = screenInstance.BeginDrawing();
			drawer.BeginShadowBar();

			DrawTitle(screenInstance, drawer);
			drawer.NewLine();
			drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
			drawer.DrawText(UsingSnapshot ? STRINGS.UI.TOOLS.USE_TOOL.ACTION_CLICK : STRINGS.UI.TOOLS.CREATE_TOOL.ACTION_DRAG, Styles_Instruction.Standard);
			drawer.AddIndent(8);

			drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
			drawer.DrawText(STRINGS.UI.TOOLS.USE_TOOL.ACTION_BACK, Styles_Instruction.Standard);
			drawer.NewLine();

			if (ShowHotkeys)
			{
				drawer.DrawText(string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.REUSELASTSNAPSHOT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSnapshotReuseAction.GetKAction()) + "]")), Styles_Instruction.Standard);
				drawer.NewLine();
				if (UsingSnapshot)
				{
					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.NEWSNAPSHOT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsReopenSelectionAction.GetKAction()) + "]")), Styles_Instruction.Standard);
					drawer.NewLine();

					drawer.DrawText(UIUtils.ColorText(string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.SELECTPREV_SNAPSHOT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSelectPrevious.GetKAction()) + "]")), SnapshotTool.Instance?.HasPrevSnapshot ?? false ? Color.white : Color.grey), Styles_Instruction.Standard);
					drawer.NewLine();

					drawer.DrawText(UIUtils.ColorText(string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.SELECTNEXT_SNAPSHOT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSelectNext.GetKAction()) + "]")), SnapshotTool.Instance?.HasNextSnapshot ?? false ? Color.white : Color.grey), Styles_Instruction.Standard);
					drawer.NewLine();
					
					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ACTION_CHANGE_ANCHOR, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSwapAnchorAction.GetKAction()) + "]")), Styles_Instruction.Standard);
					drawer.NewLine();
					drawer.DrawText(UIUtils.ColorText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ROTATE,
						UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsRotate.GetKAction()) + "]"),
						UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsRotateInverse.GetKAction()) + "]")),
						BlueprintState.CanRotate ? Color.white : Color.grey), Styles_Instruction.Standard);

					drawer.NewLine();
					drawer.DrawText(string.Format(UIUtils.ColorText(STRINGS.UI.TOOLS.USE_TOOL.FLIP, BlueprintState.CanFlipH|| BlueprintState.CanFlipV ? Color.white : Color.grey),
						UIUtils.ColorText(STRINGS.UI.TOOLS.USE_TOOL.ORIENTATION_H, BlueprintState.CanFlipH ? Color.white : Color.grey),
						UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsFlipHorizontal.GetKAction()) + "]"),
						UIUtils.ColorText(STRINGS.UI.TOOLS.USE_TOOL.ORIENTATION_V, BlueprintState.CanFlipV ? Color.white : Color.grey),
						UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsFlipVertical.GetKAction()) + "]")
						), Styles_Instruction.Standard);

					drawer.NewLine();
					bool forceRebuild = BlueprintState.ForceMaterialChange;
					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.FORCEREBUILD, (forceRebuild ? STRINGS.UI.TOOLS.USE_TOOL.REBUILD_ACTIVE : STRINGS.UI.TOOLS.USE_TOOL.REBUILD_INACTIVE), UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsToggleForce.GetKAction()) + "]")), forceRebuild ? Styles_Instruction.Selected : Styles_Instruction.Standard);
					drawer.NewLine(44);
				}
			}
			drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.TOGGLE_SHOW_HOTKEYS, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsToggleHotkeyToolTips.GetKAction()) + "]")), Styles_Instruction.Standard);


			drawer.EndShadowBar();
			drawer.EndDrawing();
		}
	}
}
