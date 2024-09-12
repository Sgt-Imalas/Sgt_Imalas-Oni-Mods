namespace CritterTraitsReborn.Traits
{
	class Tiny : TraitBuilder
	{
		public override string ID => "CritterTiny";

		public override Group Group => Group.GetGroup(Group.SizeGroupId);

		protected override void Init()
		{
			TraitHelpers.CreateScaleTrait(ID, 0.6f);
		}
	}
}
