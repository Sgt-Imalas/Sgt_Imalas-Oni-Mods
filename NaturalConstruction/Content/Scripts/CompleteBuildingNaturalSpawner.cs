using KSerialization;
using System.Collections;
using UnityEngine;
using UtilLibs;

namespace NaturalConstruction.Content.Scripts
{
	/// <summary>
	/// for sandbox insta build
	/// </summary>
	internal class CompleteBuildingNaturalSpawner : KMonoBehaviour
	{
		[MyCmpGet] SimCellOccupier sco;
		[MyCmpGet] KSelectable selectable;
		[MyCmpReq] PrimaryElement element;
		[MyCmpReq] Building building;
		[SerializeField] public bool ReplaceOnSpawn = true;
		[Serialize] float SpawnTileMass = -1;
		[MyCmpGet] MakePlantedPlantWild makeWild;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			if (!ReplaceOnSpawn)
				this.Sub(GameHashes.NewConstruction, TransferConfiguredMass);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			//GameScheduler.Instance.ScheduleNextGameTick("spawn nat tile" ,(_)=>			SpawnNaturalTile());
			//StartCoroutine(DelayedReplacement());
			if (ReplaceOnSpawn)
				SpawnNaturalTile();
			else if(sco == null)
			{
				DigOutNaturalTiles();
			}
		}

		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();
		}
		void DigOutNaturalTiles()
		{
			bool backwallBuilding = building.Def.ObjectLayer == ObjectLayer.Backwall;
			if (backwallBuilding)
			{
				return;
			}
			foreach (var tile in building.PlacementCells)
			{
				if (Grid.IsSolidCell(tile))
					SimMessages.Dig(tile);
			}
		}
		void TransferConfiguredMass(object data)
		{
			if (data is ConstructableNaturalSpawner spawner)
			{
				element.SetMass(spawner.NaturalMass);
			}
		}
		public void SpawnNaturalTile()
		{
			SgtLogger.l(this.GetProperName() + " spawn natural tile");
			PrepareSpawn();
			if (sco == null)
			{
				SpawnTileInternal();
			}
			else
			{
				sco.DestroySelf(SpawnTileInternal);
			}
		}

		SimHashes elementId;
		ushort elementIdx;
		float temperature;
		byte diseaseIdx = byte.MaxValue;
		int diseaseAmount = 0;
		float naturalMass = 100;
		void PrepareSpawn()
		{

			elementId = element.Element.id;
			elementIdx = element.Element.idx;
			temperature = element.Temperature;
			diseaseIdx = element.DiseaseIdx;
			diseaseAmount = element.DiseaseCount;
			naturalMass = element.Mass;
		}
		void SpawnTileInternal()
		{
			if (selectable.IsSelected)
				selectable.Unselect();

			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, selectable.GetName(), base.transform);
			

			if (SpawnTileMass > 0)
				naturalMass = SpawnTileMass;

			bool backwallBuilding = building.Def.ObjectLayer == ObjectLayer.Backwall;

			naturalMass *= Mathf.Clamp(Config.Instance.SpawningMassMultiplier, 1f, 2f);
			int placementCellCount = Mathf.Max(1,building.PlacementCells.Length);
			//SgtLogger.l("Number of placement cells: " + placementCellCount);
			foreach (var tile in building.PlacementCells)
			{
				if (backwallBuilding)
				{
					SimMessages.SetBackwallData(tile, elementIdx, naturalMass / placementCellCount, temperature);
					GameScenePartitioner.Instance.TriggerEvent(tile, GameScenePartitioner.Instance.backwallChangedLayer, null);
				}
				else
				{
					ModAssets.MovePickupables(tile);
					SimMessages.ReplaceAndDisplaceElement(tile, elementId, CellEventLogger.Instance.TemplateLoader, naturalMass / placementCellCount, temperature, diseaseIdx, diseaseAmount / placementCellCount);
					GameScenePartitioner.Instance.TriggerEvent(tile, GameScenePartitioner.Instance.solidChangedLayer, null);
					Grid.Foundation[tile] = false;
				}
			}

			//gameObject.Trigger((int)GameHashes.NewConstruction, (object)this);
			if (makeWild != null)
				makeWild.RefreshRoots();

			this.DeleteObject();
		}
	}
}
