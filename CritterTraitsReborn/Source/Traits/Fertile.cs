using Klei.AI;
using UnityEngine;

namespace CritterTraitsReborn.Traits
{
    class Fertile : TraitBuilder
    {
        public override string ID => "CritterFertile";

        public override Group Group => Group.GetGroup(Group.FertilityGroupId);

        protected override void Init()
        {
            TraitHelpers.GetCritterTraitDesc(ID, out var Description);
            TraitHelpers.CreateTrait(ID,
              on_add: delegate (GameObject go)
              {
                  var modifiers = go.GetComponent<Modifiers>();
                  if (modifiers != null)
                  {
                      modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, 0.25f, Description, is_multiplier: true));
                  }
              },
              positiveTrait: true
            );
        }
    }
}
