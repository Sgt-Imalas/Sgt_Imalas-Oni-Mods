using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.ModVersionCheck
{
    internal class ImalasVersionData_Dev
    {
        [JsonIgnore]
        public const string Dev_File_Local = "ModVersionData.json";

        public List<ModVersionEntry> ModVersions = new List<ModVersionEntry>();
    }
}
