using KSerialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.TwitchEvents.TwitchEventAddons
{
    internal class CrewColoursSetter:KMonoBehaviour
    {
        [Serialize]
        Color CrewColour;
        public override void OnSpawn()
        {
            base.OnSpawn();
            if(CrewColour == null)
            {
                CrewColour = GetBrightColor();
            }
            this.GetComponent<KAnimControllerBase>().TintColour = CrewColour;
        }

        Color GetBrightColor()
        {
            float r = 0, g = 0, b = 0; 
            while (r + g + b < 1f) 
            {
                r = Random.value; 
                g = Random.value;
                b = Random.value;
            }
            return new Color(r, g, b);
        }
    }
}
