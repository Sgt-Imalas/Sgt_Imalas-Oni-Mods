using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;
using YamlDotNet.Serialization;

namespace NaturalConstruction.Content.Scripts
{
	internal class TrellisAnimator : KMonoBehaviour
	{
		[MyCmpReq] Building building;
		[MyCmpReq] KSelectable selectable;
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpGet] Rotatable rotatable;

		[MyCmpGet] BuildingUnderConstruction underConstruction;
		[MyCmpGet] PlantablePlot plot;
		[SerializeField] public int DefaultHeight = 1;


		GameObject TrellisGO;
		KBatchedAnimController TrellisKbac;

		bool IsUnderConstruction => underConstruction != null;

		public override void OnSpawn()
		{
			base.OnSpawn();
			kbac.SetSymbolVisiblity("wire", false);
			SpawnTrellis();
			this.SubToHighlighingChanges(TrellisKbac);
			this.Sub(ModAssets.PlantPreviewChanged, RefreshAnim);
		}
		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();
		}
		void RefreshAnim(object _)
		{
			TrellisKbac.Play(GetCurrentAnim());
		}
		void SpawnTrellis()
		{
			TrellisGO = new GameObject("TrellisVisualizer");
			TrellisGO.SetActive(false);
			TrellisKbac = TrellisGO.AddOrGet<KBatchedAnimController>();
			TrellisKbac.AnimFiles = [Assets.GetAnim("planttrellis_kanim")];

			Vector3 trellisPos;
			if(rotatable == null || !rotatable.GetVisualizerFlipY())
			{
				trellisPos = Grid.CellToPosCBC(Grid.CellAbove(this.NaturalBuildingCell()), building.Def.SceneLayer-1);
			}
			else
			{
				trellisPos = Grid.CellToPosCTC(Grid.CellBelow(this.NaturalBuildingCell()), building.Def.SceneLayer-1);
				TrellisKbac.flipY = true;
			}
			TrellisGO.transform.SetPosition(trellisPos);
			TrellisGO.transform.parent = transform;
			TrellisGO.SetActive(true);
			TrellisKbac.Play(GetCurrentAnim());
		}


		string GetCurrentAnim()
		{
			if (plot == null || IsUnderConstruction)
			{
				return "place_"+ DefaultHeight;
			}
			if (plot.plantPreview == null)
				return "off_"+ DefaultHeight;

			OccupyArea areaOccupied = plot.plantPreview.occupyArea;
			int height = Math.Clamp(areaOccupied.GetHeightInCells(), 1, 3);
			return "off_" + height;
		}
	}
}
