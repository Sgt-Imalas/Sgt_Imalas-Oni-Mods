using HarmonyLib;
using Klei.AI;
using SaveGameModLoader.ModFilter;
using SaveGameModLoader.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace SaveGameModLoader
{
    internal class ModAssets
    {
        public static string ModPath;
        public static string ConfigPath;
        public static string ModPacksPath;
        //public static string ModID;

        public static bool FastTrackActive = false;
        public static bool ModsFilterActive = false;


        public static bool UseSteamOverlay;

        public enum BrowserChoice
        {
            undefined = 0,
            web = 1,
            steamOverlay = 2,
        }

        public static string RegistryKey = "Workshop_Browser_Choice";
        public static void ReadOrRegisterBrowserSetting()
        {
            if (KPlayerPrefs.GetInt(RegistryKey) == (int)BrowserChoice.undefined) //nothing valid set;
            {
                KPlayerPrefs.SetInt(RegistryKey, (int)BrowserChoice.steamOverlay);
            }
            UseSteamOverlay = KPlayerPrefs.GetInt(RegistryKey) == (int)BrowserChoice.steamOverlay;
        }

        public static GameObject AddCopyButton(GameObject parent, System.Action onClick, System.Action onDoubleClick, ColorStyleSetting setting)
        {

            var button = Util.KInstantiateUI(FilterPatches._copyToClipboardPrefab, parent,true);
           

            button.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0,40);
            button.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0,40);
            button.TryGetComponent<KButton>(out var bt);
            //button.TryGetComponent<Image>(out var buttonimage);
            //UtilMethods.ListAllPropertyValues(buttonimage);
            bt.bgImage.colorStyleSetting = setting;

            var bgImage = button.transform.Find("GameObject").GetComponent<Image>();
            bgImage.sprite = Assets.GetSprite(SpritePatch.copySymbol);
            bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
            bgImage.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
            button.GetComponent<ToolTip>().SetSimpleTooltip(STRINGS.UI.FRONTEND.MODLISTVIEW.COPYTOCLIPBOARD_TOOLTIP);
            bt.ClearOnClick();
            if(onClick != null)
                bt.onClick += onClick;
            if(onDoubleClick != null) 
                bt.onDoubleClick += onDoubleClick;
            button.SetActive(true);
            button.transform.SetAsFirstSibling();
            return button;
        }

        public static bool ModWithinTextFilter(string filterText, KMod.Label label)
        {
            bool isWithinText = CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                                        label.title,
                                        filterText,
                                        CompareOptions.IgnoreCase
                                    ) >= 0;
            if(!isWithinText)
            {
                if(SteamInfoQuery.FetchedModAuthors.ContainsKey(label.id))
                {
                    isWithinText = CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                                        SteamInfoQuery.FetchedModAuthors[label.id],
                                        filterText,
                                        CompareOptions.IgnoreCase
                                    ) >= 0;
                }
            }
            return isWithinText;
        }


        public static void PutCurrentToClipboard(bool linkIncluded)
        {
            List<KMod.Label> activeMods = Global.Instance.modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();
            PutToClipboard(activeMods, linkIncluded);
        }
        public static void PutToClipboard(List<KMod.Label> mods, bool includeLink)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var mod in mods)
            {
                stringBuilder.Append(mod.title);
                if (includeLink)
                {
                    stringBuilder.Append(" (");
                    if (mod.distribution_platform == KMod.Label.DistributionPlatform.Steam)
                    {
                        stringBuilder.Append("https://steamcommunity.com/sharedfiles/filedetails/?id=");
                        stringBuilder.Append(mod.id);
                    }
                    else
                    {
                        stringBuilder.Append(STRINGS.UI.FRONTEND.FILTERSTRINGS.DROPDOWN.LOCAL);
                    }
                    stringBuilder.Append(')');
                }
                stringBuilder.AppendLine();
                //stringBuilder.AppendLine(",");
            }

            var TextEditorType = Type.GetType("UnityEngine.TextEditor, UnityEngine");
            if(TextEditorType != null)
            {
                var editor = Activator.CreateInstance(TextEditorType);
                var tr = Traverse.Create(editor);
                tr.Property("text").SetValue(stringBuilder.ToString());
                tr.Method("SelectAll").GetValue();
                tr.Method("Copy").GetValue();
            }
        }
    }
}
