using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UtilLibs;

namespace SetStartDupes
{
	public class MinionCrewPreset
	{

		public string FileName;
		public string CrewName;

		public MinionCrewPreset() { }

		public List<Tuple<string, MinionStatConfig>> Crewmates;

		public void ApplyCrewPreset(CharacterSelectionController controller)
		{
			SgtLogger.Assert("controller null ", controller);
			controller.DisableProceedButton();
			if (controller.containers != null)
			{
				SgtLogger.l("deleting deliverables");
				for (int i = controller.containers.Count - 1; i >= 0; i--)
				{
					UnityEngine.Object.Destroy(controller.containers[i].GetGameObject());
				}
			}

			controller.containers = new List<ITelepadDeliverableContainer>();

			SgtLogger.l("creating new deliverables");
			for (int i = 0; i < Crewmates.Count(); ++i)
			{
				CharacterContainer characterContainer = Util.KInstantiateUI<CharacterContainer>(controller.containerPrefab.gameObject, controller.containerParent);
				characterContainer.SetController(controller);
				characterContainer.DisableSelectButton();
				controller.containers.Add(characterContainer);

				OpenPresetAssignments.Add(Crewmates[i]);

			}
			controller.selectedDeliverables = new List<ITelepadDeliverable>();
			controller.EnableProceedButton();

		}
		public static List<Tuple<string, MinionStatConfig>> OpenPresetAssignments = new List<Tuple<string, MinionStatConfig>>();

		public static void ApplySingleMinion(Tuple<string, MinionStatConfig> Mate, CharacterContainer container)
		{
			//SgtLogger.l("applying stats...");
			SgtLogger.Assert("stats were null", container.stats);
			var pers = Db.Get().Personalities.GetPersonalityFromNameStringKey(Mate.first);
			if (pers != null)
				ModAssets.ApplySkinFromPersonality(pers, container.Stats);
			Mate.second.ApplyPreset(container.Stats, true, true);


			container.characterNameTitle.OnEndEdit(Mate.second.ConfigName);
			container.SetAnimator();
			container.SetAttributes();
			container.SetInfoText();
		}


		static async Task DoWithDelay(System.Action task, int ms)
		{
			await Task.Delay(ms);
			task.Invoke();
		}

		public static MinionCrewPreset CreateCrewPreset(CharacterSelectionController controller)
		{
			List<MinionStartingStats> stats = new List<MinionStartingStats>();
			foreach (var container in controller.containers)
			{
				if (container is CharacterContainer charCon)
				{
					stats.Add(charCon.Stats);
				}
			}

			if (stats.Count == 0)
				return null;
			return CreateCrewPreset(stats);
		}

		public static MinionCrewPreset CreateCrewPreset(List<MinionStartingStats> crewmates)
		{
			int count = 0;
			var preset = new MinionCrewPreset();
			string crewTitle = string.Empty;

			preset.Crewmates = new List<Tuple<string, MinionStatConfig>>();

			foreach (var mate in crewmates)
			{
				if (count > 0)
					crewTitle += ",";

				var stats = MinionStatConfig.CreateFromStartingStats(mate);
				stats.ConfigName = mate.Name;

				if (count < 4)
					crewTitle += mate.Name;


				preset.Crewmates.Add(new(mate.personality.nameStringKey, stats));
				++count;
			}
			crewTitle += STRINGS.UNNAMEDPRESET;

			preset.CrewName = crewTitle;
			preset.FileName = FileNameWithHash(crewTitle);
			return preset;
		}

		public void OpenPopUpToChangeName(System.Action callBackAction = null)
		{
			FileNameDialog fileNameDialog = (FileNameDialog)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.FileNameDialog.gameObject, ModAssets.ParentScreen);
			fileNameDialog.SetTextAndSelect(CrewName);
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
			CrewName = newName;
			FileName = FileNameWithHash(newName);
			WriteToFile();
		}

		static string FileNameWithHash(string filename)
		{
			return filename.Replace(" ", "_");
		}

		public static MinionCrewPreset ReadFromFile(FileInfo filePath)
		{
			if (!filePath.Exists || filePath.Extension != ".json")
			{
				SgtLogger.logwarning("Not a valid crew preset.");
				return null;
			}
			else
			{
				FileStream filestream = filePath.OpenRead();
				using (var sr = new StreamReader(filestream))
				{
					string jsonString = sr.ReadToEnd();
					MinionCrewPreset crew = JsonConvert.DeserializeObject<MinionCrewPreset>(jsonString);
					return crew;
				}
			}
		}
		public void WriteToFile()
		{
			try
			{
				var path = Path.Combine(ModAssets.DupeGroupTemplatePath, FileName + ".json");

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
				var path = Path.Combine(ModAssets.DupeGroupTemplatePath, FileName + ".json");

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
