///Filelocks from game make it impossible to use


//using KMod;
//using KSerialization;
//using Newtonsoft.Json;
//using PeterHan.PLib.AVC;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using static PeterHan.PLib.AVC.JsonURLVersionChecker;
//using static STRINGS.CODEX.CRITTERSTATUS.FERTILITY;

//namespace UtilLibs.ModVersionCheck
//{
//    public class ModUpdatingState
//    {
//        [JsonIgnore]
//        public static bool UpdatingInstance =false;

//        [JsonIgnore]
//        public const string UpdateDataFile = "ModUpdateData.json";

//        public int globalState = 0;

//        public List<OutdatedModInfo> ModsToUpdate = new List<OutdatedModInfo>();
//        public bool HasModsToUpdate => ModsToUpdate.Count > 0 && ModsToUpdate.Any(i => i.state > 1);
//        public static ModUpdatingState GetModUpdatingState()
//        {
//            try
//            {
//                var filepath = Path.Combine(IO_Utils.ConfigFolder, UpdateDataFile);

//                IO_Utils.ReadFromFile<ModUpdatingState>(filepath, out var item);
//                if (item == null)
//                    item = new ModUpdatingState();
//                return item;
//            }
//            catch (Exception ex)
//            {
//                SgtLogger.l(ex.Message);
//                return new ModUpdatingState();
//            }
//        }
//        public void WriteModUpdatingState()
//        {
//            try
//            {
//                var filepath = Path.Combine(IO_Utils.ConfigFolder, UpdateDataFile);
//                IO_Utils.WriteToFile(this, filepath);
//            }
//            catch (Exception ex)
//            {
//                SgtLogger.l(ex.Message);
//            }
//        }
//        public bool ModInUpdateProcess(string labelStaticId)
//        {
//            var target = ModsToUpdate.FirstOrDefault(m => m.LabelStaticModId == labelStaticId);
//            bool inProgress = target != null && target.state != (int)ManualUpdateState.upToDate && target.state != (int)ManualUpdateState.undefined;
//            SgtLogger.l("mod " + labelStaticId + " in update progress? " + inProgress);
//            return inProgress;
//        }

//        public void AddModToUpdate(Mod mod, string downloadLink)
//        {
//            var info = new OutdatedModInfo()
//            {
//                LabelStaticModId = mod.label.defaultStaticID,
//                downloadFilePath = downloadLink,
//                targetModPath = mod.ContentPath,
//                state = mod.IsEnabledForActiveDlc() ? (int)ManualUpdateState.awaitingDisabling : (int)ManualUpdateState.downloadPending,
//                wasEnabledPreviously = mod.IsEnabledForActiveDlc(),
//            };
//            ModsToUpdate.Add(info);
//        }

//        internal void InitializeUpdateProcess()
//        {
//            globalState = 2;
//            ProgressUpdate();
//        }

//        public async void ProgressUpdate()
//        {
//            switch (globalState)
//            {
//                case 2:
//                    DisableAllPendingMods();
//                    break;
//                case 3:
//                    await DownloadAllPendingMods();
//                    break;
//                case 4:
//                    EnableAllPendingMods();
//                    break;
//            }
//        }
//        private void EnableAllPendingMods()
//        {
//            var manager = Global.Instance.modManager;
//            bool restartAfter = false;
//            foreach (var updateInfo in ModsToUpdate)
//            {
//                if (updateInfo.state == (int)ManualUpdateState.awaitingEnabling)
//                {
//                    restartAfter = true;
//                    var ToDisable = manager.mods.FirstOrDefault(m => m.label.defaultStaticID == updateInfo.LabelStaticModId);
//                    if (ToDisable != null)
//                    {
//                        ToDisable.SetEnabledForActiveDlc(false);
//                        updateInfo.state = (int)ManualUpdateState.downloadPending;
//                    }
//                    else
//                    {
//                        SgtLogger.l("failed to update " + updateInfo.LabelStaticModId);
//                        updateInfo.state = (int)ManualUpdateState.updateFailed;
//                    }
//                }
//            }
//            globalState = (int)ManualUpdateState.upToDate;
//            WriteModUpdatingState();
//            if (restartAfter)
//            {
//                manager.Save();
//                Restart();
//            }
//        }
//        private void DisableAllPendingMods()
//        {
//            var manager = Global.Instance.modManager;
//            bool restartAfter = false;
//            foreach (var updateInfo in ModsToUpdate)
//            {
//                if (updateInfo.state == (int)ManualUpdateState.awaitingDisabling)
//                {
//                    restartAfter = true;
//                    var ToDisable = manager.mods.FirstOrDefault(m => m.label.defaultStaticID == updateInfo.LabelStaticModId);
//                    if (ToDisable != null)
//                    {
//                        ToDisable.SetEnabledForActiveDlc(false);
//                        updateInfo.state = (int)ManualUpdateState.downloadPending;
//                    }
//                    else
//                    {
//                        SgtLogger.l("failed to update " + updateInfo.LabelStaticModId);
//                        updateInfo.state = (int)ManualUpdateState.updateFailed;
//                    }
//                }
//            }
//            globalState = (int)ManualUpdateState.downloadPending;
//            WriteModUpdatingState();
//            if (!restartAfter)
//            {
//                ProgressUpdate();
//            }
//            else
//            {
//                manager.Save();
//                Restart();
//            }

