using Database;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static SetStartDupes.DupeTraitManager;

namespace SetStartDupes
{
    public class ModAssets
    {
        public static string DupeTemplatePath;
        public static string DupeTemplateName = "UnnamedDuplicantPreset";
        public static bool EditingSingleDupe = false;
        public static MinionStartingStats _TargetStats;

        public static CharacterContainer PrefabToFix;
        public static GameObject StartPrefab;

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


        public static Dictionary<MinionStartingStats, DupeTraitManager> DupeTraitManagers = new Dictionary<MinionStartingStats, DupeTraitManager>();


        public static int MinimumPointsPerInterest(MinionStartingStats stats)
        {
            int SkillAmount = 0;
            foreach (var s in stats.StartingLevels)
            {
                if (s.Value > 0)
                    SkillAmount++;
            }

            return PointsPerInterests(SkillAmount);
        }

        public static int PointsPerInterests(int numberOfInterests)
        {
            int pointsPer = 0;
            if (numberOfInterests > 0)
            {
                if (numberOfInterests == 1)
                    pointsPer = 7;
                else if (numberOfInterests == 2)
                    pointsPer = 3;
                else
                    pointsPer = 1;
            }
            return pointsPer;
        }

        public static void RedoStatpointBonus(MinionStartingStats stats, Trait trait, bool isAdding = false)
        {
            ModAssets.GetTraitListOfTrait(trait.Id, out var list);
            var traitBonusHolder = list.Find(traitTo => traitTo.id == trait.Id);
            
            if (traitBonusHolder.statBonus == 0)
                return;

            int targetPoints = 0;
            int minimumPoints = MinimumPointsPerInterest(stats);
            int currentPoints = 0;

            var allTraitStats = TryGetTraitsOfCategory(NextType.allTraits);
            foreach (var activeTrait in stats.Traits)
            {
                var active = allTraitStats.Find(match => match.id == activeTrait.Id);
                if(active.statBonus != 0)
                {
                    //SgtLogger.l(active.statBonus.ToString(), active.id);
                    targetPoints += active.statBonus;
                }
            }
            //SgtLogger.l(targetPoints.ToString(), "ActiveStatBonus");

            foreach (var level in stats.StartingLevels.Values)
            {
                currentPoints += Math.Max(0, level - minimumPoints);
            }
            int difference = targetPoints - currentPoints;



            bool subtracting = difference < 0;
            if (subtracting)
                difference *= -1;


            //SgtLogger.l(difference.ToString(), subtracting?"Removing":"Adding");

            Dictionary<string, int> newVals = new Dictionary<string, int>();
            int i = 40;
            while (difference > 0 && i>0)
            {
                --i;
                foreach (var level in stats.StartingLevels.Shuffle())
                {
                    if (difference <= 0)
                        continue;

                    if (level.Value > 0 && !subtracting || subtracting && level.Value > minimumPoints)
                    {
                        int randPoints = 1;
                        difference-= randPoints;


                        if (!newVals.ContainsKey(level.Key))
                        {
                            newVals.Add(level.Key, stats.StartingLevels[level.Key] + (!subtracting ? randPoints : -randPoints));
                        }
                        else
                            newVals[level.Key] += (!subtracting ? randPoints : -randPoints);

                    }
                }
            }
            foreach (var newv in newVals)
            {
                stats.StartingLevels[newv.Key] = Math.Max(minimumPoints,newv.Value);
            }

            if (DupeTraitManagers.ContainsKey(stats))
            {
                DupeTraitManagers[stats].CalculateAdditionalSkillPoints(targetPoints);
            }
        }

        public static string GetTraitTooltip(Trait trait)
        {
            string tooltip = trait.GetTooltip();

            ModAssets.GetTraitListOfTrait(trait.Id, out var list);
            var traitBonusHolder = list.Find(traitTo => traitTo.id == trait.Id);

            if(traitBonusHolder.statBonus != 0)
            {
                tooltip += "\n\n" + STRINGS.UI.DUPESETTINGSSCREEN.TRAITBONUSPOINTS +" "+ traitBonusHolder.statBonus;
            }

            return tooltip;
        }


        public static void RemoveTrait(MinionStartingStats stats, Trait trait)
        {
            if (stats.Traits.Contains(trait))
            {
                stats.Traits.Remove(trait);
                RedoStatpointBonus(stats, trait,false);
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
                    return TraitsByType[NextType.posTrait].Concat(TraitsByType[NextType.needTrait]).Concat(TraitsByType[NextType.negTrait]).ToList();
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
            return Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + group.Id.ToUpperInvariant() + ".NAME") + " (" + Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME") + ")";
        }
        public static string GetSkillgroupDescription(SkillGroup group)
        {
            return Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + group.relevantAttributes.First().Id.ToUpperInvariant() + ".DESC");
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
    }
}
