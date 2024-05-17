using BlueprintsV2.BlueprintsV2.BlueprintData;
using STRINGS;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Blueprints {
    public sealed class UseBlueprintToolHoverCard : HoverTextConfiguration {
        [FormerlySerializedAs("PrefabErrorCount")] public int prefabErrorCount;

        public UseBlueprintToolHoverCard() {
            ToolName = BlueprintsStrings.STRING_BLUEPRINTS_USE_TOOLTIP_TITLE;
        }

        public override void UpdateHoverElements(List<KSelectable> hoveredObjects) {
            HoverTextScreen screenInstance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = screenInstance.BeginDrawing();
            drawer.BeginShadowBar();

            DrawTitle(screenInstance, drawer);
            drawer.NewLine();

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
            drawer.DrawText(BlueprintsStrings.STRING_BLUEPRINTS_USE_ACTION_CLICK, Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText(BlueprintsStrings.STRING_BLUEPRINTS_USE_ACTION_BACK, Styles_Instruction.Standard);
            drawer.NewLine(32);

            if (BlueprintsState.HasBlueprints()) {
                if (BlueprintsState.SelectedFolder.BlueprintCount > 0) {
                    drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_CYCLEFOLDERS, UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsCycleFoldersNextAction.GetKAction()) + "]"), UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsCycleFoldersPrevAction.GetKAction()) + "]")), Styles_Instruction.Standard);
                    drawer.NewLine(20);

                    drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_CYCLEBLUEPRINTS, UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsCycleBlueprintsNextAction.GetKAction()) + "]"), UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsCycleBlueprintsPrevAction.GetKAction()) + "]")), Styles_Instruction.Standard);
                    drawer.NewLine(32);

                    drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_FOLDERBLUEPRINT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsCreateFolderAction.GetKAction()) + "]")), Styles_Instruction.Standard);
                    drawer.NewLine(20);

                    drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_NAMEBLUEPRINT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsRenameAction.GetKAction()) + "]")), Styles_Instruction.Standard);
                    drawer.NewLine(20);

                    drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_DELETEBLUEPRINT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(Integration.BlueprintsDeleteAction.GetKAction()) + "]")), Styles_Instruction.Standard);

                    if (prefabErrorCount > 0) {
                        drawer.NewLine(32);
                        drawer.DrawIcon(screenInstance.GetSprite("iconWarning"));
                        drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_ERRORMESSAGE, prefabErrorCount), Styles_Instruction.Selected);
                    }

                    drawer.NewLine(32);
                    drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_SELECTEDBLUEPRINT, BlueprintsState.SelectedBlueprint.FriendlyName, BlueprintsState.SelectedFolder.SelectedBlueprintIndex + 1, BlueprintsState.SelectedFolder.BlueprintCount, BlueprintsState.SelectedFolder.Name, BlueprintsState.SelectedBlueprintFolderIndex + 1, BlueprintsState.LoadedBlueprints.Count), Styles_Instruction.Standard);
                }

                else {
                    drawer.DrawText(string.Format(BlueprintsStrings.STRING_BLUEPRINTS_USE_FOLDEREMPTY, BlueprintsState.SelectedFolder.Name), Styles_Instruction.Standard);
                }
            }

            else {
                drawer.DrawText(BlueprintsStrings.STRING_BLUEPRINTS_USE_NOBLUEPRINTS, Styles_Instruction.Standard);
            }

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
