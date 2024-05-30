
using STRINGS;
using System.Collections.Generic;

namespace BlueprintsV2.BlueprintsV2.Tools
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
            drawer.DrawText(UsingSnapshot ? STRINGS.UI.TOOLS.SNAPSHOT_TOOL.ACTION_CLICK : STRINGS.UI.TOOLS.CREATE_TOOL.ACTION_DRAG, Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.ACTION_BACK, Styles_Instruction.Standard);

            if (UsingSnapshot)
            {
                drawer.NewLine(32);
                drawer.DrawText(string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.NEWSNAPSHOT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsDeleteAction.GetKAction()) + "]")), Styles_Instruction.Standard);
            }

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
