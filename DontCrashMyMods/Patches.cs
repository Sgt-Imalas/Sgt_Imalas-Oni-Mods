using HarmonyLib;

namespace DontCrashMyMods
{
	internal class Patches
	{
		//Dont crash for no reason
		[HarmonyPatch(typeof(DlcManager))]
		[HarmonyPatch(nameof(DlcManager.IsContentActive))]
		public static class ApplyMultipliersToWeights
		{
			private static bool Prefix(ref bool __result, string dlcId)
			{
				//DebugUtil.LogWarningArgs("A mod is calling IsContentActive which is obsolete and needs to be fixed.");
				if (dlcId == "" || dlcId == "EXPANSION1_ID")
				{
					__result = DlcManager.IsContentSubscribed(dlcId);
				}
				__result = false;
				//DebugUtil.LogErrorArgs("IsContentActive was called with a newer DLC which is not allowed.");
				return false;
			}
		}
	}
}
