using RonivansLegacy_ChemicalProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS.INDUSTRIAL_PRODUCTS;

namespace Mineral_Processing_Mining.Buildings
{
	public class Mining_Drillbits_Tungsten_ItemConfig : IEntityConfig
	{
		public static string ID = "Mining_Drillbits_Tungsten_Item";
		public static readonly Tag TAG = TagManager.Create(ID,"TungstenBit");

		public GameObject CreatePrefab()
		{
			GameObject go = EntityTemplates.CreateLooseEntity(ID, MINING_DRILLBITS_TUNGSTEN_ITEM.NAME, MINING_DRILLBITS_TUNGSTEN_ITEM.DESC, 1f, false, Assets.GetAnim("drillbits_tungsten_kanim"), "object", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true, 0, SimHashes.Creature, [GameTags.IndustrialProduct, ModAssets.Tags.RandomRecipeIngredient_DestroyOnCancel]);
			go.AddOrGet<EntitySplitter>();
			go.AddOrGet<SimpleMassStatusItem>();
			return go;
		}

		public string[] GetDlcIds() => null;

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}
