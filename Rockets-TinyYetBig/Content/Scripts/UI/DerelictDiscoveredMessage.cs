using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.UI
{
	internal class DerelictDiscoveredMessage : Message
	{
		ClusterGridEntity Target;
		public DerelictDiscoveredMessage() { }
		public DerelictDiscoveredMessage(ClusterGridEntity clusterGridEntity) => Target = clusterGridEntity;

		public override string GetMessageBody() => STRINGS.UI.RTB_DERELICTDISCOVERED.TOOLTIP;

		public override string GetSound() => "AI_Notification_ResearchComplete";

		public override string GetTitle() => STRINGS.UI.RTB_DERELICTDISCOVERED.NAME;

		public override string GetTooltip() => GetMessageBody();
		public override void OnClick()
		{
			base.OnClick();
			RocketryUtils.SelectStarmapEntity(Target);
		}
	}
}
