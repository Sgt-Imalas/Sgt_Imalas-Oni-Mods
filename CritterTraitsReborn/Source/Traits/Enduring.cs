using Klei.AI;
using UnityEngine;

namespace CritterTraitsReborn.Traits
{
    class Enduring : TraitBuilder
    {
        public override string ID => "CritterEnduring";

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
                    modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 0.25f, Description, is_multiplier: true));
                }
            },
            positiveTrait: true
          );
        }
    }
}
