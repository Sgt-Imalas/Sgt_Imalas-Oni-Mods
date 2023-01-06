using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSatellites.Behaviours
{
    public interface ISatelliteCarrier
    {
        IStateMachineTarget master { get; }

        bool ModeIsDeployment { get; set; }

        bool HoldingSatellite();
        bool CanRetrieveSatellite();
        bool CanDeploySatellite(int type);

        void TryDeploySatellite(int type);
        void TryRetrieveSatellite();
    }
}
