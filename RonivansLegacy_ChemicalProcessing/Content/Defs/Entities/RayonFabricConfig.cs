using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ITEMS;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals
{
	public class RayonFabricConfig : IEntityConfig
	{
		public static string ID = "RayonFiber";
		public static readonly Tag TAG = TagManager.Create("RayonFiber");
		private AttributeModifier decorModifier = new AttributeModifier("Decor", 0.1f, Strings.Get("INGREDIENTS.RAYONFIBER.NAME"), true, false, true);

		public GameObject CreatePrefab()
		{
			GameObject go = EntityTemplates.CreateLooseEntity(ID, INGREDIENTS.RAYONFIBER.NAME, INGREDIENTS.RAYONFIBER.DESC, 1f, false, Assets.GetAnim("rayon_fiber_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true, 0, SimHashes.Creature, new List<Tag>
			{
				GameTags.IndustrialIngredient,
				GameTags.BuildingFiber
			});
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
