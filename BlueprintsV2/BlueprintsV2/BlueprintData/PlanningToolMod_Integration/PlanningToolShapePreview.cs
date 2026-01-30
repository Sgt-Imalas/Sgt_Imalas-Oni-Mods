using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration.EnumMirrors;
using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.TOOLS;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration
{
	internal class PlanningToolShapePreview : KMonoBehaviour
	{
		static Material SquareMat, CircleMat, DiamondMat;

		MeshRenderer renderer;

		public PlanShape Shape;
		public PlanColor Color;

		static bool init = false;
		static void InitMaterials()
		{
			if (init)
				return;
			init = true;

			SquareMat = new Material(Assets.instance.mopPlacerAssets.material);
			SquareMat.mainTexture = ModAssets.PlanningToolPreview_Square.texture;
			CircleMat = new Material(Assets.instance.mopPlacerAssets.material);
			CircleMat.mainTexture = ModAssets.PlanningToolPreview_Circle.texture;
			DiamondMat = new Material(Assets.instance.mopPlacerAssets.material);
			DiamondMat.mainTexture = ModAssets.PlanningToolPreview_Diamond.texture;
		}
		public void SetVisuals(PlanShape shape,PlanColor color)
		{
			Color = color;
			Shape = shape;
			RefreshVisuals();
		}
		void RefreshVisuals()
		{
			switch (Shape)
			{
				default:
				case PlanShape.Rectangle:
					renderer?.material = SquareMat;
					break;
				case PlanShape.Circle:
					renderer?.material = CircleMat;
					break;
				case PlanShape.Diamond:
					renderer?.material = DiamondMat;
					break;
			}
			Color32 color = PlanningTool_EnumMapping.AsColor(Color);
			SgtLogger.l(Shape + " color: " + color.ToString());

			renderer?.material?.color = color;
		}

		public override void OnSpawn()
		{
			InitMaterials();
			renderer = GetComponentInChildren<MeshRenderer>();
			base.OnSpawn();
			RefreshVisuals();
		}
	}
}
