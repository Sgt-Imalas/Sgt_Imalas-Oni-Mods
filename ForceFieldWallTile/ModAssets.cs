using Klei.AI;
using Satsuma;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.DETAILTABS;

namespace ForceFieldWallTile
{
	internal class ModAssets
	{
		public static Gradient ColorGradientForcefield;
		static ModAssets()
		{
			float[] gradientSteps = [0.15f, 0.40f, 0.65f, 0.9f];

			{
				ColorGradientForcefield = new Gradient();
				GradientColorKey[] colors = [
						new GradientColorKey(Color.red, gradientSteps[0]),
					new GradientColorKey(Util.ColorFromHex("FFD400"), gradientSteps[1]),
					new GradientColorKey(Util.ColorFromHex("00FF88"), gradientSteps[2]),
					new GradientColorKey(UIUtils.rgb(61, 142, 255), gradientSteps[3])];

				// Blend alpha from opaque at 0% to transparent at 100%
				GradientAlphaKey[] alphas = [
						new GradientAlphaKey(1.0f, 0.2f),
					//new GradientAlphaKey(1.0f, 0.6f),
					new GradientAlphaKey(1.0f, 1.0f)
					];
				ColorGradientForcefield.SetKeys(colors, alphas);
			}
			{ 
			//ColorGradientTint = new Gradient();
			//GradientColorKey[] colors = [
			//		new GradientColorKey(UIUtils.Darken( Color.red,30), 0.0f),new GradientColorKey(Color.red, gradientSteps[0]),
			//		new GradientColorKey(UIUtils.Darken(Util.ColorFromHex("FFD400"),30), gradientSteps[1]),
			//		new GradientColorKey(UIUtils.Darken(Util.ColorFromHex("00FF88"),30), gradientSteps[2]),
			//		new GradientColorKey(UIUtils.Darken(UIUtils.rgb(61, 142, 255),30), gradientSteps[3])];

			//// Blend alpha from opaque at 0% to transparent at 100%
			//GradientAlphaKey[] alphas = [
			//		new GradientAlphaKey(1.0f, 0.0f),
			//		//new GradientAlphaKey(1.0f, 0.5f),
			//		new GradientAlphaKey(1.0f, 1.0f)
			//	];
			//ColorGradientTint.SetKeys(colors, alphas);
			}
		}
		public static Gradient ColorGradientTint => ColorGradientForcefield;


		public static Material ForceFieldMaterial;
		public static void LoadAssets()
		{
			var bundle = AssetUtils.LoadAssetBundle("forcefieldtile_assets", platformSpecific: true);
			//ForceFieldMaterial = bundle.LoadAsset<Material>("Assets/ShieldShader/ForceField.mat");
			ForceFieldMaterial = bundle.LoadAsset<Material>("Assets/ShieldShader/ForceField.mat");
			//ForceFieldMaterial = bundle.LoadAsset<Material>("Assets/ShieldShader/Shield2.mat");
			SgtLogger.Assert("ForceField", ForceFieldMaterial);
			//ForceFieldMaterial.color = Color.white;
			Debug.Log(ForceFieldMaterial.GetTexture("_Pattern"));

		}
	}
}
