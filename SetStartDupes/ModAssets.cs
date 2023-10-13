using Database;
using Epic.OnlineServices;
using Klei.AI;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static KSerialization.DebugLog;
using static SetStartDupes.DupeTraitManager;
using static STRINGS.DUPLICANTS;
using static STRINGS.UI.DETAILTABS;

namespace SetStartDupes
{
    public class ModAssets
    {
        public static string DupeTemplatePath;
        public static string DupeGroupTemplatePath;
        public static string DupeTemplateName = "UnnamedDuplicantPreset";
        public static bool EditingSingleDupe = false;
        public static bool EditingJorge = false;

        public static GameObject StartPrefab;

        public static CharacterContainer SingleCharacterContainer;
        public static GameObject CryoDupeToApplyStatsOn = null;


        public static GameObject NextButtonPrefab;

        public static GameObject ListEntryButtonPrefab;


        public static GameObject AddNewToTraitsButtonPrefab;
        public static GameObject RemoveFromTraitsButtonPrefab;
        public static List<ITelepadDeliverableContainer> ContainerReplacement;



        public static GameObject PresetWindowPrefab;
        public static GameObject TraitsWindowPrefab;



        public static GameObject ParentScreen
        {
            get
            {
                return parentScreen;
            }
            set
            {

                if (UnityPresetScreen.Instance != null)
                {
                    //UnityPresetScreen.Instance.transform.SetParent(parentScreen.transform, false);
                    UnityEngine.Object.Destroy(UnityPresetScreen.Instance);
                    UnityPresetScreen.Instance = null;
                }
                if (UnityTraitScreen.Instance != null)
                {
                    // UnityTraitScreen.Instance.transform.SetParent(parentScreen.transform, false);
                    UnityEngine.Object.Destroy(UnityTraitScreen.Instance);
                    UnityTraitScreen.Instance = null;
                }
                parentScreen = value;
            }
        }
        private static GameObject parentScreen = null;


        public static void LoadAssets()
        {
            AssetBundle bundle = AssetUtils.LoadAssetBundle("dcs_presetwindow", platformSpecific: true);
            PresetWindowPrefab = bundle.LoadAsset<GameObject>("Assets/PresetWindow_Prefab.prefab");
            TraitsWindowPrefab = bundle.LoadAsset<GameObject>("Assets/DupeSkillsPopUp.prefab");

            //UIUtils.ListAllChildren(PresetWindowPrefab.transform);

            var TMPConverter = new TMPConverter();
            TMPConverter.ReplaceAllText(PresetWindowPrefab);
            TMPConverter.ReplaceAllText(TraitsWindowPrefab);

        }

        ///Assuming the component added by the Trait has the same class name as the trait, which is the case for all klei traits.
        public static void PurgingTraitComponentIfExists(string id, GameObject minionToRemoveFrom)
        {
            var traitCmp = minionToRemoveFrom.GetComponent(id);
            if (traitCmp != null)
            {
                SgtLogger.l("Trait Component Found, purging... " + id);
                UnityEngine.Object.Destroy(traitCmp);
            }
            if (ModApi.ActionsOnTraitRemoval.ContainsKey(id))
            {
                ModApi.ActionsOnTraitRemoval[id].Invoke(minionToRemoveFrom);
            }
        }


        public static Dictionary<MinionStartingStats, DupeTraitManager> DupeTraitManagers = new Dictionary<MinionStartingStats, DupeTraitManager>();

