using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Entities
{
	internal class BioPlasticGasketConfig : IEntityConfig
	{
		public static readonly string ID = "AIO_BioplasticGasket";
		public static readonly Tag tag = TagManager.Create(ID);

		public GameObject CreatePrefab()
		{
			GameObject looseEntity = EntityTemplates.CreateLooseEntity(ID, 
				STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.AIO_BIOPLASTICGASKET.NAME, 
				STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.AIO_BIOPLASTICGASKET.DESC, 1f, true, Assets.GetAnim("aio_bioplastic_gasket_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, additionalTags: [
			  GameTags.IndustrialProduct,
			  GameTags.MiscPickupable,
			  GameTags.PedestalDisplayable,
			  GameTags.BuildingGasket
			]);
			looseEntity.AddOrGet<EntitySplitter>();
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
