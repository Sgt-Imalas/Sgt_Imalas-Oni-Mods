using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Grid;

namespace Rockets_TinyYetBig.Content.Defs.Entities
{
	internal class BunkerLaunchpadSawbladeConfig : IEntityConfig
	{
		public static readonly string ID = "RTB_BunkerLaunchpadSawbladeEntity";

		public GameObject CreatePrefab()
		{
			GameObject entity = EntityTemplates.CreateEntity(ID, ID, false);
			entity.AddTag(ONITwitchLib.ExtraTags.OniTwitchSurpriseBoxForceDisabled);

			var kbac = entity.AddOrGet<KBatchedAnimController>();

			kbac.AnimFiles = new KAnimFile[1] { Assets.GetAnim("rocket_launchpad_bunker_saw_kanim") };
			kbac.sceneLayer = SceneLayer.GasFront; //one behind buildingback
			kbac.initialAnim = "off";
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
