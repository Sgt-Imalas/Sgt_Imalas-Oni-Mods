using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SetStartDupes
{
    public class ModApi
    {
        public static Dictionary<string, System.Action<GameObject>> ActionsOnTraitRemoval = new Dictionary<string, Action<GameObject>>();

        /// <summary>
        /// Traits that add more than a component with the same name as the trait id leave these components on the duplicant when initialising a stat selectable cryopod dupe or hermit. 
        /// Please register an action here that takes the duplicants gameobject as param and removes any additional components the trait added to the duplicant
        /// </summary>
        /// <param name="TraitID">The id of the trait</param>
        /// <param name="action">the action that should be called on removing a trait from a duplicant. provides the duplicant gameobject as param</param>
        public static void AddTraitRemovalAction(string TraitID, Action<GameObject> action)
        {
            if (ActionsOnTraitRemoval.ContainsKey(TraitID))
            {
                ActionsOnTraitRemoval[TraitID] += action;
            }
            else
            {
                ActionsOnTraitRemoval[TraitID] = action;
            }
        }



        /// <summary>
        /// By default, the voiceIdx is a random value between 0 and 3 (inclusive).
        /// Jorge has -2 as a special value.
        /// Register your custom dupe here if its supposed to have a specific voice idx
        /// </summary>
        /// <param name="nameStringKey"></param>
        /// <param name="voiceIdxOverride"></param>
        public static void AddingCustomVoiceIdx (string nameStringKey, int voiceIdxOverride)
        {
            if(!VoiceIdxOverrides.ContainsKey(nameStringKey)) 
            {
                VoiceIdxOverrides[nameStringKey] = voiceIdxOverride;
            }
        }
        public static Dictionary<string, int> VoiceIdxOverrides = new Dictionary<string, int>();

        public static int GetVoiceIdxOverrideForPersonality(string nameStringKey)
        {
            if(VoiceIdxOverrides.ContainsKey(nameStringKey))
            {
                SgtLogger.l($"Applying custom voice idx: {VoiceIdxOverrides[nameStringKey]} for personality {nameStringKey}.");
                return VoiceIdxOverrides[nameStringKey];
            }
            return UnityEngine.Random.Range(0, 4); 
        }


        public static void RegisteringJorge()
        {
            string nameStringKey = "Jorge";

            ///Hidden Unlcoakbles
            Func<bool> Unlock = () => HermitTraitCompletedOnce();
            AddHiddenDupeToSkinSelection(nameStringKey, Unlock);

            ///VoiceIdx Override
            AddingCustomVoiceIdx(nameStringKey, -2);
        }

        public static bool HermitTraitCompletedOnce() => (Game.Instance != null && Game.Instance.unlocks != null && Game.Instance.unlocks.IsUnlocked("LonelyMinion_STORY_COMPLETE") && ModConfig.Instance.HermitSkin);

        public static Dictionary<string, System.Func<bool>> HiddenPersonalitiesWithUnlockCondition = new Dictionary<string, Func<bool>>();

        /// <summary>
        /// Use this method to add certain conditions for your custom, hidden duplicants.
        /// if the condition is fulfilled, the duplicant skin becomes available in the skin selector
        /// </summary>
        /// <param name="nameStringKey">nameStringKey of the duplicant</param>
        /// <param name="UnlockCondition">Function that returns bool, checking if the duplicant should appear in the skin selector</param>
        public static void AddHiddenDupeToSkinSelection(string nameStringKey, Func<bool> UnlockCondition)
        {
            if (!HiddenPersonalitiesWithUnlockCondition.ContainsKey(nameStringKey))
            {
                HiddenPersonalitiesWithUnlockCondition[nameStringKey] = UnlockCondition;
                SgtLogger.l($"Added {nameStringKey} to hidden Unlockables");
            }
        }
    }
}
