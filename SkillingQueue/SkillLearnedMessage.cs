using Database;
using KSerialization;

namespace SkillingQueue
{
	internal class SkillLearnedMessage : Message
	{
		[Serialize]
		public string minionName;
		[Serialize]
		private ResourceRef<Skill> Skill = new ResourceRef<Skill>();

		public SkillLearnedMessage(MinionResume resume, Skill skill)
		{
			this.minionName = resume.GetProperName();
			this.Skill = new ResourceRef<Skill>(skill);
		}
		public override string GetMessageBody() => "MessageBody";

		public override bool ShowDialog() => false;
		public override string GetSound() => null;

		public override string GetTitle() => string.Format(STRINGS.SKILLQUEUE_STATUSITEMS.SQ_NEW_SKILL_LEARNED.NAME, Skill.Get().Name);

		public override string GetTooltip() => string.Format(STRINGS.SKILLQUEUE_STATUSITEMS.SQ_NEW_SKILL_LEARNED.DESCRIPTION, minionName, Skill.Get().Name);
		public override void OnClick()
		{
			base.OnClick();
		}
		public override bool IsValid() => this.Skill.Get() != null;
	}
}
