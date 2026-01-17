using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.LiquidInfo;
using BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements;
using BlueprintsV2.Visualizers;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ResearchTypes;
using static STRINGS.UI;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
	internal class ElementIndicatorVisual : IVisual
	{
		public GameObject Visualizer { get; private set; }
		public Vector2I Offset { get; private set; }

		public PlanScreen.RequirementsState RequirementsState => PlanScreen.RequirementsState.Complete;

		public string BuildingID => null;

		SimHashes ElementId;
		float Amount, Temperature;

		public static Tag GetInfoPrefabId(SimHashes elementId)
		{
			Tag prefabId;
			var element = ElementLoader.GetElement(elementId.CreateTag());
			if (element.IsSolid)
				prefabId = SolidInfoConfig.ID;
			else if (element.IsLiquid)
				prefabId = LiquidInfoConfig.ID;
			else
				prefabId = GasInfoConfig.ID;
			return prefabId;
		}
		public ElementIndicatorVisual(int cell, Vector2I offset, SimHashes elementId, float amount, float temperature)
		{
			Visualizer = GameUtil.KInstantiate(Assets.GetPrefab(GetInfoPrefabId(elementId)), Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront), Grid.SceneLayer.FXFront, "BlueprintModLiquidIndicatorVisual");
			Visualizer.SetActive(IsPlaceable(cell));
			Offset = offset;
			if (Visualizer.TryGetComponent<ElementPlanInfo>(out var info))
			{
				info.SetInfo(elementId, amount, temperature);
			}
			ElementId = elementId;
			Amount = amount;
			Temperature = temperature;
		}

		public bool IsPlaceable(int cellParam)
		{
			return Grid.IsValidCell(cellParam) && Grid.IsVisible(cellParam) && !(Grid.Solid[cellParam] && Grid.Foundation[cellParam]);
		}

		public void MoveVisualizer(int cellParam, bool forceRedraw)
		{
			Visualizer.transform.SetPosition(Grid.CellToPosCBC(cellParam, Grid.SceneLayer.FXFront));
			Visualizer.SetActive(IsPlaceable(cellParam));
		}
		public void ForceRedraw() { }

		public bool TryUse(int cellParam)
		{
			if (IsPlaceable(cellParam))
			{
				if (BlueprintState.InstantBuild)
				{
					SimMessages.ReplaceElement(cellParam, ElementId, CellEventLogger.Instance.SandBoxTool, Amount, Temperature);
				}
				else
				{
					var existingItem = Grid.Objects[cellParam, (int)ModAssets.PlannedElementLayer];

					if (existingItem != null)
					{
						existingItem.DeleteObject();
						Grid.Objects[cellParam, (int)ModAssets.PlannedElementLayer] = null;
					}

					var infoIndicator = Util.KInstantiate(Assets.GetPrefab(GetInfoPrefabId(ElementId)));
					Grid.Objects[cellParam, (int)ModAssets.PlannedElementLayer] = infoIndicator;
					Vector3 posCbc = Grid.CellToPosCBC(cellParam, MopTool.Instance.visualizerLayer);
					posCbc.z -= 0.15f;
					infoIndicator.transform.SetPosition(posCbc);
					if (infoIndicator.TryGetComponent<ElementPlanInfo>(out var info))
					{
						info.SetInfo(ElementId, Amount, Temperature, true);
					}
					infoIndicator.SetActive(true);
				}
				return true;
			}

			return false;
		}

		public PermittedRotations GetAllowedRotations() => BlueprintState.All;
		public void ApplyRotation(Orientation rotation, bool flipped, bool flippedY)
		{
			//digging doesnt get rotated
		}

		public void RefreshColor()
		{
			//no tinting
		}
	}
}
