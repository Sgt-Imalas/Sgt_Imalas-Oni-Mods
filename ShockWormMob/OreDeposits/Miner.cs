using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClusterMapRocketAnimator.UtilityStates;
using UnityEngine;
using KSerialization;

namespace ShockWormMob.OreDeposits
{
    public class Miner : KMonoBehaviour
    {
        [MyCmpGet] ElementConverter converter;
        [MyCmpGet] AttachableBuilding attatchableBuilding;

        [Serialize] Dictionary<SimHashes, float> MiningRates;
        [Serialize] public float baseMiningEff;

        public override void OnCleanUp()
        {
            CleaningUpOldAccumulators();
            base.OnCleanUp();
        }
        public override void OnSpawn()
        {
            SetMiningProductionRates();
        }

        public void SetMiningProductionRates()
        {
            foreach (var deposit in AttachableBuilding.GetAttachedNetwork(attatchableBuilding))
            {
                if (deposit.TryGetComponent(out OreDeposit depositImAttachedTo))
                {
                    this.MiningRates = depositImAttachedTo.getMiningRates();
                }
                foreach (SimHashes element in MiningRates.Keys)
                {
                    float rate = MiningRates[element];
                    float miningSpeed = baseMiningEff * rate;
                    converter.outputElements = new ElementConverter.OutputElement[]
                    {
                        new ElementConverter.OutputElement(miningSpeed, element, 363.15f, storeOutput: true, diseaseWeight: 0.0f)
                    };
                }
            }
            //this.CreatingNewAccumulators();
        }


        #region Accumulator
        private void CreatingNewAccumulators()
        {

            for (int i = 0; i < converter.consumedElements.Length; i++)
            {
                converter.consumedElements[i].Accumulator = Game.Instance.accumulators.Add("ElementsConsumed", converter);
            }

            for (int j = 0; j < converter.outputElements.Length; j++)
            {
                converter.outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", converter);
            }
        }
        private void CleaningUpOldAccumulators()
        {
            for (int i = converter.consumedElements.Length - 1; i >= 0; i--)
            {
                Game.Instance.accumulators.Remove(converter.consumedElements[i].Accumulator);
            }

            for (int i = converter.outputElements.Length - 1; i >= 0; i--)
            {
                Game.Instance.accumulators.Remove(converter.outputElements[i].accumulator);
            }
        }
        #endregion
    }
}
