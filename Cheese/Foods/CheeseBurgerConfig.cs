using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Cheese.STRINGS;

namespace Cheese.Entities
{
    internal class CheeseBurgerConfig : IEntityConfig
    {
        public const string ID = "CheeseBurger";
        public static ComplexRecipe recipe;

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(ID, ITEMS.FOOD.CHEESEBURGER.NAME, ITEMS.FOOD.CHEESEBURGER.DESC, 1f, false, Assets.GetAnim((HashedString)"frost_cheeseburger_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true), ModAssets.Foods.CheeseBurger);

        public void OnPrefabInit(GameObject inst)
        {
            if(inst.TryGetComponent<KPrefabID>(out var id))
            {
                id.AddTag(ModAssets.Tags.BrackeneProduct);
            }
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
