using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RebuildPreserve
{
    internal class BuildSettingsPreservationData : KMonoBehaviour
    {
        public static BuildSettingsPreservationData Instance;

        public Dictionary<Tuple<int, ObjectLayer>, GameObject> ToCopyFromComponents = new();
    }
}
