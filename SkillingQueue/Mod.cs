using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UtilLibs;

namespace SkillingQueue
{
	public class Mod : UserMod2
	{
		public static Harmony Harmony;
		public override void OnLoad(Harmony harmony)
		{
			Harmony = harmony;
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}
