namespace CritterTraitsReborn.Traits
{
	class Large : TraitBuilder
	{
		public override string ID => "CritterLarge";

		public override Group Group => Group.GetGroup(Group.SizeGroupId);

		protected override void Init()
		{
			TraitHelpers.CreateScaleTrait(ID, 1.25f);
		}
	}
}
