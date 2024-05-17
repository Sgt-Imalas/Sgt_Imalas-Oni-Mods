using STRINGS;
using System.Collections.Generic;

namespace Blueprints {
    public sealed class SnapshotToolHoverCard : HoverTextConfiguration {
        public bool UsingSnapshot { get; set; }

        public SnapshotToolHoverCard() {
            ToolName = BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP_TITLE;
        }

        public override void UpdateHoverElements(List<KSelectable> hoveredObjects) {
            HoverTextScreen screenInstance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = screenInstance.BeginDrawing();
            drawer.BeginShadowBar();

            DrawTitle(screenInstance, drawer);
            drawer.NewLine();

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
            drawer.DrawText(UsingSnapshot ? BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_ACTION_CLICK : BlueprintsStrings.STRING_BLUEPRINTS_CREATE_ACTION_DRAG, Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText(BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_ACTION_BACK, Styles_Instruction.Standard);

            if (UsingSnapshot) {
                drawer.NewLine(32);
                drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_SNAPSHOT_NEWSNAPSHOT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsDeleteAction.GetKAction()) + "]")), Styles_Instruction.Standard);
            }

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
