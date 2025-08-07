
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace DupePrioPresetManager
{
	internal class ModAssets
	{
		public static string DupeTemplatePath;
		public static string FoodTemplatePath;
		public static string ScheduleTemplatePath;
		public static string ResearchTemplatePath;
		public static GameObject PresetWindowPrefab;
		public static GameObject ScheduleShifterPrefab;


		public static void LoadAssets()
		{
			AssetBundle bundle = AssetUtils.LoadAssetBundle("dupe_prio_preset_window", platformSpecific: true);
			PresetWindowPrefab = bundle.LoadAsset<GameObject>("Assets/PresetWindowDupePrios.prefab");
			ScheduleShifterPrefab = bundle.LoadAsset<GameObject>("Assets/ScheduleCloner.prefab");
			//UIUtils.ListAllChildren(PresetWindowPrefab.transform);

			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(PresetWindowPrefab);
			TMPConverter.ReplaceAllText(ScheduleShifterPrefab);
		}
		public static GameObject ParentScreen
		{
			get
			{
				return parentScreen;
			}
			set
			{
				if (UnityPresetScreen_Priorities.Instance != null && parentScreen != value)
				{
					//UnityPresetScreen.Instance.transform.SetParent(parentScreen.transform, false);
					UnityEngine.Object.Destroy(UnityPresetScreen_Priorities.Instance);
					UnityPresetScreen_Priorities.Instance = null;
				}
				parentScreen = value;
			}
		}
		private static GameObject parentScreen = null;

		public static string FileNameWithHash(string filename)
		{
			return filename.Replace(" ", "_") + "_" + GenerateHash(System.DateTime.Now.ToString());
		}
		public static string GenerateHash(string str)
		{
			using (var md5Hasher = MD5.Create())
			{
				var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
				return BitConverter.ToString(data).Replace("-", "").Substring(0, 6);
			}
		}

	}
}
