using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatelites.Behaviours
{
    public static class ModAssets
    {
        public static string GetSatelliteNameRandom()
        {
            return "SateliteNameGeneratorWIP";
        }
        public static class Tags
        {
           public static Tag LS_Satellite = TagManager.Create("LS_Space_Satellite");
        }
    }
}
