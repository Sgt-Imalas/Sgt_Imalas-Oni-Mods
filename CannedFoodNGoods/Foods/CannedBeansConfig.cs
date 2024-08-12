using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EdiblesManager;
using UnityEngine;

namespace CannedFoods.Foods
{
    internal class CannedBeansConfig : IEntityConfig
    {
        public const string ID = "CF_CannedBeans";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            GameObject prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: STRINGS.ITEMS.FOOD.CF_CANNEDBEANS.NAME,
                desc: STRINGS.ITEMS.FOOD.CF_CANNEDBEANS.DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("canned_beans_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.7f,
                height: 0.35f,
                isPickupable: true,
                sortOrder: 0,
                element: SimHashes.Creature,
                additionalTags: new List<Tag>
                {
                    ModAssets.Tags.DropCanOnEat
                });


            FoodInfo foodInfo = new FoodInfo(
                id: ID,
                dlcId: DlcManager.DLC2_ID,
                caloriesPerUnit: TUNING.FOOD.FOOD_TYPES.DEEP_FRIED_NOSH.CaloriesPerUnit / 2f,
                quality: TUNING.FOOD.FOOD_TYPES.DEEP_FRIED_NOSH.Quality,
                preserveTemperatue: TUNING.FOOD.DEFAULT_PRESERVE_TEMPERATURE,
                rotTemperature: TUNING.FOOD.DEFAULT_ROT_TEMPERATURE,
                spoilTime: TUNING.FOOD.SPOIL_TIME.VERYSLOW,
                can_rot: false);

            return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
        }

        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_DLC_2;
        }

        public void OnPrefabInit(GameObject inst)
        {
            //inst.AddOrGet<CanRecycler>();
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
