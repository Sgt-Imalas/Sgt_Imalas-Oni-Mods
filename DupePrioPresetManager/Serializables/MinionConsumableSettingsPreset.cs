using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace DupePrioPresetManager
{
	internal class MinionConsumableSettingsPreset
	{
		public string FileName;
		public string ConfigName;
		public HashSet<Tag> ForbiddenTags = new HashSet<Tag>();

		[NonSerialized]
		static List<IConsumableUIItem> consumableUiItemList = null;
		public static List<IConsumableUIItem> ConsumableUIItems
		{
			get
			{
				if (consumableUiItemList == null)
				{
					consumableUiItemList = new List<IConsumableUIItem>();
					SgtLogger.l("init consumable items");
					foreach (var food in EdiblesManager.GetAllFoodTypes())
					{
						if (!consumableUiItemList.Contains(food) && food.CaloriesPerUnit > 0)
							consumableUiItemList.Add(food);
					}
					List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(GameTags.Medicine);
					for (int index = 0; index < prefabsWithTag.Count; ++index)
					{
						MedicinalPillWorkable component = prefabsWithTag[index].GetComponent<MedicinalPillWorkable>();
						if ((bool)component && !consumableUiItemList.Contains(component))
							consumableUiItemList.Add((IConsumableUIItem)component);
						else
							DebugUtil.DevLogErrorFormat("Prefab tagged Medicine does not have MedicinalPill component: {0}", (object)prefabsWithTag[index]);
					}
					consumableUiItemList.Sort((Comparison<IConsumableUIItem>)((a, b) =>
					{
						int num = a.MajorOrder.CompareTo(b.MajorOrder);
						if (num == 0)
							num = a.MinorOrder.CompareTo(b.MinorOrder);
						return num;
					}));
					SgtLogger.l("consumable items initialized");
				}
				return consumableUiItemList;
			}
		}

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

		public MinionConsumableSettingsPreset(string fileName, string configName, HashSet<Tag> forbiddenTags)
		{
			FileName = fileName;
			ConfigName = configName;
			ForbiddenTags = forbiddenTags;

			//foreach (var consumable in ConsumableUIItems)
			//{
			//    var consumableTag = consumable.ConsumableId.ToTag();
			//    bool IsDisabled = forbiddenTags.Contains(consumableTag);
			//    FoodPriorities[consumableTag.ToString()] = IsDisabled;
			//}
		}
		public MinionConsumableSettingsPreset() { }
		public static string GenerateHash(string str)
		{
			using (var md5Hasher = MD5.Create())
			{
				var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
				return BitConverter.ToString(data).Replace("-", "").Substring(0, 6);
			}
		}


		public static MinionConsumableSettingsPreset CreateFromPriorityManager(HashSet<Tag> forbiddenFoods, string nameOverride = "")
		{

			string dupeName = nameOverride.Length > 0 ? nameOverride : STRINGS.UNNAMEDPRESET;

			var config = new MinionConsumableSettingsPreset(
				FileNameWithHash(dupeName),
				dupeName, forbiddenFoods);
			return config;
		}
		public static MinionConsumableSettingsPreset ReadFromFile(FileInfo filePath)
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
					MinionConsumableSettingsPreset modlist = JsonConvert.DeserializeObject<MinionConsumableSettingsPreset>(jsonString);
					return modlist;
				}
			}
		}

		public void ChangeValue(string ValueId)
		{
			var tag = ValueId.ToTag();
			if (ForbiddenTags.Contains(tag))
				ForbiddenTags.Remove(tag);
			else
				ForbiddenTags.Add(tag);

			DeleteFile();
			WriteToFile();

		}

		public void WriteToFile()
		{
			try
			{
				var path = Path.Combine(ModAssets.FoodTemplatePath, FileName + ".json");

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
				var path = Path.Combine(ModAssets.FoodTemplatePath, FileName + ".json");

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
