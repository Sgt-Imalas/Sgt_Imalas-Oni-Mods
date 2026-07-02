using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class LightEfficiencyConverter : KMonoBehaviour, ISim1000ms
	{
		[MyCmpReq]
		Operational operational;
		[MyCmpReq]
		KSelectable selectable;
		[MyCmpReq]
		BuildingComplete buildingComplete;
		[MyCmpReq]
		ElementConverter converter;

		int[] LightCells;
		public float LightEfficiency;
		public float BonusAmount;

		public float MiniumLightRequirement = 900f, MaximumLightRequirement = 20000f;

		public override void OnSpawn()
		{
			base.OnSpawn();

			var pos = Grid.CellToXY(buildingComplete.NaturalBuildingCell());
			var lightCells = new List<int>();
			foreach (var cell in buildingComplete.PlacementCells)
			{
				if (Grid.CellToXY(cell).Y == pos.Y + 1)
				{
					lightCells.Add(cell);
				}
			}
			LightCells = lightCells.ToArray();
			RecalculateLightEfficiency();
		}

		public void Sim1000ms(float dt)
		{
			RecalculateLightEfficiency();
		}
		void RecalculateLightEfficiency()
		{
			float totalCount = 0, lighted = 0;
			foreach (var cell in LightCells)
			{
				if (!Grid.IsValidCell(cell))
					continue;
				totalCount++;
				float LightIntensity = Grid.LightIntensity[cell];

				if (LightIntensity >= MiniumLightRequirement)
				{
					lighted++;
					float bonus = Mathf.Clamp(0, (LightIntensity - MiniumLightRequirement) / (MaximumLightRequirement - MiniumLightRequirement), 5);
					if (bonus > 0)
					{
						lighted += bonus;
					}
				}
				else
				{
					lighted += LightIntensity / MiniumLightRequirement;
				}
				
			}
			float calculatedEfficiency = (totalCount > 0 ? lighted / totalCount : 0);
			if (calculatedEfficiency > 1f)
			{
				BonusAmount = (calculatedEfficiency - 1f) * 0.2f; //up to 80% extra resources at 80k lux
			}
			else
				BonusAmount = 0;

			LightEfficiency = Mathf.Clamp(calculatedEfficiency, 0.05f, 1f);
			converter.OutputMultiplier = 1 + BonusAmount;
			converter.SetWorkSpeedMultiplier(LightEfficiency); //minimum of 5% efficiency, max of 100% efficiency for work speed, but can produce more resources if light is above 100%
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Yield, StatusItemsDatabase.AlgaeGrower_LightEfficiency, this);
			selectable.ToggleStatusItem(StatusItemsDatabase.AlgaeGrower_BonusLight, operational.IsActive && BonusAmount > 0, this);

		}
	}
}
