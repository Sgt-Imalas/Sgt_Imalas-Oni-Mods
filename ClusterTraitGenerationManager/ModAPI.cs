using System.Collections.Generic;
using System.Linq;
using UtilLibs;
using static ClusterTraitGenerationManager.STRINGS.CLUSTER_NAMES;
using static ProcGen.World;

namespace ClusterTraitGenerationManager
{
    public static class ModAPI
    {
        public static class CGM_TraitTags
        {
            /// <summary>
            /// Traits with this tag will be shown in the selection screen, even if world rules forbid it.
            /// 
            /// The main purpose of this trait tag is to have traits that cannot appear
            /// by default on planets, but that can be manually added through CGM.This
            /// can be accomplished with the following trait tags:
            /// 
            /// - CGM_OverrideWorldRules_AlwaysAllow
            /// - StartWorldOnly
            /// - NonStartWorld
            /// </summary>
            public static readonly string OverrideWorldRules_AlwaysAllow = "CGM_OverrideWorldRules_AlwaysAllow";
        }



        /// <summary>
        /// you can put custom data for the following settings in each world template by adding it under that key to defaultsOverride.data to override the default selected templates and biomes for your world
        /// CGM will generate an alternative version of each planet, depending on its origin those are 
        ///     Start Version - Players can start on this planet
        ///     Warp Version - The Teleporters will connect to this planet
        ///     Outer Version - No Teleporter templates are here, only reachable via rocket
        /// The version that is not generated is the default version of your planet, the type of that is determined in CGSMClusterManager.PlanetoidDict
        /// </summary>


        //Start World

        /// <summary>
        /// Overrides the subworld type the printing pod / the warp base spawns in.
        /// Used for starter planet variants
        /// </summary>
        public const string StartWorld_StartingAreaSubworld_Key = "CGM_StartWorld_StartingAreaSubworld_Key";


        /// <summary>
        /// Starting Worlds come with additional starting water, this key overrides the type of subworld that contains the starting water.
        /// Used for starter planet variants.
        /// </summary>
        public const string StartWorld_StartingAreaWaterSubworld_Key = "CGM_StartingAreaWaterSubworld_Key";

        /// <summary>
        /// Overrides the start world starterbase template (usually the printing pod template) that is selected for this planet
        /// Used for starter planet variants.
        /// </summary>
        public const string StartWorld_Base_Key = "CGM_StartWorld_Base_Key";

        
        //Warp World 

        /// <summary>
        /// Overrides the subworld type the printing pod / the warp base spawns in.
        /// Used for warp planet variants
        /// </summary>
        public const string WarpWorld_StartingAreaSubworld_Key = "CGM_WarpWorld_StartingAreaSubworld_Key";
        /// <summary>
        /// Overrides the warp world warp base template that is selected for this planet
        /// It is assumed this template contains the warp world teleporters
        /// Used for warp planet variants.
        /// </summary>
        public const string WarpWorld_Base_Key = "CGM_WarpWorld_Base_Key";



        /// <summary>
        /// if you use any unusual teleporter template that dont match the condition checks in the method "IsATeleporterTemplate", please include them under this key.
        /// this ensures the Outer planet variant does not get a teleporter template placed
        /// </summary>
        public const string TeleporterTemplate_Key = "CGM_TeleporterTemplate_Key";


        public static bool IsATeleporterTemplate(ProcGen.World world,TemplateSpawnRules rules)
        {
            if (world != null)
            {
                if (world.defaultsOverrides != null && world.defaultsOverrides.data != null && world.defaultsOverrides.data.Count > 0)
                {
                    {
                        if (world.defaultsOverrides.data.TryGetValue(TeleporterTemplate_Key, out var teleporterTemplateKey) && teleporterTemplateKey is string)
                        {
                            if (rules.names.Any(name => name.Contains(teleporterTemplateKey as string)))
                                return true;
                        }
                    }
                }
            }

            if (rules.names.Any(name => name.Contains("poi/warp"))) ///Vanilla Teleporters
                return true; 
            if (rules.names.Any(name => name.Contains("onewayteleport"))) ///Baator Check
                return true;

            return false;
        }

        private static Dictionary<string, int> _predefinedWorldCompositionOverrides;
        public static Dictionary<string, int> PredefinedWorldCompositionOverrides
        {
            get
            {
                if (_predefinedWorldCompositionOverrides == null)
                    InitPredefs();
                return _predefinedWorldCompositionOverrides;
            }
        }
        static void InitPredefs()
        {
            _predefinedWorldCompositionOverrides = new Dictionary<string, int>();
            _predefinedWorldCompositionOverrides.Add("expansion1::worlds/MediumRadioactiveVanillaPlanetStart", (int)StartAreaType.Swamp);

        }

