using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EdiblesManager;

namespace CannedFoods.Foods
{
    public class CannedTunaConfig : IEntityConfig
    {
        public const string ID = "CF_CannedTuna";
        public static ComplexRecipe recipe;

        public GameObject CreatePrefab()
        {
            GameObject prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: STRINGS.ITEMS.FOOD.CF_CANNEDFISH.NAME,
                desc: STRINGS.ITEMS.FOOD.CF_CANNEDFISH.DESC,
                mass: 1f,
                unitMass: false,
                anim: Assets.GetAnim("canned_tuna_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 0.9f,
                height: 0.45f,
                isPickupable: true,
                sortOrder: 0,
                element: SimHashes.Creature,
                additionalTags: null);


            FoodInfo foodInfo = new FoodInfo(
                id: ID,
                dlcId: DlcManager.VANILLA_ID,
                caloriesPerUnit: 800 * 1000f,
                quality: TUNING.FOOD.FOOD_QUALITY_GREAT,
                preserveTemperatue: TUNING.FOOD.DEFAULT_PRESERVE_TEMPERATURE,
                rotTemperature: TUNING.FOOD.DEFAULT_ROT_TEMPERATURE,
                spoilTime: TUNING.FOOD.SPOIL_TIME.VERYSLOW,
                can_rot: false).AddEffects(new List<string>
                {
                    "SeafoodRadiationResistance"
                }, DlcManager.AVAILABLE_EXPANSION1_ONLY);

            return EntityTemplates.ExtendEntityToFood(prefab, foodInfo);
        }

        public string[] GetDlcIds()
        {
            return DlcManager.AVAILABLE_ALL_VERSIONS;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}

