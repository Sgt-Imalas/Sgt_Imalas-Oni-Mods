using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketryExpanded
{
    public class BombSideScreen : KMonoBehaviour, ISidescreenButtonControl
    {
        [MyCmpReq]
        private ExplosiveBomblet bomb;

        public string SidescreenTitleKey => "Bomb.STRINGS.UI.BIGBOMB.SIDESCREEN.TITLE";

        public string SidescreenStatusMessage => "";

        public void OnSidescreenButtonPressed()
        {
            bomb.Explode();
        }

        public string SidescreenButtonText => "Detonate Bomb";

        public string SidescreenButtonTooltip => "boom";

        public bool SidescreenEnabled() => true;

        public bool SidescreenButtonInteractable() => true;

        public int ButtonSideScreenSortOrder() => 20;
    }
}
