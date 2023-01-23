using KSerialization;
using LogicSatellites.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogicSatellites.Satellites
{
    class SolarLens : KMonoBehaviour
    {
        [MyCmpGet]
        SatelliteGridEntity entity;


        int OrbitWorldId = -1;
        public override void OnSpawn()
        {
            base.OnSpawn();
            var asteroid = ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(entity.Location, EntityLayer.Asteroid);
            WorldContainer world = asteroid.GetComponent<WorldContainer>();
            OrbitWorldId = world.id;

        }
    }
}
