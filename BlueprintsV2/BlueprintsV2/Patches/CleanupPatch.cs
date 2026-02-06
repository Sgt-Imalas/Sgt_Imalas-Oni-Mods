using BlueprintsV2.BlueprintsV2.Tools;
using BlueprintsV2.BlueprintsV2.UnityUI;
using BlueprintsV2.Tools;
using HarmonyLib;

namespace BlueprintsV2.Patches
{
	internal class CleanupPatch
	{
		[HarmonyPatch(typeof(Game), "DestroyInstances")]
		public static class GameDestroyInstances
		{
			public static void Postfix()
			{
				CreateBlueprintTool.DestroyInstance();
				CreateNoteTool.DestroyInstance();
				UseBlueprintTool.DestroyInstance();
				SnapshotTool.DestroyInstance();
				MultiToolParameterMenu.DestroyInstance();
				ModAssets.SelectedBlueprint = null;
				ModAssets.SelectedFolder = null;
				ModAssets.BLUEPRINTS_AUTOFILE_WATCHER.Dispose();
				CurrentBlueprintStateScreen.DestroyInstance();
				NoteToolScreen.DestroyInstance();
				SpriteSelectorScreen.DestroyInstance();
			}
		}
	}
}
