using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;
using static STRINGS.BUILDINGS.PREFABS;

namespace BuildingToken
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            
            foreach (string tokenBuilding in ModAssets.TokenBuildings)
            {
                var tag = ModAssets.AddTokenForBuilding(tokenBuilding);
                GameTags.MaterialBuildingElements.Add(tag);
            }

            //GameTags.Other.Add("x");
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
        }
    }
}
