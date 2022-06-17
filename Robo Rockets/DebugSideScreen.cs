using KnastoronOniMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboRockets
{
    class DebugSideScreen : KMonoBehaviour, ISidescreenButtonControl
    {
        [MyCmpReq]
        private RocketControlStationNoChorePrecondition AIController;

        public string SidescreenTitle => "AI Control";

        public string SidescreenStatusMessage => "";

        public void OnSidescreenButtonPressed()
        {
            if (Config.Instance.DebugFunctionsEnabled)
            {
                AIController.MakeNewPilotBot(true);
            }
        }

        public string SidescreenButtonText => "Reset AI";

        public string SidescreenButtonTooltip => "Reset AI State";

        public bool SidescreenEnabled() => true;

        public bool SidescreenButtonInteractable() => true;

        public int ButtonSideScreenSortOrder() => 20;
    }
}
