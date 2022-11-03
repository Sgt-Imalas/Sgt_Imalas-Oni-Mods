using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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


        public WorldContainer CreateSpaceStationInteriorWorld(
            GameObject craft_go,
            string interiorTemplateName,
            Vector2I spaceStationInteriorSize,
            System.Action callback)
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
                Debug.Log((object)string.Format("Created new space station interior, id: {0}, at {1} with size {2}", (object)nextWorldId, (object)offset, (object)spaceStationInteriorSize ));
                spaceStationInteriorWorld.PlaceInteriorTemplate(interiorTemplateName, (System.Action)(() =>
                {
                    ///On StationCompleteAction idk
                    if (callback != null)
                        callback();
                }));
                craft_go.AddOrGet<OrbitalMechanics>().CreateOrbitalObject(Db.Get().OrbitalTypeCategories.orbit.Id);
                ClusterManager.Instance.Trigger((int)GameHashes.WorldAdded, (object)spaceStationInteriorWorld.id);
                spaceStationInteriorWorld.AddTag(ModAssets.Tags.IsSpaceStation);

                return spaceStationInteriorWorld;
            }
            Debug.LogError((object)"Failed to create space station interior.");
            return (WorldContainer)null;
        }

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
                OrbitalMechanics component = gameObject.GetComponent<OrbitalMechanics>();
                if (!component.IsNullOrDestroyed())
                    UnityEngine.Object.Destroy((UnityEngine.Object)component);
                AxialI clusterLocation = world.GetComponent<ClusterGridEntity>().Location;

                world.SpacePodAllDupes(clusterLocation, SimHashes.Cuprite);
                world.CancelChores();
                HashSet<int> noRefundTiles;
                world.DestroyWorldBuildings(out noRefundTiles);
                ClusterManager.Instance.UnregisterWorldContainer(world);
                
                GameScheduler.Instance.ScheduleNextFrame("ClusterManager.world.TransferResourcesToDebris", (System.Action<object>)(obj => world.TransferResourcesToDebris(clusterLocation, noRefundTiles, SimHashes.Cuprite)));
                GameScheduler.Instance.ScheduleNextFrame("ClusterManager.DeleteWorldObjects", (System.Action<object>)(obj => DeleteWorldObjects(world)));
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
            Grid.FreeGridSpace(world.WorldSize, world.WorldOffset);
            WorldInventory worldInventory = (WorldInventory)null;
            if ((UnityEngine.Object)world != (UnityEngine.Object)null)
                worldInventory = world.GetComponent<WorldInventory>();
            if ((UnityEngine.Object)worldInventory != (UnityEngine.Object)null)
                UnityEngine.Object.Destroy((UnityEngine.Object)worldInventory);
            if (!((UnityEngine.Object)world != (UnityEngine.Object)null))
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object)world);
        }

    }
}
