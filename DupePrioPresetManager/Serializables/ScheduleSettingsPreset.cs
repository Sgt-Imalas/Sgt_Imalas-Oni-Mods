using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UtilLibs;

namespace DupePrioPresetManager
{
	internal class ScheduleSettingsPreset
	{
		public string FileName;
		public string ConfigName;
		public bool InDefaultList = false;
		public List<string> ScheduleGroups;

		public void OpenPopUpToChangeName(System.Action callBackAction = null)
		{
			FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay));
			fileNameDialog.SetTextAndSelect(ConfigName);
			fileNameDialog.onConfirm = (System.Action<string>)(newName =>
			{
				if (newName.EndsWith(".sav"))
				{
					int place = newName.LastIndexOf(".sav");

					if (place != -1)
						newName = newName.Remove(place, 4);
				}
				this.ChangenName(newName);

				if (callBackAction != null)
					callBackAction.Invoke();
			});
		}

		public void ChangenName(string newName)
		{
			DeleteFile();
			ConfigName = newName;
			FileName = FileNameWithHash(newName);
			WriteToFile();
		}

		static string FileNameWithHash(string filename)
		{
			return filename.Replace(" ", "_") + "_" + GenerateHash(System.DateTime.Now.ToString());
		}

		public void ToggleActiveInDefault()
		{
			InDefaultList = !InDefaultList;
			DeleteFile();
			WriteToFile();
		}

		public void ApplyPreset(Schedule schedule)
		{
			if (schedule == null)
				return;
			var dbHandler = Db.Get().ScheduleGroups;
			schedule.name = ConfigName;
			for (int i = 0; i < ScheduleGroups.Count; ++i)
			{
				var scheduleGroup = dbHandler.TryGet(ScheduleGroups[i]);
				if (scheduleGroup == null)
				{
					SgtLogger.warning("unknown schedule block type found, defaulting to worktime");
					scheduleGroup = dbHandler.Worktime;
				}
				schedule.SetBlockGroup(i, scheduleGroup);
			}
		}

		public ScheduleSettingsPreset(string fileName, string configName, List<string> blockIdx)
		{
			FileName = fileName;
			ConfigName = configName;
			ScheduleGroups = new List<string>(blockIdx);
		}
		public ScheduleSettingsPreset() { }
		public static string GenerateHash(string str)
		{
			using (var md5Hasher = MD5.Create())
			{
				var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
				return BitConverter.ToString(data).Replace("-", "").Substring(0, 6);
			}
		}


		public static ScheduleSettingsPreset CreateFromSchedule(Schedule schedule, string nameOverride = "")
		{

			string scheduleName = nameOverride.Length > 0 ? nameOverride : schedule.name;
			var blockIDs = schedule.blocks.Select(block => block.GroupId).ToList();


			var config = new ScheduleSettingsPreset(
				FileNameWithHash(scheduleName),
				scheduleName,
				blockIDs);
			return config;
		}
		public static ScheduleSettingsPreset ReadFromFile(FileInfo filePath)
		{
			if (!filePath.Exists || filePath.Extension != ".json")
			{
				SgtLogger.logwarning("Not a valid schedule preset.");
				return null;
			}
			else
			{
				FileStream filestream = filePath.OpenRead();
				using (var sr = new StreamReader(filestream))
				{
					string jsonString = sr.ReadToEnd();
					ScheduleSettingsPreset modlist = JsonConvert.DeserializeObject<ScheduleSettingsPreset>(jsonString);
					return modlist;
				}
			}
		}

		public void DeltaBlock(int index, bool increase)
		{
			if (index >= 0 && index < ScheduleGroups.Count)
			{
				var allGroups = Db.Get().ScheduleGroups.allGroups.Select(group => group.Id).ToList();
				int newIndex = allGroups.FindIndex((x) => x == ScheduleGroups[index]);

				newIndex += increase ? 1 : -1;
				newIndex = newIndex % allGroups.Count;
				if (newIndex < 0)
				{
					newIndex += allGroups.Count;
				}

				if (newIndex == -1)
				{
					SgtLogger.warning("index not found");
					newIndex = 0;
				}
				ScheduleGroups[index] = allGroups[newIndex];
				DeleteFile();
				WriteToFile();
			}
		}


		public void WriteToFile()
		{
			try
			{
				var path = Path.Combine(ModAssets.ScheduleTemplatePath, FileName + ".json");

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
		public void DeleteFile()
		{
			try
			{
				var path = Path.Combine(ModAssets.ScheduleTemplatePath, FileName + ".json");

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
