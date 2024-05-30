
using BlueprintsV2.BlueprintsV2.ModAPI;
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

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
    internal class SkinHelper
    {
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
                string currentVariantID = arg2.GetValue("currentVariantID").Value<string>();

                Traverse.Create(moodLampCmp).Method("SetVariant", new[] { typeof(string) }).GetValue(currentVariantID);

            }
        }

        internal static JObject TryStoreMoodLamp(GameObject arg)
        {
            JObject data = null;
            var moodLampCmp = arg.GetComponent("MoodLamp");
            if (moodLampCmp != null)
            {
                var currentVariantID = Traverse.Create(moodLampCmp).Field("currentVariantID").GetValue() as string;

                //SgtLogger.l($"Pattern: {pattern}, colorHex: {colorHex}");
                data = new JObject()
                {
                    {"currentVariantID", currentVariantID}
                };
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
                    if (BlueprintsState.InstantBuild)
                    {
                        sculpture.SetStage(facadeID,true);
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
            if(arg.TryGetComponent<BuildingFacade>(out var buildingFacade) && !buildingFacade.IsOriginal)
            {

                skinId = buildingFacade.CurrentFacade;
            }
            if(!ValidFacadeId(skinId))
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
