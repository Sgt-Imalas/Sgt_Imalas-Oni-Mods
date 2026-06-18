using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;
using static STRINGS.MISC.STATUSITEMS.HEALTHSTATUS;
using static STRINGS.UI;

namespace NaturalConstruction.Content.Scripts
{
	internal class ConstructableNaturalSpawner : Constructable, ISingleSliderControl
	{
		///does not work for backwall..
		//[MyCmpAdd] CopyBuildingSettings copySettings;
		[MyCmpGet] KSelectable selectable;
		[Serialize] float naturalMass = -1;
		[Serialize] float lastNaturalMass = 0;
		bool backwallBuilding;
		private static readonly EventSystem.IntraObjectHandler<ConstructableNaturalSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ConstructableNaturalSpawner>((System.Action<ConstructableNaturalSpawner, object>)((component, data) => component.OnCopySettings(data)));
		int handle = -1;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.SetWorkTime(building.Def.ConstructionTime);
		}
		public override void OnSpawn()
		{
			if (naturalMass == -1)
				naturalMass = building.Def.Mass[0];
			base.OnSpawn();
			waitForFetchesBeforeDigging = true;
			backwallBuilding = building.Def.ObjectLayer == ObjectLayer.Backwall;
			RefreshFetchIfNeeded();

			//handle = this.Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
		}
		public override void OnCleanUp()
		{
			//Unsubscribe(handle);
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
			SetWorkTime(Mathf.Clamp(naturalMass / 20f, 5, 100));
		}
		void RefreshFetchIfNeeded()
		{
			var defaultCost = building.Def.Mass.First();
			if (Mathf.Approximately(defaultCost, naturalMass))
				return;
			if (Mathf.Approximately(lastNaturalMass, naturalMass))
				return;
			materialNeedsCleared = false;
			storage.DropAll();
			buildChore?.Cancel("Re-fetch mats");
			buildChore = null;
			fetchList.Cancel("custom amount");
			fetchList.MinimumAmount?.Clear();
			fetchList.FetchOrders?.Clear();
			fetchList.Remaining?.Clear();
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

		public override void OnCompleteWork(WorkerBase worker)
		{
			//base.OnCompleteWork(worker);
			SpawnNaturalTile();
		}

		void SpawnNaturalTile()
		{
			if (selectable.IsSelected)
				selectable.Unselect();
			UnmarkArea();
			finished = true;
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, selectable.GetName(), base.transform);
			int cell = Grid.PosToCell(this);
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

			if (backwallBuilding)
			{
				SimMessages.SetBackwallData(cell, elementIdx, naturalMass, temperature);
			}
			else
			{
				SimMessages.ReplaceElement(cell, elementId, CellEventLogger.Instance.SandBoxTool, naturalMass, temperature, diseaseIdx, diseaseAmount);

			}

			storage.ConsumeAllIgnoringDisease();
			gameObject.Trigger((int)GameHashes.NewConstruction, (object)this);
			this.DeleteObject();

		}

		#region massslider
		public string SliderTitleKey => "STRINGS.UI.SANDBOXTOOLS.SETTINGS.MASS.NAME";

		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;
		public int SliderDecimalPlaces(int index) => 0;

		public float GetSliderMin(int index) => 1;

		public float GetSliderMax(int index) => 2000;

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
