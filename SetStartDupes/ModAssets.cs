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


        public static GameObject CycleButtonLeftPrefab;
        public static GameObject CycleButtonRightPrefab;
        public static List<ITelepadDeliverableContainer> ContainerReplacement;



        public static GameObject PresetWindowPrefab;
        public static GameObject TraitsWindowPrefab;
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

        public static class Colors
        {
            public static Color gold = UIUtils.Darken(Util.ColorFromHex("ffdb6e"),40);
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
        };

        public static List<DUPLICANTSTATS.TraitVal> TryGetTraitsOfCategory(NextType type)
        {
            if(!TraitsByType.ContainsKey(type))
                return new List<DUPLICANTSTATS.TraitVal>();
            else
                return TraitsByType[type];

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
            ColorStyle.hoverColor = UIUtils.Lighten(colorToPaint,10);
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