        public static void ApplySkinToExistingDuplicant(Personality Skin, GameObject Duplicant)
        {
            if (Duplicant.TryGetComponent<MinionIdentity>(out var IdentityHolder))
            {
                IdentityHolder.SetName(Skin.Name);
                IdentityHolder.nameStringKey = Skin.nameStringKey;
                IdentityHolder.genderStringKey = Skin.genderStringKey;
                IdentityHolder.personalityResourceId = Skin.IdHash;
                IdentityHolder.voiceIdx = ModApi.GetVoiceIdxOverrideForPersonality(Skin.nameStringKey);
                IdentityHolder.SetStickerType(Skin.stickerType);
                IdentityHolder.SetGender(Skin.genderStringKey);
            }

            //Changing Joy/Stress Traits if applicable
            if (ModConfig.Instance.SkinsDoReactions && Duplicant.TryGetComponent(out Traits traits))
            {
                List<Trait> traitsToRemove = new List<Trait>();

                foreach (Trait trait in traits.TraitList)
                {
                    var traitType = GetTraitListOfTrait(trait.Id, out _);
                    if (traitType == NextType.joy || traitType == NextType.stress)
                    {
                        traitsToRemove.Add(trait);
                    }
                }
                foreach (Trait trait in traitsToRemove)
                {
                    PurgingTraitComponentIfExists(trait.Id, Duplicant);
                    traits.Remove(trait);
                }
                if (!ModConfig.Instance.NoJoyReactions)
                {
                    var newJoyTrait = Db.Get().traits.TryGet(Skin.stresstrait);
                    if (newJoyTrait != null)
                        traits.Add(newJoyTrait);
                }
                if (!ModConfig.Instance.NoStressReactions)
                {
                    var newStressTrait = Db.Get().traits.TryGet(Skin.joyTrait);
                    if (newStressTrait != null)
                        traits.Add(newStressTrait);
                }
            }

            if (Duplicant.TryGetComponent<Accessorizer>(out var accessorizer))
            {

                accessorizer.ApplyMinionPersonality(Skin);
                accessorizer.UpdateHairBasedOnHat();

                ///These symbols get overidden at dupe creation, as we are editing already spawned dupes, we have to remove the old overrides and add the new overrides
                if (Duplicant.TryGetComponent<SymbolOverrideController>(out var symbolOverride))
                {
                    var headshape_symbolName = (KAnimHashedString)HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.HeadShape).symbol.hash).Replace("headshape", "cheek");
                    var cheek_symbol_snapTo = (HashedString)"snapto_cheek";
                    var hair_symbol_snapTo = (HashedString)"snapto_hair_always";

                    symbolOverride.RemoveSymbolOverride(headshape_symbolName);
                    symbolOverride.RemoveSymbolOverride(cheek_symbol_snapTo);
                    symbolOverride.RemoveSymbolOverride(hair_symbol_snapTo);

                    symbolOverride.AddSymbolOverride(cheek_symbol_snapTo, Assets.GetAnim((HashedString)"head_swap_kanim").GetData().build.GetSymbol((KAnimHashedString)headshape_symbolName), 1);
                    symbolOverride.AddSymbolOverride(hair_symbol_snapTo, accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
                    symbolOverride.AddSymbolOverride((HashedString)Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
                }
            }
            ApplyOutfit(Skin, Duplicant);
            ApplyJoyResponseOutfit(Skin, Duplicant);
        }
        public static void ApplyOutfit(Personality personality, GameObject Duplicant)
        {
            if (Duplicant.TryGetComponent<WearableAccessorizer>(out var component))
            {
                foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> outfitId in personality.outfitIds)
                {
                    Option<ClothingOutfitTarget> outfit = ClothingOutfitTarget.TryFromTemplateId(outfitId.Value);
                    if (outfit.HasValue)
                    {
                        component.AddCustomOutfit(outfit);
                    }
                }

                if (personality.outfitIds.ContainsKey(ClothingOutfitUtility.OutfitType.Clothing))
                {
                    Option<ClothingOutfitTarget> option = ClothingOutfitTarget.TryFromTemplateId(personality.outfitIds[ClothingOutfitUtility.OutfitType.Clothing]);
                    if (option.HasValue)
                    {
                        component.ApplyClothingItems(option.Value.OutfitType, option.Value.ReadItemValues());
                    }
                }

            }
        }
        public static void ApplyJoyResponseOutfit(Personality personality, GameObject go)
        {
            JoyResponseOutfitTarget joyResponseOutfitTarget = JoyResponseOutfitTarget.FromPersonality(personality);
            JoyResponseOutfitTarget.FromMinion(go).WriteFacadeId(joyResponseOutfitTarget.ReadFacadeId());
        }

