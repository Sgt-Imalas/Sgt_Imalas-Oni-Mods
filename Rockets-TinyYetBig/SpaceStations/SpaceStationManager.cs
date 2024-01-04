using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStationManager : KMonoBehaviour
    {

        private static readonly Lazy<SpaceStationManager> lazy =
        new Lazy<SpaceStationManager>(() => new SpaceStationManager());

        public static SpaceStationManager Instance { get { return lazy.Value; } }


        public static bool ActiveWorldIsSpaceStationInterior() => ClusterManager.Instance.activeWorld.HasTag(ModAssets.Tags.IsSpaceStation);
        public static bool ActiveWorldIsRocketInterior() => !ClusterManager.Instance.activeWorld.HasTag(ModAssets.Tags.IsSpaceStation) && ClusterManager.Instance.activeWorld.IsModuleInterior;
        public static bool WorldIsRocketInterior(int worldId) => !ClusterManager.Instance.GetWorld(worldId).HasTag(ModAssets.Tags.IsSpaceStation) && ClusterManager.Instance.GetWorld(worldId).IsModuleInterior;
        public static bool WorldIsSpaceStationInterior(int worldId) => ClusterManager.Instance.GetWorld(worldId).HasTag(ModAssets.Tags.IsSpaceStation) && ClusterManager.Instance.GetWorld(worldId).IsModuleInterior;

        public SpaceStation GetSpaceStationFromWorldId(int worldId) 
        { 
            if(ClusterManager.Instance.GetWorld(worldId).TryGetComponent<SpaceStation>(out var component))
            {
                return component;

            }
            return null;
        }

        public HashSet<int> SpaceStationWorlds = new HashSet<int>();

        public SpaceStationManager()
        {
            foreach(var World in ClusterManager.Instance.GetWorldIDsSorted())
            {
                if (WorldIsSpaceStationInterior(World))
                {
                    SpaceStationWorlds.Add(World);
                }
            }
        }

        public static bool GetRockets(out List<Clustercraft> rockets)
        {
            rockets = new List<Clustercraft>();
            foreach(Clustercraft potRocket in Components.Clustercrafts)
            {
                if(!potRocket.TryGetComponent<SpaceStation>(out var s))
                {
                    rockets.Add(potRocket);
                }
            }
            return rockets.Count > 0;
        }

        internal bool CanConstructSpaceStation()
        {
            return SpaceStationWorlds.Count < 10;
        }

        public WorldContainer CreateSpaceStationInteriorWorld(
            GameObject craft_go,
            string interiorTemplateName,
            Vector2I spaceStationInteriorSize,
            bool allowBuilding,
            System.Action callback,
            AxialI Coordinates, bool isDerelict = false)
        {
            Vector2I offset;
            if (Grid.GetFreeGridSpace(spaceStationInteriorSize, out offset))
            {
                int nextWorldId = this.GetNextWorldId();
                craft_go.AddComponent<WorldInventory>();
                WorldContainer spaceStationInteriorWorld = craft_go.AddComponent<WorldContainer>();
                spaceStationInteriorWorld.SetRocketInteriorWorldDetails(nextWorldId, spaceStationInteriorSize, offset);
                Vector2I vector2I = offset + spaceStationInteriorSize;
                for (int y = offset.y; y < vector2I.y; ++y)
                {
                    for (int x = offset.x; x < vector2I.x; ++x)
                    {
                        int cell = Grid.XYToCell(x, y);
                        Grid.WorldIdx[cell] = (byte)nextWorldId;
                        Pathfinding.Instance.AddDirtyNavGridCell(cell);
                    }
                }
                SgtLogger.debuglog(string.Format("Created new space station interior, id: {0}, at {1} with size {2}", (object)nextWorldId, (object)offset, (object)spaceStationInteriorSize ));
                spaceStationInteriorWorld.PlaceInteriorTemplate(interiorTemplateName, (System.Action)(() =>
                {
                    ///On StationCompleteAction idk
                    if (callback != null)
                        callback();
                }));
                craft_go.AddOrGet<OrbitalMechanics>().CreateOrbitalObject(Db.Get().OrbitalTypeCategories.orbit.Id);
                ClusterManager.Instance.Trigger((int)GameHashes.WorldAdded, (object)spaceStationInteriorWorld.id);
                spaceStationInteriorWorld.AddTag(ModAssets.Tags.IsSpaceStation);

                if(!allowBuilding)
                    spaceStationInteriorWorld.AddTag(ModAssets.Tags.NoBuildingAllowed);


                if (isDerelict)
                    spaceStationInteriorWorld.AddTag(ModAssets.Tags.IsDerelict);
                else
                    SpaceStationWorlds.Add(spaceStationInteriorWorld.id);

                    int maxRings = ClusterGrid.Instance.numRings;
                    var distance = GetDistanceFromAxial(Coordinates);
                    spaceStationInteriorWorld.cosmicRadiation = (int)Interpolate(LowEndRads,HighEndRads,3, maxRings,distance);
                    spaceStationInteriorWorld.sunlight = (int)Interpolate(LowEndLight, HighEndLight, 3, maxRings, distance); 

                return spaceStationInteriorWorld;

            }
            SgtLogger.error("Failed to create space station interior, no more grid space left");
            return (WorldContainer)null;
        }

        public static int SpaceRadiationRocket(AxialI location) => Instance.SpaceRadiation(location);
        public int SpaceRadiation(AxialI Location)
        {
            int maxRings = ClusterGrid.Instance.numRings;
            var distance = GetDistanceFromAxial(Location);
            return (int)Interpolate(240, 400, 1, maxRings, distance);
        }

        int GetDistanceFromAxial(AxialI coords)
        {
            int a = Math.Abs(coords.Q), b = Math.Abs(coords.R);
            return a > b ? a : b > a ? b : a;
        }
        float Interpolate(float min, float max, float lowEnd, float highEnd, float Value)
        {
            float step = (max - min) / (highEnd - lowEnd);
            step *= Value<lowEnd ? 0 : Value > highEnd ? highEnd - lowEnd : Value - lowEnd;
            
            return min + step;

        }

        const float LowEndLight = 30000f;
        const float HighEndLight = 140000f;
        const int LowEndRads = 200;
        const int HighEndRads = 850;

        public void DestroySpaceStationInteriorWorld(int world_id)
        {
            WorldContainer world = ClusterManager.Instance.GetWorld(world_id);
            if ((UnityEngine.Object)world == (UnityEngine.Object)null || !world.IsModuleInterior)
            {
                Debug.LogError((object)string.Format("Attempting to destroy world id {0}. The world is not a valid rocket interior", (object)world_id));
            }
            else
            {

                if (ClusterManager.Instance.activeWorldId == world_id)
                {
                    ClusterManager.Instance.SetActiveWorld(ClusterManager.Instance.GetStartWorld().id);
                }

                if (world.TryGetComponent<OrbitalMechanics>(out var component))
                    UnityEngine.Object.Destroy((UnityEngine.Object)component);

                SpaceStation station;
                world.TryGetComponent<SpaceStation>(out station);

                AxialI clusterLocation = station.Location;

                world.SpacePodAllDupes(clusterLocation, SimHashes.Cuprite);
                world.CancelChores();
                HashSet<int> noRefundTiles;
                world.DestroyWorldBuildings(out noRefundTiles);
                station.ClearAllBarriers(world, ref noRefundTiles);

                ClusterManager.Instance.UnregisterWorldContainer(world);


                GameScheduler.Instance.ScheduleNextFrame("ClusterManager.world.TransferResourcesToDebris", (System.Action<object>)(obj => world.TransferResourcesToDebris(clusterLocation, noRefundTiles, SimHashes.Cuprite)));
                GameScheduler.Instance.ScheduleNextFrame("ClusterManager.DeleteWorldObjects", (System.Action<object>)(obj => DeleteWorldObjects(world)));
                SpaceStationWorlds.Remove(world_id);
            }
        }

        private int GetNextWorldId()
        {
            HashSetPool<int, ClusterManager>.PooledHashSet pooledHashSet = HashSetPool<int, ClusterManager>.Allocate();
            foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
                pooledHashSet.Add(worldContainer.id);

            for (int nextWorldId = 0; nextWorldId < (int)byte.MaxValue; ++nextWorldId)
            {
                if (!pooledHashSet.Contains(nextWorldId))
                {
                    pooledHashSet.Recycle();
                    return nextWorldId;
                }
            }
            pooledHashSet.Recycle();
            return (int)ClusterManager.INVALID_WORLD_IDX;
        }
        private void DeleteWorldObjects(WorldContainer world)
        {
            ModAssets.FreeGridSpace_Fixed(world.WorldSize, world.WorldOffset);
            WorldInventory worldInventory = (WorldInventory)null;
            if (world != null)
            {
                if (world.TryGetComponent<WorldInventory>(out worldInventory))
                    Destroy(worldInventory);

                Destroy(world);
            }
        }


        public static bool IsSpaceStationAt(AxialI location) => GetSpaceStationWorldIdAtLocation(location) != -1;

        public static bool ValidSpaceStationConstructionLocation(AxialI location)
        {
            foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.cellContents[location])
            {
                if (clusterGridEntity.Layer == EntityLayer.Asteroid || clusterGridEntity.Layer == EntityLayer.POI && !(clusterGridEntity is ResearchDestination))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool GetSpaceStationAtLocation(AxialI location, out SpaceStation station)
        {
            station = null;
            foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.cellContents[location])
            {
                if (clusterGridEntity is SpaceStation)
                {
                    station = clusterGridEntity as SpaceStation;
                    return true;
                }
            }
            return false;
        }

        public static int GetSpaceStationWorldIdAtLocation(AxialI location)
        {
            foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.cellContents[location])
            {
                if (clusterGridEntity is SpaceStation)
                {
                    if (clusterGridEntity.TryGetComponent<WorldContainer>(out var container))
                    {
                        return container.id;
                    }
                }

            }
            return -1;
        }

    }
}
