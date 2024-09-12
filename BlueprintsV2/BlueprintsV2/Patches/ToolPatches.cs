using BlueprintsV2.BlueprintData;
using BlueprintsV2.Tools;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using static BlueprintsV2.ModAssets;

namespace BlueprintsV2.Patches
{
	internal class ToolPatches
	{

		[HarmonyPatch(typeof(FileNameDialog), "OnSpawn")]
		public static class FileNameDialogOnSpawn
		{
			//TODO!!!!!
			public static void Postfix(FileNameDialog __instance, TMP_InputField ___inputField)
			{
				if (__instance.name.StartsWith("BlueprintsV2_"))
				{
					___inputField.onValueChanged.RemoveAllListeners();
					___inputField.onEndEdit.RemoveAllListeners();

					if (__instance.name.StartsWith("BlueprintsV2_FolderDialog_"))
					{
						___inputField.onValueChanged.AddListener(delegate (string text)
						{
							for (int i = text.Length - 1; i >= 0; --i)
							{
								if (i < text.Length && ModAssets.BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Contains(text[i]))
								{
									text = text.Remove(i, 1);
								}
							}

							___inputField.text = text;
						});
					}
				}
			}
		}

		[HarmonyPatch(typeof(ToolMenu), "OnPrefabInit")]
		public static class ToolMenuOnPrefabInit
		{
			public static void Postfix()
			{
				MultiToolParameterMenu.CreateInstance();
				ToolParameterMenu.ToggleState defaultSelection = Config.Instance.DefaultMenuSelections == DefaultSelections.All ? ToolParameterMenu.ToggleState.On : ToolParameterMenu.ToggleState.Off;

				SnapshotTool.Instance.DefaultParameters =
				CreateBlueprintTool.Instance.DefaultParameters = new Dictionary<string, ToolParameterMenu.ToggleState> {
					{ ToolParameterMenu.FILTERLAYERS.WIRES, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.GASCONDUIT, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.BUILDINGS, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.LOGIC, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.BACKWALL, defaultSelection },
					{ ToolParameterMenu.FILTERLAYERS.DIGPLACER, defaultSelection},
					{ SolidTileFiltering.StoreNonSolidsOptionID, defaultSelection },
				};
			}
		}

		[HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
		public static class ToolMenuCreateBasicTools
		{
			public static void Prefix(ToolMenu __instance)
			{
				__instance.basicTools.Add(ToolMenu.CreateToolCollection(
						STRINGS.UI.TOOLS.CREATE_TOOL.NAME,
				ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE.name,
						Actions.BlueprintsCreateAction.GetKAction(),
						nameof(CreateBlueprintTool),
						string.Format(STRINGS.UI.TOOLS.CREATE_TOOL.TOOLTIP, "{Hotkey}"),
				true
					));
				__instance.basicTools.Add(ToolMenu.CreateToolCollection(
						STRINGS.UI.TOOLS.USE_TOOL.NAME,
						ModAssets.BLUEPRINTS_USE_ICON_SPRITE.name,
				Actions.BlueprintsUseAction.GetKAction(),
						typeof(UseBlueprintTool).Name,
						string.Format(STRINGS.UI.TOOLS.USE_TOOL.TOOLTIP, "{Hotkey}"),
				true
					));
				__instance.basicTools.Add(ToolMenu.CreateToolCollection(
						STRINGS.UI.TOOLS.SNAPSHOT_TOOL.NAME,
						ModAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE.name,
				Actions.BlueprintsSnapshotAction.GetKAction(),
						typeof(SnapshotTool).Name,
						string.Format(STRINGS.UI.TOOLS.SNAPSHOT_TOOL.TOOLTIP, "{Hotkey}"),
						false
					));

				BlueprintFileHandling.ReloadBlueprints(false);
			}
		}

	}
}
