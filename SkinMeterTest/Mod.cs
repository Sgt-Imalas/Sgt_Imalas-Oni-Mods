using HarmonyLib;
using KMod;
using UtilLibs;

namespace SkinMeterTest
{
	public class Mod : UserMod2
	{
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			HarmonyInstance = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}
