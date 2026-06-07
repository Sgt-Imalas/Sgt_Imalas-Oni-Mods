using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static BlueprintsV2.STRINGS.UI.BLUEPRINTSELECTOR.BLUEPRINTINFO.STATS;
using static Grid.Restriction;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers
{
	internal class Vis_BuildingPreview : KMonoBehaviour
	{
		protected RectTransform _rectTransform;
		protected KBatchedAnimController kbac;
		protected string defaultAnim;
		internal virtual void Init(BuildingConfig building)
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

			string defaultAnimState = building.BuildingDef.DefaultAnimState;
			if (!kbac.HasAnimation(defaultAnimState))
				defaultAnimState = kbac.AnimFiles.First()?.GetData()?.GetAnim(0)?.name ?? "off";
			kbac.defaultAnim = defaultAnim = defaultAnimState.ToString();
			UpdatePosition(building);
		}
		void Update()
		{
			return;
			if (rotate)
			{
				transform.Rotate(0, 0, -90 * Time.unscaledDeltaTime);
			}
		}
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

			_rectTransform.pivot = new(1f/width, 1f/heigh);

			if (correctX)
			{
				switch (orientation)
				{
					default:
						transform.localPosition += new Vector3(50, 0); break;
					case Orientation.R90:
						transform.localPosition += new Vector3(0, -50); break;
					case Orientation.R180:
						transform.localPosition += new Vector3(-50, 0); break;
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
					transform.Rotate(0,0, -90);
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

		public override void OnSpawn()
		{
			base.OnSpawn();
			kbac.Play(defaultAnim);
		}
	}
}
