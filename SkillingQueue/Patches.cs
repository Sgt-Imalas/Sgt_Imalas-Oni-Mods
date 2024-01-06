using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UtilLibs;
using static STRINGS.UI;

namespace SkillingQueue
{
    internal class Patches
    {
        //SimHashes.SkillPointAquired, MinionResume data
        /// <summary>
        /// Init. auto translation
        /// </summary>
        /// 


        public static Dictionary<MinionResume, SavedSkillQueue> ResumeQueues = new Dictionary<MinionResume, SavedSkillQueue>();

        [HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.CreatePrefab))]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix(GameObject __result)
            {
                __result.AddOrGet<SavedSkillQueue>();
            }
        }
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
        [HarmonyPatch(typeof(SkillWidget), nameof(SkillWidget.Refresh))]
        public static class SkillWidget_Refresh_Patch
        {
            public static void Postfix(SkillWidget __instance, string skillID)
            {

                __instance.skillsScreen.GetMinionIdentity(__instance.skillsScreen.CurrentlySelectedMinion, out var minionIdentity, out _);
                if(minionIdentity.TryGetComponent<MinionResume>(out var resume) && ResumeQueues.ContainsKey(resume) && ResumeQueues[resume].HasSkillQueued(skillID,out int index))
                {
                    Skill skill = Db.Get().Skills.Get(skillID);

                    float min = index;
                    float max = Mathf.Max(8,ResumeQueues[resume].QueuedSkillCount);
                    float gradientValue  =(min/max)*0.9f;
                    var targetColor = Color.HSVToRGB(gradientValue, 0.45f, 1f);

                    __instance.Name.text = skill.Name + UIUtils.ColorText(" <b>["+(++index) +"]</b>", targetColor) +"\n(" + Db.Get().SkillGroups.Get(skill.skillGroup).Name + ")" ;
                }
            }
        }
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
                if (minionIdentity.TryGetComponent<MinionResume>(out var resume) && ResumeQueues.ContainsKey(resume))
                {
                    int finalMorale = ResumeQueues[resume].GetFinalMorale();
                    int finalMoraleExpectation = ResumeQueues[resume].GetFinalMoraleExpectancy();

                    if (finalMorale > 0)
                    {
                        __instance.moraleProgressLabel.text = __instance.moraleProgressLabel.text + GameUtil.ApplyBoldString(GameUtil.ColourizeString(__instance.moraleNotchColor," [" + finalMorale + "] "));
                    }
                    if (finalMoraleExpectation > 0)
                    {
                        __instance.expectationsProgressLabel.text = __instance.expectationsProgressLabel.text + GameUtil.ApplyBoldString(GameUtil.ColourizeString(__instance.expectationNotchColor, " [" + finalMoraleExpectation + "] "));
                    }
                }
            }
        }
    }
}
