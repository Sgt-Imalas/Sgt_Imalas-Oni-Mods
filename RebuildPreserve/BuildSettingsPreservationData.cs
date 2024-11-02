using KSerialization;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace RebuildPreserve
{
	internal class BuildSettingsPreservationData : KMonoBehaviour
	{
		public static BuildSettingsPreservationData Instance;
				
		public Dictionary<Tuple<int, ObjectLayer>, GameObject> ToCopyFromComponents = new();
		[Serialize]
		public Dictionary<Tuple<int, ObjectLayer>, string> ToCopyFromPrefabIds = new();

        public override void OnSpawn()
        {
			List<Tuple<int, ObjectLayer> > toRemove = new List<Tuple<int, ObjectLayer>>();
			foreach (var entry in ToCopyFromComponents)
				if (entry.Value == null)
					toRemove.Add(entry.Key);

			foreach (var entry in toRemove)
            {
                ToCopyFromComponents.Remove(entry);
				ToCopyFromPrefabIds.Remove(entry);
            }

        }


        public void ReplaceEntry(Tuple<int, ObjectLayer> targetPos, GameObject newCachedObject, string prefabId)
		{
			RemoveEntry(targetPos);
			ToCopyFromComponents.Add(targetPos, newCachedObject);
			ToCopyFromPrefabIds.Add(targetPos, prefabId);
			newCachedObject.transform.SetParent(this.transform);
			SgtLogger.l("added cached data for " + ToCopyFromComponents[targetPos] + " at cell " + targetPos.first);
		}
		public bool TryGetEntry(Tuple<int, ObjectLayer> targetPos, out GameObject entry, out string cachedPrefabId)
		{
			entry = null;
			cachedPrefabId = null;
			return ToCopyFromComponents.TryGetValue(targetPos, out entry) && ToCopyFromPrefabIds.TryGetValue(targetPos, out cachedPrefabId);
		}

		public void RemoveEntry(Tuple<int, ObjectLayer> targetPos)
		{
			if (ToCopyFromComponents.ContainsKey(targetPos))
			{
				SgtLogger.l("removed cached data for " + ToCopyFromComponents[targetPos] + " at cell " + targetPos.first);
				UnityEngine.Object.Destroy(ToCopyFromComponents[targetPos]);
				ToCopyFromComponents.Remove(targetPos);
			}
			if (ToCopyFromPrefabIds.ContainsKey(targetPos))
			{
				ToCopyFromPrefabIds.Remove(targetPos);
			}
		}
	}
}
