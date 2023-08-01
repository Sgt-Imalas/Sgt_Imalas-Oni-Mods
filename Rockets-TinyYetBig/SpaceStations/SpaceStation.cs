using KSerialization;
using Rockets_TinyYetBig.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.ModAssets;
using static Rockets_TinyYetBig.STRINGS.UI_MOD.CLUSTERMAPROCKETSIDESCREEN;

namespace Rockets_TinyYetBig.SpaceStations
{
    class SpaceStation : Clustercraft, ISim4000ms
    {
        public Vector2I StationInteriorMaxedSize = new Vector2I(102, 103);

        [Serialize]
        private string m_name = "Space Station";

        [Serialize]
        public int SpaceStationInteriorId = -1;

        [Serialize]
        public int _currentSpaceStationType = 0;

        public SpaceStationWithStats CurrentSpaceStationType => ModAssets.SpaceStationTypes[_currentSpaceStationType];

        [Serialize]
        public int IsOrbitalSpaceStationWorldId = -1;
        [Serialize]
        public bool IsDeconstructable = true;
        [Serialize]
        public bool BuildableInterior = true;


        [Serialize]
        public bool HasOrbitUpkeep = true;
        [Serialize]
        public bool Upgradeable = true;
        [Serialize]
        public int spaceStationLevelUnlock = 0;


        public Vector2I InteriorSize = new Vector2I(30, 30);
        public string InteriorTemplate = "emptySpacefor100";

        public string ClusterAnimName = "space_station_small_kanim";
        //public string IconAnimName = "station_3";

        public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
        {
            new AnimConfig
            {
                animFile = Assets.GetAnim("space_station_small_kanim"),
                initialAnim = "idle_loop"
            }
        };

        public override string Name => this.m_name;
        //public override bool IsVisible => true;
        public override EntityLayer Layer => EntityLayer.Craft;
        public override bool SpaceOutInSameHex() => true;
        public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;
        //public override Sprite GetUISprite() => Assets.GetSprite("rocket_landing"); //Def.GetUISprite((object)this.gameObject).first;
        public override Sprite GetUISprite()
        {
            return Def.GetUISpriteFromMultiObjectAnim(AnimConfigs[0].animFile);
        }

        //public Sprite GetUISpriteAt(int i) => Def.GetUISpriteFromMultiObjectAnim(AnimConfigs[i].animFile);
        public override void OnSpawn()
        {
            //SgtLogger.debuglog("MY WorldID:" + SpaceStationInteriorId);
            if (SpaceStationInteriorId < 0)
            {
                var interiorWorld = SpaceStationManager.Instance.CreateSpaceStationInteriorWorld(gameObject, "interiors/" + InteriorTemplate, StationInteriorMaxedSize, BuildableInterior, null, Location);
                SpaceStationInteriorId = interiorWorld.id;
                SgtLogger.debuglog("new WorldID:" + SpaceStationInteriorId);
                SgtLogger.debuglog("ADDED NEW SPACE STATION INTERIOR");
            }
            base.OnSpawn();
            ClusterManager.Instance.GetWorld(SpaceStationInteriorId).AddTag(ModAssets.Tags.IsSpaceStation);


            this.SetCraftStatus(CraftStatus.InFlight);
            var destinationSelector = gameObject.GetComponent<RocketClusterDestinationSelector>();
            destinationSelector.SetDestination(this.Location);
            var planet = ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(this.Location, EntityLayer.Asteroid);
            if (planet != null)
            {
                IsOrbitalSpaceStationWorldId = planet.GetComponent<WorldContainer>().id;
            }

            var m_clusterTraveler = gameObject.GetComponent<ClusterTraveler>();
            m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
            m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
            m_clusterTraveler.onTravelCB = (System.Action)null;
            m_clusterTraveler.validateTravelCB = null;


            this.Subscribe<SpaceStation>(1102426921, NameChangedHandler);
            DrawBarriers();
        }
        private static EventSystem.IntraObjectHandler<SpaceStation> NameChangedHandler = new EventSystem.IntraObjectHandler<SpaceStation>((System.Action<SpaceStation, object>)((cmp, data) => cmp.SetStationName(data)));
        public void SetStationName(object newName)
        {
            SetStationName((string)newName);
        }

