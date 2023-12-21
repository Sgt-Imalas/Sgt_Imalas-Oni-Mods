using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.ModVersionCheck
{
    internal class VersionChecker
    {
        public static void RegisterCurrentVersion(KMod.UserMod2 userMod)
        {
            if (!userMod.mod.IsDev)
            {
                return;
            }
            var filepath = Path.Combine(IO_Utils.ConfigFolder, ImalasVersionData_Dev.Dev_File_Local);

            IO_Utils.ReadFromFile<ImalasVersionData_Dev>(filepath, out var item);
            if (item == null)
                item = new ImalasVersionData_Dev();
            var versionData = new ModVersionEntry
            {
                ModID = userMod.mod.staticID,
                MinSupportedGameVersion = userMod.mod.packagedModInfo.minimumSupportedBuild,
                ModVersion = userMod.mod.packagedModInfo.version
            };


            item.ModVersions.Add(versionData);
            IO_Utils.WriteToFile<ImalasVersionData_Dev>(item, filepath);
        }
        public static void HandleVersionChecking(KMod.UserMod2 userMod)
        {
            RegisterCurrentVersion(userMod);
            //TODO
        }
    }
}
