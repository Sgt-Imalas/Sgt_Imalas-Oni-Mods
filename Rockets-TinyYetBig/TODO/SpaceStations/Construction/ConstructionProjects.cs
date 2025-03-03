using Rockets_TinyYetBig.Elements;
using System;
using System.Collections.Generic;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
	public static class ConstructionProjects
	{
		public static ConstructionProjectAssembly SpaceStationInit => new ConstructionProjectAssembly()
		{
			Parts = new List<PartProject>()
			{
				new PartProject(SimHashes.Steel.CreateTag(), 10, 600),
				new PartProject(SimHashes.Ceramic.CreateTag(), 10, 600),
				new PartProject(SimHashes.Water.CreateTag(), 10, 600)
			},
			PreviewSprite = Assets.GetSprite("unknown"),
			OnConstructionFinishedAction = new Action<SpaceConstructable>((originSite) =>
			{
				if (originSite != null && originSite.TryGetComponent<ClusterGridEntity>(out var entity))
				{
					var station = SpaceStation.SpawnNewSpaceStation(entity.Location);
					if (station != null && station.TryGetComponent<SpaceConstructable>(out var constructable))
					{
						originSite.TransferPartsTo(constructable);
						GameScheduler.Instance.ScheduleNextFrame("RemoveConstructer", (_) => UnityEngine.Object.Destroy(originSite.gameObject));
					}
				}
			}
			)
		};
		public static ConstructionProjectAssembly DerelictStation => new ConstructionProjectAssembly()
		{
			Parts = new List<PartProject>()
			{
				new PartProject(SimHashes.Steel.CreateTag(), 100, 600),
				new PartProject(ModElements.UnobtaniumAlloy.Tag, 100, 600),
			},
			PreviewSprite = Assets.GetSprite("unknown"),
			OnConstructionFinishedAction = new Action<SpaceConstructable>((originSite) =>
			{
				if (originSite != null && originSite.TryGetComponent<ClusterGridEntity>(out var entity))
				{
					var station = SpaceStation.SpawnNewSpaceStation(entity.Location);
					if (station != null && station.TryGetComponent<SpaceConstructable>(out var constructable))
					{
						originSite.TransferPartsTo(constructable);
						GameScheduler.Instance.ScheduleNextFrame("RemoveConstructer", (_) => UnityEngine.Object.Destroy(originSite.gameObject));
					}
				}
			}
			)
		};


		public static List<ConstructionProjectAssembly> AllProjects = new List<ConstructionProjectAssembly>()
		{
			new ConstructionProjectAssembly()
			{
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
