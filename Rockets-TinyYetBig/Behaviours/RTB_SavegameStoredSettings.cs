using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Behaviours
{
    internal class RTB_SavegameStoredSettings : KMonoBehaviour
    {
        public static RTB_SavegameStoredSettings Instance;


        [Serialize]
        public bool useModuleCategories = true;
    }
}
