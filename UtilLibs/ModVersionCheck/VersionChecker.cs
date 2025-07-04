using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PeterHan.PLib.AVC.JsonURLVersionChecker;
using static UtilLibs.ModVersionCheck.OutdatedVersionInfoPatches;

namespace UtilLibs.ModVersionCheck
{
	public class VersionChecker
	{
		public const string Dev_File_Local = "ImalasModVersionData.json";
		/// <summary>
		/// PLib registry key for target version information, Dictionary<string,string> with staticModD as key and version string as value. version string as semver
		/// </summary>
		public const string ModVersionDataKey_Server = "Sgt_Imalas_ServerVersionData";

		/// <summary>
		/// PLib registry key for current version information, Dictionary<string,string> with staticModD as key and version string as value. version string as semver
		/// </summary>
		public const string ModVersionDataKey_Client = "Sgt_Imalas_ClientVersionData";
		/// <summary>
		/// Version checker version plib key. only latest active version does stuff
		/// </summary>
		public const string VersionCheckerVersion = "Sgt_Imalas_UI_VersionData";
		/// <summary>
		/// plib key for boolean if main menu "outdated mods" screen is initialized
		/// </summary>
		public const string UIInitializedKey = "Sgt_Imalas_UI_Initialized";

		/// <summary>
		/// currently fetching version data from sgt_imalas github
		/// </summary>
		public const string CurrentlyFetchingKey = "Sgt_Imalas_ModVersionData_CurrentlyFetching";
		public const string VersionDataURL = "https://raw.githubusercontent.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/master/ModVersionData.json";


		/// <summary>
		/// keys of mods that require updating
		/// </summary>
		// public const string ModsRequireUpdatesDefaultStaticIDsKey = "Sgt_Imalas_ModsRequireUpdatesDefaultStaticIDsKey";
		/// <summary>
		/// PLib registry key for download path, Dictionary<string,string> with staticModD as key and direct download link as value
		/// </summary>
		//public const string ModDownloadFetchPathsKey = "Sgt_Imalas_ModDownloadFetchPathsKey";
		public const int CurrentVersion = 10;

		public static bool OlderVersion => CurrentVersion < (PRegistry.GetData<int>(VersionCheckerVersion));

		public static void RegisterCurrentVersion(KMod.UserMod2 userMod)
		{
			var currentVersionData = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Client);
			if (currentVersionData == null)
				currentVersionData = new Dictionary<string, string>();
			currentVersionData[userMod.mod.staticID] = userMod.mod.packagedModInfo.version;
			PRegistry.PutData(ModVersionDataKey_Client, currentVersionData);

			int currentMaxVersion = PRegistry.GetData<int>(VersionCheckerVersion);
			if (currentMaxVersion < CurrentVersion)
				PRegistry.PutData(VersionCheckerVersion, CurrentVersion);

			if (userMod.mod.IsDev)
			{
				///if dev mod; write to version file
				var filepath = Path.Combine(IO_Utils.ConfigsFolder, Dev_File_Local);
				SgtLogger.l("version data filepath: " + filepath);
				try
				{
					IO_Utils.ReadFromFile<JsonURLVersionChecker.ModVersions>(filepath, out var item);
					if (item == null)
						item = new JsonURLVersionChecker.ModVersions();

					var versionData = new ModVersion
					{
						staticID = userMod.mod.staticID,
						version = userMod.mod.packagedModInfo.version
					};


					item.mods.RemoveAll(mod => mod.staticID == versionData.staticID);

					item.mods.Add(versionData);
					IO_Utils.WriteToFile<JsonURLVersionChecker.ModVersions>(item, filepath);
				}
				catch (Exception ex)
				{
					SgtLogger.l(ex.Message);
				}
			}
		}
		public static void HandleVersionChecking(KMod.UserMod2 userMod, Harmony harmony)
		{
			RegisterCurrentVersion(userMod);
			OutdatedVersionInfoPatches.MainMenuMissingModsContainerInit.InitMainMenuInfoPatch(harmony);
			//CheckVersion(userMod);
			Task.Run(() => HandleDataFetching(userMod));
		}


