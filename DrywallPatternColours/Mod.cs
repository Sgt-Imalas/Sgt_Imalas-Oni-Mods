using HarmonyLib;
using KMod;
using UtilLibs;

namespace DrywallPatternColours
{
	public class Mod : UserMod2
	{
		public static Harmony harmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			harmonyInstance = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}
