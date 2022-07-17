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
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var stringChars = new char[3];

            string returnString = string.Empty;
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            int number = random.Next(0, 999);
            returnString = new string(stringChars) + "-" + number.ToString("D3");
            return returnString;
        }
        public static class Tags
        {
           public static Tag LS_Satellite = TagManager.Create("LS_Space_Satellite");
        }
    }
}
