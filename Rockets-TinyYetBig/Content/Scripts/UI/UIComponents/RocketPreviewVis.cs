using Rockets_TinyYetBig.Content.ModDb.RocketBlueprintData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace Rockets_TinyYetBig.Content.Scripts.UI.UIComponents
{
	internal class RocketPreviewVis : KMonoBehaviour
	{
		protected RectTransform _dimensionRect;
		protected KBatchedAnimController kbac;
		protected string defaultAnim;

		protected RectTransform _kanimRect;
		protected GameObject _kanimGO;

		Image _image;

		protected void InitDimensions()
		{
			var toggle = transform.Find("KanimRenderer");
			_kanimRect = toggle.rectTransform();
			_kanimGO = toggle.gameObject;
			_dimensionRect = GetComponent<RectTransform>();
			_image = GetComponent<Image>();
		}
		void InitKbac()
		{

		}

		public RocketPreviewVis Init(BuildingDef building, int pxPerTile )
		{
			InitDimensions();
			_image.color = Color.clear;
			kbac = _kanimGO.AddComponent<KBatchedAnimController>();
			var renderer = _kanimGO.AddComponent<KBatchedAnimCanvasRenderer>();
			kbac.materialType = KAnimBatchGroup.MaterialType.UI;
			kbac.setScaleFromAnim = false;
			kbac.sceneLayer = Grid.SceneLayer.FXFront;
			kbac.AnimFiles = building.AnimFiles;
			kbac.isMovable = true;
			kbac.defaultAnim = defaultAnim = building.DefaultAnimState;
			//kbac.animScale = 0.005f * 0.008f;
			//SgtLogger.l("StartAnim " + def.name + ": " + defaultAnim);
			//UpdatePosition(building);

			var height = building.HeightInCells;
			var width = building.WidthInCells;
			_dimensionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * pxPerTile);
			_dimensionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * pxPerTile);

			_kanimRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pxPerTile);
			_kanimRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pxPerTile);
			_kanimRect.localPosition = new(0, -0.5f * pxPerTile * height);

			UIUtils.AddSimpleTooltipToObject(this.gameObject, building.Name);
			return this;
		}
		internal void InitMissing(RocketBlueprintModule module, int pxPerTile)
		{
			InitDimensions();
			var height = module.height;
			var width = module.width;
			_dimensionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * pxPerTile);
			_dimensionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * pxPerTile);

			_kanimRect.gameObject.SetActive(false);
			_image.sprite = Assets.GetSprite("Unknown");
			UIUtils.AddSimpleTooltipToObject(this.gameObject, module.Name);
		}

		/// <summary>
		/// this mirrors Rotatable since kbac offset/pivot does not seem to work for ui kbacs
		/// do not try understanding the numbers, they work properly this way.
		/// </summary>
		/// <param name="building"></param>
		//void UpdatePosition(BuildingConfig building)
		//{
		//	Orientation orientation = building.Orientation;
		//	var def = building.BuildingDef;
		//	kbac.flipX = orientation == Orientation.FlipH;
		//	kbac.flipY = orientation == Orientation.FlipV;

		//	bool correctX = building.BuildingDef.WidthInCells % 2 == 0;

		//	float width = def.WidthInCells;
		//	float heigh = def.HeightInCells;

		//	_rectTransform.pivot = new(1f / width, 1f / heigh);

		//	float xPosOffset = orientation == Orientation.FlipH ? -50 : 50;

		//	if (correctX)
		//	{
		//		switch (orientation)
		//		{
		//			default:
		//				transform.localPosition += new Vector3(xPosOffset, 0); break;
		//			case Orientation.R90:
		//				transform.localPosition += new Vector3(0, -50); break;
		//			case Orientation.R180:
		//				transform.localPosition += new Vector3(-xPosOffset, 0); break;
		//			case Orientation.R270:
		//				transform.localPosition += new Vector3(0, 50); break;
		//		}
		//	}


		//	switch (orientation)
		//	{
		//		case Orientation.Neutral:
		//		case Orientation.FlipV:
		//		case Orientation.FlipH:
		//			break;
		//		case Orientation.R90:
		//			rotate = true;
		//			transform.Rotate(0, 0, -90);
		//			transform.localPosition += new Vector3(-50, 50, 0);
		//			break;
		//		case Orientation.R180:
		//			rotate = true;
		//			transform.Rotate(0, 0, -180);
		//			transform.localPosition += new Vector3(0, 100f, 0);
		//			break;
		//		case Orientation.R270:
		//			rotate = true;
		//			transform.Rotate(0, 0, -270);
		//			transform.localPosition += new Vector3(50, 50, 0);
		//			break;
		//	}

		//	_disableToggleSize.sizeDelta = new(width * 100f, heigh * 100f);
		//	_disableToggleSize.localPosition = Vector3.zero;
		//}

		void CorrectDefaultAnim()
		{
			///Relevant for some logic buildings that usually have their anim set by the logic component
			if (!kbac.HasAnimation(defaultAnim))
			{
				//SgtLogger.l(defaultAnim + " anim not found");
				defaultAnim = kbac.AnimFiles.First()?.GetData()?.GetAnim(0)?.name ?? "grounded";
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

	}
}
