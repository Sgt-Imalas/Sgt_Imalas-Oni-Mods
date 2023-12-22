using HarmonyLib;
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

namespace UtilLibs.ModVersionCheck
{
    internal class VersionChecker
    {
        public const string Dev_File_Local = "ImalasModVersionData.json";
        public const string ModVersionDataKey_Server = "Sgt_Imalas_ServerVersionData";
        public const string ModVersionDataKey_Client = "Sgt_Imalas_ClientVersionData";
        public const string CurrentlyFetchingKey = "Sgt_Imalas_ModVersionData_CurrentlyFetching";
        public const string VersionDataURL = "https://raw.githubusercontent.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/master/ModVersionData.json";
        public static void RegisterCurrentVersion(KMod.UserMod2 userMod)
        {
            var currentVersionData = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Client);
            if(currentVersionData == null)
                currentVersionData = new Dictionary<string, string>();
            currentVersionData[userMod.mod.staticID] = userMod.mod.packagedModInfo.version;
            PRegistry.PutData(ModVersionDataKey_Client, currentVersionData);

            if (!userMod.mod.IsDev)
            {
                return;
            }
            ///if dev mod; write to version file
            try
            {
                var filepath = Path.Combine(IO_Utils.ConfigFolder, Dev_File_Local);

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
        public static void HandleVersionChecking(KMod.UserMod2 userMod, Harmony harmony)
        {
            OutdatedVersionInfoPatches.MainMenuMissingModsContainerInit.InitMainMenuInfoPatch(harmony);
            RegisterCurrentVersion(userMod);
            //CheckVersion(userMod);
            Task.Run(() => HandleDataFetching(userMod));
        }

        public static async void HandleDataFetching(KMod.UserMod2 userMod)
        {
            var data = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Server);
            if (data == null && PRegistry.GetData<bool>(CurrentlyFetchingKey) == false)
            {
                PRegistry.PutData(CurrentlyFetchingKey, true);

                SgtLogger.l("Mod Version Data was null, trying to fetch it");
                using (var client = new WebClient())
                {
                    var fetched = client.DownloadStringTaskAsync(VersionDataURL);
                    SgtLogger.l("mod version data fetched from github");
                    ParseData(fetched.Result);
                }
            }
        }
        static void ParseData(string data)
        {
            SgtLogger.l("parsing version data");

            if (!string.IsNullOrEmpty(data))
            {
                var FoundData = JsonConvert.DeserializeObject<JsonURLVersionChecker.ModVersions>(data);
                if (FoundData != null)
                {
                    Dictionary<string,string> VersionData = new Dictionary<string,string>();
                    FoundData.mods.ForEach(x => VersionData[x.staticID] = x.version);
                    PRegistry.PutData(ModVersionDataKey_Server, VersionData);
                }
                PRegistry.PutData(CurrentlyFetchingKey, false);
            }
        }

        public static bool ModsOutOfDate(out string missingModsInfo, out int linecount)
        {
            var serverVersionData = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Server);
            var localVersionData = PRegistry.GetData<Dictionary<string, string>>(ModVersionDataKey_Client);
            SgtLogger.Assert("local data was null", localVersionData);
            SgtLogger.Assert("server data was null", serverVersionData);

            linecount = 0;
            missingModsInfo = string.Empty;
            bool outdatedModFound = false;
            if (localVersionData != null && serverVersionData != null)
            {
                SgtLogger.l("starting version check");
                var manager = Global.Instance.modManager;
                StringBuilder stringBuilder = new StringBuilder();
                //stringBuilder.AppendLine( "The following active mods are currently not on their latest version:");
                foreach (var localModId in localVersionData.Keys)
                {
                    SgtLogger.l(localModId.ToString());

                    var localMod =  manager.mods.Find(mod => mod.staticID == localModId);
                    SgtLogger.Assert(localModId + " mod data was null!", localMod);
                    //SgtLogger.l("containsKey "+ serverVersionData.ContainsKey(localModId));
                    //SgtLogger.l("parse1 "+ Version.TryParse(localVersionData[localModId], out var sss));
                    //SgtLogger.l("parse2 "+ Version.TryParse(serverVersionData[localModId], out var ss));


                    if (localMod != null 
                        && serverVersionData.ContainsKey(localModId) 
                        && Version.TryParse(localVersionData[localModId], out var SourceVersion ) 
                        && Version.TryParse(serverVersionData[localModId], out var TargetVersion))
                    {

                        //SgtLogger.l(SourceVersion + "<->" +  TargetVersion , SourceVersion.CompareTo(TargetVersion));
                        if (SourceVersion.CompareTo(TargetVersion) < 0)
                        {
                            outdatedModFound = true;
                            stringBuilder.Append("<b>");
                            stringBuilder.Append(localMod.title);
                            stringBuilder.Append(":</b>");
                            stringBuilder.AppendLine();

                            stringBuilder.Append("installed: ");
                            stringBuilder.Append(SourceVersion.ToString());
                            stringBuilder.Append(", latest: ");
                            stringBuilder.AppendLine(TargetVersion.ToString());
                            SgtLogger.warning(localMod.title + " is outdated! Found local version is "+SourceVersion.ToString()+", but latest is "+TargetVersion.ToString());
                            linecount += 2;
                        }
                    }
                }
                missingModsInfo = stringBuilder.ToString();
            }
            SgtLogger.l(outdatedModFound+"");
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
                            SgtLogger.warning(userMod.mod.label.title + " is outdated!!!!!!");
                        }
                    }

                }
                catch (Exception ex)
                {
                    SgtLogger.warning($"{ex.Message}");
                }

            }
        }
    }
}
