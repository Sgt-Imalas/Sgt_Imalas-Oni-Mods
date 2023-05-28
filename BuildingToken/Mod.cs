using HarmonyLib;
using Klei;
using KMod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TemplateClasses;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;

namespace BuildingToken
{
    public class Mod : UserMod2
    {
        public static readonly string Header = "All the following buildings will get tokens added to their recipes:\n";
        public static readonly string Info = "<add your building IDs here, separated by a comma (,)>";
        public override void OnLoad(Harmony harmony)
        {

            SgtLogger.LogVersion(this);
            ModAssets.RuleFilePath = FileSystem.Normalize(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TokenBuildings.txt"));
            SgtLogger.l(ModAssets.RuleFilePath,"Building Token File Location");

            FileInfo info = new FileInfo(ModAssets.RuleFilePath);
            if (!info.Exists)
            {
                SgtLogger.l("file not found, creating a new one");
                WriteToFile(ModAssets.RuleFilePath, Header + Info);
                SgtLogger.l("File created");

                info = new FileInfo(ModAssets.RuleFilePath);
            }
            string tokenBuildings = ReadFromFile(info);
            tokenBuildings = tokenBuildings
                .Replace(Header,string.Empty)
                .Replace(Info,string.Empty)
                .Replace(" ",string.Empty)
                .Replace("\n", string.Empty);

            ModAssets.TokenBuildings = new List<string>();
            ModAssets.TokenBuildings = tokenBuildings.Split(',').ToList();



            foreach (string tokenBuilding in ModAssets.TokenBuildings)
            {
                var building = Assets.TryGetPrefab(tokenBuilding);
                
                SgtLogger.l("added token entry for a building with the id: \"" + tokenBuilding + "\".");

                var tag = ModAssets.AddTokenForBuilding(tokenBuilding);
                GameTags.MaterialBuildingElements.Add(tag);
            }

            //GameTags.Other.Add("x");
            base.OnLoad(harmony);
        }

        public static string ReadFromFile(FileInfo filePath)
        {
            if (!filePath.Exists)
            {
                SgtLogger.logwarning("Not a valid file");
                return null;
            }
            else
            {
                FileStream filestream = filePath.OpenRead();
                using (var sr = new StreamReader(filestream))
                {
                    string list = sr.ReadToEnd();
                    return list;
                }
            }
        }
        public void WriteToFile(string path, string toWrite)
        {
            try
            {
                //var path = Path.Combine(ModAssets.ScheduleTemplatePath, FileName + ".json");

                var fileInfo = new FileInfo(path);
                FileStream fcreate = fileInfo.Open(FileMode.Create);

                using (var streamWriter = new StreamWriter(fcreate))
                {
                   streamWriter.Write(toWrite);
                }
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not write file, Exception: " + e);
            }
        }
    }
}
