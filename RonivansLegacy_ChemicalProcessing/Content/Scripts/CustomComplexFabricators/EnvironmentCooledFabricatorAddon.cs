using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class EnvironmentCooledFabricatorAddon : KMonoBehaviour, IAdditionalRecipeDescriptorProvider
	{
		[MyCmpReq] ComplexFabricator fabricator;
		[MyCmpReq] Building building;
		Extents extents;

		[SerializeField]
		public float MaxTemperature = 2000;
		[SerializeField]
		public float thermalFudge = 0.8f;
		private HandleVector<int>.Handle structureTemperature;

		public override void OnSpawn()
		{
			base.OnSpawn();
			extents = building.GetExtents();
			this.structureTemperature = GameComps.StructureTemperatures.GetHandle(this.gameObject);
		}

		public void AddExhaustHeat(float kw, float dt)
		{
			StructureTemperatureComponents.ExhaustHeat(extents, kw, MaxTemperature, dt);
		}

		public List<Descriptor> GetAdditionalRecipeEffects(ComplexRecipe recipe)
		{
			return [];
			PrimaryElement ingredientMaterial = Assets.GetPrefab(recipe.results[0].material).GetComponent<PrimaryElement>();
			
			float joules = -GameUtil.CalculateEnergyDeltaForElementChange(recipe.results[0].amount, ingredientMaterial.Element.specificHeatCapacity, ingredientMaterial.Element.highTemp, fabricator.heatedTemperature);
			float fudgedJoules = joules * this.thermalFudge;
			float kjPerSec = (fudgedJoules / recipe.time) / 1000;


			return [new Descriptor(string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.REFINEMENT_ENERGY, (object)GameUtil.GetFormattedJoules(joules)), "TMP!: the building will increase its heat production by: {0} while the recipe runs")];

		}
	}
}
