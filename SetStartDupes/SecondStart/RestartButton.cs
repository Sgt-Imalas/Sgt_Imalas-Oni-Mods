using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace SetStartDupes.SecondStart
{
    internal class RestartButton : KMonoBehaviour, ISidescreenButtonControl
    {
        [Serialize]
        bool RestartClicked = false;

        public string SidescreenButtonText => STRINGS.UI.STARTAGAIN.SIDESCREEN_TEXT;

        public string SidescreenButtonTooltip => STRINGS.UI.STARTAGAIN.SIDESCREEN_TOOLTIP;

        public int ButtonSideScreenSortOrder() => 800;

        public int HorizontalGroupID() => -1;

        public void OnSidescreenButtonPressed()
        {
            SgtLogger.l("dostoff"); RestartClicked = true;
        }

        public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
        {
        }

        public bool SidescreenButtonInteractable() => !RestartClicked;

        public bool SidescreenEnabled() => GameClock.Instance!=null && GameClock.Instance.GetTimePlayedInSeconds() < 300;
    }
}
