using AquaticMinnowMinion.Content.ModDb;
using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static STRINGS.DUPLICANTS;

namespace AquaticMinnowMinion.Patches
{
	/// <summary>
	/// these shenanigans are necessary because the game does not like multiple minion models having the same skills in the same categories, it fucks up the layout
	/// hence why im simulating it with giving aquatic dupes the regular screen, with swimming disabled and adaptation instead
	/// </summary>
	internal class SkillsScreen_Patches
	{
		static Tag ActualMinionTag;


		[HarmonyPatch(typeof(SkillsScreen), nameof(SkillsScreen.RefreshSkillWidgets))]
		public class SkillsScreen_RefreshSkillWidgets_Patch
		{
			public static void Postfix(SkillsScreen __instance)
			{
				var percievedModel = __instance.SelectedMinionModel();
				var swimmingId = Db.Get().SkillGroups.SwimmingSkills.Id;
				var standardModel = GameTags.Minions.Models.Standard;

				if (percievedModel != standardModel)
					return;

				foreach (KeyValuePair<string, GameObject> skillWidget in __instance.skillWidgets)
				{
					Skill skill = Db.Get().Skills.Get(skillWidget.Key);
					if (skill.requiredDuplicantModel != null)
					{
						bool shouldBeActive = skill.requiredDuplicantModel == percievedModel;

						if(skill.skillGroup == Aq_SkillGroups.ADAPTATION_ID)
							shouldBeActive = ActualMinionTag == ModAssets.Tags.AquaticMinion;
						else if(skill.skillGroup == swimmingId)
							shouldBeActive = ActualMinionTag == standardModel;

						skillWidget.Value.SetActive(shouldBeActive);
					}
				}
			}
		}


		[HarmonyPatch(typeof(SkillsScreen), nameof(SkillsScreen.SelectedMinionModel))]
		public class SkillsScreen_SelectedMinionModel_Patch
		{
			public static void Postfix(SkillsScreen __instance, ref Tag __result)
			{
				ActualMinionTag = __result;
				if (__result == ModAssets.Tags.AquaticMinion)
					__result = GameTags.Minions.Models.Standard;
			}
		}
	}
}
