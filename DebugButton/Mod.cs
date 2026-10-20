using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UtilLibs;

namespace DebugButton
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			harmony.RegisterForLocalization(typeof(STRINGS), true);
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
		}
	}
}
