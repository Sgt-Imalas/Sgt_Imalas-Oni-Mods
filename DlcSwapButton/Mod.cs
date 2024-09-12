using HarmonyLib;
using KMod;
using UtilLibs;

namespace DlcSwapButton
{
	public class Mod : UserMod2
	{
		public static Mod Instance;
		public override void OnLoad(Harmony harmony)
		{
			Instance = this;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}
