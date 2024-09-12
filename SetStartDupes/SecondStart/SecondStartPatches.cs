using UnityEngine;

namespace SetStartDupes.SecondStart
{
	internal class SecondStartPatches
	{
		/// <summary>
		/// !!! do not patch directly, it breaks translations....
		/// </summary>

		//[HarmonyPatch(typeof(HeadquartersConfig), nameof(HeadquartersConfig.ConfigureBuildingTemplate))]
		public class HeadquartersConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				//go.AddOrGet<RestartButton>();
			}
		}
	}
}
