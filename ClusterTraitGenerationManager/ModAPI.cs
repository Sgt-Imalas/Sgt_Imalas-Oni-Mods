using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
    public static class ModAPI
    {
        /// <summary>
        /// you can put custom data for the following settings in each world template by adding it under that key to defaultsOverride.data to override the default selected templates and biomes for your world
        /// CGM will generate an alternative version of each planet, depending on its origin those are 
        ///     Start Version - Players can start on this planet
        ///     Warp Version - The Teleporters will connect to this planet
        ///     Outer Version - No Teleporter templates are here, only reachable via rocket
        /// The version that is not generated is the default version of your planet, the type of that is determined in CGSMClusterManager.PlanetoidDict()
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
        /// if you use any unusual teleporter templates that dont match the condition checks in the method [ADD THAT], please include them under this key.
        /// this ensures the Outer planet variant does not get a teleporter template placed
        /// </summary>
        public const string TeleporterTemplate_Key = "CGM_TeleporterTemplate_Key";


        public const string DefaultSandstoneStartBiome = "expansion1::subworlds/sandstone/SandstoneStart";
        public const string DefaultSandstoneWarpBiome = "expansion1::subworlds/sandstone/med_SandstoneWarp";
        public const string DefaultSwampStartBiome =  "expansion1::subworlds/swamp/SwampStart";
        public const string DefaultSwampWarpBiome = "expansion1::subworlds/swamp/SwampWarpStart";
        public const string DefaultForestStartBiome = "expansion1::subworlds/forest/med_ForestStart";
        public const string DefaultForestWarpBiome = "expansion1::subworlds/forest/ForestWarpStart";

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

                int swampBiomes = 0, forestBiomes = 0, sandstoneBiomes = 0;
                if(world.subworldFiles!=null && world.subworldFiles.Count > 0)
                {
                    foreach(var subworld in world.subworldFiles)
                    {
                        if (subworld.name != null)
                        {
                            if (subworld.name.ToLowerInvariant().Contains("forest"))
                                ++forestBiomes;
                            if (subworld.name.ToLowerInvariant().Contains("swamp"))
                                ++swampBiomes;
                            if (subworld.name.ToLowerInvariant().Contains("sandstone")|| subworld.name.ToLowerInvariant().Contains("barren"))
                                ++sandstoneBiomes;
                        }
                    }
                }

                if (swampBiomes > 0 || forestBiomes > 0 || sandstoneBiomes > 0)
                {
                    SgtLogger.l($"SandstoneBiomes: {sandstoneBiomes}, ForestBiomes: {forestBiomes}, SwampBiomes: {swampBiomes}");
                    if (sandstoneBiomes>=swampBiomes && sandstoneBiomes >= forestBiomes)
                    {
                        SgtLogger.l(Warp ? "Sandstone Warp Biome" : "Sandstone Start Biome");
                        return Warp ? DefaultSandstoneWarpBiome : DefaultSandstoneStartBiome;
                    }

                    else if (forestBiomes >= swampBiomes && forestBiomes >= sandstoneBiomes)
                    {
                        SgtLogger.l(Warp ? "Forest Warp Biome" : "Forest Start Biome");
                        return Warp ? DefaultForestWarpBiome : DefaultForestStartBiome;
                    }
                    else if (swampBiomes >= forestBiomes && swampBiomes >= sandstoneBiomes)
                    {

                        SgtLogger.l(Warp ? "Swampy Warp Biome" : "Swampy Start Biome");
                        return Warp ? DefaultSwampWarpBiome : DefaultSwampStartBiome;
                    }
                }



                if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("forest") 
                    || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("forest"))
                {
                    SgtLogger.l(Warp ? "Forest Warp Biome" : "Forest Start Biome");
                    return Warp ? DefaultForestWarpBiome : DefaultForestStartBiome;
                }

                if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("marsh") 
                || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("marsh")
                || world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("swamp") 
                || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains(value: "swamp"))
                {
                    return Warp ? DefaultSwampWarpBiome : DefaultSwampStartBiome;
                }
            }
            else
                SgtLogger.warning("world was null");
            SgtLogger.l(Warp ? "Sandstone Warp Biome" : "Sandstone Start Biome");
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

                int swampBiomes = 0, forestBiomes = 0, sandstoneBiomes = 0;
                if (world.subworldFiles != null && world.subworldFiles.Count > 0)
                {
                    foreach (var subworld in world.subworldFiles)
                    {
                        if (subworld.name != null)
                        {
                            if (subworld.name.ToLowerInvariant().Contains("forest"))
                                ++forestBiomes;
                            if (subworld.name.ToLowerInvariant().Contains("swamp"))
                                ++swampBiomes;
                            if (subworld.name.ToLowerInvariant().Contains("sandstone") || subworld.name.ToLowerInvariant().Contains("barren"))
                                ++sandstoneBiomes;
                        }
                    }
                }
                if (swampBiomes > 0 || forestBiomes > 0 || sandstoneBiomes > 0)
                {
                    SgtLogger.l($"SandstoneBiomes: {sandstoneBiomes}, ForestBiomes: {forestBiomes}, SwampBiomes: {swampBiomes}");
                    if (sandstoneBiomes >= swampBiomes && sandstoneBiomes >= forestBiomes)
                    {
                        SgtLogger.l("Sandstone Water Biome");
                        return DefaultSandstoneWater;
                    }

                    else if (forestBiomes >= swampBiomes && forestBiomes >= sandstoneBiomes)
                    {
                        SgtLogger.l( "Forest Water Biome");
                        return DefaultForestWater;
                    }
                    else if (swampBiomes >= forestBiomes && swampBiomes >= sandstoneBiomes)
                    {

                        SgtLogger.l("Swampy Water Biome");
                        return  DefaultSwampWater;
                    }
                }



                if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("forest") 
                    || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("forest"))
                {
                    SgtLogger.l("Forest Water Source");
                    return DefaultForestWater;
                }
                if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("marsh") 
                    || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("marsh")
                || world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("swamp") 
                || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains(value: "swamp"))
                {
                    SgtLogger.l("Swampy Water Source");
                    return DefaultSwampWater;
                }
            }
            else
                SgtLogger.warning("world was null");

            SgtLogger.l("Sandstone Water Source");
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

                int swampBiomes = 0, forestBiomes = 0, sandstoneBiomes = 0;
                if (world.subworldFiles != null && world.subworldFiles.Count > 0)
                {
                    foreach (var subworld in world.subworldFiles)
                    {
                        if (subworld.name != null)
                        {
                            if (subworld.name.ToLowerInvariant().Contains("forest"))
                                ++forestBiomes;
                            if (subworld.name.ToLowerInvariant().Contains("swamp"))
                                ++swampBiomes;
                            if (subworld.name.ToLowerInvariant().Contains("sandstone") || subworld.name.ToLowerInvariant().Contains("barren"))
                                ++sandstoneBiomes;
                        }
                    }
                }
                if (swampBiomes > 0 || forestBiomes > 0 || sandstoneBiomes > 0)
                {
                    SgtLogger.l($"SandstoneBiomes: {sandstoneBiomes}, ForestBiomes: {forestBiomes}, SwampBiomes: {swampBiomes}");
                    if (sandstoneBiomes >= swampBiomes && sandstoneBiomes >= forestBiomes)
                    {
                        SgtLogger.l(Warp ? "Sandstone Warp Biome" : "Sandstone Start Biome");
                        return Warp ? DefaultSandstoneWarpBase : DefaultSandstoneStartBase;
                    }

                    else if (forestBiomes >= swampBiomes && forestBiomes >= sandstoneBiomes)
                    {
                        SgtLogger.l(Warp ? "Forest Warp Biome" : "Forest Start Biome");
                        return Warp ? DefaultForestWarpBase : DefaultForestStartBase;
                    }
                    else if (swampBiomes >= forestBiomes && swampBiomes >= sandstoneBiomes)
                    {

                        SgtLogger.l(Warp ? "Swampy Warp Biome" : "Swampy Start Biome");
                        return Warp ? DefaultSwampWarpBase : DefaultSwampStartBase;
                    }
                }



                if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("forest") || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("forest"))
                {
                    SgtLogger.l(Warp ? "Forest Warp Base" : "Forest Start Base");
                    return !Warp ? DefaultForestStartBase : DefaultForestWarpBase;
                }

                if (world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("marsh") || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains("marsh")
                    || world.startSubworldName != null && world.startSubworldName.ToLowerInvariant().Contains("swamp") || world.asteroidIcon != null && world.asteroidIcon.ToLowerInvariant().Contains(value: "swamp"))
                {
                    SgtLogger.l(Warp ? "Swamp Warp Base" : "Swamp Start Base");
                    return !Warp ? DefaultSwampStartBase : DefaultSwampWarpBase;
                }
            }
            else
                SgtLogger.warning("world was null");

            SgtLogger.l(Warp ? "Sandstone Warp Base" : "Sandstone Start Base");
            return !Warp ? DefaultSandstoneStartBase : DefaultSandstoneWarpBase; //default
        }
    }
}
