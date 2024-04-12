using Epic.OnlineServices.Platform;
using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UtilLibs.ModVersionCheck
{
    internal class OutdatedVersionInfoPatches
    {
        public const string UICMPName = "SGT_IMALAS_VERSION_INFO";
        //[HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
        public static class MainMenuMissingModsContainerInit
        {
            public static void InitMainMenuInfoPatch(Harmony harmony)
            {

                var type = AccessTools.TypeByName("MainMenu");
                if (type == null)
                {
                    SgtLogger.warning("MainMenu was null");
                    return;
                }
                SgtLogger.l("patching MainMenu.OnPrefabInit");
                var m_TargetMethod = AccessTools.Method(type, "OnPrefabInit");
                if(m_TargetMethod == null)
                {
                    SgtLogger.warning("MainMenu.OnPrefabInit was null!");
                    return;
                }

                //var m_Transpiler = AccessTools.Method(typeof(LoadModConfigPatch), "Transpiler");
                // var m_Prefix = AccessTools.Method(typeof(Steam_MakeMod), "Prefix");
                var m_Postfix = AccessTools.Method(typeof(MainMenuMissingModsContainerInit), "Postfix");

                harmony.Patch(m_TargetMethod,
                    null, //new HarmonyMethod(m_Prefix),
                    new HarmonyMethod(m_Postfix,Priority.LowerThanNormal)
                    );
            }

            public static void Postfix(MainMenu __instance)
            {

                if (VersionChecker.OlderVersion
                    || VersionChecker.UI_Built()                    
                    )
                {
                    SgtLogger.l("version info already initiated");
                    return;
                }

                VersionChecker.SetUIConstructed(true);
                if (!VersionChecker.ModsOutOfDate(50, out var infoString, out int linecount))
                {
                    SgtLogger.l("no mods out of date");
                    return;
                }

                SgtLogger.l(PRegistry.GetData<int>(VersionChecker.VersionCheckerVersion).ToString(), "Current UI handler version:");

                //UIUtils.ListAllChildrenPath(__instance.transform);
                SgtLogger.l("grabbing ref.");
                var options = Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject);                
                SgtLogger.Assert("options", options); 
                if (options == null)
                    return;

                var feedbackClone = Util.KInstantiateUI<FeedbackScreen>(options.feedbackScreenPrefab.gameObject);
                SgtLogger.Assert("feedbackClone", feedbackClone);
                if (feedbackClone == null)
                    return;
                //UIUtils.ListAllChildrenPath(feedbackClone.transform);
                var _infoBox = Util.KInstantiateUI(feedbackClone.transform.Find("Content/GameObject/InfoBox").gameObject);
                SgtLogger.Assert("_infoBox", _infoBox);
                UnityEngine.Object.Destroy(options.gameObject);
                UnityEngine.Object.Destroy(feedbackClone.gameObject);
                if (_infoBox == null)
                    return;


                GameObject parent = null; GameObject info = null;
                int height = 75 + linecount * 20;
                RectTransform.Edge verticalEdge = RectTransform.Edge.Top, horizontalEdge = RectTransform.Edge.Left;
                int verticalInset = 250, horizontalInset = 25;
                if (__instance.transform.Find("MainMenuMenubar/BottomRow") != null) ///BASE GAME
                {
                    parent = __instance
                        //.transform.Find("MainMenuMenubar/BottomRow"
                        //+"/BottomRow/RightAnchor/MOTD"
                        //)
                        .gameObject;
                    info = Util.KInstantiateUI(_infoBox, parent, true);
                    //var rect = info.rectTransform();
                    //rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 250, height);
                    //rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 25, 295);
                    verticalEdge = RectTransform.Edge.Bottom;
                    horizontalEdge = RectTransform.Edge.Right;
                    verticalInset = 245;
                    horizontalInset = 25;
                }
                else if(__instance.transform.Find("UI Group")!=null) ///SPACED OUT
                {
                    parent = __instance.transform.Find("UI Group").gameObject;
                    info = Util.KInstantiateUI(_infoBox, parent, true);
                    verticalEdge = RectTransform.Edge.Top;
                    horizontalEdge = RectTransform.Edge.Left;
                    //rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 25, height);
                    //rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 325, 295);
                    verticalInset = 25;
                    horizontalInset = 325;
                }
                if(info!= null) 
                {
                    info.name = UICMPName;
                    if(info.transform.Find("Header").gameObject.TryGetComponent<LocText>(out var header))
                    {
                        header.text = UIUtils.ColorText("Outdated Mods:", UIUtils.rgb(237, 89, 92));
                    }
                    var text = info.transform.Find("Description");
                    if (text.gameObject.TryGetComponent<LocText>(out var desc))
                    {
                        desc.text = infoString;// "The following mods are currently not on their latest version:...";
                    }
                    if (info.transform.Find("BG"))
                    {
                        info.transform.Find("BG").FindOrAddComponent<Outline>();
                    }
                    RectTransform rect = info.rectTransform();

                    rect.SetInsetAndSizeFromParentEdge(horizontalEdge, horizontalInset, 298);
                    rect.SetInsetAndSizeFromParentEdge(verticalEdge , verticalInset, height );
                }
            }
        }
    }
}
