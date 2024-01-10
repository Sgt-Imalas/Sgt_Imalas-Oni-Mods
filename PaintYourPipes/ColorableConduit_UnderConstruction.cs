using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintYourPipes
{
    internal class ColorableConduit_UnderConstruction:KMonoBehaviour
    {
        [Serialize]
        public string ColorHex=string.Empty;
        [Serialize]
        public bool HasData = false;

    }
}
