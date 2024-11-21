using HarmonyLib;
using Klei.AI;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilLibs;
using static ProcGen.ClusterLayout;
using static ProcGen.World;

namespace TinyFixes
{
    internal class Patches
    {
        /// <summary>
        /// Fix the reactor meter by removing that obsolete frame scale hack thing from an earlier reactor implementation
        /// </summary>
        [HarmonyPatch(typeof(Reactor), nameof(Reactor.OnSpawn))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                Reactor.meterFrameScaleHack = 1;
            }
        }

        /// <summary>
        /// fix that check to actually check for immunities instead of hardcoding for the WarmTouch effect (which breaks the effect for WarmTouchFood)
        /// </summary>
        [HarmonyPatch(typeof(ColdImmunityMonitor), nameof(ColdImmunityMonitor.HasImmunityEffect))]
        public static class ColdImmunityMonitor_HasImmunityEffect
        {
            static Effect ColdAir;
            public static void Postfix(ref bool __result, ColdImmunityMonitor.Instance smi)
            {
                if (__result)
                    return;
                if (ColdAir == null)
                    ColdAir = Db.Get().effects.Get("ColdAir");

                var effects = smi.GetComponent<Effects>();
                if (effects.HasImmunityTo(ColdAir))
                    __result = true;
            }
        }
        /// <summary>
        /// add proper cold air effect immunity to WarmTouch and WarmTouchFood so the tooltips actually reflect that
        /// </summary>
        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            public static void Postfix()
            {
                Effect frostImmunityEffect = Db.Get().effects.Get("WarmTouch");
                frostImmunityEffect.immunityEffectsNames = frostImmunityEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();

                Effect frostImmunityFoodEffect = Db.Get().effects.Get("WarmTouchFood");
                frostImmunityFoodEffect.immunityEffectsNames = frostImmunityFoodEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();
            }
        }

        [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnPrefabInit))]
        public class MainMenu_OnPrefabInit
        {
            public class VanillaStarmapLocation
            {
                public string Id;
                public string Name;
                public string Image;
            }

            public class Asteroid
            {
                public string Id;
                public string Name;
                public string Image;
                public bool DisableWorldTraits = false;
                public List<TraitRule> TraitRules;
                public float worldTraitScale;
            }
            public class ClusterLayout
            {
                public string Id;
                public string Name;
                public string Prefix;
                public int menuOrder;
                public int startWorldIndex;
                public string[] RequiredDlcsIDs;
                public string[] ForbiddenDlcIDs;
                public List<string> WorldPlacementIDs;
                public int clusterCategory;
                public int fixedCoordinate;
            }
            public class DataExport
            {
                public List<ClusterLayout> clusters = new();
                public List<Asteroid> asteroids = new();
                public List<WorldTrait> worldTraits = new();

            }
            public class WorldTrait
            {
                public string Id;
                public string Name, ColorHex;
                public List<string> forbiddenDLCIds, exclusiveWith, exclusiveWithTags, traitTags;
                public Dictionary<string, int> globalFeatureMods { get; set; }

                public WorldTrait()
                {
                    exclusiveWith = new List<string>();
                    exclusiveWithTags = new List<string>();
                    forbiddenDLCIds = new List<string>();
                    traitTags = new List<string>();
                    Name = string.Empty;
                    Id = string.Empty;
                }
            }


            public static void Postfix()
            {
                var export = new DataExport();
                foreach (var cluster in SettingsCache.clusterLayouts.clusterCache.Values)
                {
                    var data = new ClusterLayout();
                    data.Id = cluster.filePath;
                    data.Name = Strings.Get(cluster.name);
                    data.Prefix = cluster.coordinatePrefix;
                    data.menuOrder = cluster.menuOrder;
                    data.RequiredDlcsIDs = cluster.requiredDlcIds;
                    data.ForbiddenDlcIDs = cluster.forbiddenDlcIds;
                    //data.WorldPlacements = cluster.worldPlacements;
                    data.startWorldIndex = cluster.startWorldIndex;

                    data.WorldPlacementIDs = cluster.worldPlacements.Select(pl => pl.world).ToList();
                    data.clusterCategory = (int)cluster.clusterCategory;
                    data.fixedCoordinate = cluster.fixedCoordinate;
                    export.clusters.Add(data);
                }
                foreach (var world in SettingsCache.worlds.worldCache.Values)
                {
                    var data = new Asteroid();
                    data.Id = world.filePath;
                    data.Name = Strings.Get(world.name);
                    data.DisableWorldTraits = world.disableWorldTraits;
                    data.TraitRules = world.worldTraitRules;
                    data.worldTraitScale = world.worldTraitScale;

                    export.asteroids.Add(data);
                }
                foreach (var trait in SettingsCache.worldTraits.Values)
                {
                    var data = new WorldTrait();
                    data.Id = trait.filePath;
                    data.Name = Strings.Get(trait.name);
                    data.ColorHex = trait.colorHex;
                    data.forbiddenDLCIds = trait.forbiddenDLCIds;
                    data.exclusiveWith = trait.exclusiveWith;
                    data.exclusiveWithTags = trait.exclusiveWithTags;
                    data.traitTags = trait.traitTags;
                    data.globalFeatureMods = trait.globalFeatureMods;

                    export.worldTraits.Add(data);
                }
                Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(export));
            }
        }
    }
}