        public static void ApplySkinFromPersonality(Personality personality, MinionStartingStats stats)
        {
            if (ModConfig.Instance.SkinsDoReactions)
            {
                if (!ModConfig.Instance.NoJoyReactions)
                {
                    stats.stressTrait = Db.Get().traits.TryGet(personality.stresstrait);
                }
                if (!ModConfig.Instance.NoStressReactions)
                {
                    stats.joyTrait = Db.Get().traits.TryGet(personality.joyTrait);
                }
            }

            //stats.congenitaltrait = Db.Get().traits.TryGet(personality.congenitaltrait);
            stats.personality = personality;
            stats.stickerType = personality.stickerType;
            stats.GenderStringKey = personality.genderStringKey;
            stats.NameStringKey = personality.nameStringKey;
            stats.voiceIdx = ModApi.GetVoiceIdxOverrideForPersonality(personality.nameStringKey);
        }

        public static int MinimumPointsPerInterest(MinionStartingStats stats, SkillGroup checkForMultiplesOf = null)
        {
            int SkillAmount = 0;


            Dictionary<string, int> skillGroupCount = new Dictionary<string, int>();


            foreach (var aptitude in stats.skillAptitudes)
            {

                if (aptitude.Value > 0)
                {
                    SkillAmount++;
                    foreach (var atb in aptitude.Key.relevantAttributes)
                    {
                        if (skillGroupCount.ContainsKey(atb.Id))
                        {
                            skillGroupCount[atb.Id] = skillGroupCount[atb.Id]+1;
                        }
                        else
                        {
                            skillGroupCount[atb.Id] = 1;
                        }
                    }

                }
            }


            if (checkForMultiplesOf != null && stats.skillAptitudes.ContainsKey(checkForMultiplesOf))
            {
                foreach (var attribute in checkForMultiplesOf.relevantAttributes)
                {
                    if (skillGroupCount.ContainsKey(attribute.Id) && skillGroupCount[attribute.Id] > 1 )
                    {
                        return PointsPerInterests(SkillAmount)* skillGroupCount[attribute.Id];
                    }
                }
            }
            SkillAmount = Math.Max(SkillAmount, 0);
            return PointsPerInterests(SkillAmount);
        }

        /// <summary>
        /// Grab from game files to include compatibility with always3Interests
        /// </summary>
        /// <param name="numberOfInterests"></param>
        /// <returns></returns>
        public static int PointsPerInterests(int numberOfInterests)
        {
            int interestIndex = numberOfInterests - 1;
            if (interestIndex < 0)
                return 0;

            int Maximum = DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES.Length - 1;

            if (interestIndex > Maximum)
                interestIndex = Maximum;
            return DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[interestIndex];
        }

        public static Dictionary<MinionStartingStats, int> OtherModBonusPoints = new Dictionary<MinionStartingStats, int>();

        public static int GetTraitBonus(MinionStartingStats stats)
        {
            int targetPoints = 0;
            var allTraitStats = TryGetTraitsOfCategory(NextType.allTraits);
            foreach (var activeTrait in stats.Traits)
            {
                var active = allTraitStats.Find(match => match.id == activeTrait.Id);
                if (active.statBonus != 0)
                {
                    //SgtLogger.l(active.statBonus.ToString(), active.id);
                    targetPoints += active.statBonus;
                }
            }
            if (OtherModBonusPoints.ContainsKey(stats))
            {
                SgtLogger.l("Had additional " + OtherModBonusPoints[stats] + " points from other mods");
                targetPoints += OtherModBonusPoints[stats];
            }

            return targetPoints;
        }

