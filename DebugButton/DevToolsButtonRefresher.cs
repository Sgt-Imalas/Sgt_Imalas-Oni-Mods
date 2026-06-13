using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DebugButton
{
	internal class DevToolsButtonRefresher : KMonoBehaviour, IRender200ms
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
				toolTip.SetSimpleTooltip(STRINGS.UI.TOOLS.DEV_TOOLS.TOOLTIP_TOGGLE);

				if (DevToolManager.Instance.showImGui)
				{
					toggle.ChangeState(2);
				}
				else
				{
					toggle.ChangeState(1);
				}
			}
		}
	}
}
