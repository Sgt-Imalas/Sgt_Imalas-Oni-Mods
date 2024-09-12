using UnityEngine;

namespace CritterTraitsReborn.Traits
{
	class Noisy : TraitBuilder
	{
		public override string ID => "CritterNoisy";

		public override Group Group => Group.GetGroup(Group.NoiseGroupId);

		protected override void Init()
		{
			TraitHelpers.CreateTrait(ID,
			  on_add: delegate (GameObject go)
			  {
				  go.AddOrGet<Components.Noisy>();
			  },
			  positiveTrait: false
			);
		}
	}
}
