using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

		int[] cells;
		public float LightEfficiency;

		public float MiniumLightRequirement = 1000f, MaximumLightRequirement = 20000f;

		public override void OnSpawn()
		{
			base.OnSpawn();

			var pos = Grid.CellToXY(buildingComplete.NaturalBuildingCell());
			var lightCells = new List<int>();
			foreach (var cell in buildingComplete.PlacementCells)
			{
				if (Grid.CellToXY(cell).Y != pos.Y)
				{
					lightCells.Add(cell);
				}
			}
			RecalculateLightEfficiency();
		}

		public void Sim1000ms(float dt)
		{
			RecalculateLightEfficiency();
		}
		void RecalculateLightEfficiency()
		{
			float totalCount = 0, lighted = 0;
			foreach (var cell in buildingComplete.PlacementCells)
			{
				if (!Grid.IsValidCell(cell))
					continue;
				totalCount++;
				float LightIntensity = Grid.LightIntensity[cell];


				if (LightIntensity >= MiniumLightRequirement)
				{
					lighted++;
					float bonus = Mathf.Clamp(0,(LightIntensity - MiniumLightRequirement) / (MaximumLightRequirement - MiniumLightRequirement),4);
					if (bonus > 0)
					{
						lighted += bonus;
					}
				}
			}
			LightEfficiency = Mathf.Max(0.1f, totalCount > 0 ? lighted / totalCount : 0);
			converter.SetWorkSpeedMultiplier(LightEfficiency); //minimum of 5% efficiency
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Yield, StatusItemsDatabase.AlgaeGrower_LightEfficiency, this);
		}
	}
}
