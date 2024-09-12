using PeterHan.PLib.Core;
using System.Collections.Generic;

namespace UtilLibs.ModSyncing
{
	public static class ModSyncUtils
	{
		public const string SyncModKey = "Sgt_Imalas_SyncModsKey";
		public static bool IsModSyncMod(string defaultStaticModID)
		{
			var data = PRegistry.GetData<List<string>>(SyncModKey);

			if (data == null)
				return false;

			return data.Contains(defaultStaticModID)
				|| defaultStaticModID.ToLowerInvariant().Contains("modupdatedate") || defaultStaticModID.Contains("2018291283"); //mod updater
		}
		public static bool IsModSyncMod(KMod.Mod mod) => IsModSyncMod(mod.label.defaultStaticID);
		public static void RegisterModAsSyncMod(KMod.Mod mod)
		{
			var data = PRegistry.GetData<List<string>>(SyncModKey);
			if (data == null || data.Count == 0)
			{
				data = new List<string>();
			}
			data.Add(mod.label.defaultStaticID);
			PRegistry.PutData(SyncModKey, data);
		}


	}
}
