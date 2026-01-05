using PipPlantNotify.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Grid;

namespace PipPlantNotify.Content.Defs.Entities
{
	internal class MarkerConfig : IEntityConfig
	{
		public static readonly string ID = "PipPlantNotification_Marker";

		public GameObject CreatePrefab()
		{
			GameObject entity = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = entity.AddOrGet<KBatchedAnimController>();

			kbac.AnimFiles = new KAnimFile[1] { Assets.GetAnim("pip_pointer_arrows_kanim") };
			kbac.sceneLayer = SceneLayer.FXFront;
			kbac.initialAnim = "pointing_loop";
			kbac.initialMode = KAnim.PlayMode.Loop;
			kbac.forceUseGameTime = true;
			entity.AddComponent<TimedSelfDelete>();

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
