using HarmonyLib;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
    }
}