        public void SetStationName(string newName)
        {
            m_name = newName;
            base.name = "Space Station: " + newName;
            ClusterManager.Instance.Trigger(1943181844, newName);
        }
        private bool CanTravel(bool tryingToLand) => !tryingToLand;
        private float GetSpeed() => 1f;
        public void DestroySpaceStation()
        {
            this.SetExploding();
            SpaceStationManager.Instance.DestroySpaceStationInteriorWorld(this.SpaceStationInteriorId);
            UnityEngine.Object.Destroy(this.gameObject);
        }


        public void DrawOuterBarriers(WorldContainer world)
        {

            // below world
            for (var x = 0; x < world.WorldSize.x; x++)
            {
                var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y);
                SimMessages.ReplaceElement(cell, ModElements.SpaceStationForceField.SimHash, null, 0);
                Grid.Visible[cell] = 0;
                Grid.PreventFogOfWarReveal[cell] = true;
            }

            // left of world
            for (var y = 0; y < world.WorldSize.y; y++)
            {
                var cell = Grid.XYToCell(world.WorldOffset.x, world.WorldOffset.y + y);
                SimMessages.ReplaceElement(cell, ModElements.SpaceStationForceField.SimHash, null, 0);
                Grid.Visible[cell] = 0;
                Grid.PreventFogOfWarReveal[cell] = true;
            }

            // right of world
            for (var y = 0; y < world.WorldSize.y; y++)
            {
                var cell = Grid.XYToCell(world.WorldOffset.x + world.WorldSize.x-1, world.WorldOffset.y + y);
                SimMessages.ReplaceElement(cell, ModElements.SpaceStationForceField.SimHash, null, 0);
                Grid.Visible[cell] = 0;
                Grid.PreventFogOfWarReveal[cell] = true;
            }
        }

        const int lvl1Width = 50;
        const int lvl2Width = 74;
        const int lvl3Width = 100;

        public void DrawLeveledBarriers(WorldContainer world, int borderSize, bool locking = true)
        {

            int horizontalRow = world.WorldOffset.y + world.Height - borderSize - 3;
            int verticalLeft = world.WorldOffset.x + ((world.Width - borderSize) / 2);
            int verticalRight = world.WorldOffset.x + world.Width - ((world.Width - borderSize) / 2) - 1;

            // horizontal world
            for (var x = 0; x < world.WorldSize.x; x++)
            {
                for(int y = 0; y <= horizontalRow; y++)
                {
                    var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y + y);
                    SimMessages.ReplaceElement(cell, locking ? ModElements.SpaceStationForceField.SimHash : SimHashes.Vacuum, null, 0);
                    Grid.Visible[cell] = locking ? byte.MinValue : byte.MaxValue;
                    Grid.PreventFogOfWarReveal[cell] = locking;
                }
            }

            // vertical world
            for (var y = 0; y < world.WorldSize.y; y++)
            {
                for (var x = 0; x < world.WorldSize.x; x++)
                {
                    var conX = x + world.WorldOffset.x;

                    if ( (conX < verticalLeft && conX < verticalRight ) || (conX > verticalRight && conX > verticalLeft))
                    {
                        var cell = Grid.XYToCell(world.WorldOffset.x + x, world.WorldOffset.y + y);
                        SimMessages.ReplaceElement(cell, locking ? ModElements.SpaceStationForceField.SimHash : SimHashes.Vacuum, null, 0);
                        Grid.Visible[cell] = locking ? byte.MinValue : byte.MaxValue;
                        Grid.PreventFogOfWarReveal[cell] = locking;
                    }
                }
            }
        }

        int counter = 0;
        public new void Sim4000ms(float dt)
        {
            counter++;
            counter %= 9;

            if(counter==0)
            {
                var world = ClusterManager.Instance.GetWorld(SpaceStationInteriorId);
                DrawLeveledBarriers(world, lvl1Width);
            }
            if (counter == 3)
            {
                var world = ClusterManager.Instance.GetWorld(SpaceStationInteriorId);
                DrawLeveledBarriers(world, lvl1Width,false);
                DrawLeveledBarriers(world, lvl2Width);                
            }
            if(counter == 6)
            {

                var world = ClusterManager.Instance.GetWorld(SpaceStationInteriorId);
                DrawLeveledBarriers(world, lvl1Width, false);
                DrawOuterBarriers(world);
            }

        }

        public void DrawBarriers()
        {
            if (SpaceStationInteriorId == -1)
            {
                SgtLogger.l("No world for space station found, id is -1");
                return;
            }
            var world = ClusterManager.Instance.GetWorld(SpaceStationInteriorId);
            DrawOuterBarriers(world);
            DrawLeveledBarriers(world, lvl1Width);
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

    }
}
