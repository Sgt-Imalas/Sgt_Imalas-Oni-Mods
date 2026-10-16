using HarmonyLib;
using KMod;

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
