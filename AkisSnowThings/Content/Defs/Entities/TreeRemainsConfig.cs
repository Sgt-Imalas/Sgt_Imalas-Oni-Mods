using AkisSnowThings.Content.Defs.Plants;
using AkisSnowThings.Content.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static AkisSnowThings.STRINGS;

namespace AkisSnowThings.Content.Defs.Entities
{
	/// <summary>
	/// pine tree drop filler item as only one plant can make one resource and normal trees occupy wood
	/// </summary>
	internal class TreeRemainsConfig : IEntityConfig
	{
		public const string ID = "SnowSculptures_PineTreeRemains";

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public GameObject CreatePrefab()
		{
			GameObject looseEntity = EntityTemplates.CreateLooseEntity(ID,
				CREATURES.SPECIES.SNOWSCULPTURES_EVERGREEN_TREE.REMAINS.NAME, 
				CREATURES.SPECIES.SNOWSCULPTURES_EVERGREEN_TREE.REMAINS.DESC, 
				1, false, Assets.GetAnim((HashedString)"wood_kanim"),
				"object", 
				Grid.SceneLayer.Front, 
				EntityTemplates.CollisionShape.CIRCLE, 
				0.5f, 
				0.5f,
				true,
				0,
				SimHashes.WoodLog);
			looseEntity.AddOrGet<EntitySplitter>();
			looseEntity.AddComponent<ResourceDropper>();
			return looseEntity;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
