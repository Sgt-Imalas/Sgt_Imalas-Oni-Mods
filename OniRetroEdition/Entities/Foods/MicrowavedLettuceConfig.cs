using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EdiblesManager;

namespace OniRetroEdition.Entities.Foods
{
    internal class MicrowavedLettuceConfig : IEntityConfig
    {
        public const string ID = "MicrowavedLettuce";
        public static ComplexRecipe recipe;
        public static FoodInfo MICROWAVEDLETTUCE;

        public GameObject CreatePrefab()
        {
            MICROWAVEDLETTUCE = new FoodInfo(ID, "", 600000f, 1, 255.15f, 277.15f, 9600f, can_rot: true);

            List<string> effects1 = new List<string>
            {
                "SeafoodRadiationResistance"
            };
            MICROWAVEDLETTUCE = MICROWAVEDLETTUCE.AddEffects(effects1, DlcManager.AVAILABLE_EXPANSION1_ONLY);


            return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(ID, (string)global::STRINGS.ITEMS.FOOD.MICROWAVEDLETTUCE.NAME, (string)global::STRINGS.ITEMS.FOOD.MICROWAVEDLETTUCE.DESC, 1f, false, Assets.GetAnim((HashedString)"microwaved_lettuce_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true), MICROWAVEDLETTUCE);
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
