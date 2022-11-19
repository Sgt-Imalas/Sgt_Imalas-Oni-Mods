using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.RocketFueling
{
    public class FuelLoaderComponent:KMonoBehaviour
    {
        [Serialize]
        public LoaderType loaderType;

        public enum LoaderType
        {
            Fuel,
            SolidOx,
            LiquidOx
        }
    }
}
