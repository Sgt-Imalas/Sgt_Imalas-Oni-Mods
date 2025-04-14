using _3GuBsVisualFixesNTweaks.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Grid;
using static KAnim;

namespace _3GuBsVisualFixesNTweaks.Defs.Entities
{
	public class PlacerWallPreviewConfig : IEntityConfig
	{
		public static readonly string ID = "VFNT_PlaceWallFx";

		public GameObject CreatePrefab()
		{
			GameObject entity = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = entity.AddOrGet<KBatchedAnimController>();

			kbac.AnimFiles = new KAnimFile[1] { Assets.GetAnim("vfnt_wall_marker_kanim") };
			kbac.sceneLayer = SceneLayer.FXFront;
			kbac.initialAnim = "place";
			entity.AddOrGet<PlaceWallIndicator>();
			return entity;
		}

		public void OnPrefabInit(GameObject go)
		{
		}

		public void OnSpawn(GameObject go)
		{
		}
		

		public string[] GetDlcIds() => null;
	}
}
