
using BlueprintsV2.BlueprintsV2.BlueprintData;
using STRINGS;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace BlueprintsV2.BlueprintsV2.Tools
{
    public sealed class UseBlueprintToolHoverCard : HoverTextConfiguration
    {
        [FormerlySerializedAs("PrefabErrorCount")] public int prefabErrorCount;

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
            drawer.NewLine(32);

            //if (ModAssets.BlueprintFileHandling.HasBlueprints() && ModAssets.SelectedFolder != null)
            //{
            //    if (BlueprintState.SelectedFolder.BlueprintCount > 0)
            //    {
            //        drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.FOLDERBLUEPRINT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsCreateFolderAction.GetKAction()) + "]")), Styles_Instruction.Standard);
            //        drawer.NewLine(20);

            //        drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.DELETEBLUEPRINT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsDeleteAction.GetKAction()) + "]")), Styles_Instruction.Standard);

            //        if (prefabErrorCount > 0)
            //        {
            //            drawer.NewLine(32);
            //            drawer.DrawIcon(screenInstance.GetSprite("iconWarning"));
            //            drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ERRORMESSAGE, prefabErrorCount), Styles_Instruction.Selected);
            //        }

            //        //drawer.NewLine(32);
            //        //drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.SELECTEDBLUEPRINT, BlueprintState.SelectedBlueprint.FriendlyName, BlueprintState.SelectedFolder.SelectedBlueprintIndex + 1, BlueprintState.SelectedFolder.BlueprintCount, BlueprintState.SelectedFolder.Name, BlueprintState.SelectedBlueprintFolderIndex + 1, BlueprintState.LoadedBlueprints.Count), Styles_Instruction.Standard);
            //    }

            //    else
            //    {
            //        drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.FOLDEREMPTY, BlueprintState.SelectedFolder.Name), Styles_Instruction.Standard);
            //    }
            //}

            //else
            //{
                drawer.DrawText(STRINGS.UI.TOOLS.USE_TOOL.NOBLUEPRINTS, Styles_Instruction.Standard);
            //}

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
