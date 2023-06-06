using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UL_UniversalLyzer
{
    public class ModAssets
    {
        public static Dictionary<SimHashes, float> ElectrolyzerPowerCosts = new Dictionary<SimHashes, float>();

        public static void InitializeLyzerPowerCosts()
        {
            AddPowerCost(SimHashes.Water, Config.Instance.consumption_water);
            AddPowerCost(SimHashes.DirtyWater, Config.Instance.consumption_pollutedwater);
            AddPowerCost(SimHashes.SaltWater, Config.Instance.consumption_saltwater);
            AddPowerCost(SimHashes.Brine, Config.Instance.consumption_brine);
        }

        public static float GetWattageForElement(SimHashes element)
        {
            if (ElectrolyzerPowerCosts.ContainsKey(element))
            {
                return ElectrolyzerPowerCosts[element];
            }
            return ElectrolyzerPowerCosts[SimHashes.Water];

        }
        public static void AddPowerCost(SimHashes element, float power)
        {
            ElectrolyzerPowerCosts[element] = power;
        }

    }
}
