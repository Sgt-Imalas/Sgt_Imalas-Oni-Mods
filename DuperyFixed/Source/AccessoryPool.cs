using System.Collections.Generic;

namespace Dupery
{
	class AccessoryPool
	{
		private Dictionary<string, Dictionary<string, string>> pool;

		public AccessoryPool()
		{
			pool = new Dictionary<string, Dictionary<string, string>>
			{
				{ "Hair", new Dictionary<string, string>() }
			};
		}

		public bool TryGetId(string slotId, string accessoryKey, out string id)
		{
			id = null;
			if (!pool.ContainsKey(slotId) || accessoryKey == null)
				return false;

			return pool[slotId].TryGetValue(accessoryKey, out id);
		}

		public void AddId(string slotId, string accessoryKey, string accessoryId)
		{
			if (!pool.ContainsKey(slotId))
				pool[slotId] = new Dictionary<string, string>();

			pool[slotId][accessoryKey] = accessoryId;
		}
	}
}