		public static async void HandleDataFetching(KMod.UserMod2 userMod)
		{
			var data = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Server);
			if (data == null && PRegistry.GetData<bool>(CurrentlyFetchingKey) == false)
			{
				PRegistry.PutData(CurrentlyFetchingKey, true);

				SgtLogger.l("Mod Version Data was null, trying to fetch it", "SgtImalas_VersionCheck");
				using (var client = new WebClient())
				{
					var fetched = client.DownloadStringTaskAsync(VersionDataURL);
					SgtLogger.l("mod version data fetched from github", "SgtImalas_VersionCheck");
					await fetched;
					ParseData(fetched.Result);
				}
			}
		}
		static void ParseData(string data)
		{
			SgtLogger.l("parsing version data", "SgtImalas_VersionCheck");

			if (!string.IsNullOrEmpty(data))
			{
				var FoundData = JsonConvert.DeserializeObject<JsonURLVersionChecker.ModVersions>(data);
				if (FoundData != null)
				{
					Dictionary<string, string> VersionData = new Dictionary<string, string>();
					FoundData.mods.ForEach(x => VersionData[x.staticID] = x.version);
					PRegistry.PutData(ModVersionDataKey_Server, VersionData);
				}
				PRegistry.PutData(CurrentlyFetchingKey, false);
				SgtLogger.l("version data recieved", "SgtImalas_VersionCheck");
			}
		}
		private static List<ModVersionCheckResults> PlibVersionChecks()
		{

			IEnumerable<PForwardedComponent> allComponents = PRegistry.Instance.GetAllComponents("PeterHan.PLib.AVC.PVersionCheck");
			List<ModVersionCheckResults> versionChecks = new List<ModVersionCheckResults>();
			List<string> usedIDs = new List<string>();
			if (allComponents != null)
			{
				foreach (PForwardedComponent item in allComponents)
				{
					ICollection<ModVersionCheckResults> instanceDataSerialized = item.GetInstanceDataSerialized<ICollection<ModVersionCheckResults>>();
					if (instanceDataSerialized == null)
					{
						continue;
					}
					foreach (ModVersionCheckResults item2 in instanceDataSerialized)
					{
						if (usedIDs.Contains(item2.ModChecked))
							continue;

						usedIDs.Add(item2.ModChecked);
						versionChecks.Add(item2);
					}
				}
			}
			SgtLogger.l(versionChecks.Count.ToString(), "plib mod count");
			return versionChecks;
		}

		private static void AppendOutdatedMod(StringBuilder stringBuilder, string modTitle, string latestModVersion, string currentModVersion, ref int linecount, ref int modsOverLineCount, int maxLines)
		{
			if (linecount < maxLines)
			{
				stringBuilder.Append("<b>");
				stringBuilder.Append(modTitle);
				stringBuilder.Append(":</b>");
				stringBuilder.AppendLine();

				stringBuilder.Append("installed: ");
				stringBuilder.Append(currentModVersion);
				stringBuilder.Append(", latest: ");
				stringBuilder.AppendLine(latestModVersion);
				linecount += 2;
			}
			else
			{
				modsOverLineCount++;
			}
		}

		/// <summary>
		/// doesnt work due to file lock of disabled mods
		/// </summary>
		/// <param name="maxLines"></param>
		/// <param name="missingModsInfo"></param>
		/// <param name="linecount"></param>
		/// <returns></returns>

		//public static void StartOutdatedModUpdating()
		//{
		//    var ModsToUpdate = PRegistry.GetData<List<string>>(ModsRequireUpdatesDefaultStaticIDsKey); 
		//    var availableDownloadLinks = PRegistry.GetData<Dictionary<string,string>>(ModDownloadFetchPathsKey);

		//    //test, RM later
		//    if (availableDownloadLinks == null ||availableDownloadLinks.Count == 0)
		//        availableDownloadLinks = new Dictionary<string, string>()
		//        {
		//            {"Backwalls","https://github.com/aki-art/ONI-Mods/releases/download/v4.2.0.0-backwalls/backwalls-v1.4.2.zip"}
		//        };
		//    if(ModsToUpdate==null || ModsToUpdate.Count ==0)
		//        ModsToUpdate = new List<string>()
		//        {
		//            {"2921809973.Steam"}
		//        };
		//    //
		//    SgtLogger.l("starting oudated updating");


		//    var manager = Global.Instance.modManager;
		//    var state = ModUpdatingState.GetModUpdatingState();
		//    foreach(var modToUpdate in ModsToUpdate)
		//    {
		//        SgtLogger.l("outdated mod: "+modToUpdate);
		//        if (state.ModInUpdateProcess(modToUpdate))
		//            continue;


		//        var mod = manager.mods.FirstOrDefault(m => m.label.defaultStaticID == modToUpdate);
		//        if (mod == null)
		//        {
		//            SgtLogger.l("outdated mod not found: " + modToUpdate);
		//            manager.mods.ForEach(m => SgtLogger.l(m.label.defaultStaticID, m.title));
		//            continue;
		//        }
		//        if (!availableDownloadLinks.ContainsKey(mod.staticID))
		//        {
		//            SgtLogger.warning("no available download link for outdated mod " + mod.title + " found!", "Mod Updating");
		//            continue;
		//        }
		//        string downloadLink = availableDownloadLinks[mod.staticID];
		//        state.AddModToUpdate(mod, downloadLink);

		//    }
		//    if (state.HasModsToUpdate)
		//    {
		//        state.WriteModUpdatingState();
		//        state.InitializeUpdateProcess();
		//    }
		//}


		public static bool ModsOutOfDate(int maxLines, out string missingModsInfo, out int linecount)
		{
			var serverVersionData = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Server);
			var localVersionData = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Client);
			SgtLogger.Assert("local data was null", localVersionData);
			SgtLogger.Assert("server data was null", serverVersionData);

			linecount = 0;
			int modsOverLineCount = 0;
			missingModsInfo = string.Empty;
			bool outdatedModFound = false;
			if (localVersionData != null && localVersionData.Count > 0 &&
				serverVersionData != null && serverVersionData.Count > 0)
			{
				SgtLogger.l("starting version check", "Sgt_Imalas-VersionChecker");
				var manager = Global.Instance.modManager;
				StringBuilder stringBuilder = new StringBuilder();

				SgtLogger.l("checking for outdated plib version checkers", "Sgt_Imalas-VersionChecker");
				foreach (var versionEntry in PlibVersionChecks())
				{
					if (versionEntry.IsUpToDate)
						continue;

					var localMod = manager.mods.Find(mod => mod.staticID == versionEntry.ModChecked);

					//SgtLogger.l(versionEntry.ModChecked + " " + versionEntry.NewVersion + " " + versionEntry.IsUpToDate + " loc " + localMod, "plib check test");
					if (localMod == null)
						continue;

					AppendOutdatedMod(stringBuilder, localMod.title, versionEntry.NewVersion, localMod.label.version.ToString(), ref linecount, ref modsOverLineCount, maxLines);
					outdatedModFound = true;

				}

				// List<string> modLabelIdsThatRequireUpdating = new List<string>();

				int counter = 0;


				//SgtLogger.l("checking for disabled outdated mods based on version data", "Sgt_Imalas-VersionChecker");
				foreach (var localModId in serverVersionData.Keys)
				{
					//if(localVersionData.ContainsKey(localModId))
					//{
					//    continue;
					//}

					//SgtLogger.l(localModId.ToString());

					var modsWithId = manager.mods.FindAll(modEntry => modEntry.staticID == localModId);

					//var localMod = manager.mods.FirstOrDefault(mod => mod.IsEnabledForActiveDlc() && mod.staticID == localModId);
					//SgtLogger.Assert(localModId + " mod data was null!", localMod);
					if (modsWithId == null || modsWithId.Count == 0)
					{
						continue;
					}
					foreach (var localMod in modsWithId)
					{
						if (localMod != null && localMod.packagedModInfo != null)
						{
							counter++;



							///Semver version comparison
							///
							string sourceVersionString = localMod.packagedModInfo.version;
							if (sourceVersionString.Count(c => c == '.') < 3)
							{
								sourceVersionString += ".0";
							}

							string targetVersionString = serverVersionData[localModId];
							if (targetVersionString.Count(c => c == '.') < 3)
							{
								targetVersionString += ".0";
							}

							if (Version.TryParse(sourceVersionString, out var SourceVersion)
							&& Version.TryParse(targetVersionString, out var TargetVersion))
							{

								//SgtLogger.l(SourceVersion + "<->" +  TargetVersion , SourceVersion.CompareTo(TargetVersion));
								if (SourceVersion.CompareTo(TargetVersion) < 0)
								{
									AppendOutdatedMod(stringBuilder, localMod.title, TargetVersion.ToString(), SourceVersion.ToString(), ref linecount, ref modsOverLineCount, maxLines);
									SgtLogger.warning(localMod.title + " is outdated! Found local version is " + SourceVersion.ToString() + ", but latest is " + TargetVersion.ToString());
									//modLabelIdsThatRequireUpdating.Add(localMod.label.defaultStaticID);
									outdatedModFound = true;
								}
							}
							///fallback check for if semver doesnt work
							else if (localMod.packagedModInfo.version != serverVersionData[localModId])
							{
								var target = serverVersionData[localModId];
								var source = localMod.packagedModInfo.version;
								AppendOutdatedMod(stringBuilder, localMod.title, target, source, ref linecount, ref modsOverLineCount, maxLines);
								SgtLogger.warning(localMod.title + " is not the target version! Found local version is " + source + ", but target is " + target.ToString());
								//modLabelIdsThatRequireUpdating.Add(localMod.label.defaultStaticID);
								outdatedModFound = true;
							}
						}
					}
				}
				if (modsOverLineCount > 0)
				{
					linecount++;
					stringBuilder.AppendLine($"<b>...and {modsOverLineCount} other</b>");
				}
				SgtLogger.l("version checked " + counter + " mods.", "Sgt_Imalas-VersionChecker");
				// PRegistry.PutData(ModsRequireUpdatesDefaultStaticIDsKey, modLabelIdsThatRequireUpdating);
				missingModsInfo = stringBuilder.ToString();
			}
			return outdatedModFound;
		}



		public static void CheckVersion(KMod.UserMod2 userMod)
		{
			using (var client = new WebClient())
			{
				try
				{
					string responseBody = client.DownloadString(VersionDataURL);
					if (responseBody == null)
						return;

					var FoundData = JsonConvert.DeserializeObject<JsonURLVersionChecker.ModVersions>(responseBody);
					if (FoundData == null)
						return;

					var foundMod = FoundData.mods.First(mod => mod.staticID == userMod.mod.staticID);
					if (foundMod != null && Version.TryParse(foundMod.version, out var TargetVersion) && Version.TryParse(userMod.mod.packagedModInfo.version, out var SourceVersion))
					{
						SgtLogger.l(foundMod.version + "<->" + userMod.mod.packagedModInfo.version);
						SgtLogger.l(SourceVersion.CompareTo(TargetVersion).ToString(), "comparison");

						if (SourceVersion.CompareTo(TargetVersion) < 0)
						{
							SgtLogger.warning(userMod.mod.label.title + " is outdated!");
						}
					}

				}
				catch (Exception ex)
				{
					SgtLogger.warning($"{ex.Message}");
				}

			}
		}

		internal static bool UI_Built() => PRegistry.GetData<bool>(UIInitializedKey);

		internal static void SetUIConstructed(bool constructed) => PRegistry.PutData(UIInitializedKey, constructed);

		internal static void FixVersionPatch(UserMod2 usermod, Harmony harmony)
		{
			This = usermod;
			var m_TargetMethod = AccessTools.Method(typeof(KMod.Mod), "ScanContent");
			if (m_TargetMethod == null)
			{
				SgtLogger.warning("KMod.Mod.ScanContent was null!");
				return;
			}

			var m_Postfix = AccessTools.Method(typeof(VersionChecker), nameof(VersionChecker.FixVersionPostfix));

			harmony.Patch(m_TargetMethod, postfix: new HarmonyMethod(m_Postfix, Priority.LowerThanNormal)
				);
		}
		static KMod.UserMod2 This;
		static void FixVersionPostfix(KMod.Mod __instance)
		{
			if (__instance.label.id == This.mod.label.id && __instance.label.distribution_platform == This.mod.label.distribution_platform)
				__instance.packagedModInfo.version = This.assembly.GetFileVersion();
		}
	}
}
