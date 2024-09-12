using KSerialization;

namespace SkillingQueue
{
	internal class SkillQueueCompleteMessage : Message
	{
		[Serialize]
		public string minionName;

		public SkillQueueCompleteMessage(MinionResume resume)
		{
			this.minionName = resume.GetProperName();
		}
		public override string GetMessageBody() => "MessageBody";

		public override string GetSound() => null;
		public override bool ShowDialog() => false;

		public override string GetTitle() => string.Format(STRINGS.SKILLQUEUE_STATUSITEMS.SQ_QUEUE_COMPLETED.NAME, minionName);

		public override string GetTooltip() => string.Format(STRINGS.SKILLQUEUE_STATUSITEMS.SQ_QUEUE_COMPLETED.DESCRIPTION, minionName);
		public override void OnClick()
		{
			base.OnClick();
		}
	}
}
