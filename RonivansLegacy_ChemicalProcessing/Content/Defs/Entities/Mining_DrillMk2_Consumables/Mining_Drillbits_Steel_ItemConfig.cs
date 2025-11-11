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
	public class Mining_Drillbits_Steel_ItemConfig : IEntityConfig
	{
		public static string ID = "Mining_Drillbits_Steel_Item";
		public static readonly Tag TAG = TagManager.Create(ID);

		public GameObject CreatePrefab()
		{
			GameObject go = EntityTemplates.CreateLooseEntity(ID, MINING_DRILLBITS_STEEL_ITEM.NAME, MINING_DRILLBITS_STEEL_ITEM.DESC, 1f, true, Assets.GetAnim("drillbits_steel_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.35f, 0.35f, true, 0, SimHashes.Creature, [GameTags.IndustrialProduct,ModAssets.Tags.RandomRecipeIngredient_DestroyOnCancel, ModAssets.Tags.MineralProcessing_Drillbit]);
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
