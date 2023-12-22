using PeterHan.PLib.AVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PeterHan.PLib.AVC.JsonURLVersionChecker;

namespace UtilLibs.ModVersionCheck
{
    internal class VersionChecker
    {
        public static void RegisterCurrentVersion(KMod.UserMod2 userMod)
        {
#if DEBUG
            try
            {
                if (!userMod.mod.IsDev)
                {
                    return;
                }
                var filepath = Path.Combine(IO_Utils.ConfigFolder, ImalasVersionData_Dev.Dev_File_Local);

                IO_Utils.ReadFromFile<JsonURLVersionChecker.ModVersions>(filepath, out var item);
                if (item == null)
                    item = new JsonURLVersionChecker.ModVersions();
                var versionData = new ModVersion
                {
                    staticID = userMod.mod.staticID,
                    version = userMod.mod.packagedModInfo.version
                };


                item.mods.Add(versionData);
                IO_Utils.WriteToFile<JsonURLVersionChecker.ModVersions>(item, filepath);
            }
            catch (Exception ex)
            {
                SgtLogger.l(ex.Message);
            }
#endif
        }
        public static void HandleVersionChecking(KMod.UserMod2 userMod)
        {
            RegisterCurrentVersion(userMod);
            //CheckVersion(userMod);
        }
        public static void CheckVersion(KMod.UserMod2 userMod)
        {


        }
    }
}
