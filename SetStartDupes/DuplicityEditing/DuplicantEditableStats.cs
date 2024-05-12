using Database;
using Klei.AI;
using SetStartDupes.DuplicityEditing.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UtilLibs;
using YamlDotNet.Core.Tokens;
using static UnityEngine.GraphicsBuffer;

namespace SetStartDupes.DuplicityEditing
{
    internal class DuplicantEditableStats
    {
        public bool EditsPending;

        float totalExperience;
        Dictionary<string, bool> MasteryBySkillID;
        Dictionary<string, int> AttributeLevels;
        public Dictionary<HashedString,float> AptitudeBySkillGroup;
        public Dictionary<string, float> HealthAmounts;

        public string JoyTraitId, StressTraitId;
        public HashSet<string> Traits;

        internal static DuplicantEditableStats GenerateFromMinion(MinionAssignablesProxy minion)
        {
            var go = minion.GetTargetGameObject();
            var stats = new DuplicantEditableStats();

            if(go.TryGetComponent<StoredMinionIdentity>(out var storedMinionIdentity))
            {
                
            }
            else if(go.TryGetComponent<MinionIdentity>(out var minionIdentity))
            {
                if(minionIdentity.TryGetComponent<AttributeLevels>(out var attributeLevels))
                {
                    var attributes = Db.Get().Attributes;
                    //Attribute Levels
                    stats.AttributeLevels = new();
                    foreach(var attribute in AttributeHelper.GetEditableAttributes())
                    {
                        stats.AttributeLevels[attribute.Id] = attributeLevels.GetLevel(attribute);
                    }
                }
                if(minionIdentity.TryGetComponent<Accessorizer>(out var accessorizer))
                {
                    //Looks
                    //TODO
                }
                if (minionIdentity.TryGetComponent<MinionResume>(out var minionResume))
                {
                    //XP
                    stats.totalExperience = minionResume.TotalExperienceGained;
                    //Skills
                    stats.MasteryBySkillID = new Dictionary<string, bool>(minionResume.MasteryBySkillID);
                    //Interests
                    stats.AptitudeBySkillGroup = new(minionResume.AptitudeBySkillGroup);
                }
                if(minionIdentity.TryGetComponent<Traits>(out var traits))
                {
                    //Traits
                    stats.Traits = new HashSet<string>();
                    foreach (var traitId in traits.TraitIds)
                    {
                        if (DUPLICANTSTATS.JOYTRAITS.Any( trait => trait.id == traitId))
                        {
                            stats.JoyTraitId = traitId;
                        }
                        else if (DUPLICANTSTATS.STRESSTRAITS.Any(trait => trait.id == traitId))
                        {
                            stats.StressTraitId = traitId;
                        }
                        else
                            stats.Traits.Add(traitId);
                    }
                }
                //Health Amounts
                stats.HealthAmounts = new ();
                foreach (var amount in AmountHelper.GetEditableAmounts())
                {
                    var instance = amount.Lookup(go);
                    if(instance == null)
                    {
                        SgtLogger.error("amount instance " + amount.Name + " not found for dupe " + go.GetProperName());
                        continue;
                    }
                    stats.HealthAmounts.Add(amount.Id, instance.value);
                }
            }

            return stats;
        }

        public bool HasJoyTrait => JoyTraitId != null;
        public bool HasStressTrait => StressTraitId != null;

        public void RemoveAptitude(HashedString id)
        {
            if (AptitudeBySkillGroup.ContainsKey(id))
            {
                AptitudeBySkillGroup.Remove(id);
                EditsPending = true;
            }
        }
        public void AddAptitude(HashedString id)
        {
            if (!AptitudeBySkillGroup.ContainsKey(id))
            {
                AptitudeBySkillGroup[id] = DUPLICANTSTATS.APTITUDE_BONUS;
                EditsPending = true;
            }
        }


        public void SetExperience(float xp)
        {
            totalExperience = xp;
            EditsPending = true;
        }
        public float GetExperience()
        {
            return totalExperience;
        }

        public void SetAttributeLevel(Klei.AI.Attribute attribute,int level)
        {
            if(!AttributeLevels.ContainsKey(attribute.Id))
            {
                SgtLogger.error(attribute.Name + " was not a viable attribute");
                return;
            }
            AttributeLevels[attribute.Id] = level;
            EditsPending = true;
        }

        public int GetAttributeLevel(Klei.AI.Attribute attribute)
        {
            if (!AttributeLevels.ContainsKey(attribute.Id))
            {
                SgtLogger.error(attribute.Name + " was not a viable attribute");
                return -1;
            }
            return AttributeLevels[attribute.Id];
        }

