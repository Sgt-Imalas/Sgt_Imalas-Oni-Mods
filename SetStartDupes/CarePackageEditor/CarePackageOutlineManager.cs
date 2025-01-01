using Database;
using Klei.CustomSettings;
using SetStartDupes.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS;

namespace SetStartDupes.CarePackageEditor
{
    public class CarePackageOutlineManager
    {
        public static Dictionary<string, List<CarePackageOutline>> VanillaCarePackagesByDlc = null;
        public static List<CarePackageOutline> VanillaCarePackages = null;

        public static List<CarePackageOutline> ExtraCarePackages = null;

        public static void FetchVanillaCarePackages()
        {
            ImmigrationPatch.GeneratingFrontendList = true;

            var managerCarrier = Util.NewGameObject(null, "ImmigrationCarrier");
            var immigrationInstance = managerCarrier.AddComponent<Immigration>();
            VanillaCarePackagesByDlc = new();
            SgtLogger.l("fetching vanilla care packages");
            immigrationInstance.ConfigureBaseGameCarePackages();

            VanillaCarePackagesByDlc[DlcManager.VANILLA_ID] = immigrationInstance.carePackages.Select(package => new CarePackageOutline(package)).ToList();
            SgtLogger.l(VanillaCarePackagesByDlc[DlcManager.VANILLA_ID].Count() + " care packages in vanilla");
            immigrationInstance.ConfigureMultiWorldCarePackages();
            VanillaCarePackagesByDlc[DlcManager.EXPANSION1_ID] = immigrationInstance.carePackages.Select(package => new CarePackageOutline(package)).ToList();
            SgtLogger.l(VanillaCarePackagesByDlc[DlcManager.EXPANSION1_ID].Count() + " care packages in SO");

            immigrationInstance.SetupDLCCarePackages();

            foreach (var dlcPackages in immigrationInstance.carePackagesByDlc)
            {
                if (DlcManager.IsContentSubscribed(dlcPackages.Key))
                {
                    VanillaCarePackagesByDlc[dlcPackages.Key] = dlcPackages.Value.Select(package => new CarePackageOutline(package)).ToList();
                    SgtLogger.l(VanillaCarePackagesByDlc[dlcPackages.Key].Count() + " care packages in " + dlcPackages.Key);
                }
            }

            Immigration.DestroyInstance();
            UnityEngine.Object.Destroy(managerCarrier);

            VanillaCarePackages = new();

            foreach (var entry in VanillaCarePackagesByDlc.Values)
            {
                VanillaCarePackages.AddRange(entry);
            }
            ImmigrationPatch.GeneratingFrontendList = false;
        }

        public static void LoadVanillaCarePackages()
        {
            if (VanillaCarePackagesByDlc == null || VanillaCarePackages == null)
            {
                VanillaCarePackagesByDlc = new();
                FetchVanillaCarePackages();
            }
        }
        public static List<CarePackageInfo> GetAllAdditionalCarePackages()
        {
            ResetExtraCarePackages();
            var result = new List<CarePackageInfo>();
            foreach(var entry in ExtraCarePackages)
            {
                if (entry.TryMakeCarePackageInfo(out var info)) 
                {
                    SgtLogger.l($"adding extra carepackage: {entry.Amount} x {info.id}");
                    result.Add(info);
                }

            }
            return result;
        }
        public static void ResetExtraCarePackages()
        {
            ExtraCarePackages = new();
            ExtraCarePackages.Clear();
            AddExtraCarePackages();
        }
        public static void AddExtraCarePackages()
        {
            ///Base Game:


            ///missing seeds:
            //Sporechid
            ExtraCarePackages.Add(new CarePackageOutline(EvilFlowerConfig.SEED_ID, 1).CycleCondition(95).DiscoverCondition());
            //Buddy Bud
            ExtraCarePackages.Add(new CarePackageOutline(BulbPlantConfig.SEED_ID, 1).CycleCondition(36).DiscoverCondition());
            //Nosh Bean
            ExtraCarePackages.Add(new CarePackageOutline(BeanPlantConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition());
            //Sleet Wheat
            ExtraCarePackages.Add(new CarePackageOutline(ColdWheatConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition());
            //Waterweed
            ExtraCarePackages.Add(new CarePackageOutline(SeaLettuceConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition());
            //Dasha Saltvine
            ExtraCarePackages.Add(new CarePackageOutline(SaltPlantConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition());
            //Mealwood
            ExtraCarePackages.Add(new CarePackageOutline(BasicSingleHarvestPlantConfig.SEED_ID, 3).CycleCondition(24).DiscoverCondition());

            ///missing minerals:
            ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Granite, 1000).CycleCondition(24).DiscoverCondition());
            ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Obsidian, 1000).CycleCondition(24).DiscoverCondition());
            //Abyssalite
            ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Katairite, 1000).CycleCondition(48).DiscoverCondition());
            ///missing ores+metals
            ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.IronOre, 2000).CycleCondition(12).DiscoverCondition());
            ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Wolframite, 1000).CycleCondition(48).DiscoverCondition());
            ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Tungsten, 200).CycleCondition(48).DiscoverCondition());


            ///Spaced Out:
            
            ///missing ores
            ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.UraniumOre, 100).CycleCondition(48).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));
            //Bog Bucket
            ExtraCarePackages.Add(new CarePackageOutline(SwampHarvestPlantConfig.SEED_ID, 3).CycleCondition(24).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));
			//Tranquil Toes
			ExtraCarePackages.Add(new CarePackageOutline(ToePlantConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));
            //Saturn Critter Trap
            ExtraCarePackages.Add(new CarePackageOutline("CritterTrapPlantSeed", 1).CycleCondition(96).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));


            ///missing critters
            //Beetiny
            ExtraCarePackages.Add(new CarePackageOutline(BabyBeeConfig.ID, 1).DlcRequired(DlcManager.EXPANSION1_ID) //discover either on uranium ore or beetiny finding, but only after cycle 24
                .CycleCondition(24).DiscoverCondition()
                .OR()
                .CycleCondition(24).DiscoverElementCondition(SimHashes.UraniumOre));

            ///FP:

            //Carved Lumen Quartz
            ExtraCarePackages.Add(new CarePackageOutline(PinkRockCarvedConfig.ID, 1).CycleCondition(48).DiscoverCondition().DlcRequired(DlcManager.DLC2_ID));

        }
    }
}
