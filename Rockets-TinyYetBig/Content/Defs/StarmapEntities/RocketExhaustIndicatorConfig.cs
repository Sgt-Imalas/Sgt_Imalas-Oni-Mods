using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Grid;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.StarmapEntities
{
    class RocketExhaustIndicatorConfig : IEntityConfig
	{
		public static readonly string ID = "RTB_RocketExhaustIndicator";

		public GameObject CreatePrefab()
		{
			GameObject entity = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = entity.AddOrGet<KBatchedAnimController>();

			kbac.AnimFiles = new KAnimFile[1] { Assets.GetAnim("rtb_rocketexhaustindicator_kanim") };
			kbac.sceneLayer = SceneLayer.FXFront;
			kbac.initialAnim = "place";
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
