using STRINGS;
using System.Collections.Generic;

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
			drawer.NewLine(32);

			if (ModAssets.BlueprintFileHandling.HasBlueprints())
			{
				if (ModAssets.SelectedBlueprint != null)
				{
					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ACTION_SELECT, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsReopenSelectionAction.GetKAction()) + "]")), Styles_Instruction.Standard);
					drawer.NewLine(20);

					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ACTION_CHANGE_ANCHOR, UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsSwapAnchorAction.GetKAction()) + "]")), Styles_Instruction.Standard);

					if (prefabErrorCount > 0)
					{
						drawer.NewLine(32);
						drawer.DrawIcon(screenInstance.GetSprite("iconWarning"));
						drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.ERRORMESSAGE, prefabErrorCount), Styles_Instruction.Selected);
					}

					drawer.NewLine(32);
					bool forceRebuild = UseBlueprintTool.Instance?.ForceMaterialChange ?? false;
					
					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.FORCEREBUILD, (forceRebuild ? STRINGS.UI.TOOLS.USE_TOOL.REBUILD_ACTIVE : STRINGS.UI.TOOLS.USE_TOOL.REBUILD_INACTIVE ), UI.FormatAsHotkey("[" + GameUtil.GetActionString(ModAssets.Actions.BlueprintsToggleForce.GetKAction()) + "]")), Styles_Instruction.Standard);


					drawer.NewLine(32);
					drawer.DrawText(string.Format(STRINGS.UI.TOOLS.USE_TOOL.SELECTEDBLUEPRINT, ModAssets.SelectedBlueprint.FriendlyName), Styles_Instruction.Standard);
				}

				else
				{
					drawer.DrawText(STRINGS.UI.TOOLS.USE_TOOL.NONESELECTED, Styles_Instruction.Standard);
				}
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
