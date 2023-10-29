using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    public class PartProject : KMonoBehaviour
    {
        [Serialize] Tag _resourceTag = null;
        [Serialize] float _resourceAmountMass = 0;
        [Serialize] float _constructionTime = 600f;
        [Serialize] bool _primaryConstruction = false;

        public PartProject(Tag resourceTag, float resourceAmountMass, float constructionTime, bool primaryConstruction =false)
        {
            _resourceTag = resourceTag;
            _resourceAmountMass = resourceAmountMass;
            _constructionTime = constructionTime;
            _primaryConstruction = primaryConstruction;
        }
        public float ConstructionTime => _constructionTime;
        public float ResourceAmountMass => _resourceAmountMass;
        public Tag ResourceTag => _resourceTag;
        public bool IsPrimary => _primaryConstruction;

    }
}
