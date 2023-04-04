using Database;
using HarmonyLib;
using Klei.AI;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    if (__instance.GetSeasonIds().Count == 0|| __instance.GetSeasonIds().Contains("MeteorShowers"))
                    {

                        if (SettingsCache.worlds.HasWorld(__instance.worldName))
                        {
                            var Data = SettingsCache.worlds.GetWorldData(__instance.worldName);
                            if (Data.seasons.Count > 0)
                            {
                                __instance.m_seasonIds.Clear();
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
                        }
                        else
                            SgtLogger.logwarning("Planet not found in world data");
                    }
                    else
                        SgtLogger.l("no migration required for " + __instance.worldName);
                }
            }
        }
    }
}
