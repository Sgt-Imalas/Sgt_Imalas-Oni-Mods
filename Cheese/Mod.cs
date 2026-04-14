using HarmonyLib;
using KMod;
using UtilLibs;

namespace Cheese
{
	public class Mod : UserMod2
	{
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			HarmonyInstance = harmony;
			SgtLogger.LogVersion(this, harmony);

			base.OnLoad(harmony);
			ModAssets.LoadAll();
			ElementUtilNamespace.SgtElementUtil.ExecuteElementEnumPatches(harmony);
		}
	}
}
