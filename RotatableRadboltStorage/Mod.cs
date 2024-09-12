using HarmonyLib;
using KMod;
using UtilLibs;

namespace RotatableRadboltStorage
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
