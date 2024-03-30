using KSerialization;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RebuildPreserve
{
    internal class BuildSettingsPreservationData : KMonoBehaviour
    {
        public static BuildSettingsPreservationData Instance;

        [Serialize]
        public Dictionary<Tuple<int, ObjectLayer>, GameObject> ToCopyFromComponents = new();
        public void ReplaceEntry(Tuple<int, ObjectLayer> targetPos, GameObject newCachedObject)
        {
            RemoveEntry(targetPos);
            ToCopyFromComponents.Add(targetPos, newCachedObject);
            newCachedObject.transform.SetParent(this.transform);
            SgtLogger.l("added cached data for " + ToCopyFromComponents[targetPos] + " at cell " + targetPos.first);
        }
        public bool TryGetEntry(Tuple<int, ObjectLayer> targetPos, out GameObject entry)
        {
            entry = null;
            return ToCopyFromComponents.TryGetValue(targetPos, out entry);
        }

        public void RemoveEntry(Tuple<int, ObjectLayer> targetPos)
        {
            if (ToCopyFromComponents.ContainsKey(targetPos))
            {
                SgtLogger.l("removed cached data for " + ToCopyFromComponents[targetPos] + " at cell " + targetPos.first);
                UnityEngine.Object.Destroy(ToCopyFromComponents[targetPos]);
                ToCopyFromComponents.Remove(targetPos);
            }
        }
    }
}
