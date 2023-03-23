using UnityEngine;
using UtilLibs;

namespace CritterTraitsReborn.Traits
{
    class Rad : TraitBuilder
    {
        public override string ID => "CritterRad";

        public override Group Group => Group.GetGroup(Group.GlowGroupId);

        protected override void Init()
        {
            TraitHelpers.CreateTrait(ID,
              on_add: delegate (GameObject go)
              {
                  float range = Random.Range(3, 6);
                  CritterUtil.AddObjectLight(go, UIUtils.rgba(50, 255, 50, 0.9), 2, 100);
                  CritterUtil.AddObjectRadEmitter(go, Random.Range(100,301), range);
              },
              positiveTrait: true
            );
        }
    }
}
