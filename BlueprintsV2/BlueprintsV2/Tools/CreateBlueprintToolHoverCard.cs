using System.Collections.Generic;

namespace BlueprintsV2.BlueprintsV2.Tools
{
    public sealed class CreateBlueprintToolHoverCard : HoverTextConfiguration
    {
        public CreateBlueprintToolHoverCard()
        {
            ToolName = STRINGS.UI.TOOLS.CREATE_TOOL.TOOLTIP_TITLE;
        }

        public override void UpdateHoverElements(List<KSelectable> hoveredObjects)
        {
            HoverTextScreen screenInstance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = screenInstance.BeginDrawing();
            drawer.BeginShadowBar();

            DrawTitle(screenInstance, drawer);
            drawer.NewLine();

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
            drawer.DrawText(STRINGS.UI.TOOLS.CREATE_TOOL.ACTION_DRAG, Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText(STRINGS.UI.TOOLS.CREATE_TOOL.ACTION_BACK, Styles_Instruction.Standard);

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
