using Imalas_TwitchChaosEvents.BeeGeyser;
using ONITwitchLib;
using ONITwitchLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util_TwitchIntegrationLib;
using UtilLibs;
using static Grid;
using static STRINGS.ELEMENTS;
using static STRINGS.WORLD_TRAITS;

namespace Imalas_TwitchChaosEvents.Events
{
    internal class BeeVolcanoEvent : ITwitchEventBase
    {
        public string ID => "ChaosTwitch_BEEVolcano";

        public string EventGroupID => null;

        public string EventName => STRINGS.CHAOSEVENTS.BEEVOLCANO.NAME;

        public EventWeight EventWeight => EventWeight.WEIGHT_NORMAL;

        private OccupyArea prefabOccupyArea;
        public Action<object> EventAction => (_) =>
        {
            prefabOccupyArea = Assets.GetPrefab(BeeGeyserConfig.ID).GetComponent<OccupyArea>();


            var worlds = new List<WorldContainer>(ClusterManager.Instance.WorldContainers)
                .Where((world)=> world.IsDiscovered && !world.isModuleInterior)
                .OrderBy((world)=> Components.LiveMinionIdentities.GetWorldItems(world.id).Count);

            var targetWorld = ClusterManager.Instance.activeWorld.IsModuleInterior? ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorld.ParentWorldId) : ClusterManager.Instance.activeWorld;

            SgtLogger.l("world found for beeser: " + targetWorld.name);
            List<Room> rooms = new List<Room>();
            List<CavityInfo> backupCavities= new List<CavityInfo>();
            foreach (Room room in Game.Instance.roomProber.rooms)
            {
                var middle = Grid.PosToCell(room.cavity.GetCenter());

                if (!Grid.IsVisible(middle) || !Grid.IsValidCellInWorld(middle, targetWorld.id))
                {
                    continue;
                }

                rooms.Add(room);
            }

            foreach (var  RoomCavity in rooms)
            {
                SgtLogger.l("checking room: " + (RoomCavity.cavity.maxX - RoomCavity.cavity.minX)+","+ (RoomCavity.cavity.maxY - RoomCavity.cavity.minY));
                var cavity = RoomCavity.cavity;

                if (cavity.maxX - cavity.minX < 2)
                {
                    SgtLogger.l("too thin");
                    continue;
                }

                var height = cavity.maxY - cavity.minY;

                if (height < 4)
                {
                    SgtLogger.l("too small");
                    continue;
                }

                var cell = GetValidPlacementInCavity(cavity);

                if (cell == -1)
                {
                    SgtLogger.l("no cell found");
                    continue;
                }


                var go =GameUtil.KInstantiate(global::Assets.GetPrefab(BeeGeyserConfig.ID), CellToPos(cell), Grid.SceneLayer.Building);
                go.SetActive(true);

                ONITwitchLib.ToastManager.InstantiateToastWithGoTarget(
                    STRINGS.CHAOSEVENTS.BEEVOLCANO.TOAST,
                    STRINGS.CHAOSEVENTS.BEEVOLCANO.TOASTTEXT,
                    go);


                return;
            }
            //if no room was found
            foreach (var cavity in Game.Instance.roomProber.cavityInfos)
            {
                var middle = Grid.PosToCell(cavity.GetCenter());

                if (!Grid.IsVisible(middle) || !Grid.IsValidCellInWorld(middle, targetWorld.id))
                {
                    continue;
                }

                backupCavities.Add(cavity);
            }

            foreach (var cavity in backupCavities)
            {
                SgtLogger.l("checking cavity: " + cavity.room.GetProperName());

                if (cavity.maxX - cavity.minX < 2)
                {
                    SgtLogger.l("too thin");
                    continue;
                }

                var height = cavity.maxY - cavity.minY;

                if (height < 4)
                {
                    SgtLogger.l("too small");
                    continue;
                }

                var cell = GetValidPlacementInCavity(cavity);

                if (cell == -1)
                {
                    SgtLogger.l("no cell found");
                    continue;
                }


                var go = GameUtil.KInstantiate(global::Assets.GetPrefab(BeeGeyserConfig.ID), CellToPos(cell), Grid.SceneLayer.Building);
                go.SetActive(true);

                ONITwitchLib.ToastManager.InstantiateToastWithGoTarget(
                    STRINGS.CHAOSEVENTS.BEEVOLCANO.TOAST,
                    STRINGS.CHAOSEVENTS.BEEVOLCANO.TOASTTEXT,
                    go);


                return;
            }


        };
        
        private int GetValidPlacementInCavity(CavityInfo cavity)
        {
            var minX = cavity.minX ; // no need to check up against wall
            var maxX = cavity.maxX - 1;
            var minY = cavity.maxY-3;
            var maxY = cavity.maxY-3;

            for (var x = minX; x <= maxX; x++)
            {
                for (var y = maxY; y >= minY; y--)
                {
                    var cell = Grid.XYToCell(x, y);
                    if (prefabOccupyArea.TestArea(cell, null, (cell, _) => Grid.IsValidCell(cell) && !Grid.Solid[cell])
                        && prefabOccupyArea.CanOccupyArea(cell, ObjectLayer.Building))
                    {
                        return cell;
                    }
                }
            }

            return -1;
        }

        public Func<object, bool> Condition => (_) =>  GameClock.Instance.GetCycle() > 50;

        public Danger EventDanger => Danger.High;
    }
}
