using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTraitGenerationManager.FixedTraitExperiment
{
    /// <summary>
    /// access non static values on WorldContainer
    /// </summary>
    internal class DummyWorldContainer : WorldContainer
    {
        public override void OnSpawn()
        {
        }
        public override void OnCleanUp()
        {
        }
        public override void OnPrefabInit()
        {

        }
    }
}
