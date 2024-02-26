using Database;
using HarmonyLib;
using Klei;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static InventoryOrganization;

namespace OniRetroEdition.BuildingDefModification
{
    public class AddNewSkins
    {
        // manually patching, because referencing BuildingFacades class will load strings too early
        public static void Patch(Harmony harmony)
        {
            SgtLogger.l("init Patch 2");
            var targetType = AccessTools.TypeByName("Database.BuildingFacades");
            var target = AccessTools.Constructor(targetType, new[] { typeof(ResourceSet) });
            var postfix = AccessTools.Method(typeof(InitFacadePatchForDrywalls), "PostfixPatch");


            harmony.Patch(target, postfix: new HarmonyMethod(postfix));
        }

        public class InitFacadePatchForDrywalls
        {
            public static void PostfixPatch(object __instance)
            {
                SgtLogger.l("Start Skin Patch");
                var resource = (ResourceSet<BuildingFacadeResource>)__instance;
                SkinsAdder.Instance.TargetIDWithAnimnameForSoundCopy.Clear();
                //AddFacade(resource, "RetroReservoirSkin_TEST", "Retro Reservoir", "", PermitRarity.Universal, LiquidReservoirConfig.ID, "old_liquidreservoir_kanim");

                foreach (var entry in SkinsAdder.Instance.newSkins)
                {
                    if(Assets.GetAnim(entry.Anim) == null)
                    {
                        SgtLogger.warning(entry.Anim + " was not a valid anim, skipping");
                        continue;
                           
                    }
                    //if (Assets.GetBuildingDef(entry.BuildingId) == null)
                    //{
                    //    SgtLogger.warning(entry.BuildingId + " was not a valid buildingID, skipping");
                    //    continue;
                    //}
                    SkinsAdder.Instance.AddAnimForSoundCopy(entry.BuildingId,entry.Anim);
                    SgtLogger.l("adding skin: "+entry.SkinName);
                    AddFacade(resource, entry.SkinId, entry.SkinName, entry.SkinDescription, PermitRarity.Universal, entry.BuildingId, entry.Anim);

                }

                SgtLogger.l("Patch Executed");
            }

            public static void AddFacade(
                ResourceSet<BuildingFacadeResource> set,
                string id,
                LocString name,
                LocString description,
                PermitRarity rarity,
                string prefabId,
                string animFile,
                Dictionary<string, string> workables = null)
            {
                set.resources.Add(new BuildingFacadeResource(id, name, description, rarity, prefabId, animFile, DlcManager.AVAILABLE_ALL_VERSIONS, workables));
            }
        }
    }
    public static class SublevelCategoryPatch
    {
        public static void Patch(Harmony harmony)
        {
            SgtLogger.l("init category");
            var targetType = AccessTools.TypeByName("InventoryOrganization");
            var target = AccessTools.Method(targetType, "GenerateSubcategories");
            var postfix = AccessTools.Method(typeof(SublevelCategoryPatch), "PostfixMethod");


            harmony.Patch(target, postfix: new HarmonyMethod(postfix));
        }
        public static void PostfixMethod()
        {
            SupplyClosetUtils.AddSubcategory(InventoryPermitCategories.BUILDINGS, "BUILDING_ONI_RETRO", Assets.GetSprite("conveyor_box_retro"), 132, SkinsAdder.Instance.SkinIDs);
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class Assets_OnPrefabInit_Patch
    {
        public static void Prefix()
        {
            AddNewSkins.Patch(Mod.HarmonyInstance);
            SublevelCategoryPatch.Patch(Mod.HarmonyInstance);
        }
    }


    internal class SkinsAdder
    {

        [JsonIgnore]
        public Dictionary<string, List<string>> TargetIDWithAnimnameForSoundCopy => _targetIDWithAnimnameForSoundCopy;
        [JsonIgnore]
        private Dictionary<string,List<string>> _targetIDWithAnimnameForSoundCopy = new ();

        public void AddAnimForSoundCopy(string buildingID, string animName)
        {
            if(!_targetIDWithAnimnameForSoundCopy.ContainsKey(buildingID))
                _targetIDWithAnimnameForSoundCopy.Add(buildingID, new List<string> {  animName });
            else
                _targetIDWithAnimnameForSoundCopy[buildingID].Add(animName);
        }


        [JsonIgnore]
        public static string BuildingConfigPath;

        [JsonIgnore]
        public static SkinsAdder Instance;

        public List<SkinToAdd> newSkins = new List<SkinToAdd>();
        [JsonIgnore]
        public string[] SkinIDs => newSkins.Select(item => item.SkinId).ToArray();

        public static void InitializeFolderPath()
        {

            SgtLogger.debuglog("Initializing file path..");
            BuildingConfigPath = FileSystem.Normalize(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Skins.json"));

            if (!new FileInfo(BuildingConfigPath).Exists)
            {
                SkinsAdder tmp = new SkinsAdder();
                tmp.newSkins = new List<SkinToAdd>()
                {
                    new SkinToAdd()
                    {
                        BuildingId = "BuildingID",
                        SkinId = "SkinID",
                        SkinName = "SkinName",
                        SkinDescription = "SkinDescription",
                        Anim = "skin_anim_kanim"
                    }

                };
                IO_Utils.WriteToFile<SkinsAdder>(tmp, BuildingConfigPath);
            }

            try
            {
                IO_Utils.ReadFromFile<SkinsAdder>(BuildingConfigPath, out Instance);
            }
            catch (Exception e)
            {
                SgtLogger.error("Could not create folder, Exception:\n" + e);
            }
            SgtLogger.log("Folders succesfully initialized");
        }
    }
    internal class SkinToAdd
    {
        public string BuildingId;
        public string SkinId;
        public string SkinName;
        public string SkinDescription;
        public string Anim;
    }
}
