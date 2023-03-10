using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTraitGenerationManager.SettinPrefabComps
{
    internal class CycleHandler: KMonoBehaviour,ICustomPlanetoidSetting
    {
        

        public override void OnSpawn()
        {
            base.OnSpawn();

        }

        public override void OnCleanUp()
        {

            base.OnCleanUp();
        }

        public void HandleData(object data)
        {
            throw new NotImplementedException();
        }
    }
}
