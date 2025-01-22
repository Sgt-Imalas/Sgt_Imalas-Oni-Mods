using Beached_ModAPI;
using Database;
using Klei.AI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UtilLibs;

namespace SetStartDupes
{
	public class MinionStatConfig
	{
		public string FileName;
		public string ConfigName;
		public List<string> Traits = new List<string>();
		public string stressTrait;
		public string joyTrait;
		public List<KeyValuePair<string, int>> StartingLevels = new List<KeyValuePair<string, int>>();
		public List<KeyValuePair<string, float>> skillAptitudes = new List<KeyValuePair<string, float>>();
		public float StarterXP = 0;
		public float Age = 0;
		public string PersonalityID;
		public string DLCID = "";
		public Tag Model = null;

		public void OpenPopUpToChangeName(System.Action callBackAction = null)
		{
			FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, ModAssets.ParentScreen);
			fileNameDialog.SetTextAndSelect(ConfigName);
			fileNameDialog.onConfirm = (System.Action<string>)(newName =>
			{
				if (newName.EndsWith(".sav"))
				{
					int place = newName.LastIndexOf(".sav");

					if (place != -1)
						newName = newName.Remove(place, 4);
				}
				this.ChangeName(newName);

				if (callBackAction != null)
					callBackAction.Invoke();
			});
		}

		public void SetName(string newName)
		{
			ConfigName = newName;
			FileName = FileNameWithHash(newName);
		}

		public void ChangeName(string newName)
		{
			DeleteFile();
			SetName(newName);
			WriteToFile();
		}

		static string FileNameWithHash(string filename)
		{
			return filename.Replace(" ", "_");// + "_" + GenerateHash(System.DateTime.Now.ToString());
		}

		public MinionStatConfig(string fileName, string configName, List<Trait> traits, Trait stressTrait, Trait joyTrait, List<KeyValuePair<string, int>> startingLevels, List<KeyValuePair<SkillGroup, float>> skillAptitudes, Tag model)
		{
			FileName = fileName;
			ConfigName = configName;
			Traits = traits.Select(trait => trait.Id).ToList();
			Traits.RemoveAll(trait => trait == ANCIENTKNOWLEDGE);
			this.stressTrait = stressTrait.Id;
			this.joyTrait = joyTrait.Id;
			StartingLevels = startingLevels;
			this.skillAptitudes = skillAptitudes.Select(kvp => new KeyValuePair<string, float>(kvp.Key.Id, kvp.Value)).ToList();

			if (DlcManager.IsExpansion1Active())
				DLCID = DlcManager.EXPANSION1_ID;
			Model = model;
		}
		public MinionStatConfig() { }
		public MinionStatConfig(string fileName, string configName, List<string> traits, string stressTrait, string joyTrait, List<KeyValuePair<string, int>> startingLevels, List<KeyValuePair<string, float>> skillAptitudes, Tag model)
		{
			FileName = fileName;
			ConfigName = configName;
			Traits = traits;
			this.stressTrait = stressTrait;
			this.joyTrait = joyTrait;
			StartingLevels = startingLevels;
			this.skillAptitudes = skillAptitudes;
			if (DlcManager.IsExpansion1Active())
				DLCID = DlcManager.EXPANSION1_ID;
			//WriteToFile();
			Model = model;
		}

