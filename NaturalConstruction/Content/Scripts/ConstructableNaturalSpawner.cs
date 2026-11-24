using KSerialization;
using System;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace NaturalConstruction.Content.Scripts
{
	internal class ConstructableNaturalSpawner : Constructable, ISingleSliderControl
	{
		///does not work for backwall..
		[MyCmpAdd] CopyBuildingSettings copySettings;
		[MyCmpGet] KSelectable selectable;
		[Serialize] float naturalMass = -1;
		[Serialize] float lastNaturalMass = 0;

		[SerializeField] public bool ReplaceOnFinish = true;

		bool backwallBuilding;
		public bool Backwall => backwallBuilding;
		private static readonly Action<object, object> OnCopySettingsDelegate = (component, data) => ((ConstructableNaturalSpawner)component).OnCopySettings(data);
		int handle = -1;
		int occupyCellCount = 1;

		public float NaturalMass => naturalMass;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.SetWorkTime(building.Def.ConstructionTime);
		}
		public override void OnSpawn()
		{
			if (naturalMass == -1)
			{
				naturalMass = building.Def.Mass[0];
				lastNaturalMass = naturalMass;
			}
			GetComponent<KBatchedAnimController>().SetSymbolVisiblity("wire", false);
			occupyCellCount = building.PlacementCells.Length;
			base.OnSpawn();
			waitForFetchesBeforeDigging = true;
			backwallBuilding = building.Def.ObjectLayer == ObjectLayer.Backwall;
			RefreshFetchIfNeeded();
			handle = this.Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate, this);
		}
		internal bool DoReplaceOnFinishConstruction() => ReplaceOnFinish;
		public override void OnCleanUp()
		{
			Unsubscribe(handle);
			base.OnCleanUp();
		}
		private void OnCopySettings(object data)
		{
			ConstructableNaturalSpawner component = ((GameObject)data).GetComponent<ConstructableNaturalSpawner>();
			if (component == null || component.naturalMass == naturalMass)
				return;

			naturalMass = component.naturalMass;
			RefreshFetchIfNeeded();
		}

		void RefreshConstructionTime()
		{
			if (!Config.Instance.ScalingConstructionTime)
				return;
			SetWorkTime(Mathf.Clamp(naturalMass / 20f, 3, 90));
		}
		void RefreshFetchIfNeeded()
		{
			if (Mathf.Approximately(lastNaturalMass, naturalMass))
				return;
			materialNeedsCleared = false;
			storage.DropAll();
			buildChore?.Cancel("Re-fetch mats");
			buildChore = null;
			SgtLogger.l("build chore canceled");

			if (fetchList != null)
			{
				fetchList.Cancel("custom amount");
				fetchList.MinimumAmount?.Clear();
				fetchList.FetchOrders?.Clear();
				fetchList.Remaining?.Clear();
			}
			else
			{
				fetchList = new FetchList2(storage, Db.Get().ChoreTypes.BuildFetch);
			}
			Recipe.Ingredient[] allIngredients = Recipe.GetAllIngredients(selectedElementsTags);
			foreach (Recipe.Ingredient ingredient in allIngredients)
			{
				fetchList.Add(ingredient.tag, null, naturalMass);
				MaterialNeeds.UpdateNeed(ingredient.tag, naturalMass - lastNaturalMass, base.gameObject.GetMyWorldId());
			}
			lastNaturalMass = naturalMass;
			fetchList.Submit(OnFetchListComplete, check_storage_contents: true);

			RefreshConstructionTime();
		}

		//public override void OnCompleteWork(WorkerBase worker)
		//{
		//	//base.OnCompleteWork(worker);
		//	SpawnNaturalTile();
		//}

		public void SpawnNaturalTile(WorkerBase workerForGameplayEvent)
		{
			SgtLogger.l(this.GetProperName() + " spawn natural tile at construction finish");
			if (selectable.IsSelected)
				selectable.Unselect();
			UnmarkArea();
			finished = true;
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, selectable.GetName(), base.transform);
			var element = storage.items.First()?.GetComponent<PrimaryElement>();
			ushort elementIdx;
			float temperature;
			byte diseaseIdx = byte.MaxValue;
			int diseaseAmount = 0;
			SimHashes elementId = SimHashes.SandStone;
			if (element != null)
			{
				elementId = element.Element.id;
				elementIdx = element.Element.idx;
				temperature = element.Temperature;
				diseaseIdx = element.DiseaseIdx;
				diseaseAmount = element.DiseaseCount;
			}
			else
			{
				var configuredId = SelectedElementsTags.FirstOrDefault();
				if (configuredId == default || ElementLoader.GetElement(configuredId) == null)
				{
					SgtLogger.warning(configuredId + " not found as element, defaulting to sandstone!");
					configuredId = SimHashes.SandStone.CreateTag();
				}

				var fallback = ElementLoader.GetElement(configuredId);
				elementId = fallback.id;
				elementIdx = fallback.idx;
				temperature = UtilMethods.GetKelvinFromC(20);
			}
			float multiplier = Mathf.Clamp(Config.Instance.SpawningMassMultiplier, 1f, 2f);
			int placementCellCount = building.PlacementCells.Length;
			foreach (var tile in building.PlacementCells)
			{
				if (backwallBuilding)
				{
					SimMessages.SetBackwallData(tile, elementIdx, naturalMass / placementCellCount, temperature);
					GameScenePartitioner.Instance.TriggerEvent(tile, GameScenePartitioner.Instance.backwallChangedLayer, null);
				}
				else
				{

					//SgtLogger.l("ISREPLACEMENTTILE: " + IsReplacementTile);
					ModAssets.MovePickupables(tile);
					SimMessages.ReplaceAndDisplaceElement(tile, elementId, CellEventLogger.Instance.TemplateLoader, naturalMass / placementCellCount, temperature, diseaseIdx, diseaseAmount / placementCellCount);
					GameScenePartitioner.Instance.TriggerEvent(tile, GameScenePartitioner.Instance.solidChangedLayer, null);
					//for (int layer = 0; layer < 45; ++layer)
					//{
					//	if (Grid.Objects[cell, layer] != null && Grid.Objects[cell, layer].GetComponent<Constructable>() != null)
					//		SgtLogger.l("Object at layer " + layer + ": " + Grid.Objects[cell, layer]);
					//}
					Grid.Foundation[tile] = false;
				}
			}
			storage.ConsumeAllIgnoringDisease();
			//gameObject.Trigger((int)GameHashes.NewConstruction, (object)this);
			this.DeleteObject();
		}



		#region massslider
		public string SliderTitleKey => "STRINGS.UI.SANDBOXTOOLS.SETTINGS.MASS.NAME";

		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;
		public int SliderDecimalPlaces(int index) => 0;

		public float GetSliderMin(int index) => ModAssets.MinMass;

		public float GetSliderMax(int index) => ModAssets.MaxMass * occupyCellCount;

		public float GetSliderValue(int index) => naturalMass;

		public void SetSliderValue(float percent, int index)
		{
			naturalMass = percent;
			RefreshFetchIfNeeded();
		}
		public string GetSliderTooltipKey(int index) => null;

		public string GetSliderTooltip(int index) => global::STRINGS.UI.SANDBOXTOOLS.SETTINGS.MASS.TOOLTIP;
		#endregion
	}
}
