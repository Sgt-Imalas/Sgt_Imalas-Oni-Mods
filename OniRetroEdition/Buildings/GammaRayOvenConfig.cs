using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace OniRetroEdition.Buildings
{
    internal class GammaRayOvenConfig : IBuildingConfig
    {
        public const string ID = "GammaRayOven";
        public static EffectorValues DECOR = TUNING.BUILDINGS.DECOR.PENALTY.TIER2;

        public override BuildingDef CreateBuildingDef()
        {
            bool radiationEnabled = DlcManager.IsExpansion1Active() && Config.Instance.GammaRayOvenRadbolts;

            float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] allMetals = TUNING.MATERIALS.ALL_METALS;
            EffectorValues decor = TUNING.BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, radiationEnabled ? 3 : 2, "microbemusher_kanim", 30, 30f, tieR4, allMetals, 800f, BuildLocationRule.OnFloor, decor, noise);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = radiationEnabled ? 60f : 120;
            buildingDef.ExhaustKilowattsWhenActive = 0.5f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;
            buildingDef.PowerInputOffset = new(1, 1);
            buildingDef.LogicInputPorts = new()
            {
                LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, buildingDef.PowerInputOffset, (string) global::STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL, (string)  global::STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, (string)  global::STRINGS.UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE)
            };
            if (radiationEnabled)
            {
                buildingDef.UseHighEnergyParticleInputPort = true;
                buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 2);
            }
            buildingDef.AddLogicPowerPort = true;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Glass";
            buildingDef.AudioSize = "large";
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
            energyParticleStorage.capacity = 200f;
            energyParticleStorage.autoStore = true;
            energyParticleStorage.PORT_ID = "HEP_STORAGE";
            energyParticleStorage.showCapacityStatusItem = true;

            go.AddOrGet<DropAllWorkable>();
            Prioritizable.AddRef(go);
            go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
            go.AddOrGet<Radiator>().intensity = 50;
            GammaRayOven fabricator = go.AddOrGet<GammaRayOven>();
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "anim_interacts_musher_kanim")
            };
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            BuildingTemplates.CreateComplexFabricatorStorage(go, (ComplexFabricator)fabricator);
            this.ConfigureRecipes();
            go.AddOrGetDef<PoweredController.Def>();
        }

        private void ConfigureRecipes()
        {
            bool radiationEnabled = DlcManager.IsExpansion1Active() && Config.Instance.GammaRayOvenRadbolts;
            ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(MushBarConfig.ID.ToTag(), 1f)
            };
            ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[]
            {
                new ComplexRecipe.RecipeElement(GammaMushConfig.ID.ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
            };
            GammaMushConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MicrobeMusher", (IList<ComplexRecipe.RecipeElement>)recipeElementArray1, (IList<ComplexRecipe.RecipeElement>)recipeElementArray2), recipeElementArray1, recipeElementArray2, radiationEnabled ? 10 : 0)
            {
                time = 30f,
                description = global::STRINGS.ITEMS.FOOD.GAMMAMUSH.RECIPEDESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag>()
                {
                    (Tag) ID
                },
                sortOrder = 1
            };

        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
