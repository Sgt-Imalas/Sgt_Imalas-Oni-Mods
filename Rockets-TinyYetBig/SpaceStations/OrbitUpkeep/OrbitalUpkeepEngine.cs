using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.SpaceStations.OrbitUpkeep
{
    internal class OrbitalUpkeepEngine : KMonoBehaviour, ISim1000ms
    {
        [Serialize]
        Ref<OrbitalUpkeepObject> worldUpkeepMng;
        [MyCmpGet]
        Operational operational;

        [MyCmpGet]
        Storage storage;

        public readonly CellOffset[] tier1Engine =
        {
            new CellOffset(0,-1),
        };



        public Tag fuelElement = SimHashes.Void.CreateTag();
        public float consumptionRate = 1f;

        public SimHashes exhaustElement = SimHashes.CarbonDioxide;
        public float exhaustProductionRate = 10f;
        public float exhaustProductionTemperature = 293.15f;


        public void Sim1000ms(float dt)
        {
            if (operational.IsOperational && this.HasFuelForBurn())
            {
                this.ConsumeFuel();
                this.ProvideUpkeep();
            }
        }

        private bool HasFuelForBurn()
        {
            throw new NotImplementedException();
        }

        private void ConsumeFuel()
        {

        }

        private void ProvideUpkeep()
        {
            throw new NotImplementedException();
        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            var worldUpkeepMng = ClusterManager.Instance.GetMyWorld().GetSMI<OrbitalUpkeepObject>();

        }
    }
}
