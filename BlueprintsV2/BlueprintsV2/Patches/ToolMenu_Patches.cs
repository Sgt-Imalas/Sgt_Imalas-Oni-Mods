using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration;
using BlueprintsV2.BlueprintsV2.Tools;
using BlueprintsV2.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlueprintsV2.ModAssets;
using static UtilLibs.UIUtils;

namespace BlueprintsV2.BlueprintsV2.Patches
{
	class ToolMenu_Patches
	{
		static ToolMenu.ToolCollection SnapshotToolCollection, CreateBlueprintToolCollection, UseBlueprintToolCollection;

		[HarmonyPatch(typeof(ToolMenu), nameof(ToolMenu.OnKeyUp))]
		public class ToolMenu_OnKeyUp_Patch
		{
			public static void Postfix(ToolMenu __instance, KButtonEvent e)
			{
				if (e.Consumed)
					return;

				if (DetailsScreen.Instance?.isEditing ?? false)
					return;

				if (e.IsAction(Actions.BlueprintsSnapshotReuseAction.GetKAction())
					&& SnapshotTool.HasSnapshotsStored
					&& __instance.currentlySelectedCollection != SnapshotToolCollection
					)
				{
					e.Consumed = true;
					__instance.ChooseCollection(SnapshotToolCollection);
					__instance.ChooseTool(SnapshotToolCollection.tools[0]);
					SnapshotTool.Instance.TryVisualizeLastSnapshot();
				}

			}
		}
		[HarmonyPatch(typeof(ToolMenu), "OnPrefabInit")]
		public static class ToolMenuOnPrefabInit
		{
			public static void Postfix()
			{
				MultiToolParameterMenu.CreateInstance();
				ToolParameterMenu.ToggleState defaultSelection, buildingSelection;
				switch (Config.Instance.DefaultMenuSelections)
				{
					case DefaultSelections.All:
						defaultSelection = ToolParameterMenu.ToggleState.On;
						buildingSelection = ToolParameterMenu.ToggleState.On;
						break;
					case DefaultSelections.BuildingsOnly:
						defaultSelection = ToolParameterMenu.ToggleState.Off;
						buildingSelection = ToolParameterMenu.ToggleState.On;
						break;
					default:
					case DefaultSelections.None:
						defaultSelection = ToolParameterMenu.ToggleState.Off;
						buildingSelection = ToolParameterMenu.ToggleState.Off;
						break;
				}

				SnapshotTool.Instance.DefaultParameters =
				CreateBlueprintTool.Instance.DefaultParameters = new Dictionary<string, ToolParameterMenu.ToggleState> {
					{ ToolParameterMenu.FILTERLAYERS.WIRES, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.GASCONDUIT, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.BUILDINGS, buildingSelection },
					{ ToolParameterMenu.FILTERLAYERS.LOGIC, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.BACKWALL, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.DIGPLACER, defaultSelection},
					{ BlueprintCreationFilterKeys.NonSolidDigCommandssOptionID, defaultSelection },
					{ BlueprintCreationFilterKeys.Collect_Natural_Elements_ID, false ? ToolParameterMenu.ToggleState.On : ToolParameterMenu.ToggleState.Off},
					{ BlueprintCreationFilterKeys.Collect_Notes_ID, defaultSelection},
					{ BlueprintCreationFilterKeys.PlanningToolMod_ShapesID, Config.Instance. DefaultCollect_PlanningToolMod_Shapes ? ToolParameterMenu.ToggleState.On : ToolParameterMenu.ToggleState.Off},
				};
				if (!PlanningTool_Integration.ModActive)
				{
					SnapshotTool.Instance.DefaultParameters.Remove(BlueprintCreationFilterKeys.PlanningToolMod_ShapesID);
					CreateBlueprintTool.Instance.DefaultParameters.Remove(BlueprintCreationFilterKeys.PlanningToolMod_ShapesID);
				}


			}
		}

		[HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
		public static class ToolMenuCreateBasicTools
		{
			public static void Prefix(ToolMenu __instance)
			{
				CreateBlueprintToolCollection = ToolMenu.CreateToolCollection(
						STRINGS.UI.TOOLS.CREATE_TOOL.NAME,
				ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE.name,
						Actions.BlueprintsCreateAction.GetKAction(),
						nameof(CreateBlueprintTool),
						string.Format(STRINGS.UI.TOOLS.CREATE_TOOL.TOOLTIP, "{Hotkey}"),
				true
					);

				UseBlueprintToolCollection = ToolMenu.CreateToolCollection(
						STRINGS.UI.TOOLS.USE_TOOL.NAME,
						ModAssets.BLUEPRINTS_USE_ICON_SPRITE.name,
				Actions.BlueprintsUseAction.GetKAction(),
						typeof(UseBlueprintTool).Name,
						string.Format(STRINGS.UI.TOOLS.USE_TOOL.TOOLTIP, "{Hotkey}"),
				true
					);

				SnapshotToolCollection = ToolMenu.CreateToolCollection(
						STRINGS.UI.TOOLS.SNAPSHOT_TOOL.NAME,
						ModAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE.name,
				Actions.BlueprintsSnapshotAction.GetKAction(),
						typeof(SnapshotTool).Name,
						string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.TOOLTIP, "{Hotkey}", Actions.BlueprintsSnapshotReuseAction.GetFormattedPActionDescription()),
						false
					);
				__instance.basicTools.Add(CreateBlueprintToolCollection);
				__instance.basicTools.Add(UseBlueprintToolCollection);
				__instance.basicTools.Add(SnapshotToolCollection);

				BlueprintFileHandling.ReloadBlueprints(false);
			}
		}

	}
}
