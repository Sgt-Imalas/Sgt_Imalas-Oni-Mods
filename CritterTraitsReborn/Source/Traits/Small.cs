namespace CritterTraitsReborn.Traits
{
	class Small : TraitBuilder
	{
		public override string ID => "CritterSmall";

		public override Group Group => Group.GetGroup(Group.SizeGroupId);

		protected override void Init()
		{
			TraitHelpers.CreateScaleTrait(ID, 0.8f);
		}
	}
}
