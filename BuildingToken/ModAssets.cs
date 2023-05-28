using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;
using static STRINGS.BUILDINGS.PREFABS;

namespace BuildingToken
{
    internal class ModAssets
    {
        public static string RuleFilePath;
        public static Dictionary<string,Tag> BuildingTokenTags = new Dictionary<string,Tag>();
        public static Dictionary<string,string> BuildingTokens = new Dictionary<string, string>();
        public static Tag AddTokenForBuilding(string id)
        {
            Tag ToAddTag = TagManager.Create("BT_" + id, string.Format(STRINGS.BT_BUILDINGTOKEN.NAME,Strings.Get("STRINGS.BUILDINGS.PREFABS."+ id.ToUpperInvariant() + ".NAME")));
            BuildingTokenTags.Add(id,ToAddTag);
            return ToAddTag;
        }

        public static List<string> TokenBuildings = new List<string>();
    }
}
