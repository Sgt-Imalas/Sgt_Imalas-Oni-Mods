using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UtilLibs;

namespace GoodByeFrostByte
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			SgtLogger.LogVersion(this, harmony);
			base.OnLoad(harmony);
		}
	}
}
