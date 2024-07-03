using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.Entities.Foods
{
    internal class PopCornConfig : IEntityConfig
    {
        public const string ID = "PopCorn";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            EdiblesManager.FoodInfo POPCORN = new EdiblesManager.FoodInfo(ID, "", 1100000f, 2, 255.15f, 277.15f, 9600f, can_rot: true);

            return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(ID, (string)global::STRINGS.ITEMS.FOOD.POPCORN.NAME, (string)global::STRINGS.ITEMS.FOOD.POPCORN.DESC, 1f, false, Assets.GetAnim((HashedString)"zestysalsa_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true), POPCORN);
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
