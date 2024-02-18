using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    [Serializable]
    public class PartProject : KMonoBehaviour
    {
        [Serialize] Tag _resourceTag = null;
        [Serialize] float _resourceAmountMass = 0;
        [Serialize] float _totalConstructionTime = 600f;
        [Serialize] bool _primaryConstruction = false;
        [Serialize] public bool IsConstructionProcess = true;

        [Serialize] public float CurrentConstructionTime = -1;

        public PartProject(Tag resourceTag, float resourceAmountMass, float constructionTime, bool primaryConstruction =false)
        {
            _resourceTag = resourceTag;
            _resourceAmountMass = resourceAmountMass;
            _totalConstructionTime = constructionTime;
            _primaryConstruction = primaryConstruction;
        }
        public float TotalConstructionTime => _totalConstructionTime;
        public float ResourceAmountMass => _resourceAmountMass;
        public Tag ResourceTag => _resourceTag;
        public bool IsPrimary => _primaryConstruction;

    }
}
