﻿using HarmonyLib;
using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Docking;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets_TinyYetBig.Elements
{
	/// <summary>
	/// Component adds a rainbow gradient glow to natural tiles of Neutronium Alloy.
	/// </summary>
	public class RainbowSpec : KMonoBehaviour
	{
		public static RainbowSpec Instance;

		private Gradient rainbowGradient;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Instance = this;

			rainbowGradient = new Gradient();

			var colors = new List<Color> {
				Color.HSVToRGB(0,1,1 ),
				Color.HSVToRGB(45f/360f,1,1 ),
				Color.HSVToRGB(90f/360f,1,1 ),
				Color.HSVToRGB(135f/360f,1,1 ),
				Color.HSVToRGB(180f/360f,1,1 ),
				Color.HSVToRGB(225f/360f,1,1 ),
				Color.HSVToRGB(270f/360f,1,1 ),
				Color.HSVToRGB(315f/360f,1,1 )
			};

			var colorKey = new GradientColorKey[colors.Count];

			for (var i = 0; i < colors.Count; i++)
			{
				colorKey[i] = new GradientColorKey(colors[i], (i + 1f) / colors.Count);
			}

			var alphaKey = new GradientAlphaKey[1];
			alphaKey[0].alpha = 1.0f;
			alphaKey[0].time = 0.0f;

			rainbowGradient.SetKeys(colorKey, alphaKey);
		}

		private void Update()
		{
			if (World.Instance == null)
			{
				return;
			}

			var materials = World.Instance.groundRenderer.elementMaterials;

			if (materials != null && materials.Count > 0)
			{
				var camera = Camera.main.transform.position;
				var scale = CameraController.Instance.zoomFactor;

				var t = (Mathf.Cos(camera.x / scale) + Mathf.Sin(camera.y / scale)) / 4f + 0.5f;

				var rainbowColor = rainbowGradient.Evaluate(t);

				var NeutroniumAlloyMat = materials[ModElements.UnobtaniumAlloy];
				NeutroniumAlloyMat.alpha.SetColor("_ShineColour", rainbowColor);
				NeutroniumAlloyMat.opaque.SetColor("_ShineColour", rainbowColor);
			}
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Instance = null;
		}
	}
}