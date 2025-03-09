using HarmonyLib;
using Klei;
using KMod;
using ModProfileManager_Addon.API;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UtilLibs;
using UtilLibs.ModSyncing;

namespace ModProfileManager_Addon
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);

			ModAssets.ModPath = FileSystem.Normalize(Path.Combine(Path.Combine(KMod.Manager.GetDirectory(), "config"), "[ModSync]StoredModConfigs"));
			ModAssets.ModPacksPath = FileSystem.Normalize(Path.Combine(ModAssets.ModPath, "[StandAloneModLists]"));
			ModAssets.PendingCustomDataPath = Path.Combine(this.path, "PendingCustomData.json");

			System.IO.Directory.CreateDirectory(ModAssets.ModPath);
			System.IO.Directory.CreateDirectory(ModAssets.ModPacksPath);
			ModAssets.LoadAssets();
			DialogUtil.PatchDialogCrash();

			ModSyncUtils.RegisterModAsSyncMod(this.mod);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			Mod_API.RegisterCustomModOptionHandlers();
			Mod_API.ApplyCustomDataOnLoad();
		}

		/// <summary>
		/// Example for how to store custom mod data
		/// 1 registration per type possible
		/// </summary>
		/// <param name="data"></param>
#if DEBUG
        public static void ModOptions_SetData(JObject data)
        {
            if(data != null)
            {
                var t1 = data.GetValue("CheeseKey");
                if (t1 == null)
                    return;
                var text = t1.Value<string>();
                SgtLogger.l("OnApplyModData: " + text);

            }
        }
        public static JObject ModOptions_GetData()
        {
            SgtLogger.l("Writing Cheese");
            return new JObject()
            {
               { "CheeseKey", "CheeseHasBeenWritten"}
            };
        }
#endif
	}
}