        public static void RedoStatpointBonus(MinionStartingStats stats, Trait trait, bool isAdding = false)
        {
            ModAssets.GetTraitListOfTrait(trait.Id, out var list);

            if (list == null)
                return;

            var traitBonusHolder = list.Find(traitTo => traitTo.id == trait.Id);

            if (traitBonusHolder.statBonus == 0)
                return;

            if (DupeTraitManagers.ContainsKey(stats))
            {
                if (DupeTraitManagers[stats].CalculateAdditionalSkillPointsTrueIfChanged())
                {
                    DupeTraitManagers[stats].RecalculateSkillPoints();
                    DupeTraitManagers[stats].ResetPool();
                }
            }
        }

        public static string GetTraitTooltip(Trait trait)
        {
            if (trait == null)
                return string.Empty;
            string tooltip = trait.GetTooltip();


            ModAssets.GetTraitListOfTrait(trait.Id, out var list);
            if (list == null)
                return tooltip;

            var traitBonusHolder = list.Find(traitTo => traitTo.id == trait.Id);

            if (traitBonusHolder.statBonus != 0)
            {
                tooltip += "\n\n" + GetTraitStatBonusTooltip(trait);
            }

            return tooltip;
        }
        public static string GetTraitStatBonusTooltip(Trait trait, bool withDescriptor = true)
        {
            string tooltip = string.Empty;
            ModAssets.GetTraitListOfTrait(trait.Id, out var list);

            if (list == null)
                return tooltip;

            var traitBonusHolder = list.Find(traitTo => traitTo.id == trait.Id);
            if (traitBonusHolder.statBonus == 0)
                return tooltip;

            if (withDescriptor)
                tooltip += STRINGS.UI.DUPESETTINGSSCREEN.TRAITBONUSPOINTS + " ";
            tooltip += UIUtils.ColorNumber(traitBonusHolder.statBonus);
            return tooltip;
        }


        public static void RemoveTrait(MinionStartingStats stats, Trait trait)
        {
            if (stats.Traits.Contains(trait))
            {
                stats.Traits.Remove(trait);
                RedoStatpointBonus(stats, trait, false);
            }

        }
        public static void AddTrait(MinionStartingStats stats, Trait trait)
        {
            stats.Traits.Add(trait);
            RedoStatpointBonus(stats, trait, true);
        }



        public static class Colors
        {
            public static Color gold = UIUtils.Darken(Util.ColorFromHex("ffdb6e"), 40);
            public static Color purple = Util.ColorFromHex("a961f9");
            public static Color magenta = Util.ColorFromHex("fd43ff");
            public static Color green = Util.ColorFromHex("367d48");
            public static Color red = Util.ColorFromHex("802024");
            public static Color grey = Util.ColorFromHex("404040");



            ///Color.Lerp(originalColor, Color.black, .5f); To darken by 50%
            ///Color.Lerp(originalColor, Color.white, .5f); To lighten by 50% 
        }

        private static Dictionary<NextType, List<DUPLICANTSTATS.TraitVal>> TraitsByType = new Dictionary<NextType, List<DUPLICANTSTATS.TraitVal>>()
        {
            {
                NextType.geneShufflerTrait,
                DUPLICANTSTATS.GENESHUFFLERTRAITS
            },
            {
                NextType.posTrait,
                DUPLICANTSTATS.GOODTRAITS
            },
            {
                NextType.negTrait,
                DUPLICANTSTATS.BADTRAITS
            },
            {
                NextType.needTrait,
                DUPLICANTSTATS.NEEDTRAITS
            },
            {
                NextType.joy,
                DUPLICANTSTATS.JOYTRAITS
            },
            {
                NextType.stress,
                DUPLICANTSTATS.STRESSTRAITS
            },
            {
                NextType.special,
                DUPLICANTSTATS.SPECIALTRAITS
            },
            {
                NextType.cogenital,
                DUPLICANTSTATS.CONGENITALTRAITS
            }
        };