		public static string GenerateHash(string str)
		{
			using (var md5Hasher = MD5.Create())
			{
				var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
				return BitConverter.ToString(data).Replace("-", "").Substring(0, 6);
			}
		}
		internal static void RegisterTearDuplicant(StoredMinionIdentity storedMinionIdentity)
		{
			CreateFromStoredMinionIdentiy(storedMinionIdentity).WriteToFile(true);
		}
		internal static void RegisterTearDuplicant(MinionIdentity minionIdentity)
		{
			CreateFromMinionIdentiy(minionIdentity).WriteToFile(true);
		}
		public static MinionStatConfig CreateFromStoredMinionIdentiy(StoredMinionIdentity dupe)
		{
			List<KeyValuePair<string, int>> startingLevels = new List<KeyValuePair<string, int>>();
			List<KeyValuePair<string, float>> skillAptitudes = new List<KeyValuePair<string, float>>();
			List<string> traitsId = new List<string>();
			string stress = null, joy = null;

			foreach (var trait in dupe.traitIDs)
			{
				switch (ModAssets.GetTraitListOfTrait(trait))
				{
					case DupeTraitManager.NextType.joy:
						joy = trait;
						break;
					case DupeTraitManager.NextType.stress:
						stress = trait;
						break;
					default:
						traitsId.Add(trait);
						break;
				}
			}

			foreach (var attribute in dupe.attributeLevels)
			{
				startingLevels.Add(new KeyValuePair<string, int>(attribute.attributeId, attribute.level));
			}

			var groups = Db.Get().SkillGroups;
			foreach (var skillAptitude in dupe.AptitudeBySkillGroup)
			{
				var group = groups.Get(skillAptitude.Key);

				if (group == null)
				{
					SgtLogger.error(skillAptitude.Key + " was no viable skillgroup!");
					continue;
				}

				skillAptitudes.Add(new KeyValuePair<string, float>(group.Id, skillAptitude.Value));
			}

			var config = new MinionStatConfig(
				FileNameWithHash(SaveGame.Instance.BaseName + "_" + dupe.name),
				dupe.name,
				traitsId,
				stress,
				joy,
				startingLevels,
				skillAptitudes, 
				dupe.model);
			config.StarterXP = dupe.TotalExperienceGained;
			config.Age = GameClock.Instance.GetCycle() - dupe.arrivalTime;
			config.PersonalityID = dupe.nameStringKey;


			return config;
		}
		public static MinionStatConfig CreateFromMinionIdentiy(MinionIdentity dupe)
		{
			if (dupe.TryGetComponent<MinionResume>(out var resume)
			   && dupe.TryGetComponent<Traits>(out var traits)
			   && dupe.TryGetComponent<AttributeLevels>(out var attributes)
			   )
			{
				List<KeyValuePair<string, int>> startingLevels = new List<KeyValuePair<string, int>>();
				List<KeyValuePair<SkillGroup, float>> skillAptitudes = new List<KeyValuePair<SkillGroup, float>>();
				List<Trait> traitsId = new List<Trait>();
				Trait stress = null, joy = null;

				foreach (var trait in traits.TraitList)
				{
					switch (ModAssets.GetTraitListOfTrait(trait))
					{
						case DupeTraitManager.NextType.joy:
							joy = trait;
							break;
						case DupeTraitManager.NextType.stress:
							stress = trait;
							break;
						default:
							traitsId.Add(trait);
							break;
					}
				}

				foreach (AttributeLevel attribute in attributes.levels)
				{
					startingLevels.Add(new KeyValuePair<string, int>(attribute.attribute.Attribute.Id, attribute.level));
				}

				var groups = Db.Get().SkillGroups;
				foreach (var skillAptitude in resume.AptitudeBySkillGroup)
				{
					var group = groups.Get(skillAptitude.Key);

					if (group == null)
					{
						SgtLogger.error(skillAptitude.Key + " was no viable skillgroup!");
						continue;
					}

					skillAptitudes.Add(new KeyValuePair<SkillGroup, float>(group, skillAptitude.Value));
				}

				var config = new MinionStatConfig(
					FileNameWithHash(SaveGame.Instance.BaseName + "_" + dupe.name),
					dupe.name,
					traitsId,
					stress,
					joy,
					startingLevels,
					skillAptitudes,
					dupe.model);
				config.StarterXP = resume.TotalExperienceGained;
				config.Age = GameClock.Instance.GetCycle() - dupe.arrivalTime;
				config.PersonalityID = dupe.nameStringKey;

				return config;
			}
			return null;
		}

