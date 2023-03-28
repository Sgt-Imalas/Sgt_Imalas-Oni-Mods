using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EdiblesManager;
using static ResearchTypes;
using UnityEngine;

namespace CannedFoods.Foods
{
    internal class CannedBreadConfig : IEntityConfig
    {
        public const string ID = "CF_CannedBread";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            GameObject prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: STRINGS.ITEMS.FOOD.CF_CANNEDBREAD.NAME,
                desc: STRINGS.ITEMS.FOOD.CF_CANNEDBREAD.DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("canned_bread_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.5f,
                height: 0.7f,
                isPickupable: true,
                sortOrder: 0,
                element: SimHashes.Creature,
                additionalTags: new List<Tag>
                {
                    ModAssets.Tags.DropCanOnEat
                });


            FoodInfo foodInfo = new FoodInfo(
                id: ID,
                dlcId: DlcManager.VANILLA_ID,
                caloriesPerUnit: TUNING.FOOD.FOOD_TYPES.SPICEBREAD.CaloriesPerUnit / 2f,
                quality: TUNING.FOOD.FOOD_TYPES.SPICEBREAD.Quality,
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
            //inst.AddOrGet<CanRecycler>();
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}
