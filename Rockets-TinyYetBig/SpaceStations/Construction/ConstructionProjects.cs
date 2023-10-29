using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    public static class ConstructionProjects
    {
        public static ConstructionProjectAssembly SpaceStationInit = new ConstructionProjectAssembly()
        {
            IsUpgrade = false,
            Parts = new List<PartProject>()
                {
                    new PartProject(SimHashes.Steel.CreateTag(), 10, 600),
                    new PartProject(SimHashes.Ceramic.CreateTag(), 10, 600),
                    new PartProject(SimHashes.Water.CreateTag(), 10, 600)
                },
            PreviewSprite = Assets.GetSprite("unknown"),
            OnConstructionFinishedAction = new Action<GameObject>( (SiteGO) => 
            {
                if(SiteGO != null && SiteGO.TryGetComponent<ClusterGridEntity>(out var entity))
                {
                    SpaceStation.SpawnNewSpaceStation(entity.Location);
                }
            }
            )
        };


        public static List<ConstructionProjectAssembly> AllProjects = new List<ConstructionProjectAssembly>()
        {
            new ConstructionProjectAssembly()
            {
                IsUpgrade = false,
                Parts = new List<PartProject>()
                {
                    new PartProject(SimHashes.Steel.CreateTag(), 10, 600),
                    new PartProject(SimHashes.Ceramic.CreateTag(), 10, 600),
                    new PartProject(SimHashes.Water.CreateTag(), 10, 600)
                },
                PreviewSprite = Assets.GetSprite("unknown")
            }
        };
    }
}
