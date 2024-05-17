using BlueprintsV2.BlueprintsV2.BlueprintData;
using HarmonyLib;
using System.IO;
using TMPro;
using UnityEngine;
using UtilLibs;

namespace Blueprints
{
    public static class Utilities
    {
        public static string GetBlueprintDirectory()
        {
            string folderLocation = Path.Combine(Util.RootFolder(), "blueprints");
            if (!Directory.Exists(folderLocation))
            {
                Directory.CreateDirectory(folderLocation);
            }

            return folderLocation;
        }

        public static bool AttachFileWatcher()
        {
            string blueprintDirectory = GetBlueprintDirectory();

            BlueprintsAssets.BLUEPRINTS_AUTOFILE_WATCHER = new FileSystemWatcher
            {
                Path = blueprintDirectory,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
                Filter = "*.*"
            };

            BlueprintsAssets.BLUEPRINTS_AUTOFILE_WATCHER.Created += (sender, eventArgs) =>
            {
                if (BlueprintsAssets.BLUEPRINTS_AUTOFILE_IGNORE.Contains(eventArgs.FullPath))
                {
                    BlueprintsAssets.BLUEPRINTS_AUTOFILE_IGNORE.Remove(eventArgs.FullPath);
                    return;
                }

                if (eventArgs.FullPath.EndsWith(".blueprint") || eventArgs.FullPath.EndsWith(".json"))
                {
                    if (LoadBlueprint(eventArgs.FullPath, out Blueprint blueprint))
                    {
                        PlaceIntoFolder(blueprint);
                    }
                }
            };

            BlueprintsAssets.BLUEPRINTS_AUTOFILE_WATCHER.EnableRaisingEvents = true;
            return false;
        }

        public static void ReloadBlueprints(bool ingame)
        {
            BlueprintsState.LoadedBlueprints.Clear();
            LoadFolder(GetBlueprintDirectory());

            if (ingame && BlueprintsState.HasBlueprints())
            {
                BlueprintsState.ClearVisuals();
                BlueprintsState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), BlueprintsState.SelectedBlueprint);
            }
        }

        public static void LoadFolder(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            string[] subfolders = Directory.GetDirectories(folder);

            foreach (string file in files)
            {
                if (file.EndsWith(".blueprint") || file.EndsWith(".json"))
                {
                    if (LoadBlueprint(file, out Blueprint blueprint))
                    {
                        PlaceIntoFolder(blueprint);
                    }
                }
            }

            foreach (string subfolder in subfolders)
            {
                LoadFolder(subfolder);
            }
        }

        public static bool LoadBlueprint(string blueprintLocation, out Blueprint blueprint)
        {
            try
            {
                if (IO_Utils.ReadFromFile(blueprintLocation, out blueprint))
                    return true;
            }
            catch { };
            blueprint = new Blueprint(blueprintLocation);
            if (!blueprint.ReadBinary())
            {
                blueprint.ReadJson();
            }

            return !blueprint.IsEmpty();
        }

        public static void PlaceIntoFolder(Blueprint blueprint)
        {
            int index = -1;

            for (int i = 0; i < BlueprintsState.LoadedBlueprints.Count; ++i)
            {
                if (BlueprintsState.LoadedBlueprints[i].Name == blueprint.Folder)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                BlueprintFolder newFolder = new BlueprintFolder(blueprint.Folder);
                newFolder.AddBlueprint(blueprint);

                BlueprintsState.LoadedBlueprints.Add(newFolder);
            }

            else
            {
                BlueprintsState.LoadedBlueprints[index].AddBlueprint(blueprint);
            }
        }

        public static bool IsBuildable(this BuildingDef buildingDef)
        {
            if (!BlueprintsAssets.Options.RequireConstructable)
            {
                return true;
            }

            return PlanScreen.Instance.IsDefBuildable(buildingDef);
        }
    }

    public static class UIUtilities
    {
        public static FileNameDialog CreateTextDialog(string title, bool allowEmpty = false, System.Action<string, FileNameDialog> onConfirm = null)
        {
            GameObject textDialogParent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog textDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, textDialogParent);
            textDialog.name = "BlueprintsMod_TextDialog_" + title;

            TMP_InputField inputField = Traverse.Create(textDialog).Field("inputField").GetValue<TMP_InputField>();
            KButton confirmButton = Traverse.Create(textDialog).Field("confirmButton").GetValue<KButton>();
            if (inputField != null && confirmButton && confirmButton != null && allowEmpty)
            {
                confirmButton.onClick += delegate
                {
                    if (textDialog.onConfirm != null && inputField.text != null && inputField.text.Length == 0)
                    {
                        textDialog.onConfirm.Invoke(inputField.text);
                    }
                };
            }

            if (onConfirm != null)
            {
                textDialog.onConfirm += delegate (string result)
                {
                    onConfirm.Invoke(result.Substring(0, Mathf.Max(0, result.Length - 4)), textDialog);
                };
            }

            Transform titleTransform = textDialog.transform.Find("Panel")?.Find("Title_BG")?.Find("Title");
            if (titleTransform != null && titleTransform.GetComponent<LocText>() != null)
            {
                titleTransform.GetComponent<LocText>().text = title;
            }

            return textDialog;
        }

        public static FileNameDialog CreateFolderDialog(System.Action<string, FileNameDialog> onConfirm = null)
        {
            string title = BlueprintsStrings.STRING_BLUEPRINTS_FOLDERBLUEPRINT_TITLE;

            FileNameDialog folderDialog = CreateTextDialog(title, true, onConfirm);
            folderDialog.name = "BlueprintsMod_FolderDialog_" + title;

            return folderDialog;
        }
    }
}
