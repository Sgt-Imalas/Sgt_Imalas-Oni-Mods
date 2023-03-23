using Klei.AI;
using UnityEngine;

namespace CritterTraitsReborn.Traits
{
    class ShortLived : TraitBuilder
    {
        public override string ID => "CritterShortLived";

        public override Group Group => Group.GetGroup(Group.LifespanGroupId);

        protected override void Init()
        {
            TraitHelpers.GetCritterTraitDesc(ID, out var Description);
            TraitHelpers.CreateTrait(ID,
              on_add: delegate (GameObject go)
              {
                  var modifiers = go.GetComponent<Modifiers>();
                  if (modifiers != null)
                  {
                      modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, -0.20f, Description, is_multiplier: true));
                  }
              },
              positiveTrait: false
            );
        }
    }
}