        public void RemoveTrait(string id)
        {
            if (StressTraitId == id)
                StressTraitId = null;
            else if(JoyTraitId == id)
                JoyTraitId = null;
            else
                Traits.Remove(id);

            EditsPending = true;
        }
        public void AddTrait(string id)
        {
            if (DUPLICANTSTATS.JOYTRAITS.Any(trait => trait.id == id))
            {
                JoyTraitId = id;
            }
            else if (DUPLICANTSTATS.STRESSTRAITS.Any(trait => trait.id == id))
            {
                StressTraitId = id;
            }
            else if(!Traits.Contains(id))
                Traits.Add(id);

            EditsPending = true;
        }
        internal bool HasMasteredSkill(Skill skill)
        {
            if (!MasteryBySkillID.ContainsKey(skill.Id))
            {
                //SgtLogger.warning(skill.Name + ":"+skill.Id+ " was not found in the saved list","has mastered");
                return false;
            }
            return MasteryBySkillID[skill.Id];
        }
        internal void SetSkillLearned(Skill target,bool skillLearned)
        {
            //if (!MasteryBySkillID.ContainsKey(target.Id))
                //SgtLogger.warning(target.Name + ":" + target.Id + " was not found in the saved list","set mastered");

            if (MasteryBySkillID.ContainsKey(target.Id) && MasteryBySkillID[target.Id] == skillLearned)
                return;

            MasteryBySkillID[target.Id] = skillLearned;
            EditsPending = true;
        }

        internal void Apply(MinionAssignablesProxy minion)
        {
            var go = minion.GetTargetGameObject();
            EditsPending = false;
            if (go == null)
                return;

            if (go.TryGetComponent<AttributeLevels>(out var attributeLevels))
            {
                //Attribute Levels

                foreach (var attribute in AttributeHelper.GetEditableAttributes())
                {
                    if (!AttributeLevels.ContainsKey(attribute.Id))
                        continue;

                    int level = attributeLevels.GetLevel(attribute);
                    if(level!= AttributeLevels[attribute.Id])
                    {
                        attributeLevels.SetLevel(attribute.Id, AttributeLevels[attribute.Id]);
                        attributeLevels.SetExperience(attribute.Id, 0);
                    }
                }
            }
            //Traits
            if (go.TryGetComponent<Traits>(out var traits))
            {
                var targetTraits = new List<string>(Traits);
                if (JoyTraitId != null)
                    targetTraits.Add(JoyTraitId);
                if (StressTraitId != null)
                    targetTraits.Add(StressTraitId);

                List<string> ToAdd = targetTraits.Except(traits.TraitIds).ToList(), 
                            ToRemove = traits.TraitIds.Except(targetTraits).ToList();

                var traitDb = Db.Get().traits;
                foreach(var toAddTrait in ToAdd)
                {
                    var trait = traitDb.Get(toAddTrait);

                    if(trait == null)
                    {
                        SgtLogger.warning("trait to add not existing: "+ toAddTrait);
                        continue;
                    }

                    traits.Add(trait);
                }
                foreach(var toRemoveTrait in ToRemove)
                {
                    var trait = traitDb.Get(toRemoveTrait); 
                    if(trait == null)
                    {
                        SgtLogger.warning("trait to remove not existing: " + toRemoveTrait);
                        continue;
                    }
                    traits.Remove(trait);
                    ModAssets.PurgingTraitComponentIfExists(toRemoveTrait, go);
                }
            }
            //Health Amounts
            foreach (var amount in AmountHelper.GetEditableAmounts())
            {
                var instance = amount.Lookup(go);
                if (instance == null || !HealthAmounts.ContainsKey(amount.Id))
                {
                    SgtLogger.error("amount instance " + amount.Name + " not found for dupe " + go.GetProperName());
                    continue;
                }
                instance.SetValue(HealthAmounts[amount.Id]); 
            }
            if (go.TryGetComponent<MinionResume>(out var minionResume))
            {
                //XP
                minionResume.totalExperienceGained = totalExperience;
                //Skills
                foreach(var skill in MasteryBySkillID)
                {
                    bool hasSkill = minionResume.HasMasteredSkill(skill.Key);
                    bool shouldHaveSkill = skill.Value;
                    if (hasSkill!=shouldHaveSkill)
                    {
                        if (shouldHaveSkill)
                        {
                            SgtLogger.l(skill.Key, "mastering");
                            minionResume.MasterSkill(skill.Key);
                        }
                        else
                        {
                            SgtLogger.l(skill.Key, "unmastering");
                            minionResume.UnmasterSkill(skill.Key);
                        }
                    }
                }
                //Interests
                minionResume.AptitudeBySkillGroup = new (AptitudeBySkillGroup);
            }
        }
        
        internal void SetAmount(Amount target, float newAmount)
        {
            HealthAmounts[target.Id] =newAmount;
            EditsPending = true;
        }
    }
}
