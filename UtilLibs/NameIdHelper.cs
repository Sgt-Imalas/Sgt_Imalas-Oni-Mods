using System.Collections.Generic;

namespace UtilLibs
{
	public static class NameIdHelper
	{
		public static Dictionary<string, string> NamesById = new();
		public static bool TryGetIdFromName(string name, out string Id)
		{
			name = name.ToLowerInvariant();

			Id = name;
			if (Db.Get() == null)
			{
				SgtLogger.error("Db not initialized yet!");
				return false;
			}

			if (NamesById.TryGetValue(name, out var id))
			{
				Id = id;
				return true;
			}

			foreach (var prefabWithId in Assets.PrefabsByTag) 
			{
				var prefab = prefabWithId.Value;
				if (prefab.TryGetComponent<ClusterGridEntity>(out _))
					continue;

				string properName = prefab.gameObject.GetProperName();
				properName = STRINGS.UI.StripLinkFormatting(properName).ToLowerInvariant();

				NamesById[properName] = prefabWithId.Key.ToString();

				if (properName == name)
				{
					Id = prefabWithId.Key.ToString();

					return true;
				}
			}
			SgtLogger.warning("could not find prefab with the name " + name);
			return false;

		}
	}
}