//        }
//        async Task DownloadAllPendingMods()
//        {
//            SgtLogger.l("using webclient");
//            using (WebClient client = new WebClient())
//            {
//                SgtLogger.l("setting paths");
//                var tmpPath = System.IO.Path.GetTempPath();
//                string targetZipPath, targetExtractPath, trueExtractPath;

//                foreach (var updateInfo in ModsToUpdate)
//                {
//                    if (updateInfo.state == (int)ManualUpdateState.downloadPending)
//                    {
//                        if (!updateInfo.VerifyLink())
//                            continue;
//                         var info = new FileInfo(updateInfo.downloadFilePath);
//                        targetZipPath = Path.Combine(tmpPath , info.Name);
//                        targetExtractPath = Path.Combine(tmpPath, Path.GetFileNameWithoutExtension(info.Name));
//                        //try
//                        //{
//                        //    System.IO.Directory.CreateDirectory(ModAssets.ModPath);
//                        //}
//                        //catch (Exception e)
//                        //{
//                        //    SgtLogger.error("Could not create folders, Exception:\n" + e);
//                        //}
//                        SgtLogger.l("downloading");

//                        await client.DownloadFileTaskAsync(updateInfo.downloadFilePath, targetZipPath);
//                        SgtLogger.l("extracting to "+ targetExtractPath);
//                        using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(targetZipPath))
//                        {
//                            zip.ExtractAll(targetExtractPath);
//                        }

//                        if(FindEmbeddedModFiles(targetExtractPath, out trueExtractPath))
//                        {
//                            var files = new DirectoryInfo(trueExtractPath).GetFiles();
//                            foreach (FileInfo file in files)
//                            {
//                                try
//                                {
//                                    file.CopyTo(updateInfo.targetModPath, true);
//                                }
//                                catch (Exception e)
//                                {
//                                    SgtLogger.logError("Error while copying mod file "+file.Name+" to mod folder, Error: " + e);
//                                }
//                            }
//                            updateInfo.state = (int)ManualUpdateState.awaitingEnabling;
//                        }
//                    }
//                }
//            }
//        }

//        private bool FindEmbeddedModFiles(string startPath, out string foundPath)
//        {
//            var extracts = new DirectoryInfo(startPath);
//            if (extracts.GetFiles().Length > 0 || extracts.GetFiles().Any(info => info.Extension == "dll"))
//            {
//                foundPath = startPath;
//                return true;
//            }
//            else if (extracts.GetDirectories().Length > 0)
//            {
//                foreach(var dir in extracts.GetDirectories())
//                {
//                    if(FindEmbeddedModFiles(dir.FullName, out foundPath))
//                        return true;
//                }
//            }
            
//            foundPath = string.Empty;
//            return false;
            
//        }

//        private void Restart() => App.instance.Restart();
//    }

//    public enum ManualUpdateState
//    {
//        updateFailed = -1,
//        undefined = 0,
//        upToDate = 1,
//        awaitingDisabling = 2,
//        downloadPending = 3,
//        awaitingEnabling = 4,
//    }
//    public class OutdatedModInfo
//    {
//        public string LabelStaticModId;
//        public int state;
//        public string downloadFilePath;
//        public string targetModPath;
//        public bool wasEnabledPreviously;

//        public bool VerifyLink()
//        {
//            if(downloadFilePath != null && downloadFilePath.Length > 0)
//            {
//                var extension = new FileInfo(downloadFilePath).Extension;

//                SgtLogger.l(downloadFilePath, "path");
//                SgtLogger.l(extension, "extension");

//                if (extension == ".zip" || extension == ".rar")
//                    return true;
//            }
//            SgtLogger.error("File path not valid for " + LabelStaticModId);
//            this.state = (int)ManualUpdateState.updateFailed;
//            return false;
//        }
//    }
//}
