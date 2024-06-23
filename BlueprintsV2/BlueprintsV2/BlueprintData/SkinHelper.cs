
using BlueprintsV2.ModAPI;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static LogicGateVisualizer;
using static STRINGS.UI.BUILDCATEGORIES;

namespace BlueprintsV2.BlueprintData
{
    internal class SkinHelper
    {
        ///checks for both SignsTagsAndRibbons and WoodenSetFurniture
        internal static void TryApplySelectableSign(GameObject arg1, JObject arg2)
        {
            var SelectableSignCmp = arg1.GetComponent("SignsTagsAndRibbons.SelectableSign"); 

            if(SelectableSignCmp == null)
            {
                SelectableSignCmp = arg1.GetComponent("WoodenSetFurniture.SelectableSign");                
            }

            if (SelectableSignCmp != null)
            {
                string variant = arg2.GetValue("variant").Value<string>();

                Traverse.Create(SelectableSignCmp).Method("SetVariant", new[] { typeof(string) }).GetValue(variant);
            }
        }

        internal static JObject TryStoreSelectableSign(GameObject arg)
        {
            JObject data = null;
            var SelectableSignCmp = arg.GetComponent("SignsTagsAndRibbons.SelectableSign");

            if (SelectableSignCmp == null)
            {
                SelectableSignCmp = arg.GetComponent("WoodenSetFurniture.SelectableSign");
            }

            if (SelectableSignCmp != null)
            {
                var selectedIndex = (int)Traverse.Create(SelectableSignCmp).Field("selectedIndex").GetValue();
                var AnimationNames = (List<string>)Traverse.Create(SelectableSignCmp).Field("AnimationNames").GetValue();

                SgtLogger.l(selectedIndex.ToString(), "index");
                if (selectedIndex >= 0 && selectedIndex < AnimationNames.Count)
                {
                    data = new JObject()
                    {
                        {"variant", AnimationNames[selectedIndex]}
                    };
                }
            }
            return data;
        }

        internal static void TryApplyBackwall(GameObject arg1, JObject arg2)
        {
            var backwallCmp = arg1.GetComponent("Backwall");

            if (backwallCmp != null)
            {
                string colorHex = arg2.GetValue("colorHex").Value<string>();
                string pattern = arg2.GetValue("pattern").Value<string>();

                //GameScheduler.Instance.ScheduleNextFrame("backwall pattern",(_)=>
                Traverse.Create(backwallCmp).Method("TrySetPattern", new[] { typeof(string) }).GetValue(pattern);
                //GameScheduler.Instance.ScheduleNextFrame("backwall color",(_)=>
                Traverse.Create(backwallCmp).Method("SetColor", new[] { typeof(string) }).GetValue(colorHex);
                Traverse.Create(backwallCmp).Field("copiedColor").SetValue(true);
                //GameScheduler.Instance.ScheduleNextFrame("backwall" ,(_)=> Traverse.Create(backwallCmp).Method("TrySetPattern", new[] { typeof(string) }).GetValue(arg2));

            }
        }

        internal static JObject TryStoreBackwall(GameObject arg)
        {
            JObject data = null;
            var backwallCmp = arg.GetComponent("Backwall");
            if (backwallCmp != null)
            {
                var settingsStruct = Traverse.Create(backwallCmp).Field("settings").GetValue();
                var colorHex = Traverse.Create(settingsStruct).Field("colorHex").GetValue() as string;
                var pattern = Traverse.Create(settingsStruct).Field("pattern").GetValue() as string;

                //SgtLogger.l($"Pattern: {pattern}, colorHex: {colorHex}");
                data = new JObject()
                {
                    {"colorHex", colorHex},
                    {"pattern", pattern}
                };
            }
            return data;
        }

        internal static void TryApplyMoodLamp(GameObject arg1, JObject arg2)
        {
            var moodLampCmp = arg1.GetComponent("MoodLamp");

            if (moodLampCmp != null)
            {
                string currentVariantID = arg2.GetValue("currentVariantID")?.Value<string>();

                if (currentVariantID != null) 
                    Traverse.Create(moodLampCmp).Method("SetVariant", new[] { typeof(string) }).GetValue(currentVariantID);
            }
            if(arg2.TryGetValue("colorHex", out var colorHexToken))
            {
                var tintableLampCmp = arg1.GetComponent("TintableLamp");
                if(tintableLampCmp != null)                {

                    string colorHex = colorHexToken.Value<string>();
                    var color = Util.ColorFromHex(colorHex);
                    Traverse.Create(tintableLampCmp).Method("SetColor", new[] { typeof(Color) }).GetValue(color);
                }

            }
        }

