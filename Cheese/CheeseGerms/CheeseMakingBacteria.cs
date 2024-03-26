using Cheese.ModElements;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Cheese.CheeseGerms
{
    internal class CheeseMakingBacteria : Disease
    {
        public const string ID = nameof(CheeseMakingBacteria);
        public float UVHalfLife { get; private set; } // for Romen's UV Lamp mod, do not rename from UVHalfLife

        public CheeseMakingBacteria(bool statsOnly) : base(
            id: ID,
            strength: 20,
            temperature_range: new Disease.RangeInfo(248.15f, 278.15f, 313.15f, 348.15f),
            temperature_half_lives: new Disease.RangeInfo(10f, 1200f, 1200f, 10f),
            pressure_range: new Disease.RangeInfo(0.0f, 0.0f, 1000f, 1000f),
            pressure_half_lives: Disease.RangeInfo.Idempotent(),
            1.0f,
            statsOnly)
        {
            UVHalfLife = 10;
        }
        /// <summary>
        /// Credit: pether.pg diseases expanded
        /// </summary>
        public override void PopulateElemGrowthInfo()
        {
            this.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
            
            this.AddGrowthRule(GermUtils.GrowthRule_Default());

            this.AddGrowthRule(GermUtils.ThriveAndSpreadInElement(ModElementRegistration.CheeseMolten));
            this.AddGrowthRule(GermUtils.ThriveAndSpreadInElement(ModElementRegistration.Cheese));
            this.AddGrowthRule(GermUtils.ThriveAndSpreadInElement(SimHashes.Milk));
            this.AddGrowthRule(GermUtils.ThriveAndSpreadInElement(SimHashes.Brine));
            this.AddGrowthRule(GermUtils.ThriveAndSpreadInElement(SimHashes.SaltWater));
            this.AddGrowthRule(GermUtils.ThriveInElement(SimHashes.SlimeMold));
            this.AddGrowthRule(GermUtils.SurviveInElement(SimHashes.Water));

            this.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
            //this.AddExposureRule(new ElementExposureRule(ModElementRegistration.CheeseMolten)
            //{
            //    populationHalfLife = float.PositiveInfinity
            //}); 
            //this.AddExposureRule(new ElementExposureRule(SimHashes.Milk)
            //{
            //    populationHalfLife = float.PositiveInfinity
            //});
        }
    }
}
