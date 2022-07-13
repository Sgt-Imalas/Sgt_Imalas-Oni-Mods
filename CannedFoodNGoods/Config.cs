using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CannedFoods
{
    [Serializable]
    [RestartRequired]
    [ModInfo("Canned Food")]
    public class Config : SingletonOptions<Config>
    {
        [Option("More realistic can material", "changes the material used for the cans from copper to aluminum to reflect the material used in the real world.")]
        [JsonProperty]
        public bool UsesAluminumForCans { get; set; }
        public Config()
        {
            UsesAluminumForCans = false;
        }
    }
}
