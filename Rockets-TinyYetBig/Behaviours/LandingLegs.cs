using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Behaviours
{
    class LandingLegs : KMonoBehaviour
    {
        [MyCmpGet]
        Clustercraft clustercraft;
        public string SidescreenButtonText => "Land";

        public string SidescreenButtonTooltip => "Land on planet currently orbited";

        public int ButtonSideScreenSortOrder()
        {
            return 20;
        }

        public void OnSidescreenButtonPressed()
        {
            SgtLogger.debuglog("LAND");
        }

        public bool SidescreenButtonInteractable()
        => clustercraft.GetStableOrbitAsteroid() != null;

        public bool SidescreenEnabled()
        {
            return true;
        }
    } 
}
