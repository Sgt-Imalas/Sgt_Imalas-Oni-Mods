using HarmonyLib;
using KMod;
using UtilLibs;

namespace DontCrashMyMods
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}
