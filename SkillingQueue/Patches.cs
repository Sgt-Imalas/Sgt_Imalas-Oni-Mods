using Database;
using HarmonyLib;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;

namespace SkillingQueue
{
	internal class Patches
	{
		//SimHashes.SkillPointAquired, MinionResume data
		public static Dictionary<MinionResume, SavedSkillQueue> ResumeQueues = new Dictionary<MinionResume, SavedSkillQueue>();


		//in their infinite competency klei completely broke patching minionConfig class directly >:( 

		/// <summary>
		/// Add Skill queue component to dupe prefab
		/// </summary>
		[HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.CreatePrefab))]
		public static class MinionConfig_CreatePrefab_Patch
		{
			
			public static void Postfix(GameObject __result)
			{
				__result.AddOrGet<SavedSkillQueue>();
			}
		}



        //[HarmonyPatch(typeof(MinionResume), nameof(MinionResume.AddExperience))]
        //public static class DEBUG_MULTIPLIER
        //{
        //    public static void Prefix(ref float amount)
        //    {
        //        amount *= 100f;
        //    }
        //}

        /// <summary>
        /// refresh queue on skill learned
        /// </summary>
        [HarmonyPatch(typeof(MinionResume), nameof(MinionResume.MasterSkill))]
		public static class MinionResume_MasterSkill_Patch
		{
			public static void Postfix(MinionResume __instance, string skillId)
			{
				if (ResumeQueues.ContainsKey(__instance))
				{
					ResumeQueues[__instance].LearnedSkill(skillId);
				}
			}
		}

		/// <summary>
		/// Add skill queue position indicator to skill widget header text
		/// </summary>
		[HarmonyPatch(typeof(SkillWidget), nameof(SkillWidget.Refresh))]
		public static class SkillWidget_Refresh_Patch
		{
			public static void Postfix(SkillWidget __instance, string skillID)
			{

				__instance.skillsScreen.GetMinionIdentity(__instance.skillsScreen.CurrentlySelectedMinion, out var minionIdentity, out _);
				if (minionIdentity == null)
					return;

				if (minionIdentity.TryGetComponent<MinionResume>(out var resume) && ResumeQueues.ContainsKey(resume))
				{
					string keyCode = GameUtil.GetKeycodeLocalized(KKeyCode.LeftShift) ??
						"SHIFT";
					string currentTooltip = __instance.tooltip.GetMultiString(0);
					currentTooltip += "\r\n\r\n";
					if (ResumeQueues[resume].HasSkillQueued(skillID, out int index))
					{
						Skill skill = Db.Get().Skills.Get(skillID);

						float min = index;
						float max = Mathf.Max(8, ResumeQueues[resume].QueuedSkillCount);
						float gradientValue = (min / max) * 0.9f;
						var targetColor = Config.Instance.RainbowIndicator ? Color.HSVToRGB(gradientValue, 0.45f, 1f) : UIUtils.rgb(255, 251, 187);

						string positionText = " <b>[" + (++index) + "]</b>";

						positionText = UIUtils.ColorText(positionText, targetColor);

						__instance.Name.text = skill.Name + positionText + "\n(" + Db.Get().SkillGroups.Get(skill.skillGroup).Name + ")";

						currentTooltip += string.Format(STRINGS.SKILLQUEUE.DEQUEUE, index, keyCode);
					}
					else
					{
						currentTooltip += string.Format(STRINGS.SKILLQUEUE.QUEUE, keyCode);
					}
					__instance.tooltip.SetSimpleTooltip(currentTooltip);
				}
			}
		}
		/// <summary>
		/// enqueue / dequeue skill
		/// </summary>
		[HarmonyPatch(typeof(SkillWidget), nameof(SkillWidget.OnPointerClick))]
		public static class SkillWidget_OnPointerClick_Patch
		{
			public static void Postfix(SkillWidget __instance, PointerEventData eventData)
			{
				bool shiftClicked = (Global.GetInputManager().GetDefaultController().mActiveModifiers & Modifier.Shift) != 0;

				__instance.skillsScreen.GetMinionIdentity(__instance.skillsScreen.CurrentlySelectedMinion, out var minionIdentity, out var _);
				if (minionIdentity == null || !minionIdentity.TryGetComponent<MinionResume>(out var resume) || !ResumeQueues.ContainsKey(resume))
					return;

				SavedSkillQueue queue = ResumeQueues[resume];
				if (shiftClicked)
				{
					if (queue.HasSkillQueued(__instance.skillID, out _))
					{
						queue.RemoveSkillAndPostrequisites(__instance.skillID);
						__instance.skillsScreen.RefreshAll();
					}
					else
					{
						if (queue.CanMasterSkillAndPrerequisites(__instance.skillID) && !resume.HasMasteredSkill(__instance.skillID))
						{
							queue.AddSkillWithPrerequisites(__instance.skillID);
							queue.TryLearnSkills();
							__instance.skillsScreen.RefreshAll();
						}
					}
				}

			}
		}

		/// <summary>
		/// Add expected morale and morale expectation for finished queue to morale text
		/// </summary>
		[HarmonyPatch(typeof(SkillsScreen), nameof(SkillsScreen.RefreshProgressBars))]
		public static class SkillsScreen_RefreshProgressBars_Patch
		{
			public static void Postfix(SkillsScreen __instance)
			{
				if (__instance.currentlySelectedMinion == null || __instance.currentlySelectedMinion.IsNull())
				{
					return;
				}
				__instance.GetMinionIdentity(__instance.currentlySelectedMinion, out var minionIdentity, out _);
				if (minionIdentity == null)
					return;


				if (minionIdentity.TryGetComponent<MinionResume>(out var resume) && ResumeQueues.ContainsKey(resume))
				{
					int finalMorale = ResumeQueues[resume].GetFinalMorale();
					int finalMoraleExpectation = ResumeQueues[resume].GetFinalMoraleExpectancy();

					if (finalMorale > 0)
					{
						__instance.moraleProgressLabel.text = __instance.moraleProgressLabel.text + GameUtil.ApplyBoldString(GameUtil.ColourizeString(__instance.moraleNotchColor, " [" + finalMorale + "] "));
					}
					if (finalMoraleExpectation > 0)
					{
						__instance.expectationsProgressLabel.text = __instance.expectationsProgressLabel.text + GameUtil.ApplyBoldString(GameUtil.ColourizeString(__instance.expectationNotchColor, " [" + finalMoraleExpectation + "] "));
					}
				}
			}
		}
		/// <summary>
		/// /// Init. auto translation
		/// /// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}
