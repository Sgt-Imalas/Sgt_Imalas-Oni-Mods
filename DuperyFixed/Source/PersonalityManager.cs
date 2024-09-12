using KMod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dupery
{
	class PersonalityManager
	{
		public const string PERSONALITIES_FILE_NAME = "PERSONALITIES.json";
		public const string OVERRIDE_FILE_NAME = "OVERRIDE_PERSONALITIES.json";
		public const string OVERRIDE_IMPORT_FILE_NAME = "OVERRIDE.{0}.json";

		public const int MINIMUM_PERSONALITY_COUNT = 4;

		private string loadFailureMessage = "Failed to load the '{0}' file from the Dupery mod folder. Please fix any JSON syntax errors or delete the file.";

		private Dictionary<string, PersonalityOutline> stockPersonalities;
		private Dictionary<string, PersonalityOutline> customPersonalities;
		private Dictionary<string, Dictionary<string, PersonalityOutline>> importedPersonalities;
		private Dictionary<string, Dictionary<string, string>> accessoryOwnershipMap;

		public Dictionary<string, PersonalityOutline> StockPersonalities { get { return stockPersonalities; } }
		public Dictionary<string, PersonalityOutline> CustomPersonalities { get { return customPersonalities; } }
		public Dictionary<string, Dictionary<string, PersonalityOutline>> ImportedPersonalities { get { return importedPersonalities; } }

		public PersonalityManager()
		{
			accessoryOwnershipMap = new Dictionary<string, Dictionary<string, string>>();

			// Load stock personalities
			stockPersonalities = new Dictionary<string, PersonalityOutline>();

			int personalitiesCount = Db.Get().Personalities.Count;
			for (int i = 0; i < personalitiesCount; i++)
			{
				Personality dbPersonality = Db.Get().Personalities[i];
				stockPersonalities[dbPersonality.nameStringKey] = PersonalityOutline.FromStockPersonality(dbPersonality);
			}

			string overrideFilePath = Path.Combine(DuperyPatches.DirectoryName, OVERRIDE_FILE_NAME);
			if (!File.Exists(overrideFilePath))
				Logger.Log($"Creating {OVERRIDE_FILE_NAME} override file for modifying stock personalities.");

			try
			{
				OverridePersonalities(overrideFilePath, ref stockPersonalities);
			}
			catch (PersonalityLoadException)
			{
				Logger.LogError(string.Format(loadFailureMessage, OVERRIDE_FILE_NAME));
			}

			Logger.Log($"Loaded the {stockPersonalities.Count} stock personalities.");

			// Load user created personalities
			string customPersonalitiesFilePath = Path.Combine(DuperyPatches.DirectoryName, PERSONALITIES_FILE_NAME);
			customPersonalities = new Dictionary<string, PersonalityOutline>();
			if (File.Exists(customPersonalitiesFilePath))
			{
				try
				{
					Logger.Log($"Reading custom personalities from {PERSONALITIES_FILE_NAME}...");
					customPersonalities = ReadPersonalities(customPersonalitiesFilePath);
				}
				catch (PersonalityLoadException)
				{
					Logger.LogError(string.Format(loadFailureMessage, PERSONALITIES_FILE_NAME));
				}

				if (customPersonalities != null)
					Logger.Log($"Loaded {customPersonalities.Count} user created personalities.");
			}
			else
			{
				Logger.Log($"{PERSONALITIES_FILE_NAME} not found, a fresh one will be generated.");
				WritePersonalities(customPersonalitiesFilePath, customPersonalities);
			}

			// Prepare for imported personalities
			this.importedPersonalities = new Dictionary<string, Dictionary<string, PersonalityOutline>>();
		}

		public void Cleanup()
		{
			this.stockPersonalities = null;
			this.customPersonalities = null;
			this.importedPersonalities = null;
		}

		public string FindDescription(string nameStringKey)
		{
			string description = null;
			Personality personality = Db.Get().Personalities.resources.Find(p => p.nameStringKey == nameStringKey);
			if (personality == null)
			{
				if (!DuperyPatches.Localizer.TryGet("Dupery.STRINGS.MISSING_DUPLICANT_DESCRIPTION", out description))
					description = STRINGS.MISSING_DUPLICANT_DESCRIPTION;
			}
			else
			{
				description = personality.description;
			}

			return description;
		}

		public void TryAssignAccessory(string duplicantId, string slotId, string accessoryKey)
		{
			if (DuperyPatches.AccessoryManager.TryGetAccessoryId(slotId, accessoryKey, out string accessoryId))
			{
				if (!accessoryOwnershipMap.ContainsKey(duplicantId))
				{
					accessoryOwnershipMap[duplicantId] = new Dictionary<string, string>();
				}

				accessoryOwnershipMap[duplicantId][slotId] = accessoryId.ToString();
			}
		}

		public string FindOwnedAccessory(string duplicantId, string slotId)
		{
			string accessoryId = null;

			if (accessoryOwnershipMap.ContainsKey(duplicantId))
			{
				accessoryOwnershipMap[duplicantId].TryGetValue(slotId, out accessoryId);
			}

			return accessoryId;
		}

		public bool TryImportPersonalities(string importFilePath, Mod mod)
		{
			Dictionary<string, PersonalityOutline> modPersonalities;
			try
			{
				modPersonalities = ReadPersonalities(importFilePath);
			}
			catch (PersonalityLoadException)
			{
				Logger.Log($"Failed to load {PERSONALITIES_FILE_NAME} file from mod {mod.title}.");
				return false;
			}

			string overrideFileName = string.Format(OVERRIDE_IMPORT_FILE_NAME, mod.staticID.Replace(" ", "_"));
			string overrideFilePath = Path.Combine(DuperyPatches.DirectoryName, overrideFileName);
			if (!File.Exists(overrideFilePath))
				Logger.Log($"Creating {overrideFileName} override file for {mod.title}.");

			try
			{
				OverridePersonalities(overrideFilePath, ref modPersonalities);
			}
			catch (PersonalityLoadException)
			{
				Logger.LogError(string.Format(loadFailureMessage, overrideFileName));
			}

			foreach (string key in modPersonalities.Keys)
				modPersonalities[key].SetSourceModId(mod.staticID);

			importedPersonalities[mod.staticID] = modPersonalities;
			Logger.Log($"{importedPersonalities.Count} personalities imported from {mod.title}.");

			return true;
		}

		public void OverridePersonalities(string overrideFilePath, ref Dictionary<string, PersonalityOutline> personalities)
		{
			Dictionary<string, PersonalityOutline> currentOverrides = null;
			if (File.Exists(overrideFilePath))
			{
				currentOverrides = ReadPersonalities(overrideFilePath);
			}

			Dictionary<string, PersonalityOutline> newOverrides = new Dictionary<string, PersonalityOutline>();
			foreach (string key in personalities.Keys)
			{
				PersonalityOutline overridingPersonality = null;
				if (currentOverrides != null)
					currentOverrides.TryGetValue(key, out overridingPersonality);

				if (overridingPersonality != null)
				{
					personalities[key].OverrideValues(overridingPersonality);
					newOverrides[key] = overridingPersonality;
				}
				else
				{
					newOverrides[key] = new PersonalityOutline { Printable = personalities[key].Printable, Randomize = false };
				}
			}

			WritePersonalities(overrideFilePath, newOverrides);
		}

		public static Dictionary<string, PersonalityOutline> ReadPersonalities(string personalitiesFilePath)
		{
			Dictionary<string, PersonalityOutline> jsonPersonalities;

			try
			{
				using (StreamReader streamReader = new StreamReader(personalitiesFilePath))
					jsonPersonalities = JsonConvert.DeserializeObject<Dictionary<string, PersonalityOutline>>(streamReader.ReadToEnd());
			}
			catch (Exception)
			{
				throw new PersonalityLoadException();
			}

			return jsonPersonalities;
		}

		public static void WritePersonalities(string personalitiesFilePath, Dictionary<string, PersonalityOutline> jsonPersonalities)
		{
			using (StreamWriter streamWriter = new StreamWriter(personalitiesFilePath))
			{
				string json = JsonConvert.SerializeObject(jsonPersonalities, Formatting.Indented);
				streamWriter.Write(json);
			}
		}
	}
}
