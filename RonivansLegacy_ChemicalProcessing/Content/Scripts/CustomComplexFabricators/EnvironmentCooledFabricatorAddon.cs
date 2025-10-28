using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class EnvironmentCooledFabricatorAddon : KMonoBehaviour, IAdditionalRecipeDescriptorProvider, IOnWorkTickActionProvider
	{
		[MyCmpReq] ComplexFabricator fabricator;
		[MyCmpReq] Building building;
		[MyCmpReq] Operational operational;
		Extents extents;

		[SerializeField]
		public float MaxTemperature = 2000;
		[SerializeField]
		public float thermalFudge = 1f/25f;
		private HandleVector<int>.Handle structureTemperature;

		[SerializeField]
		public float selfHeatPercentage = 1f / 4f;

		ComplexRecipe lastRecipeChecked = null;
		float cachedExhaust = -1f;

		public override void OnSpawn()
		{
			base.OnSpawn();
			extents = building.GetExtents();
			this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
		}

		void AddExhaustHeat(float kw, float dt)
		{
			StructureTemperatureComponents.ExhaustHeat(extents, kw, MaxTemperature, dt);
			var payload = GameComps.StructureTemperatures.GetPayload(structureTemperature);
			payload.energySourcesKW = GameComps.StructureTemperatures.AccumulateProducedEnergyKW(payload.energySourcesKW, kw, STRINGS.BUILDINGS.PREFABS.CHEMICAL_SELECTIVEARCFURNACE.REFINEMENT_HEAT_EXHAUST);
		}
		void AddSelfHeat(float kw, float dt)
		{
			GameComps.StructureTemperatures.ProduceEnergy(structureTemperature, kw * dt, (string)STRINGS.BUILDINGS.PREFABS.CHEMICAL_SELECTIVEARCFURNACE.REFINEMENT_SELF_HEAT, dt);
		}
		void ProduceExhaustHeat(float dt)
		{
			var recipe = fabricator.CurrentWorkingOrder;
			if (recipe == null)
				return;

			float totalKJ = GetRecipeKJ(recipe);

			float recipeTime = recipe.time;
			float recipeTimePercentage = dt / recipeTime;

			float kjPerSecond = totalKJ / recipeTime;

			float selfHeat = kjPerSecond * selfHeatPercentage;
			float exhaustHeat = kjPerSecond * (1f - selfHeatPercentage);

			//SgtLogger.l("Total recipe time: " + recipeTime + " dt: " + dt + ", percentage: " + recipeTimePercentage + ", total kj: " + totalKJ + ", Producing per sec: " + kjPerSecond);
			AddSelfHeat(selfHeat, dt);
			AddExhaustHeat(exhaustHeat, dt);
		}

		float GetRecipeKJ(ComplexRecipe recipe)
		{
			if (recipe == null)
				return 0;

			if (recipe == lastRecipeChecked)
				return cachedExhaust;

			PrimaryElement firstProduct = Assets.GetPrefab(recipe.results[0].material).GetComponent<PrimaryElement>();
			float kiloJoules = -GameUtil.CalculateEnergyDeltaForElementChange(recipe.results[0].amount, firstProduct.Element.specificHeatCapacity, firstProduct.Element.highTemp, fabricator.heatedTemperature);
			cachedExhaust = kiloJoules * thermalFudge;
			return cachedExhaust;
		}

		public List<Descriptor> GetAdditionalRecipeEffects(ComplexRecipe recipe)
		{
			float fudgedJoules = GetRecipeKJ(recipe) * 1000f; //game is off by a factor of 1000 here in vanilla
			float joulesPerSecond = fudgedJoules / recipe.time;
			return [
				new Descriptor(
					string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.REFINEMENT_ENERGY, GameUtil.GetFormattedJoules(fudgedJoules)),
					string.Format(STRINGS.BUILDINGS.PREFABS.CHEMICAL_SELECTIVEARCFURNACE.REFINEMENT_HEAT_TOOLTIP_CONTINUOUS,GameUtil.GetFormattedJoules(fudgedJoules), GameUtil.GetFormattedJoules(joulesPerSecond))
					)];

		}

		public void OnWorkTick(WorkerBase worker, float dt)
		{
			if (!operational.IsActive)
				return;
			ProduceExhaustHeat(dt);
		}
	}
}
