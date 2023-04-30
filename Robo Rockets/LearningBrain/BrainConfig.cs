using RoboRockets;
using RoboRockets;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ComplexRecipe;

namespace RoboRockets.LearningBrain
{
    class BrainConfig : IEntityConfig
    {
        public const string ID = "RR_BrainFlyer";
        public static ComplexRecipe recipe;
        public static RecipeElement[] ProductionCosts = new RecipeElement[]
        {
            new RecipeElement(SimHashes.Glass.CreateTag(),200f),
            new RecipeElement(SimHashes.Steel.CreateTag(),100f),
            new RecipeElement(SimHashes.Ethanol.CreateTag(), 360f),
            new RecipeElement(GeneShufflerRechargeConfig.tag, 1f)
        };

        public GameObject CreatePrefab()
        {
            GameObject prefab = EntityTemplates.CreateLooseEntity(
                id: ID,
                name: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.NAME,
                desc: STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RR_BRAINFLYER.DESC,
                mass: 50f,
                unitMass: true,
                anim: Assets.GetAnim("brain_item_kanim"),
                initialAnim: "object",
                sceneLayer: Grid.SceneLayer.Front,
                collisionShape: EntityTemplates.CollisionShape.RECTANGLE,
                width: 1.9f,
                height: 1.9f,
                isPickupable: true,
                sortOrder: 0,
                element: SimHashes.Creature,
                additionalTags: new List<Tag>()
                {
                    GameTags.IndustrialIngredient,
                    ModAssets.Tags.SpaceBrain,
                    GameTags.PedestalDisplayable
                });

            prefab.AddOrGet<UserNameable>();
            prefab.AddComponent<FlyingBrain>(); 
            prefab.AddOrGet<DemolishableDroppable>();
            prefab.AddOrGet<OccupyArea>().OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(1, 1); 
            prefab.AddOrGet<CharacterOverlay>().shouldShowName = true;

            return prefab;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {

        }
    }
}
