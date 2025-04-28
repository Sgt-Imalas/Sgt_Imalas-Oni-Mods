
using BlueprintsV2.BlueprintData;
using STRINGS;
using System.Collections.Generic;

namespace BlueprintsV2.Tools
{
	public sealed class SnapshotToolHoverCard : HoverTextConfiguration
	{
		public bool UsingSnapshot { get; set; }

		public SnapshotToolHoverCard()
		{
			ToolName = STRINGS.UI.TOOLS.SNAPSHOT_TOOL.TOOLTIP_TITLE;
		}

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

			if (UsingSnapshot)
			{
				drawer.NewLine(32);
				drawer.DrawText(string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.NEWSNAPSHOT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsReopenSelectionAction.GetKAction()) + "]")), Styles_Instruction.Standard);

				drawer.NewLine(32);
				drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ACTION_CHANGE_ANCHOR, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSwapAnchorAction.GetKAction()) + "]")), Styles_Instruction.Standard);

				drawer.NewLine(32);
				bool forceRebuild = BlueprintState.ForceMaterialChange;
				drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.FORCEREBUILD, (forceRebuild ? STRINGS.UI.TOOLS.USE_TOOL.REBUILD_ACTIVE : STRINGS.UI.TOOLS.USE_TOOL.REBUILD_INACTIVE), UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsToggleForce.GetKAction()) + "]")), Styles_Instruction.Standard);

			}

			drawer.EndShadowBar();
			drawer.EndDrawing();
		}
	}
}
