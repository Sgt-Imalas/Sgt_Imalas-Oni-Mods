using Delaunay.Geo;
using Klei;
using ProcGen;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VoronoiTree;
using static Klei.WorldDetailSave;
using static ProcGen.Mob;
using static ProcGen.SubWorld;

namespace Starmap_Shenanigans
{
    internal class Yggdrasil
    {
        public static bool shouldIncreaseGrid = false;
        public static bool UpdateProgress(StringKey stringKeyRoot, float completePercent, WorldGenProgressStages.Stages stage)
        {
            return true;
        }
        public static void OnError(OfflineWorldGen.ErrorInfo errorInfo)
        {
            Debug.LogError($"Yggdrasil: WorldGen Error: {errorInfo.errorDesc} - {errorInfo.exception.Message}");
        }


        //parameter is any Key in the SettingsCache.worlds.worldCache dictionary which houses all loaded world files
        //these keys happen to be the same as the world filepaths such as "expansion1::worlds/NiobiumMoonlet"
        public static void CreateAsteroid(string worldCacheKey, AxialI axialI, int seed = -1)
        {

            if (!SpeedControlScreen.Instance.IsPaused)
                SpeedControlScreen.Instance.Pause();
            
            var world = SettingsCache.worlds.worldCache[worldCacheKey];              
            var worldGen = new WorldGen(world.filePath, null, null, false);            
            var worldSize = world.worldsize;


            var gridSize = BestFit.GetGridOffset(ClusterManager.Instance.WorldContainers, worldSize, out Vector2I worldOffset);
            if (gridSize.y > Grid.HeightInCells)
            {
                Debug.LogWarning("Yggdrasil: Not enough space to create world.\nIncrease Grid on next load.");
                shouldIncreaseGrid = true;
                return;
            }
            SimMessages.SimDataResizeGridAndInitializeVacuumCells(gridSize, worldSize.x, worldSize.y, worldOffset.x, worldOffset.y);

            var worldId = ClusterManager.Instance.GetNextWorldId();
            GameObject gameObject = Util.KInstantiate(Assets.GetPrefab((Tag)"Asteroid"));
            var worldContainer = gameObject.GetComponent<WorldContainer>();
            worldContainer.SetID(worldId);
            worldContainer.fullyEnclosedBorder = true;
            worldContainer.worldOffset = worldOffset;
            worldContainer.worldSize = worldSize;
            worldContainer.isDiscovered = true;
            worldContainer.isModuleInterior = false;
            worldContainer.m_seasonIds = new List<string>();//world.seasons; probably need to start gameplay events manually for meteor showers to work, left blank for now
            worldContainer.worldDescription = world.description;



            gameObject.GetComponent<AsteroidGridEntity>().m_name = worldContainer.GetRandomName();
            gameObject.GetComponent<AsteroidGridEntity>().m_asteroidAnim = worldGen.Settings.world.asteroidIcon;

            //Debug.Log($"Yggdrasil: Creating world {world.filePath} {worldId} at offset {worldOffset}");

            Vector2I vector2I = worldOffset + worldSize;
            for (int y = worldOffset.y; y < vector2I.y; ++y)
            {
                for (int x = worldOffset.x; x < vector2I.x; ++x)
                {
                    int cell = Grid.XYToCell(x, y);
                    Grid.WorldIdx[cell] = (byte)worldId;
                    Pathfinding.Instance.AddDirtyNavGridCell(cell);
                }
            }

            gameObject.GetComponent<AsteroidGridEntity>().Location = axialI;
            gameObject.SetActive(true);
            ClusterManager.Instance.BoxingTrigger((int)GameHashes.WorldAdded, worldId);

            

            if (world.filePath == "expansion1::worlds/StrangeAsteroidKleiFest2023Cluster")
                seed = 7;
            worldGen.Initialise(UpdateProgress, OnError, seed, seed, seed, seed);
            worldGen.SetWorldSize(worldSize.x, worldSize.y);
            worldGen.GenerateOffline();


            List<WorldTrait> placedStoryTraits = new List<WorldTrait>();
            
            WorldgenSimData simData = new WorldgenSimData();
            
            RenderOnline(worldGen, ref simData, ref placedStoryTraits, worldId);//in lieu of worldGen.RenderOffline



            var newOverworldCells = new List<OverworldCell>();
            foreach (var overworldCell in worldGen.OverworldCells)
            {
                var poly = overworldCell.poly;
                var newVerts = new List<Vector2>();
                foreach (var vertex in poly.Vertices)
                {
                    newVerts.Add(new Vector2(vertex.x + worldOffset.x, vertex.y + worldOffset.y));
                }
                Polygon newPoly = new Polygon(newVerts);
                var newOverworldCell = new OverworldCell(SettingsCache.GetCachedSubWorld(overworldCell.node.type).zoneType, overworldCell);
                newOverworldCell.poly = newPoly;
                SaveLoader.Instance.clusterDetailSave.overworldCells.Add(newOverworldCell);
                newOverworldCells.Add(newOverworldCell);
            }

            for (int i = 0; i < cells.Length; i++)
            {
                int cell = GetCellFromSubGridCell(i, worldSize.x, worldOffset);
                var simCell = cells[i];
                var diseaseCell = dc[i];
                SimMessages.ModifyCell(cell, simCell.elementIdx, simCell.temperature, simCell.mass, diseaseCell.diseaseIdx, diseaseCell.elementCount, SimMessages.ReplaceType.Replace);

                foreach (var overworldCell in newOverworldCells)
                {
                    if (overworldCell.poly.Contains(GetPOSFromSubGridCell(i, worldSize.x, worldOffset)))
                    {
                        var zoneType = overworldCell.zoneType;
                        byte zoneId = zoneType == SubWorld.ZoneType.Space ? byte.MaxValue : (byte)zoneType;
                        SimMessages.ModifyCellWorldZone(cell, zoneId);
                        World.Instance.zoneRenderData.worldZoneTypes[cell] = zoneType;
                        break;
                    }
                }
            }

            foreach (var poiSpawner in worldGen.POISpawners)
            {
                TemplateLoader.Stamp(poiSpawner.container, new Vector2(worldOffset.x + poiSpawner.position.x, worldOffset.y + poiSpawner.position.y), () => { });
            }

            foreach (var terrainCell in worldGen.data.terrainCells)
            {
                if (terrainCell.HasMobs)
                {
                    foreach (var mob in terrainCell.mobs)
                    {
                        Vector2 pos2 = GetPOSFromSubGridCell(mob.Key, worldSize.x, worldOffset);
                        var pos = new Vector3(pos2.x + 0.5f, pos2.y, Grid.GetLayerZ(Grid.SceneLayer.Creatures));
                        var go = Util.KInstantiate(Assets.GetPrefab(mob.Value), pos);

                        //necessary to delay a frame so that plants don't die immediately instead of remaining planted
                        GameScheduler.Instance.ScheduleNextFrame("SpawnMob", (object obj) =>
                        {
                            var spawnedMob = (GameObject)obj;
                            spawnedMob.SetActive(true);                            
                        }, go);
                    }
                }
            }
            //step to next frame to spawn all mobs
            SpeedControlScreen.Instance.DebugStepFrame();

        }

