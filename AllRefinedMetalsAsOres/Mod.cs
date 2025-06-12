using HarmonyLib;
using KMod;
using System;

namespace AllRefinedMetalsAsOres
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
			Debug.Log($"{mod.staticID} - Mod Version: {mod.packagedModInfo.version} ");
		}     
    }
}
