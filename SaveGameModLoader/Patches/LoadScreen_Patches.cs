using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LoadScreen;

namespace SaveGameModLoader.Patches
{
	internal class LoadScreen_Patches
	{
		[HarmonyPatch(typeof(LoadScreen), nameof(LoadScreen.ShowColony))]
		public static class AddModSyncButtonLogic
		{
			public static void Postfix(LoadScreen __instance, List<LoadScreen.SaveGameFileDetails> saves, int selectIndex = -1)
			{
				__instance.colonyViewRoot.TryGetComponent<HierarchyReferences>(out var hierarchyReferences);
				var container = hierarchyReferences.GetReference<RectTransform>("Content").transform; //save entry container

				int saveCount = saves.Count;
				int rectTransformCount = container.childCount;

				int childrenOffset = rectTransformCount - saveCount;

				for (int i = rectTransformCount - 1; i >= childrenOffset; i--)
				{
					var saveEntryTransform = container.GetChild(i);
					var saveEntry = saves[i - childrenOffset];

					InsertModButtonCode(saveEntryTransform.rectTransform(), saveEntry);
				}
			}

			public static void InsertModButtonCode(
				  RectTransform entry
				, LoadScreen.SaveGameFileDetails FileDetails
				)
			{

				string baseName = FileDetails.BaseName;
				string fileName = FileDetails.FileName;

				var btn = entry.Find("SyncButton").GetComponent<KButton>();

				if (btn != null)
				{
					var colonyList = ModlistManager.Instance.TryGetColonyModlist(baseName);
					bool hasStoredModlist = colonyList != null && colonyList.TryGetModListEntry(fileName, out _);
					//only allow changes in main menu

					bool CanLoadSave = App.GetCurrentSceneName() == "frontend" && FileDetails.FileInfo.IsCompatableWithCurrentDlcConfiguration(out _, out _);

					btn.isInteractable = CanLoadSave;
					if (CanLoadSave)
					{
						btn.onClick += (() =>
						{
							if (hasStoredModlist)
								ModlistManager.Instance.InstantiateModViewForPathOnly(fileName);
							else
								ModlistManager.Instance.InstantiateModViewForListOnly(fileName, ModAssets.GetModsFromSaveHeader(fileName));
						});
					}
				}
			}
		}



		[HarmonyPatch(typeof(LoadScreen), nameof(LoadScreen.GetColoniesDetails))]
		public class LoadScreen_GetColoniesDetails_Patch
		{
			[HarmonyPrepare]
			public static bool Prepare() => Config.Instance.HideIncompatibleSaves;
			public static void Postfix(LoadScreen __instance, Dictionary<string, List<SaveGameFileDetails>> __result)
			{
				HashSet<string> toRemoveSaves = [];
				bool isSpacedOut = DlcManager.IsExpansion1Active();


				foreach(var saveList in __result)
				{

					if (!saveList.Value.Any())
						continue;

					if (!__instance.CheckSaveDLCsCompatable(saveList.Value.First()))
						toRemoveSaves.Add(saveList.Key);

				}
				foreach(var toRemove in  toRemoveSaves)
					__result.Remove(toRemove);

			}
		}
	}
}