        public static List<DUPLICANTSTATS.TraitVal> TryGetTraitsOfCategory(NextType type, List<Trait> traitsForCost = null)
        {
            if (type != NextType.allTraits)
            {
                if (!TraitsByType.ContainsKey(type))
                    return new List<DUPLICANTSTATS.TraitVal>();
                else
                    return TraitsByType[type];

            }
            else
            {
                //if (traitsForCost == null || !ModConfig.Instance.BalanceAddRemove)
                if (true)
                {
                    if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
                    {
                        return TraitsByType[NextType.special].Concat(TraitsByType[NextType.geneShufflerTrait]).Concat(TraitsByType[NextType.posTrait]).Concat(TraitsByType[NextType.needTrait]).Concat(TraitsByType[NextType.negTrait]).ToList();
                    }
                    else
                    {
                        return TraitsByType[NextType.posTrait].Concat(TraitsByType[NextType.needTrait]).Concat(TraitsByType[NextType.negTrait]).ToList();
                    }
                }
                else
                {
                    float negative = 0, positive = 0;
                    foreach (var trait in traitsForCost)
                    {
                        switch (GetTraitListOfTrait(trait.Id, out var traitVals))
                        {

                            case NextType.posTrait:
                                positive += 1;
                                break;
                            case NextType.negTrait:
                                negative += 1;
                                break;
                            case NextType.needTrait:
                                negative += 1f / 3f;
                                break;
                            case NextType.geneShufflerTrait:
                                positive += 2.5f;
                                break;
                        }
                    }
                    var Allowed = new List<DUPLICANTSTATS.TraitVal>();

                    if (positive - negative < -3.2f)
                    {
                        Allowed.AddRange(TraitsByType[NextType.geneShufflerTrait]);
                    }

                    if (positive - negative < 2f)
                    {
                        Allowed.AddRange(TraitsByType[NextType.posTrait]);
                    }
                    if (positive - negative > -3f)
                    {
                        Allowed.AddRange(TraitsByType[NextType.needTrait]);
                    }
                    if (positive - negative > -5f)
                    {
                        Allowed.AddRange(TraitsByType[NextType.negTrait]);
                    }
                    return Allowed;
                }

            }

        }
        public static string GetSkillgroupName(SkillGroup group)
        {
            return GetChoreGroupNameForSkillgroup(group) + " (" + Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME") + ")";
        }
        public static string GetSkillgroupDescription(SkillGroup group, MinionStartingStats stats = null)
        {
            string description;
            if (group.choreGroupID != "")
            {
                ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(group.choreGroupID);
                description = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION_CHOREGROUP, group.Name, DUPLICANTSTATS.APTITUDE_BONUS, choreGroup.description);
            }
            else
                description = string.Format((string)DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, group.Name, DUPLICANTSTATS.APTITUDE_BONUS);

            if (stats == null)
                return description;

            float startingLevel = (float)stats.StartingLevels[group.relevantAttributes[0].Id];
            string attributes = group.relevantAttributes[0].Name + ": +" + startingLevel.ToString();
            List<AttributeConverter> convertersForAttribute = Db.Get().AttributeConverters.GetConvertersForAttribute(group.relevantAttributes[0]);
            for (int index = 0; index < convertersForAttribute.Count; ++index)
                attributes = attributes + "\n    • " + convertersForAttribute[index].DescriptionFromAttribute(convertersForAttribute[index].multiplier * startingLevel, (GameObject)null);

            return description + "\n\n" + attributes;
        }

        public static NextType GetTraitListOfTrait(string traitId, out List<DUPLICANTSTATS.TraitVal> TraitList)
        {
            if (DUPLICANTSTATS.GENESHUFFLERTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.GENESHUFFLERTRAITS;
                return NextType.geneShufflerTrait;
            }
            else if (DUPLICANTSTATS.GOODTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.GOODTRAITS;
                return NextType.posTrait;
            }
            else if (DUPLICANTSTATS.BADTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.BADTRAITS;
                return NextType.negTrait;
            }
            else if (DUPLICANTSTATS.NEEDTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.NEEDTRAITS;
                return NextType.needTrait;
            }
            else if (DUPLICANTSTATS.JOYTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.JOYTRAITS;
                return NextType.joy;
            }
            else if (DUPLICANTSTATS.STRESSTRAITS.FindIndex(t => t.id == traitId) != -1)
            {
                TraitList = DUPLICANTSTATS.STRESSTRAITS;
                return NextType.stress;
            }
            TraitList = null;
            return NextType.undefined;

        }

        public static Color GetColourFromType(DupeTraitManager.NextType type)
        {
            Color colorToPaint;
            switch (type)
            {
                case DupeTraitManager.NextType.joy:
                case DupeTraitManager.NextType.posTrait:
                    colorToPaint = Colors.green;
                    break;
                case DupeTraitManager.NextType.negTrait:
                case DupeTraitManager.NextType.stress:
                    colorToPaint = Colors.red;
                    break;
                case DupeTraitManager.NextType.needTrait:
                    colorToPaint = Colors.gold;
                    break;
                case DupeTraitManager.NextType.geneShufflerTrait:
                    colorToPaint = Colors.purple;
                    break;
                default:
                    colorToPaint = Colors.grey;
                    break;

            }
            return colorToPaint;
        }

        public static void ApplyTraitStyleByKey(KImage img, DupeTraitManager.NextType type)
        {
            var colorToPaint = GetColourFromType(type);

            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            ColorStyle.inactiveColor = colorToPaint;
            ColorStyle.hoverColor = UIUtils.Lighten(colorToPaint, 10);
            ColorStyle.activeColor = UIUtils.Lighten(colorToPaint, 25);
            ColorStyle.disabledColor = colorToPaint;
            img.colorStyleSetting = ColorStyle;
            img.ApplyColorStyleSetting();
        }

        public static void ApplyDefaultStyle(KImage img)
        {
            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            ColorStyle.inactiveColor = new Color(0.25f, 0.25f, 0.35f);
            ColorStyle.hoverColor = new Color(0.30f, 0.30f, 0.40f);
            ColorStyle.activeColor = new Color(0.35f, 0.35f, 0.45f);
            ColorStyle.disabledColor = new Color(0.35f, 0.35f, 0.45f);
            img.colorStyleSetting = ColorStyle;
            img.ApplyColorStyleSetting();
        }
        public static void ApplyGoodTraitStyle(KImage img)
        {
            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            ColorStyle.inactiveColor = UIUtils.rgb(68, 135, 85);
            ColorStyle.hoverColor = UIUtils.rgb(87, 173, 109);
            ColorStyle.activeColor = UIUtils.rgb(106, 211, 133);
            ColorStyle.disabledColor = UIUtils.rgb(106, 211, 133);
            img.colorStyleSetting = ColorStyle;
            img.ApplyColorStyleSetting();
        }
        public static void ApplyBadTraitStyle(KImage img)
        {
            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            ColorStyle.inactiveColor = UIUtils.rgb(140, 36, 41);
            ColorStyle.hoverColor = UIUtils.rgb(178, 45, 52);
            ColorStyle.activeColor = UIUtils.rgb(216, 54, 63);
            ColorStyle.disabledColor = UIUtils.rgb(216, 54, 63);
            img.colorStyleSetting = ColorStyle;
            img.ApplyColorStyleSetting();
        }

        public static string GetChoreGroupNameForSkillgroup(SkillGroup group)
        {
            if (group.choreGroupID == string.Empty && group.Id.ToLowerInvariant() == "suits")
                return Strings.Get("STRINGS.DUPLICANTS.ROLES.GROUPS.SUITS");

            if (group.choreGroupID != null)
                return Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + group.choreGroupID.ToUpperInvariant() + ".NAME");
            return Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + group.Id.ToUpperInvariant() + ".NAME");
        }
    }
}
