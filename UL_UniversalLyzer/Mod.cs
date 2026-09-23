using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;
using static STRINGS.UI.SPACEDESTINATIONS.HARVESTABLE_POI;

namespace UL_UniversalLyzer
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);

			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);
			ModAssets.InitializeOrUpdateLyzerPowerCosts();

			//GameTags.MaterialBuildingElements.Add(ModAssets.Tags.RadiationShielding);
			//GameTags.MaterialBuildingElements.Add(ModAssets.Tags.NeutroniumDust);

			SgtLogger.debuglog("Initialized");
			SgtLogger.LogVersion(this, harmony);

			RegisterPipedEverything();
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
		const string PipedEverythingKey = "PipedEverything.PostMod";
		void RegisterPipedEverything()
		{
			var mods = PRegistry.GetData<List<string>>(PipedEverythingKey);
			if (mods == null)
				mods = new List<string>();

			mods.Add("$UniversalElectrolyzer");

			if (Config.Instance.IsPiped == false)
			{
				SgtLogger.l("Disabling piping on electrolyzer");
				mods.Add("DEL");
				mods.Add("""{"Id":"Electrolyzer","Input":false,"OffsetX":99,"OffsetY":99,"Filter":["Gas"]}""");
			}
			else
			{
				SgtLogger.l("Adding additional piping on electrolyzer");
				mods.Add("ADD");
				mods.Add("""{"Id":"Electrolyzer","Input":false,"OffsetX":0,"OffsetY":0,"Filter":["ChlorineGas"],"Color":{"r":144,"g":208,"b":92,"a":255}}""");
				mods.Add("""{"Id":"Electrolyzer","Input":false,"OffsetX":1,"OffsetY":0,"Filter":["ContaminatedOxygen"],"Color":{"r":83,"g":118,"b":102,"a":255}}""");
			}

			PRegistry.PutData(PipedEverythingKey, mods);
		}
	}
}
