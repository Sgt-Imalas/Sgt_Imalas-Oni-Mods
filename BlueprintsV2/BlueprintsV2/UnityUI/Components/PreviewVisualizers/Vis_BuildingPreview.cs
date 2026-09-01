using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.BLUEPRINTINFO.STATS;
using static Grid.Restriction;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers
{
	internal class Vis_BuildingPreview : KMonoBehaviour
	{
		protected RectTransform _rectTransform;
		protected KBatchedAnimController kbac;
		protected string defaultAnim;

		private Color _color = Color.white;
		private Color _desaturated = new(1, 1, 1, 0.25f);
		private Color _disabledHighlighted = new(1, 1, 1, 0.50f);
		internal virtual Vis_BuildingPreview Init(BuildingConfig building)
		{
			_rectTransform = GetComponent<RectTransform>();
			var def = building.BuildingDef;
			kbac = gameObject.AddComponent<KBatchedAnimController>();
			var renderer = gameObject.AddComponent<KBatchedAnimCanvasRenderer>();
			kbac.materialType = KAnimBatchGroup.MaterialType.UI;
			//kbac.visibilityType = KAnimControllerBase.VisibilityType.Always;
			kbac.setScaleFromAnim = false;
			kbac.sceneLayer = Grid.SceneLayer.FXFront;
			kbac.AnimFiles = building.BuildingDef.AnimFiles;
			kbac.isMovable = true;

			kbac.defaultAnim = defaultAnim = building.BuildingDef.DefaultAnimState;
			//SgtLogger.l("StartAnim " + def.name + ": " + defaultAnim);
			UpdatePosition(building);
			return this;
		}
		//void Update()
		//{
		//	return;
		//	if (rotate)
		//	{
		//		transform.Rotate(0, 0, -90 * Time.unscaledDeltaTime);
		//	}
		//}
		bool rotate = false;

		/// <summary>
		/// this mirrors Rotatable since kbac offset/pivot does not seem to work for ui kbacs
		/// do not try understanding the numbers, they work properly this way.
		/// </summary>
		/// <param name="building"></param>
		void UpdatePosition(BuildingConfig building)
		{
			Orientation orientation = building.Orientation;
			var def = building.BuildingDef;
			kbac.flipX = orientation == Orientation.FlipH;
			kbac.flipY = orientation == Orientation.FlipV;

			bool correctX = building.BuildingDef.WidthInCells % 2 == 0;

			float width = def.WidthInCells;
			float heigh = def.HeightInCells;

			_rectTransform.pivot = new(1f / width, 1f / heigh);

			float xPosOffset = orientation == Orientation.FlipH ? -50 : 50;

			if (correctX)
			{
				switch (orientation)
				{
					default:
						transform.localPosition += new Vector3(xPosOffset, 0); break;
					case Orientation.R90:
						transform.localPosition += new Vector3(0, -50); break;
					case Orientation.R180:
						transform.localPosition += new Vector3(-xPosOffset, 0); break;
					case Orientation.R270:
						transform.localPosition += new Vector3(0, 50); break;
				}
			}


			switch (orientation)
			{
				case Orientation.Neutral:
				case Orientation.FlipV:
				case Orientation.FlipH:
					break;
				case Orientation.R90:
					rotate = true;
					transform.Rotate(0, 0, -90);
					transform.localPosition += new Vector3(-50, 50, 0);
					break;
				case Orientation.R180:
					rotate = true;
					transform.Rotate(0, 0, -180);
					transform.localPosition += new Vector3(0, 100f, 0);
					break;
				case Orientation.R270:
					rotate = true;
					transform.Rotate(0, 0, -270);
					transform.localPosition += new Vector3(50, 50, 0);
					break;
			}
		}

		void CorrectDefaultAnim()
		{
			///Relevant for some logic buildings that usually have their anim set by the logic component
			if (!kbac.HasAnimation(defaultAnim))
			{
				//SgtLogger.l(defaultAnim + " anim not found");
				defaultAnim = kbac.AnimFiles.First()?.GetData()?.GetAnim(0)?.name ?? "off";
			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			CorrectDefaultAnim();
			kbac.Play(defaultAnim);

			kbac.SetSymbolVisiblity("booster", false);
			kbac.SetSymbolVisiblity("blue_light_bloom", false);
		}

		internal void RefreshOpacity(bool layerActive, bool useLowOpacity, bool highLighted)
		{
			bool disabledButHighlighted = !layerActive && highLighted;
			if (disabledButHighlighted)
				kbac.TintColour = _disabledHighlighted;
			else if (useLowOpacity)
				kbac.TintColour = _desaturated;
			else
				kbac.TintColour = Color.white;

			kbac.SetVisiblity(layerActive || highLighted);
		}
	}
}
