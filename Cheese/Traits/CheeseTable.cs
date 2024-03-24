using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheese.Traits
{
    internal class CheeseTable:KMonoBehaviour
    {

        public override void OnSpawn()
        {
            base.OnSpawn();
            ModAssets.CheeseTableTargets.Add(this);
        }
        public override void OnCleanUp()
        {
            ModAssets.CheeseTableTargets.Remove(this);
            base.OnCleanUp();
        }
    }
}
