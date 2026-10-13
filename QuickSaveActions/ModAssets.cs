using Klei.AI;
using PeterHan.PLib.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuickSaveActions
{
	internal class ModAssets
	{
		public static PAction QuickSave, QuickLoad;

		public static void InitActions()
		{
			var mng = new PActionManager();

			QuickSave = mng.CreateAction("QSA.QuickSave", STRINGS.UI.QSA.QUICKSAVE, new PKeyBinding(KKeyCode.F5));
			QuickLoad = mng.CreateAction("QSA.QuickLoad", STRINGS.UI.QSA.QUICKLOAD, new PKeyBinding(KKeyCode.F9));
		}

		static bool SaveLoaderInitialized() => SaveLoader.Instance != null;

		public static bool TryQuickSave()
		{
			if (!SaveLoaderInitialized())
				return false;

			string savePath = SaveLoader.GetActiveSaveFilePath() ?? SaveLoader.GetAutosaveFilePath();
			if (string.IsNullOrEmpty(savePath))
				return false;

			SaveLoader.Instance.Save(savePath, true);
			return true;
		}
		public static bool TryQuickLoad()
		{
			if (!SaveLoaderInitialized())
				return false;

			string savePath = SaveLoader.GetActiveSaveFilePath() ?? SaveLoader.GetAutosaveFilePath();
			if (string.IsNullOrEmpty(savePath) || !File.Exists(savePath))
				return false;

			SaveLoader.SetActiveSaveFilePath(savePath);
			LoadingOverlay.Load(() => LoadScreen.DoLoad(savePath));
			return true;
		}
	}
}
