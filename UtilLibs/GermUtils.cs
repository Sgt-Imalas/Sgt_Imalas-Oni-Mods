using Klei.AI.DiseaseGrowthRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    /// <summary>
    /// Credit: Pether.pg - Diseases Expanded
    /// </summary>
    public static class GermUtils
    {// Disease:             0.0, 100, +inf, 1000, +inf, 1000, 0.001, 1
        // FoodGerms:           2.6, 0.4, 12000, 1000, 3000, 1000, 0.001, 1
        // PollenGerms:         0.6, 0.4, 3000, 500, 10, 3000, 0.001, 1
        // RadiationPoisoning:  0.0, 0.0, 600, +inf, 600, 10000, 0.000, 1
        // SlimeGerms:          2.6, 0.4, 12000, 500, 1200, 1000, 0.001, 1
        // ZombieSpores:        2.6, 0.4, 12000, 500, 1200, 1000, 0.001, 1
        public static GrowthRule GrowthRule(float f_underPopulationDeathRate, float f_minCountPerKG, float f_populationHalfLife,
            float f_maxCountPerKG, float f_overPopulationHalfLife, int i_minDiffusionCount, float f_diffusionScale, byte b_minDiffusionInfestationTickCount)
        {
            return new GrowthRule()
            {
                underPopulationDeathRate = new float?(f_underPopulationDeathRate),
                minCountPerKG = new float?(f_minCountPerKG),
                populationHalfLife = new float?(f_populationHalfLife),
                maxCountPerKG = new float?(f_maxCountPerKG),
                overPopulationHalfLife = new float?(f_overPopulationHalfLife),
                minDiffusionCount = new int?(i_minDiffusionCount),
                diffusionScale = new float?(f_diffusionScale),
                minDiffusionInfestationTickCount = new byte?(b_minDiffusionInfestationTickCount)
            };
        }

        public static GrowthRule GrowthRule_Default()
        {
            return GrowthRule(2.666667f, 0.4f, 12000, 500, 1200, 1000, 0.001f, 1);
        }

        public static StateGrowthRule StateGrowthRule_diffScale_minDiffCount(Element.State state, float minCountPerKG, float populationHalfLife,
            float overPopulationHalfLife, float diffusionScale, int minDiffusionCount)
        {
            StateGrowthRule stateGrowthRule = new StateGrowthRule(state);
            stateGrowthRule.minCountPerKG = new float?(minCountPerKG);
            stateGrowthRule.populationHalfLife = new float?(populationHalfLife);
            stateGrowthRule.overPopulationHalfLife = new float?(overPopulationHalfLife);

            stateGrowthRule.diffusionScale = new float?(diffusionScale);
            stateGrowthRule.minDiffusionCount = new int?(minDiffusionCount);

            return stateGrowthRule;
        }

        public static StateGrowthRule StateGrowthRule_maxPerKg_diffScale_minDiffCount(Element.State state, float minCountPerKG, float populationHalfLife,
            float overPopulationHalfLife, float maxCountPerKG, float diffusionScale, int minDiffusionCount)
        {
            StateGrowthRule stateGrowthRule = new StateGrowthRule(state);
            stateGrowthRule.minCountPerKG = new float?(minCountPerKG);
            stateGrowthRule.populationHalfLife = new float?(populationHalfLife);
            stateGrowthRule.overPopulationHalfLife = new float?(overPopulationHalfLife);

            stateGrowthRule.maxCountPerKG = new float?(maxCountPerKG);
            stateGrowthRule.diffusionScale = new float?(diffusionScale);
            stateGrowthRule.minDiffusionCount = new int?(minDiffusionCount);

            return stateGrowthRule;
        }

        public static StateGrowthRule StateGrowthRule_maxPerKg_DiffScale(Element.State state, float minCountPerKG, float populationHalfLife,
            float overPopulationHalfLife, float maxCountPerKG, float diffusionScale)
        {
            StateGrowthRule stateGrowthRule = new StateGrowthRule(state);
            stateGrowthRule.minCountPerKG = new float?(minCountPerKG);
            stateGrowthRule.populationHalfLife = new float?(populationHalfLife);
            stateGrowthRule.overPopulationHalfLife = new float?(overPopulationHalfLife);

            stateGrowthRule.maxCountPerKG = new float?(maxCountPerKG);
            stateGrowthRule.diffusionScale = new float?(diffusionScale);

            return stateGrowthRule;
        }

        public static ElementGrowthRule ElementGrowthRule(SimHashes element, float? updr, float? phl, float? ophl,
            float? ds, float? min, float? max, int? mdc, byte? mditc)
        {
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.underPopulationDeathRate = updr;
            elementGrowthRule.populationHalfLife = phl;
            elementGrowthRule.overPopulationHalfLife = ophl;
            elementGrowthRule.diffusionScale = ds;
            elementGrowthRule.minCountPerKG = min;
            elementGrowthRule.maxCountPerKG = max;
            elementGrowthRule.minDiffusionCount = mdc;
            elementGrowthRule.minDiffusionInfestationTickCount = mditc;

            return elementGrowthRule;
        }


        public static ElementGrowthRule DieInElement(SimHashes element, float scale = 1)
        {
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.populationHalfLife = 10 / scale;
            elementGrowthRule.overPopulationHalfLife = 10 / scale;
            elementGrowthRule.minDiffusionCount = (int)(100000 * scale);
            elementGrowthRule.diffusionScale = 0.001f;

            return elementGrowthRule;
        }

        public static ElementGrowthRule SurviveInElement(SimHashes element)
        {
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.underPopulationDeathRate = new float?(0.0f);
            elementGrowthRule.populationHalfLife = new float?(float.PositiveInfinity);
            elementGrowthRule.overPopulationHalfLife = new float?(6000f);
            return elementGrowthRule;
        }

        public static ElementGrowthRule SurviveAndSpreadInElement(SimHashes element)
        {
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.underPopulationDeathRate = new float?(0.0f);
            elementGrowthRule.populationHalfLife = new float?(float.PositiveInfinity);
            elementGrowthRule.overPopulationHalfLife = new float?(3000f);
            elementGrowthRule.maxCountPerKG = new float?(1000f);
            elementGrowthRule.diffusionScale = new float?(0.005f);
            return elementGrowthRule;
        }

        public static ElementGrowthRule ThriveInElement(SimHashes element, float scale = 1)
        {
            // For negative half-life, the smaller abslolute value, the faster growth
            // Therefore, scaling up requires making the absolute value smaller
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.underPopulationDeathRate = 0;
            elementGrowthRule.populationHalfLife = -3000f / scale;
            elementGrowthRule.overPopulationHalfLife = 3000f * scale;
            return elementGrowthRule;
        }

        public static ElementGrowthRule ThriveAndSpreadInElement(SimHashes element, float scale = 1)
        {
            // For negative half-life, the smaller abslolute value, the faster growth
            // Therefore, scaling up requires making the absolute value smaller
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.underPopulationDeathRate = 0;
            elementGrowthRule.populationHalfLife = -3000f / scale;
            elementGrowthRule.overPopulationHalfLife = 3000f * scale;
            elementGrowthRule.maxCountPerKG = 5000f * scale;
            elementGrowthRule.diffusionScale = 0.05f;
            return elementGrowthRule;
        }

        public static ElementExposureRule KillingExposure(SimHashes element, float scale = 1)
        {
            ElementExposureRule elementExposureRule = new ElementExposureRule(element);
            elementExposureRule.populationHalfLife = new float?(10f / scale);
            return elementExposureRule;
        }

        public static ElementGrowthRule GrowthLike_FoodPoison_PollutedOxygen(SimHashes element)
        {
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.populationHalfLife = new float?(12000f);
            elementGrowthRule.maxCountPerKG = new float?(10000f);
            elementGrowthRule.overPopulationHalfLife = new float?(3000f);
            elementGrowthRule.diffusionScale = new float?(0.05f);
            return elementGrowthRule;
        }

        public static ElementGrowthRule GrowthLike_FoodPoison_PollutedWater(SimHashes element)
        {
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.populationHalfLife = new float?(-12000f);
            elementGrowthRule.overPopulationHalfLife = new float?(12000f);
            return elementGrowthRule;
        }

        public static TagGrowthRule GrowthLike_FoodPoison_Edible(Tag tag)
        {
            TagGrowthRule tagGrowthRule = new TagGrowthRule(tag);
            tagGrowthRule.populationHalfLife = new float?(-12000f);
            tagGrowthRule.overPopulationHalfLife = new float?(float.PositiveInfinity);
            return tagGrowthRule;
        }

        public static ElementGrowthRule GrowthLike_Slimelung_PollutedOxygen(SimHashes element)
        {
            ElementGrowthRule elementGrowthRule = new ElementGrowthRule(element);
            elementGrowthRule.underPopulationDeathRate = new float?(0.0f);
            elementGrowthRule.populationHalfLife = new float?(-300f);
            elementGrowthRule.overPopulationHalfLife = new float?(1200f);
            return elementGrowthRule;
        }

        public static ElementGrowthRule GrowthLike_Slimelung_Oxygen(SimHashes element)
        {
            ElementGrowthRule elementGrowthRule4 = new ElementGrowthRule(element);
            elementGrowthRule4.populationHalfLife = new float?(1200f);
            elementGrowthRule4.overPopulationHalfLife = new float?(10f);
            return elementGrowthRule4;
        }

        public static ElementExposureRule ExposureLike_Slimelung_PollutedOxygen(SimHashes element)
        {
            ElementExposureRule elementExposureRule = new ElementExposureRule(element);
            elementExposureRule.populationHalfLife = new float?(-12000f);
            return elementExposureRule;
        }

        public static ElementExposureRule ExposureLike_Anything_ChlorineGas(SimHashes element)
        {
            ElementExposureRule elementExposureRule = new ElementExposureRule(element);
            elementExposureRule.populationHalfLife = new float?(10f);
            return elementExposureRule;
        }

        public static StateGrowthRule StateLike_Slimelung_Gas(Element.State state)
        {
            StateGrowthRule stateGrowthRule = new StateGrowthRule(state);
            stateGrowthRule.minCountPerKG = new float?(250f);
            stateGrowthRule.populationHalfLife = new float?(12000f);
            stateGrowthRule.overPopulationHalfLife = new float?(1200f);
            stateGrowthRule.maxCountPerKG = new float?(10000f);
            stateGrowthRule.minDiffusionCount = new int?(5100);
            stateGrowthRule.diffusionScale = new float?(0.005f);
            return stateGrowthRule;
        }


    }
}
