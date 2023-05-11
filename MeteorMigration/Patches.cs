using Database;
using HarmonyLib;
using Klei.AI;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static MeteorMigration.ModAssets;

namespace MeteorMigration
{
    internal class Patches
    {
        /// <summary>
        /// Add Meteor Showers to old planets
        /// </summary>
        [HarmonyPatch(typeof(WorldContainer))]
        [HarmonyPatch(nameof(WorldContainer.RefreshFixedTraits))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Postfix(WorldContainer __instance)
            {
                if (!__instance.IsModuleInterior)
                {
                    if (__instance.GetSeasonIds().Count == 0 || __instance.GetSeasonIds().Contains("MeteorShowers"))
                    {
                        __instance.m_seasonIds.Clear();

                        if (SettingsCache.worlds.HasWorld(__instance.worldName))
                        {
                            var Data = SettingsCache.worlds.GetWorldData(__instance.worldName);
                            if (Data.seasons.Count > 0)
                            {
                                __instance.m_seasonIds.AddRange(Data.seasons);
                                SgtLogger.l("Migrated Meteor Showers for " + __instance.worldName + ".");

                                string seasonstring = string.Empty;
                                foreach (var season in Data.seasons)
                                    seasonstring += season.ToString() + ", ";

                                if (Data.seasons.Count == 1)
                                    seasonstring.Replace(",", string.Empty);

                                SgtLogger.l("Previous meteor season type for this planet: none.");
                                SgtLogger.l("New meteor season type for this planet: " + seasonstring + ".");

                            }
                            else
                                SgtLogger.l("meteor season type for this planet: none.");
                        }
                        else
                            SgtLogger.logwarning("Planet\"" + __instance.worldName+"\" not found in world data");
                    }
                    else
                        SgtLogger.l("no migration required for " + __instance.worldName);
                }
            }
        }

        /// <summary>
        /// The following method can be copy-pasted by other mods that had seasons previously and now are broken as a fix for existing worlds
        /// </summary>
        //[HarmonyPatch(typeof(WorldContainer), "RefreshFixedTraits")]
        //public static class Baator_CrashFix
        //{
        //    public static void Postfix(WorldContainer __instance)
        //    {
        //        if (!__instance.IsModuleInterior)
        //        {
        //            if (__instance.GetSeasonIds().Contains("MeteorShowers"))
        //            {
        //                typeof(WorldContainer).GetField("m_seasonIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, new List<string>());

        //                if (SettingsCache.worlds.HasWorld(__instance.worldName))
        //                {
        //                    var Data = SettingsCache.worlds.GetWorldData(__instance.worldName);
        //                    if (Data.seasons.Count > 0)
        //                    {

        //                        typeof(WorldContainer).GetField("m_seasonIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance,new List<string>(Data.seasons));

        //                        Debug.Log("Baator: Fixed Meteor Showers for " + __instance.worldName + ".");
        //                    }
        //                }
        //                else
        //                    Debug.LogWarning("Planet not found in world data");
        //            }
        //        }
        //    }
        //}
    }
}