        internal static JObject TryStoreMoodLamp(GameObject arg)
        {
            JObject data = null;
            string currentVariantID = null;
            string colorHex = null;

            var moodLampCmp = arg.GetComponent("MoodLamp");
            if (moodLampCmp != null)
            {
                currentVariantID = Traverse.Create(moodLampCmp).Field("currentVariantID").GetValue() as string;                
            }
            var tintableLampCmp = arg.GetComponent("TintableLamp"); 
            if (tintableLampCmp != null)
            {
                colorHex = Traverse.Create(tintableLampCmp).Field("colorHex").GetValue() as string;
            }

            if (currentVariantID != null)
            {
                data = new JObject()
                {
                    {"currentVariantID", currentVariantID}
                };

                if(colorHex != null)
                    data.Add("colorHex",colorHex);
            }


            return data;
        }

        internal static JObject TryStoreArtableSkin(GameObject arg)
        {
            string skinId = string.Empty;
            if (arg.TryGetComponent<Artable>(out var artable))
            {
                if (!string.IsNullOrEmpty(artable.userChosenTargetStage))
                {
                    skinId = artable.userChosenTargetStage;
                }
                else if (!string.IsNullOrEmpty(artable.CurrentStage))
                {
                    skinId = artable.CurrentStage;
                }
                else
                    skinId = "Default";
            }
            if (!ValidArtableId(skinId))
            {
                return null;
            }
            return new JObject()
            {
                { "CurrentStage", skinId }
            };
        }
        public static void TryApplyArtableSkin(GameObject building, JObject facadeObj)
        {
            if (facadeObj == null)
                return;

            var token = facadeObj.SelectToken("CurrentStage");
            if (token == null)
                return;

            string facadeID = token.Value<string>();

            if (building.TryGetComponent<Artable>(out var sculpture))
            {
                if (ValidArtableId(facadeID))
                {
                    if (BlueprintState.InstantBuild)
                    {
                        sculpture.SetStage(facadeID, true);
                        sculpture.userChosenTargetStage = null;
                    }
                    else
                    {
                        sculpture.chore?.Cancel("blueprint applied");
                        sculpture.SetUserChosenTargetState(facadeID);
                    }
                }
            }
        }
        internal static JObject TryStoreBuildingSkin(GameObject arg)
        {
            string skinId = string.Empty;
            if (arg.TryGetComponent<BuildingFacade>(out var buildingFacade) && !buildingFacade.IsOriginal)
            {

                skinId = buildingFacade.CurrentFacade;
            }
            if (!ValidFacadeId(skinId))
            {
                return null;
            }
            return new JObject()
            {
                { "CurrentFacade", skinId }
            };
        }
        public static void TryApplyBuildingSkin(GameObject building, JObject facadeObj)
        {
            if (facadeObj == null)
                return;

            var token = facadeObj.SelectToken("CurrentFacade");
            if (token == null)
                return;

            string facadeID = token.Value<string>();

            if (building.TryGetComponent<BuildingFacade>(out var buildingFacade))
            {
                if (ValidFacadeId(facadeID))
                {
                    buildingFacade.ApplyBuildingFacade(Db.GetBuildingFacades().Get(facadeID));
                    if(building.GetComponent("FacadeRestorer")!= null && building.TryGetComponent<KBatchedAnimController>(out var kbac)
                        )
                    {
                        //refresh the anim on akis facade restorer aero pods
                        SgtLogger.l("fixing decor pack aero pod");
                        kbac.Play("off");
                    }
                }
            }
        }
        static bool ValidFacadeId(string facadeID)
        {
            return !facadeID.IsNullOrWhiteSpace() && facadeID != "DEFAULT_FACADE" && Db.GetBuildingFacades().TryGet(facadeID) != null
                // && Db.GetBuildingFacades().Get(facadeID).IsUnlocked()
                ;
        }
        static bool ValidArtableId(string artableID)
        {
            return !artableID.IsNullOrWhiteSpace() && artableID != "Default" && Db.GetArtableStages().TryGet(artableID) != null
                // && Db.GetBuildingFacades().Get(facadeID).IsUnlocked()
                ;
        }
    }
}
