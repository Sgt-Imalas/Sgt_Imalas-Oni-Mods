using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DebugButton
{
	internal class SpeedButtonRefresher : KMonoBehaviour, IRender200ms
	{
		[MyCmpReq] MultiToggle toggle;
		[MyCmpReq] ToolTip toolTip;

		public void Render200ms(float dt)
		{
			RefreshToggleState();
		}
		public void RefreshToggleState()
		{
			if (!DebugHandler.enabled)
			{
				toggle.ChangeState(0);
				toolTip.SetSimpleTooltip(STRINGS.UI.TOOLS.TOOLTIP_DEBUG_LOCKED);
			}
			else
			{
				toolTip.SetSimpleTooltip(GameUtil.ReplaceHotkeyString(STRINGS.UI.TOOLS.DEBUG_SUPERSPEED_TOGGLE.TOOLTIP_TOGGLE, Action.DebugUltraTestMode));

				if (Time.timeScale > 8f)
				{
					toggle.ChangeState(2);
				}
				else
				{
					toggle.ChangeState(1);
					if (Patches._debugHandler != null && Patches._debugHandler.ultraTestMode)
						Patches._debugHandler.ultraTestMode = false;
				}
			}
		}
	}
}
