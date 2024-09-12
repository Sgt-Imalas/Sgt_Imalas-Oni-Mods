using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UtilLibs;

namespace CrittersShedFurOnBrush
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			ModAssets.InitSheddables();
			SgtLogger.LogVersion(this, harmony);
			base.OnLoad(harmony);
		}
	}
}
