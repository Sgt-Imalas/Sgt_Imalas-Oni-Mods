using HarmonyLib;
using KMod;

namespace MultiTimeDisplays
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			Debug.Log($"{mod.staticID} - Mod Version: {mod.packagedModInfo.version} ");
			base.OnLoad(harmony);
		}
	}
}
