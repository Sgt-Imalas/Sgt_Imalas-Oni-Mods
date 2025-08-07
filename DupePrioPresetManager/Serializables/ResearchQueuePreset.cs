using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace DupePrioPresetManager.Serializables
{
	internal class ResearchQueuePreset
	{
		public string FileName;
		public string ConfigName;
		public string GameVersion = "";
		public List<string> QueuedResearchs = [];


		public static ResearchQueuePreset CreatePreset(string presetName)
		{
			var preset = new ResearchQueuePreset();
			preset.FileName = ModAssets.FileNameWithHash(presetName);
			preset.ConfigName = presetName;
			preset.GameVersion = DlcManager.IsExpansion1Active() ? DlcManager.EXPANSION1_ID : DlcManager.VANILLA_ID;
			preset.QueuedResearchs = Research.Instance.GetResearchQueue().Select(techitem => techitem.tech.Id).ToList();
			return preset;
		}

		public void ApplyPreset()
		{
			var research = Research.Instance;
			var researchScreen = ManagementMenu.Instance?.researchScreen;
			if (research == null)
			{
				SgtLogger.l("Research is not initialized yet, cannot apply preset.");
				return;
			}

			if (!QueuedResearchs.Any())
			{
				SgtLogger.l("No queued researchs in preset, nothing to apply.");
				return;
			}
			SgtLogger.l("Applying research preset with " + QueuedResearchs.Count + " queued researchs.");

			if(researchScreen != null)
			{
				researchScreen.CancelResearch();
			}

			var techs = Db.Get().Techs;
			for (int i = 0; i < QueuedResearchs.Count; i++)
			{
				string techId = QueuedResearchs[i];
				var tech = techs.TryGet(techId);
				if(tech == null)
				{
					SgtLogger.l("Research tech with id " + techId + " not found, skipping.");
					continue;
				}
				if (i==0)
				{
					research.SetActiveResearch(tech);
				}
				else
				{
					research.AddTechToQueue(tech);
				}
				researchScreen?.GetEntry(tech)?.QueueStateChanged(true);
			}
		}

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
				this.ChangenName(newName);

				if (callBackAction != null)
					callBackAction.Invoke();
			});
		}


		public void ChangenName(string newName)
		{
			DeleteFile();
			ConfigName = newName;
			FileName = ModAssets.FileNameWithHash(newName);
			WriteToFile();
		}
		public void WriteToFile()
		{
			try
			{
				var path = Path.Combine(ModAssets.ResearchTemplatePath, FileName + ".json");

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
				var path = Path.Combine(ModAssets.ResearchTemplatePath, FileName + ".json");

				var fileInfo = new FileInfo(path);
				fileInfo.Delete();
			}
			catch (Exception e)
			{
				SgtLogger.logError("Could not delete file, Exception: " + e);
			}
		}

		internal bool IsValidForCurrentDlc()
		{
			if(DlcManager.IsExpansion1Active())
			{
				return GameVersion == DlcManager.EXPANSION1_ID;
			}
			else
			{
				return GameVersion == DlcManager.VANILLA_ID;
			}
		}
	}
}
