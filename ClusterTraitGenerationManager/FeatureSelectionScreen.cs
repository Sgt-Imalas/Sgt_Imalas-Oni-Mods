using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterTraitGenerationManager
{
    internal class FeatureSelectionScreen : KModalScreen
    {

        GameObject PlanetoidCategoryPrefab;
        GameObject PlanetoidEntryPrefab;

        List<GameObject> PlanetoidEntries;

        GameObject RandomPlanetoidEntry;

        public override void OnSpawn()
        {
            base.OnSpawn();
            PlanetoidCategoryPrefab = KleiInventoryScreen.
        }
    }
}
