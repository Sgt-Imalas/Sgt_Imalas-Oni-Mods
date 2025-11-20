using HarmonyLib;
using Klei;
using KSerialization;
using SaveGameModLoader.ModFilter;
using SaveGameModLoader.Patches;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static SaveGame;

namespace SaveGameModLoader
{
	internal class ModAssets
	{
		public static string ModPath;
		public static string ConfigPath;
		public static string ModPacksPath;
		//public static string ModID;

		public static bool FastTrackActive = false;
		public static bool ModsFilterActive = false;


		public static bool UseSteamOverlay;

		public enum BrowserChoice
		{
			undefined = 0,
			web = 1,
			steamOverlay = 2,
		}

		public static string RegistryKey = "Workshop_Browser_Choice";
		public static void ReadOrRegisterBrowserSetting()
		{
			if (KPlayerPrefs.GetInt(RegistryKey) == (int)BrowserChoice.undefined) //nothing valid set;
			{
				KPlayerPrefs.SetInt(RegistryKey, (int)BrowserChoice.steamOverlay);
			}
			UseSteamOverlay = KPlayerPrefs.GetInt(RegistryKey) == (int)BrowserChoice.steamOverlay;
		}

		static List<string> forbiddenNames = new List<string>()
		{
			"CON", "PRN", "AUX", "NUL","COM1"
			,"COM2" ,"COM3" , "COM4", "COM5" , "COM6","COM7" , "COM8", "COM9" , "LPT1"
			,"LPT2" ,"LPT3" ,"LPT4" ,"LPT5" ,"LPT6" ,"LPT7" ,"LPT8" , "LPT9"
		};

		public static string GetSanitizedNamePath(string source)
		{
			SgtLogger.l("Sanitizing...");
			//SgtLogger.l(source, "1");
			source = Path.GetFileName(source);
			//SgtLogger.l(source, "2");
			source = ReplaceInvalidChars(source);
			//SgtLogger.l(source, "3");

			if (forbiddenNames.Contains(source.ToUpperInvariant()))
			{
				SgtLogger.l("file name was one of the forbidden ones, replacing..");
				source = Path.GetRandomFileName();
				//SgtLogger.l(source, "4");
			}

			return source;
		}

		public static string ReplaceInvalidChars(string filename)
		{
			return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
		}
		//public static void ReorderVisualModState(List<ModsScreen.DisplayedMod> displayedMods, List<KMod.Mod> mods)
		//{
		//    ///no text entered => default sorting
		//    if (FilterManager.ModFilterTextCmp != null && FilterManager.ModFilterText.Length == 0 || FilterPatches._filterManager != null && FilterPatches._filterManager.Text.Length == 0)
		//        return;


		//    return;//Todo for later;
		//    Dictionary<KMod.Mod, RectTransform> originalPos = new();
		//    for (int i = 0; i < displayedMods.Count; i++)
		//    {
		//        var displayedMod = displayedMods[i];
		//        var mod = mods[displayedMod.mod_index];
		//        originalPos.Add(mod, displayedMod.rect_transform);
		//    }


		//    var sorted =
		//        mods
		//        .OrderBy(mod => MPM_Config.Instance.ModPinned(mod.label.defaultStaticID))
		//        .ThenByDescending(mod => mod.label.title);


		//    foreach (var mod in sorted)
		//    {
		//        if (originalPos.ContainsKey(mod))
		//            originalPos[mod].SetAsFirstSibling();
		//        else
		//            SgtLogger.l(mod.label.title + " not found in dictionary");
		//    }

		//}


		public static GameObject AddCopyButton(GameObject parent, System.Action onClick, System.Action onDoubleClick, ColorStyleSetting setting)
		{

			var button = Util.KInstantiateUI(FilterPatches._copyToClipboardPrefab, parent, true);


			button.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 40);
			button.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 40);
			button.TryGetComponent<KButton>(out var bt);
			//button.TryGetComponent<Image>(out var buttonimage);
			//UtilMethods.ListAllPropertyValues(buttonimage);
			bt.bgImage.colorStyleSetting = setting;