        public const string DefaultSandstoneStartBiome = "expansion1::subworlds/sandstone/SandstoneStart";
        public const string DefaultSandstoneWarpBiome = "expansion1::subworlds/sandstone/SandstoneWarpStart";
        public const string DefaultSwampStartBiome =  "expansion1::subworlds/swamp/SwampStart";
        public const string DefaultSwampWarpBiome = "expansion1::subworlds/swamp/SwampWarpStart";
        public const string DefaultForestStartBiome = "expansion1::subworlds/forest/med_ForestStart";
        public const string DefaultForestWarpBiome = "expansion1::subworlds/forest/ForestWarpStart";

        public enum StartAreaType
        {
            Undefined = 0,
            Sandstone = 1,
            Forest = 2,
            Swamp = 3,
        }

        static StartAreaType GetPrimaryWorldCompositionFromWorldAnnotations(ProcGen.World world, bool includeMarsh = false, bool includeBarren = false)
        {
            if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("forest")
                    || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("forest")
                    || world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("forest"))
            {
                return StartAreaType.Forest;
            }

            if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("marsh")
            || world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("swamp")
            || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("marsh")
            || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains(value: "swamp")
            || world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("swamp"))
            {
                return StartAreaType.Swamp;
            }
            return StartAreaType.Undefined;

        }

        static StartAreaType GetPrimaryWorldComposition(ProcGen.World world)
        {
            var type = GetPrimaryWorldCompositionFromPredefined(world);

            if (type == StartAreaType.Undefined)
                type = GetPrimaryWorldCompositionFromBiomes(world);
            if (type == StartAreaType.Undefined)
                type = GetPrimaryWorldCompositionFromWorldAnnotations(world);

#if DEBUG
            if (type == StartAreaType.Undefined || type == StartAreaType.Sandstone)
                SgtLogger.l("Winning StartType: Sandstone", world.filePath + "," + Strings.Get(world.name));
            if ( type == StartAreaType.Forest)
                SgtLogger.l("Winning StartType: Forest", world.filePath + "," + Strings.Get(world.name));
            if (type == StartAreaType.Swamp)
                SgtLogger.l("Winning StartType: Swamp", world.filePath + "," + Strings.Get(world.name));
#endif

            return type;
        }

        static StartAreaType GetPrimaryWorldCompositionFromPredefined(ProcGen.World world)
        {
            if (PredefinedWorldCompositionOverrides.ContainsKey(world.filePath))
            {
                return (StartAreaType)PredefinedWorldCompositionOverrides[world.filePath];
            }

            return StartAreaType.Undefined;
        }
        static StartAreaType GetPrimaryWorldCompositionFromBiomes(ProcGen.World world, bool includeMarsh = false, bool includeBarren = false)
        {
            float swampBiomes = 0, forestBiomes = 0, sandstoneBiomes = 0;
            if (world.subworldFiles != null && world.subworldFiles.Count > 0)
            {
                foreach (ProcGen.WeightedSubworldName subworld in world.subworldFiles)
                {
                    if (subworld.name != null)
                    {
                        if (subworld.name.ToLowerInvariant().Contains("forest"))
                        {
                            forestBiomes++;
                            //if(subworld.minCount>1)
                            //    forestBiomes += subworld.minCount-1;

                            //if (subworld.maxCount!= int.MaxValue)
                            //    forestBiomes += subworld.maxCount;

                        }
                        if (subworld.name.ToLowerInvariant().Contains("swamp") || (includeMarsh && subworld.name.ToLowerInvariant().Contains("marsh")))
                        {
                            swampBiomes++;
                            //if (subworld.minCount > 1)
                            //    swampBiomes += subworld.minCount - 1;

                            //if (subworld.maxCount != int.MaxValue)
                            //    swampBiomes += subworld.maxCount;
                        }
                        if (subworld.name.ToLowerInvariant().Contains("sandstone") || (includeBarren && subworld.name.ToLowerInvariant().Contains("barren")))
                        {
                            sandstoneBiomes++;
                            //if (subworld.minCount > 1)
                            //    sandstoneBiomes += subworld.minCount - 1;

                            //if (subworld.maxCount != int.MaxValue)
                            //    sandstoneBiomes += subworld.maxCount;
                        }
                    }
                }
            }
            if (swampBiomes > 0 || forestBiomes > 0 || sandstoneBiomes > 0)
            {
#if DEBUG
                SgtLogger.l($"SandstoneBiomes: {sandstoneBiomes}, ForestBiomes: {forestBiomes}, SwampBiomes: {swampBiomes}", world.filePath+","+Strings.Get(world.name));
#endif
                if (forestBiomes >= swampBiomes && forestBiomes >= sandstoneBiomes)
                {
                    return StartAreaType.Forest;
                }
                if (swampBiomes >= forestBiomes && swampBiomes >= sandstoneBiomes)
                {
                    return StartAreaType.Swamp;
                }
                if (sandstoneBiomes >= swampBiomes && sandstoneBiomes >= forestBiomes)
                {
                    return StartAreaType.Sandstone;
                }
            }
            return StartAreaType.Undefined;
        }

        public static string GetStartAreaSubworld(ProcGen.World world, bool Warp)
        {
            if (world != null)
            {
                if (world.defaultsOverrides != null && world.defaultsOverrides.data != null && world.defaultsOverrides.data.Count > 0)
                {
                    if (!Warp)
                    {
                        if (world.defaultsOverrides.data.TryGetValue(StartWorld_StartingAreaSubworld_Key, out var SubworldOverride) && SubworldOverride is string)
                        {
                            return SubworldOverride as string;
                        }
                    }
                    else
                    {
                        if (world.defaultsOverrides.data.TryGetValue(WarpWorld_StartingAreaSubworld_Key, out var SubworldOverride) && SubworldOverride is string)
                        {
                            return SubworldOverride as string;
                        }
                    }
                }
                StartAreaType type;
                type = GetPrimaryWorldComposition(world);


                if (type != StartAreaType.Undefined)
                {
                    switch (type)
                    {
                        case StartAreaType.Swamp:
                            return Warp ? DefaultSwampWarpBiome : DefaultSwampStartBiome;
                        case StartAreaType.Forest:
                            return Warp ? DefaultForestWarpBiome : DefaultForestStartBiome;
                        case StartAreaType.Sandstone:
                        case StartAreaType.Undefined:
                            return Warp ? DefaultSandstoneWarpBiome : DefaultSandstoneStartBiome;
                    }
                }
            }
            else
                SgtLogger.warning("world was null");

#if DEBUG
            SgtLogger.l("defaulting...");
            SgtLogger.l(Warp ? "Sandstone Warp Biome" : "Sandstone Start Biome");
#endif
            return Warp ? DefaultSandstoneWarpBiome : DefaultSandstoneStartBiome; //default
        }



        public const string DefaultSandstoneWater = "expansion1::subworlds/sandstone/SandstoneMiniWater";
        public const string DefaultSwampWater = "expansion1::subworlds/swamp/SwampMini";
        public const string DefaultForestWater = "subworlds/forest/ForestMiniWater";

        public static string GetStartAreaWaterSubworld(ProcGen.World world)
        {
            if (world != null)
            {
                if (world.defaultsOverrides != null && world.defaultsOverrides.data != null && world.defaultsOverrides.data.Count > 0)
                {
                    if (world.defaultsOverrides.data.TryGetValue(StartWorld_StartingAreaWaterSubworld_Key, out var SubworldOverride) && SubworldOverride is string)
                    {
                        return SubworldOverride as string;
                    }
                }

                var type = GetPrimaryWorldComposition(world);


                if (type != StartAreaType.Undefined)
                {
                    switch(type)
                    {
                        case StartAreaType.Swamp:
                            return DefaultSwampWater;
                        case StartAreaType.Forest:  
                            return DefaultForestWater;
                        case StartAreaType.Sandstone:
                        case StartAreaType.Undefined:
                            return DefaultForestWater;
                    }
                }
            }
            else
                SgtLogger.warning("world was null");

#if DEBUG
            SgtLogger.l("Sandstone Water Source");
#endif
            return DefaultSandstoneWater; //default
        }

        public const string DefaultSandstoneStartBase = "bases/sandstoneBase";
        public const string DefaultSandstoneWarpBase = "expansion1::bases/warpworldSandstoneBase";
        public const string DefaultSwampStartBase = "expansion1::bases/swampBase";
        public const string DefaultSwampWarpBase = "expansion1::bases/warpworldSwampBase";
        public const string DefaultForestStartBase = "bases/forestBase";
        public const string DefaultForestWarpBase = "expansion1::bases/warpworldForestBase";

        public static string GetStarterBaseTemplate(ProcGen.World world, bool Warp)
        {
            if (world != null)
            {
                if (world.defaultsOverrides != null && world.defaultsOverrides.data != null && world.defaultsOverrides.data.Count > 0)
                {
                    if (!Warp)
                    {
                        if (world.defaultsOverrides.data.TryGetValue(StartWorld_Base_Key, out var SubworldOverride) && SubworldOverride is string)
                        {
                            return SubworldOverride as string;
                        }
                    }
                    else
                    {
                        if (world.defaultsOverrides.data.TryGetValue(WarpWorld_Base_Key, out var SubworldOverride) && SubworldOverride is string)
                        {
                            return SubworldOverride as string;
                        }
                    }
                }


                var type = GetPrimaryWorldComposition(world);


                if (type != StartAreaType.Undefined)
                {
                    switch (type)
                    {
                        case StartAreaType.Swamp:
                            return Warp ? DefaultSwampWarpBase : DefaultSwampStartBase;
                        case StartAreaType.Forest:
                            return Warp ? DefaultForestWarpBase : DefaultForestStartBase;
                        case StartAreaType.Sandstone:
                        case StartAreaType.Undefined:
                            return Warp ? DefaultSandstoneWarpBase : DefaultSandstoneStartBase;
                    }
                }
            }
            else
                SgtLogger.warning("world was null");

#if DEBUG
            SgtLogger.l(Warp ? "Sandstone Warp Base" : "Sandstone Start Base");
#endif
            return !Warp ? DefaultSandstoneStartBase : DefaultSandstoneWarpBase; //default
        }
    }
}
