using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LogicSatellites.Behaviours.ModAssets;

namespace LogicSatellites.Behaviours
{
    class SatelliteSelection : KMonoBehaviour, ISidescreenButtonControl
    {
        [Serialize]
        public int SatelliteType = 0;
        [MyCmpGet]
        SatelliteCarrierModule module;

        public int NextSat()
        {
            var awailables = UpdateUnlockedSatellites();
            if (awailables.Count <= 1)
                return SatelliteType;
            else
            {
                ++SatelliteType;
                SatelliteType %= awailables.Count;
                return SatelliteType;
            }    
        }

        List<int> UpdateUnlockedSatellites()
        {
            var list = new List<int>();

            if (ExplorationSatellite.ParentTech.IsComplete())
                list.Add((int)SatType.Exploration);
            if (SolarSatellite.ParentTech.IsComplete())
                list.Add((int)SatType.SolarLens);

            return list;

        }

        public string SidescreenButtonText => !module.smi.HoldingSatellite() ? ((SatType)SatelliteType).ToString()+ " Satellite" : "Scrap Satellite";

        public string SidescreenButtonTooltip => !module.smi.HoldingSatellite() ? "Cycle through the different types of satellites": "Scrap the current satellite to retrieve the parts";

        public int ButtonSideScreenSortOrder() => 21;

        public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();
        public void OnSidescreenButtonPressed()
        {
            if (!module.smi.HoldingSatellite())
            {

                //NextSat();
            }
            else
            {
                //Debug.Log("EJECTING?");
                module.smi.EjectParts();

            }
        }

        public bool SidescreenButtonInteractable() => true;

        public bool SidescreenEnabled() => true;
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

        }
    }
}