			var bgImage = button.transform.Find("GameObject").GetComponent<Image>();
			bgImage.sprite = Assets.GetSprite(SpritePatch.copySymbol);
			bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
			bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
			button.GetComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.COPYTOCLIPBOARD_TOOLTIP);
			bt.ClearOnClick();
			if (onClick != null)
				bt.onClick += onClick;
			if (onDoubleClick != null)
				bt.onDoubleClick += onDoubleClick;
			button.SetActive(true);
			button.transform.SetAsFirstSibling();
			return button;
		}

		public static bool ModWithinTextFilter(string filterText, KMod.Mod mod)
		{
			bool isWithinText = ModNameFilter(filterText, mod);
			if (!isWithinText)
			{
				isWithinText = ModAuthorFilter(filterText, mod);
			}
			return isWithinText;
		}
		public static bool ModNameFilter(string filterText, KMod.Mod mod)
		{
			return CultureInfo.InvariantCulture.CompareInfo.IndexOf(
										mod.label.title,
										filterText,
										CompareOptions.IgnoreCase
									) >= 0;
		}
		public static bool ModAuthorFilter(string filterText, KMod.Mod mod)
		{
			//original game filter code, dont allow uninstalled mods/language mods from showing
			if (mod.label.distribution_platform == KMod.Label.DistributionPlatform.Steam && SteamInfoQuery.FetchedModData.ContainsKey(mod.label.id))
			{
				return
					 mod.status != 0
				 && mod.status != KMod.Mod.Status.UninstallPending
				 && !mod.HasOnlyTranslationContent()
				 && CultureInfo.InvariantCulture.CompareInfo.IndexOf(
										SteamInfoQuery.FetchedModData[mod.label.id].authorName,
										filterText,
										CompareOptions.IgnoreCase
									) >= 0;
			}
			return false;
		}

		public static void PutCurrentToClipboard(bool linkIncluded)
		{
			List<KMod.Label> activeMods = Global.Instance.modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();
			PutToClipboard(activeMods, linkIncluded);
		}
		public static void PutToClipboard(List<KMod.Label> mods, bool includeLink)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var mod in mods)
			{
				stringBuilder.Append(mod.title);
				if (includeLink)
				{
					stringBuilder.Append(" (");
					if (mod.distribution_platform == KMod.Label.DistributionPlatform.Steam)
					{
						stringBuilder.Append("https://steamcommunity.com/sharedfiles/filedetails/?id=");
						stringBuilder.Append(mod.id);
					}
					else
					{
						stringBuilder.Append(STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.LOCAL);
					}
					stringBuilder.Append(')');
				}
				stringBuilder.AppendLine();
				//stringBuilder.AppendLine(",");
			}

			var TextEditorType = Type.GetType("UnityEngine.TextEditor, UnityEngine");
			if (TextEditorType != null)
			{
				var editor = Activator.CreateInstance(TextEditorType);
				var tr = Traverse.Create(editor);
				tr.Property("text").SetValue(stringBuilder.ToString());
				tr.Method("SelectAll").GetValue();
				tr.Method("Copy").GetValue();
			}
		}


		public static List<KMod.Label> GetModsFromSaveHeader(string filePath)
		{
			if (!File.Exists(filePath))
			{
				SgtLogger.error("Filepath for save was empty!: " + filePath);
				return null;
			}
			try
			{
				byte[] saveBytes = File.ReadAllBytes(filePath);
				IReader reader = new FastReader(saveBytes);
				SaveGame.Header header;
				//read the gameInfo to advance the filereader
				var GameInfo = SaveGame.GetHeader(reader, out header, filePath);
				KSerialization.Manager.DeserializeDirectory(reader);
				if (header.IsCompressed)
				{
					int length = saveBytes.Length - reader.Position;
					byte[] compressedBytes = new byte[length];
					Array.Copy((Array)saveBytes, reader.Position, compressedBytes, 0, length);
					byte[] uncompressedBytes = SaveLoader.DecompressContents(compressedBytes);
					return Load(new FastReader(uncompressedBytes), GameInfo);
				}
				else
				{
					return Load(reader, GameInfo);
				}

			}
			catch (Exception ex)
			{
				SgtLogger.error("Error while trying to fetch modlist for save "+filePath+":\n"+ex.Message);
				return null;
			}
		}
		private static List<KMod.Label> Load(IReader reader, GameInfo gameInfo)
		{

			Debug.Assert(reader.ReadKleiString() == "world");
			Deserializer deserializer = new Deserializer(reader);
			SaveFileRoot saveFileRoot = new SaveFileRoot();
			deserializer.Deserialize(saveFileRoot);
			if ((gameInfo.saveMajorVersion == 7 || gameInfo.saveMinorVersion < 8) && saveFileRoot.requiredMods != null)
			{
				saveFileRoot.active_mods = new List<KMod.Label>();
				foreach (ModInfo requiredMod in saveFileRoot.requiredMods)
					saveFileRoot.active_mods.Add(new KMod.Label()
					{
						id = requiredMod.assetID,
						version = (long)requiredMod.lastModifiedTime,
						distribution_platform = KMod.Label.DistributionPlatform.Steam,
						title = requiredMod.description
					});
				saveFileRoot.requiredMods.Clear();
			}
			return new(saveFileRoot.active_mods);
		}
	}
}
