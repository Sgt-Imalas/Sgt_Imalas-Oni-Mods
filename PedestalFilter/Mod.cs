using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UtilLibs;

namespace PedestalFilter
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary();
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
	}
}
