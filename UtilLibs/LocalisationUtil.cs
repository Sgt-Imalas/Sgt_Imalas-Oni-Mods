using HarmonyLib;
using Klei.CustomSettings;
using KMod;
using Mono.Cecil.Cil;
using MonoMod.Utils;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilLibs
{
	public static class LocalisationUtil
	{
		static Type stringType;


		static Dictionary<string, Dictionary<string, string>> LocalizedStrings = null;
		static Dictionary<string, string> CachedTranslationMods = new()
		{
			{"de", "929139073" }, //german translation mod
		};

		public static bool TryGetTranslatedString(string key, out string translatedString)
		{
			translatedString = null;
			var languageType = Localization.GetSelectedLanguageType();
			if (languageType == Localization.SelectedLanguageType.None)
				return false;

			var languageCode = Localization.GetCurrentLanguageCode();

			return TryGetTranslatedString(languageCode, languageType, key, out translatedString);
		}
		public static bool TryGetTranslatedString(string languageCode, string key, out string translatedString)
		{
			translatedString = null;
			string kleiCode = languageCode + "_klei";
			Localization.SelectedLanguageType languageType = Localization.SelectedLanguageType.None;

			if (Localization.PreinstalledLanguages.Contains(kleiCode))
				languageType = Localization.SelectedLanguageType.Preinstalled;
			else if (CachedTranslationMods.TryGetValue(languageCode, out var modID))
				languageType = Localization.SelectedLanguageType.UGC;

			if (languageType == Localization.SelectedLanguageType.None)
				return false;
			return TryGetTranslatedString(kleiCode, languageType, key, out translatedString);
		}



		/// <summary>
		/// Loads the current localization to get translated strings for the custom game settings fixing
		/// </summary>
		/// <param name="key"></param>
		/// <param name="translatedString"></param>
		/// <returns></returns>
		public static bool TryGetTranslatedString(string languageCodeKlei, Localization.SelectedLanguageType languageType, string key, out string translatedString)
		{
			string languageCode = languageCodeKlei.Replace("_klei", string.Empty);

			translatedString = null;
			if (LocalizedStrings == null)
				LocalizedStrings = new();

			if (!LocalizedStrings.TryGetValue(languageCode, out var translatedStrings))
			{
				if (languageType == Localization.SelectedLanguageType.Preinstalled && !string.IsNullOrEmpty(languageCodeKlei) && languageCodeKlei != Localization.DEFAULT_LANGUAGE_CODE)
				{
					var translationFile = Localization.GetPreinstalledLocalizationFilePath(languageCodeKlei);
					if (!File.Exists(translationFile))
						return false;
					try
					{
						SgtLogger.l("Loading game translations from: " + translationFile);
						var data = File.ReadAllLines(translationFile, Encoding.UTF8);
						if (!LocalizedStrings.ContainsKey(languageCode))
							LocalizedStrings[languageCode] = [];
						LocalizedStrings[languageCode].AddRange(Localization.ExtractTranslatedStrings(data, false));
						translatedStrings = LocalizedStrings[languageCode];
					}
					catch (Exception ex)
					{
						SgtLogger.error("Error while trying to load translations: \n" + ex.Message);
					}
				}
				else if (languageType == Localization.SelectedLanguageType.UGC && LanguageOptionsScreen.HasInstalledLanguage())
				{
					string savedLanguageMod = LanguageOptionsScreen.GetSavedLanguageMod();
					if (savedLanguageMod.IsNullOrWhiteSpace() && CachedTranslationMods.TryGetValue(languageCodeKlei, out var modID))
					{
						savedLanguageMod = modID;
					}

					try
					{
						KMod.Mod mod = Global.Instance.modManager.mods.Find((Predicate<KMod.Mod>)(m => m.label.id == savedLanguageMod));
						if (mod == null)
						{
							Debug.LogWarning(("Tried loading a translation from a non-existent mod id: " + savedLanguageMod));
							return false;
						}
						string translationFile = LanguageOptionsScreen.GetLanguageFilename(mod);
						if (!File.Exists(translationFile))
							return false;
						SgtLogger.l("Loading modded translations from: " + translationFile);
						var data = File.ReadAllLines(translationFile, Encoding.UTF8);
						if (!LocalizedStrings.ContainsKey(languageCode))
							LocalizedStrings[languageCode] = [];
						LocalizedStrings[languageCode].AddRange(Localization.ExtractTranslatedStrings(data, false));
						translatedStrings = LocalizedStrings[languageCode];
					}
					catch (Exception ex)
					{
						SgtLogger.error("Error while trying to load translations: \n" + ex.Message);
					}
				}

				var currentModLocFile = Path.Combine(IO_Utils.ModPath, "translations", languageCode + ".po");
				if (File.Exists(currentModLocFile))
				{
					try
					{
						SgtLogger.l("Loading local mod translations from: " + currentModLocFile);
						var data = File.ReadAllLines(currentModLocFile, Encoding.UTF8);
						if (!LocalizedStrings.ContainsKey(languageCode))
							LocalizedStrings[languageCode] = [];
						var modLoc = Localization.ExtractTranslatedStrings(data, false);
						SgtLogger.l("Extracted " + modLoc.Count + " localizationStrings");
						foreach (var loc in modLoc)
						{
							string locKey = loc.Key;
							int start = locKey.IndexOf("STRINGS");
							var DeNamespaced = locKey.Substring(start);
							LocalizedStrings[languageCode].Add(DeNamespaced, loc.Value);
						}

						translatedStrings = LocalizedStrings[languageCode];
					}
					catch (Exception ex)
					{
						SgtLogger.error("Error while trying to load translations: \n" + ex.Message);
					}
				}
				//SgtLogger.l("Localization reloaded:");
				//foreach (var kvp in translatedStrings)
				//{
				//	SgtLogger.l(kvp.Value, kvp.Key);
				//}
			}
			if (translatedStrings == null)
			{
				SgtLogger.error("strings was null");
				return false;
			}
			return translatedStrings.TryGetValue(key, out translatedString);
		}

		public static void ManualTranslationPatch(Harmony harmony, Type type)
		{
			stringType = type;
			var m_TargetMethod = AccessTools.Method("Localization, Assembly-CSharp:Initialize");
			//var m_Transpiler = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Transpiler");
			var m_Postfix = AccessTools.Method(typeof(LocalisationUtil), "Postfix");

			harmony.Patch(m_TargetMethod, postfix: new HarmonyMethod(m_Postfix));
		}
		public static void Postfix()
		{
			if (stringType != null)
				LocalisationUtil.Translate(stringType, true);
		}

		public static void Translate(Type root, bool generateTemplate = false)
		{
			Localization.RegisterForTranslation(root);
			if (generateTemplate)
				GenerateStringTemplates(root);
			OverLoadStrings();
			LocString.CreateLocStringKeys(root, null);
		}
		public static void GenerateStringTemplates(Type root)
		{
			var translationFolder = Path.Combine(IO_Utils.ModPath, "translations");
			System.IO.Directory.CreateDirectory(translationFolder);

			Localization.GenerateStringsTemplate(root, Path.Combine(Manager.GetDirectory(), "strings_templates"));
			Localization.GenerateStringsTemplate(root.Namespace, Assembly.GetExecutingAssembly(), Path.Combine(IO_Utils.ModPath, "translation_template.pot"), null);
			Localization.GenerateStringsTemplate(root.Namespace, Assembly.GetExecutingAssembly(), Path.Combine(translationFolder, "translation_template.pot"), null);

		}

		// Loads user created translations
		private static void OverLoadStrings()
		{
			string code = Localization.GetLocale()?.Code;

			if (code.IsNullOrWhiteSpace()) return;

			string path = Path.Combine(UtilMethods.ModPath, "translations", Localization.GetLocale().Code + ".po");

			if (File.Exists(path))
			{
				Localization.OverloadStrings(Localization.LoadStringsFile(path, false));
				Debug.Log($"Found translation file for {code}.");
			}
		}
	}
}
