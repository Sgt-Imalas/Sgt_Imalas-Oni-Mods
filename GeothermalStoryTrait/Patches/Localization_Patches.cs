using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeothermalStoryTrait.Patches
{
	public class Localization_Patches
	{
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_GEOTHERMAL_PUMP.NAME", global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALCONTROLLER.NAME);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_GEOTHERMAL_PUMP.DESCRIPTION", global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALCONTROLLER.EFFECT);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_GEOTHERMAL_PUMP.DESCRIPTION_SHORT", global::STRINGS.BUILDINGS.PREFABS.GEOTHERMALCONTROLLER.DESC);
			}
		}
	}
}
