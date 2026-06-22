using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinnowStoryTrait.Patches
{
	public class Localization_Patches
	{
		[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_MINNOWTRAIT.NAME", global::STRINGS.BUILDINGS.PREFABS.MINNOW_IMPERATIVE_POI_A.NAME);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_MINNOWTRAIT.DESCRIPTION", global::STRINGS.COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.WINCONDITION_AQUATIC_DESCRIPTION);
				Strings.Add("STRINGS.CODEX.STORY_TRAITS.CGM_MINNOWTRAIT.DESCRIPTION_SHORT", global::STRINGS.COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.WINCONDITION_AQUATIC_DESCRIPTION);
			}
		}
	}
}
