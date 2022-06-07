using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EdiblesManager;

namespace CannedFoods.Foods
{
    class CannedBBQConfig : IEntityConfig
    {
        public const string ID = "CF_CannedBBQ";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            GameObject prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: STRINGS.ITEMS.FOOD.CF_CANNEDBBQ.NAME,
                desc: STRINGS.ITEMS.FOOD.CF_CANNEDBBQ.DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("canned_bbq_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.4f,
                height: 0.5f,
                isPickupable: true,
                sortOrder: 0,
                element: SimHashes.Creature,
                additionalTags: null);


            FoodInfo foodInfo = new FoodInfo(
                id: ID,
                dlcId: DlcManager.VANILLA_ID,
                caloriesPerUnit: 2000f * 1000f,
                quality: TUNING.FOOD.FOOD_QUALITY_GREAT,
                preserveTemperatue: TUNING.FOOD.DEFAULT_PRESERVE_TEMPERATURE,
                rotTemperature: TUNING.FOOD.DEFAULT_ROT_TEMPERATURE,
                spoilTime: TUNING.FOOD.SPOIL_TIME.VERYSLOW,
                can_rot: false);

            return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
        }

        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public void OnPrefabInit(GameObject inst)
        {
            inst.AddOrGet<CanRecycler>();
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
