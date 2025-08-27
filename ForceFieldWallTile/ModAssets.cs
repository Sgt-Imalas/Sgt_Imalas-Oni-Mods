using Klei.AI;
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
