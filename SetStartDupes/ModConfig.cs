using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.CODEX.MYLOG.BODY;

namespace SetStartDupes
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("Duplicant Stat Selector")]
    class ModConfig : SingletonOptions<ModConfig>
    {


        [Option("STRINGS.UI.DSS_OPTIONS.DUPLICANTSTARTAMOUNT.NAME", "STRINGS.UI.DSS_OPTIONS.DUPLICANTSTARTAMOUNT.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.GAMESTART")]
        [Limit(1, 100)]
        [JsonProperty]
        public int DuplicantStartAmount { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.STARTUPRESOURCES.NAME", "STRINGS.UI.DSS_OPTIONS.STARTUPRESOURCES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.GAMESTART")]
        [JsonProperty]
        public bool StartupResources { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.SUPPORTEDDAYS.NAME", "STRINGS.UI.DSS_OPTIONS.SUPPORTEDDAYS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.GAMESTART")]
        [JsonProperty]
        [Limit(0, 10)]
        public int SupportedDays { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME.NAME", "STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.PRINTINGPOD")]
        [JsonProperty]
        public bool ModifyDuringGame { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME.NAME", "STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.PRINTINGPOD")]
        [JsonProperty]
        public bool RerollDuringGame { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIME.NAME", "STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.PRINTINGPOD")]
        [JsonProperty]
        [Limit(0.1, 10)]
        public float PrintingPodRechargeTime { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.PRINTINGPOD")]
        [JsonProperty]
        public bool CarePackagesOnly { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYDUPECAP.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYDUPECAP.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.PRINTINGPOD")]
        [JsonProperty]
        [Limit(1, 200)]
        public int CarePackagesOnlyDupeCap { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYPACKAGECAP.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYPACKAGECAP.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.PRINTINGPOD")]
        [JsonProperty]
        [Limit(1, 5)]
        public int CarePackagesOnlyPackageCap { get; set; }

        [Option("STRINGS.UI.DSS_OPTIONS.SKINSDOREACTS.NAME", "STRINGS.UI.DSS_OPTIONS.SKINSDOREACTS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.EXTRAS")]
        [JsonProperty]
        public bool SkinsDoReactions { get; set; }

        public ModConfig()
        {
            DuplicantStartAmount = 3;
            PrintingPodRechargeTime = 3;
            ModifyDuringGame = false;
            RerollDuringGame = false;
            StartupResources = false;
            SupportedDays = 5;

            CarePackagesOnly = false;
            CarePackagesOnlyDupeCap = 16;
            CarePackagesOnlyPackageCap = 3;

            SkinsDoReactions = false;

        }
    }
}
