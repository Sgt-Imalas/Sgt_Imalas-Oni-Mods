using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using static DistributionPlatform;

namespace _WorldGenStateCapture
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);
			harmonyInstance = harmony;
			Debug.Log($"{mod.staticID} - Mod Version: {mod.packagedModInfo.version} ");
			THIS = this.mod;
		}
		public static Harmony harmonyInstance;
		public static KMod.Mod THIS;
	}
}