		public static MinionStatConfig CreateFromStartingStats(MinionStartingStats startingStats)
		{
			List<KeyValuePair<string, float>> skillAptitudes = new List<KeyValuePair<string, float>>();
			foreach (var kvp in startingStats.skillAptitudes)
			{
				skillAptitudes.Add(new KeyValuePair<string, float>(kvp.Key.Id, kvp.Value));
			}
			string dupeName = startingStats.Name + " " + STRINGS.UNNAMEDPRESET;

			var config = new MinionStatConfig(
				FileNameWithHash(dupeName),
				dupeName,
				startingStats.Traits,
				startingStats.stressTrait,
				startingStats.joyTrait,
				startingStats.StartingLevels.ToList(),
				startingStats.skillAptitudes.ToList(),
				startingStats.personality.model);

			if (ModAssets.Beached_LifegoalsActive)
				config.Traits.Add(Beached_API.GetCurrentLifeGoal(startingStats).Id);

			return config;
		}

		static string STICKERBOMBER = "StickerBomber";
		static string ANCIENTKNOWLEDGE = "AncientKnowledge";
		static string CHATTY = "Chatty";
		public void ApplyPreset(MinionStartingStats referencedStats, bool overrideName, bool overrideReaction)
		{
			if (overrideName)
				referencedStats.Name = this.ConfigName.Replace(STRINGS.UNNAMEDPRESET, string.Empty);

			bool HadChatty = referencedStats.Traits.Any(trait => trait.Id == CHATTY);
			bool HadAncientKnowledge = referencedStats.Traits.Any(trait => trait.Id == ANCIENTKNOWLEDGE);
			referencedStats.Traits.Clear();
			var traitRef = Db.Get().traits;


			var baseTrait = BaseMinionConfig.GetMinionBaseTraitIDForModel(referencedStats.personality.model);
			if (!Traits.Contains(baseTrait))
					Traits.Add(baseTrait);

			if (HadAncientKnowledge)
			{
				Traits.Add(ANCIENTKNOWLEDGE);
			}
			else
				Traits.RemoveAll(trait => trait == ANCIENTKNOWLEDGE);

			if (HadChatty)
			{
				Traits.Add(CHATTY);
			}
			else
				Traits.RemoveAll(trait => trait == CHATTY);

			SgtLogger.l("Applying traits");
			foreach (var traitID in this.Traits)
			{
				var Trait = traitRef.TryGet(traitID);

				if (Trait != null && ModAssets.GetTraitListOfTrait(Trait) == DupeTraitManager.NextType.Beached_LifeGoal)
				{
					Beached_API.RemoveLifeGoal(referencedStats);
					Beached_API.SetLifeGoal(referencedStats, Trait, false);
					continue;
				}

				if (Trait != null && ModAssets.TraitAllowedInCurrentDLC(traitID))
				{
					referencedStats.Traits.Add(Trait);
				}
			}

			SgtLogger.l("Applying starting levels");
			referencedStats.StartingLevels.Clear();
			HashSet<string> validAttributes = new HashSet<string>(DUPLICANTSTATS.ALL_ATTRIBUTES);
			foreach (var startLevel in this.StartingLevels)
			{
				if (validAttributes.Contains(startLevel.Key))
					referencedStats.StartingLevels[startLevel.Key] = startLevel.Value;
				else
					SgtLogger.warning("couldnt apply attribute level for " + startLevel.Key + " as it is not a valid attribute.");
			}
			foreach (var requiredAttribute in validAttributes)
			{
				if (!referencedStats.StartingLevels.ContainsKey(requiredAttribute))
				{
					SgtLogger.l("adding missing attribute level " + requiredAttribute + ", defaulting to 0.");
					referencedStats.StartingLevels[requiredAttribute] = 0;
				}
			}

			SgtLogger.l("Applying joy reaction");
			if (!Config.Instance.NoJoyReactions)
			{
				if (overrideReaction)
					referencedStats.joyTrait = traitRef.Get(this.joyTrait);
			}
			else
			{
				referencedStats.joyTrait = traitRef.Get("None");
			}
			///fixes invis stickers;
			if (joyTrait == STICKERBOMBER && referencedStats.stickerType.IsNullOrWhiteSpace())
			{
				referencedStats.stickerType = ModAssets.GetRandomStickerType();
			}


			SgtLogger.l("Applying stress reaction");
			if (!Config.Instance.NoStressReactions)
			{
				if (overrideReaction)
					referencedStats.stressTrait = traitRef.Get(this.stressTrait);
			}
			else
			{
				referencedStats.stressTrait = traitRef.Get("None");
			}

			if (ModAssets.DupeTraitManagers.ContainsKey(referencedStats))
			{
				ModAssets.DupeTraitManagers[referencedStats].ResetPool();
			}

			var AptitudeRef = Db.Get().SkillGroups;
			referencedStats.skillAptitudes.Clear();

			SgtLogger.l("Applying skill aptitudes");
			foreach (var skillAptitude in this.skillAptitudes)
			{
				SkillGroup targetGroup = AptitudeRef.TryGet(skillAptitude.Key);
				if (targetGroup != null)
				{
					referencedStats.skillAptitudes[targetGroup] = skillAptitude.Value;
				}
			}


			if (ModAssets.OtherModBonusPoints.ContainsKey(referencedStats))
			{
				ModAssets.OtherModBonusPoints.Remove(referencedStats);
			}
			if (ModAssets.DupeTraitManagers.ContainsKey(referencedStats))
			{
				ModAssets.DupeTraitManagers[referencedStats].RecalculateAll();
			}
		}

