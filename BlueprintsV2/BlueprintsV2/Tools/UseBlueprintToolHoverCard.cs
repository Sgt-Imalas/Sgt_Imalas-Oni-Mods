using BlueprintsV2.BlueprintData;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.Tools
{
	public sealed class UseBlueprintToolHoverCard : HoverTextConfiguration
	{
		public int prefabErrorCount = 0;

		public UseBlueprintToolHoverCard()
		{
			ToolName = STRINGS.UI.TOOLS.USE_TOOL.TOOLTIP_TITLE;
		}

		public override void UpdateHoverElements(List<KSelectable> hoveredObjects)
		{
			HoverTextScreen screenInstance = HoverTextScreen.Instance;
			HoverTextDrawer drawer = screenInstance.BeginDrawing();
			drawer.BeginShadowBar();

			DrawTitle(screenInstance, drawer);
			drawer.NewLine();

			drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
			drawer.DrawText(STRINGS.UI.TOOLS.USE_TOOL.ACTION_CLICK, Styles_Instruction.Standard);
			drawer.AddIndent(8);

			drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
			drawer.DrawText(STRINGS.UI.TOOLS.USE_TOOL.ACTION_BACK, Styles_Instruction.Standard);

			if (ModAssets.BlueprintFileHandling.HasBlueprints())
			{
				if (ModAssets.SelectedBlueprint != null)
				{
					var selectedBp = ModAssets.SelectedBlueprint;
					var folder = ModAssets.GetCurrentFolder();

					if (BlueprintState.ExtendedCardTooltips)
					{
						drawer.NewLine(32);
						drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ACTION_SELECT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsReopenSelectionAction.GetKAction()) + "]")), Styles_Instruction.Standard);
						drawer.NewLine();

						drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ACTION_CHANGE_ANCHOR, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSwapAnchorAction.GetKAction()) + "]")), Styles_Instruction.Standard);
						drawer.NewLine();
						drawer.DrawText(UIUtils.ColorText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ROTATE,
							UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsRotate.GetKAction()) + "]"),
							UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsRotateInverse.GetKAction()) + "]")),
							BlueprintState.CanRotate ? Color.white : Color.grey), Styles_Instruction.Standard);

						drawer.NewLine(20);
						drawer.DrawText(string.Format(UIUtils.ColorText(STRINGS.UI.TOOLS.USE_TOOL.FLIP, BlueprintState.CanFlipH || BlueprintState.CanFlipV ? Color.white : Color.grey),
							UIUtils.ColorText(STRINGS.UI.TOOLS.USE_TOOL.ORIENTATION_H, BlueprintState.CanFlipH ? Color.white : Color.grey),
							UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsFlipHorizontal.GetKAction()) + "]"),
							UIUtils.ColorText(STRINGS.UI.TOOLS.USE_TOOL.ORIENTATION_V, BlueprintState.CanFlipV ? Color.white : Color.grey),
							UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsFlipVertical.GetKAction()) + "]")
							), Styles_Instruction.Standard);



						bool forceRebuild = BlueprintState.ForceMaterialChange;
						drawer.NewLine();
						drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.FORCEREBUILD, (forceRebuild ? STRINGS.UI.TOOLS.USE_TOOL.REBUILD_ACTIVE : STRINGS.UI.TOOLS.USE_TOOL.REBUILD_INACTIVE), UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsToggleForce.GetKAction()) + "]")), forceRebuild ? Styles_Instruction.Selected : Styles_Instruction.Standard);

					}
					if (prefabErrorCount > 0)
					{
						drawer.NewLine(45);
						drawer.DrawIcon(screenInstance.GetSprite("iconWarning"));
						drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ERRORMESSAGE, prefabErrorCount), Styles_Instruction.Selected);
					}
					drawer.NewLine(BlueprintState.ExtendedCardTooltips ? 45 : 22); 
					
					drawer.DrawText(UIUtils.ColorText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.SELECTPREV, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSelectPrevious.GetKAction()) + "]")), folder.HasPrevSnapshot(ModAssets.SelectedBlueprint) ? Color.white : Color.grey), Styles_Instruction.Standard);
					drawer.NewLine(20);

					drawer.DrawText(UIUtils.ColorText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.SELECTNEXT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSelectNext.GetKAction()) + "]")), folder.HasNextSnapshot(ModAssets.SelectedBlueprint) ? Color.white : Color.grey), Styles_Instruction.Standard);
					drawer.NewLine();
					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.SELECTEDBLUEPRINT, selectedBp.FriendlyName, folder.GetBlueprintIndex(selectedBp) + 1, folder.BlueprintCount, folder.Name), Styles_Instruction.Standard);
				}
				else
				{
					drawer.DrawText(STRINGS.UI.TOOLS.USE_TOOL.NONESELECTED, Styles_Instruction.Standard);
				}
				drawer.NewLine(32);
				drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.TOGGLE_SHOW_HOTKEYS, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsToggleHotkeyToolTips.GetKAction()) + "]")), Styles_Instruction.Standard);

			}
			else
			{
				drawer.DrawText(STRINGS.UI.TOOLS.USE_TOOL.NOBLUEPRINTS, Styles_Instruction.Standard);
			}

			drawer.EndShadowBar();
			drawer.EndDrawing();
		}
	}
}
