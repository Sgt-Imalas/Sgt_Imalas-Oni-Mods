using Satsuma;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;
using static STRINGS.NAMEGEN.WORLD;

namespace NaturalConstruction.Content.Scripts
{
	internal class MakePlantedPlantWild : KMonoBehaviour
	{
		[MyCmpReq] PlantablePlot plot;
		[MyCmpReq] CompleteBuildingNaturalSpawner tileSpawner;
		[MyCmpReq] Prioritizable prio;
		[MyCmpGet] Rotatable rotatable;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.Sub(GameHashes.OccupantChanged, OnPlanted);
		}
		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();
		}

		void OnPlanted(object _)
		{
			UnlinkPlant();
		}
		void UnlinkPlant()
		{
			var plant = plot.occupyingObject;
			if (plant == null)
				return;
			//if (plant.TryGetComponent<ReceptacleMonitor>(out var plotMonitor))
			//	plotMonitor.SetReceptacle(null);
			plot.UnsubscribeFromOccupant();

			if (plant.TryGetComponent(out roots))
				roots.canBeUprooted = false;

			if (plant.TryGetComponent<Prioritizable>(out var plantPrio))
				plantPrio.SetMasterPriority(prio.GetMasterPriority());

			plot.occupyingObject = null;
			tileSpawner.SpawnNaturalTile();
		}
		private UprootedMonitor roots;
		public void RefreshRoots()
		{
			if (roots == null)
				return;
			roots.StartCoroutine(RefreshRoots(roots));
		}
		static IEnumerator RefreshRoots(UprootedMonitor roots)
		{
			//SgtLogger.l("Roots Pos: " + roots.position);
			yield return new WaitForSeconds(0.2f);
			roots.canBeUprooted = true;
			//roots.position = Grid.PosToCell(roots);
			//roots.UnregisterMonitoredCellsPartitionerEntries();
			//SgtLogger.l("Roots Pos 2: " + roots.position);
			//roots.RegisterMonitoredCellsPartitionerEntries();
		}
	}
}