		public static MinionStatConfig ReadFromFile(FileInfo filePath)
		{
			if (!filePath.Exists || filePath.Extension != ".json")
			{
				SgtLogger.logwarning("Not a valid dupe preset.");
				return null;
			}
			else
			{
				FileStream filestream = filePath.OpenRead();
				using (var sr = new StreamReader(filestream))
				{
					string jsonString = sr.ReadToEnd();
					MinionStatConfig preset = JsonConvert.DeserializeObject<MinionStatConfig>(jsonString);
					preset.Migrate();
					return preset;
				}
			}
		}

		private void Migrate()
		{
			if(Model == null)
			{
				Model = GameTags.Minions.Models.Standard;
			}
		}

		public string SkillGroupName(string groupID)
		{
			if (groupID == null)
				return "";
			else
			{

				var skillGroup = Db.Get().SkillGroups.TryGet(groupID);
				if (skillGroup == null)
				{
					return STRINGS.MISSINGSKILLGROUP;
				}
				else
				{
					string relevantSkillID = skillGroup.relevantAttributes.First().Id;
					return string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, ModAssets.GetChoreGroupNameForSkillgroup(skillGroup), SkillGroup(skillGroup), SkillLevel(relevantSkillID));
				}
			}
		}
		public string SkillGroupDesc(string groupID)
		{
			if (groupID == null)
				return "";
			else
			{
				var skillGroup = Db.Get().SkillGroups.TryGet(groupID);
				return ModAssets.GetSkillgroupDescription(skillGroup, id: groupID);
			}
		}
		public string SkillGroup(SkillGroup group)
		{
			return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME");
		}
		string SkillLevel(string skillID)
		{
			return StartingLevels.Find((skill) => skill.Key == skillID).Value.ToString();
		}

		public void WriteToFile(bool tearDupe = false)
		{
			try
			{
				string templatePath = tearDupe ? ModAssets.DupeTearTemplatePath : ModAssets.DupeTemplatePath;
				var path = Path.Combine(templatePath, FileName + ".json");

				var fileInfo = new FileInfo(path);
				FileStream fcreate = fileInfo.Open(FileMode.Create);

				var JsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
				using (var streamWriter = new StreamWriter(fcreate))
				{
					streamWriter.Write(JsonString);
				}
			}
			catch (Exception e)
			{
				SgtLogger.logError("Could not write file, Exception: " + e);
			}
		}
		public void DeleteFile(bool tearDupe = false)
		{
			try
			{
				string templatePath = tearDupe ? ModAssets.DupeTearTemplatePath : ModAssets.DupeTemplatePath;
				var path = Path.Combine(templatePath, FileName + ".json");

				var fileInfo = new FileInfo(path);
				fileInfo.Delete();
			}
			catch (Exception e)
			{
				SgtLogger.logError("Could not delete file, Exception: " + e);
			}
		}
	}
}
