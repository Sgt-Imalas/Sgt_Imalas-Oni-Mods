using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Defs.Items
{
	public class FossilNoduleConfig : IEntityConfig
	{
		public static string ID = "DecorPackB_FossilNodule";
		public string[] GetDlcIds() => null;

		public GameObject CreatePrefab()
		{
			var prefab = EntityTemplates.CreateLooseEntity(
				ID,
				STRINGS.ITEMS.DECORPACKB_FOSSILNODULE.NAME,
				STRINGS.ITEMS.DECORPACKB_FOSSILNODULE.DESC,
				1f,
				false,
				Assets.GetAnim("dp_fossil_nodule_kanim"),
				"object",
				Grid.SceneLayer.Front,
				EntityTemplates.CollisionShape.RECTANGLE,
				0.8f,
				0.33f,
				true,
				0,
				SimHashes.Lime,
				[
					ModAssets.Tags.BuildingFossilNodule,
					GameTags.PedestalDisplayable
				]);

			return prefab;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
