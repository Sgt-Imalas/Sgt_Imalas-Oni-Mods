using BlueprintsV2.BlueprintData;
using Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers
{
	internal class Vis_SpritePreview : KMonoBehaviour
	{
		protected Image SpriteRenderer;
		private Color _color = Color.white;
		private Color _desaturated = new(1, 1, 1, 0.25f);
		private Color _disabledHighlighted = new(1, 1, 1, 0.50f);
		internal Vis_SpritePreview Init()
		{
			SpriteRenderer = transform.Find("TileMask/TileVis").gameObject.GetComponent<Image>();
			var rect = SpriteRenderer.rectTransform();

			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);

			//_mask = transform.Find("TileMask").GetComponent<RectMask2D>();
			SpriteRenderer.gameObject.SetActive(true);
			return this;
		}
		public void SetDisplayed(Tuple<Sprite,Color> tuple)
		{
			SpriteRenderer.sprite = tuple.first;
			var color = tuple.second;
			color.a = 1;
			SpriteRenderer.color = color;
			_color = color;
			_desaturated = color;
			_desaturated.a = 0.25f;

			_disabledHighlighted = color;
			_desaturated.a = 0.50f;
		}


		internal void RefreshOpacity(bool layerActive, bool useLowOpacity, bool highLighted)
		{
			bool disabledButHighlighted = !layerActive && highLighted;
			if (disabledButHighlighted)
				SpriteRenderer.color = _disabledHighlighted;
			else if (!layerActive)
				SpriteRenderer.color = Color.clear;
			else if (useLowOpacity)
				SpriteRenderer.color = _desaturated;
			else
				SpriteRenderer.color = _color;
		}
	}
}
