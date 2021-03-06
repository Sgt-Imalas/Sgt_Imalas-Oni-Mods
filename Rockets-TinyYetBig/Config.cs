using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig
{
    [Serializable]
    [RestartRequired]
    [ModInfo("Rocketry Tewaks")]
    public class Config : SingletonOptions<Config>
    {

        [Option("Cartographic Module Scan Range", "Cartographic Modules will instantly reveal hexes in this radius.")]
        [Limit(0, 3)]
        [JsonProperty]
        public int ScannerModuleRange { get; set; }

        public Config()
        {
            ScannerModuleRange = 1;
        }
    }
}
