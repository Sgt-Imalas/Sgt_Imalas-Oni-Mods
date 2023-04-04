using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig
{
    public class CustomHarvestablePOI : HarvestablePOIClusterGridEntity
    {
        public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
        {
            new AnimConfig
            {
                animFile = Assets.GetAnim("insert_custom_anim_name_kanim"),
                initialAnim = (m_Anim.IsNullOrWhiteSpace() ? "cloud" : m_Anim)
            }    
        };
    }
}