        // WorldGen.RenderOffline assumes the grid size is equal to the world size, so we have to temporarily resize the grid
        internal static void RenderOnline(WorldGen worldGen, ref WorldgenSimData worldgenSimData, ref List<WorldTrait> placedStoryTraits, int worldId)
        {
            var gridBackup = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
            var worldSize = worldGen.GetSize();
            Grid.WidthInCells = worldSize.x;
            Grid.HeightInCells = worldSize.y;
            Grid.CellCount = worldSize.x * worldSize.y;
        
            HashSet<int> borderCells = new HashSet<int>();
            worldGen.POIBounds = new List<RectInt>();
            worldGen.WriteOverWorldNoise(worldGen.successCallbackFn);
            worldGen.RenderToMap(worldGen.successCallbackFn, ref worldgenSimData, ref borderCells, ref worldGen.POIBounds);
            foreach (int key in borderCells)
            {
                cells[key].SetValues(WorldGen.unobtaniumElement, ElementLoader.elements);
                worldGen.claimedPOICells[key] = 1;
            }
            worldGen.POISpawners = TemplateSpawning.DetermineTemplatesForWorld(worldGen.Settings, worldGen.data.terrainCells, worldGen.myRandom, ref worldGen.POIBounds, worldGen.isRunningDebugGen, ref placedStoryTraits, worldGen.successCallbackFn);
            worldGen.SpawnMobsAndTemplates(worldId, ref worldgenSimData, new HashSet<int>(worldGen.claimedPOICells.Keys));


            Grid.WidthInCells = gridBackup.x;
            Grid.HeightInCells = gridBackup.y;
            Grid.CellCount = gridBackup.x * gridBackup.y;
        }

        internal static int GetCellFromSubGridCell(int subGridCell, int subGridWidth, Vector2I worldOffset)
        {
            int subGridX = subGridCell % subGridWidth;
            int subGridY = subGridCell / subGridWidth;
            int x = worldOffset.x + subGridX;
            int y = worldOffset.y + subGridY;
            return Grid.XYToCell(x, y);
        }
        internal static Vector2 GetPOSFromSubGridCell(int subGridCell, int subGridWidth, Vector2I worldOffset)
        {
            int subGridX = subGridCell % subGridWidth;
            int subGridY = subGridCell / subGridWidth;
            float x = worldOffset.x + subGridX;
            float y = worldOffset.y + subGridY;
            return new Vector2(x, y);
        }
    }
}
