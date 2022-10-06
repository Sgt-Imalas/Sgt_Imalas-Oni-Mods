using Klei.AI;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace SetStartDupes
{
    public class ModAssets
    {
        public static CharacterContainer PrefabToFix;
        public static GameObject StartPrefab;
        public static bool HasShrunkenDown = false;

        public static GameObject NextButtonPrefab;

        public static GameObject ListEntryButtonPrefab;


        public static void ApplyDefaultStyle(KImage img)
        {
            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            ColorStyle.inactiveColor = new Color(0.25f, 0.25f, 0.35f);
            ColorStyle.hoverColor = new Color(0.30f, 0.30f, 0.40f);
            ColorStyle.activeColor = new Color(0.35f, 0.35f, 0.45f);
            ColorStyle.disabledColor = new Color(0.7f, 0.7f, 0.7f);
            img.colorStyleSetting = ColorStyle;
            img.ApplyColorStyleSetting();
        }
        public static void ApplyGoodTraitStyle(KImage img)
        {
            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            ColorStyle.inactiveColor = UIUtils.rgb(68, 135, 85);
            ColorStyle.hoverColor = UIUtils.rgb(87, 173, 109);
            ColorStyle.activeColor = UIUtils.rgb(106, 211, 133);
            ColorStyle.disabledColor = new Color(0.7f, 0.7f, 0.7f);
            img.colorStyleSetting = ColorStyle;
            img.ApplyColorStyleSetting();
        }
        public static void ApplyBadTraitStyle(KImage img)
        {
            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
            ColorStyle.inactiveColor = UIUtils.rgb(140, 36, 41);
            ColorStyle.hoverColor = UIUtils.rgb(178, 45, 52);
            ColorStyle.activeColor = UIUtils.rgb(216, 54, 63);
            ColorStyle.disabledColor = new Color(0.7f, 0.7f, 0.7f);
            img.colorStyleSetting = ColorStyle;
            img.ApplyColorStyleSetting();
        }
    }
}
