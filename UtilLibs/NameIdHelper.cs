using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			foreach (var prefab in Assets.PrefabsByTag) 
			{
				string properName = prefab.Value.gameObject.GetProperName();
				properName = STRINGS.UI.StripLinkFormatting(properName).ToLowerInvariant();

				NamesById[properName] = prefab.Key.ToString();

				if (properName == name)
				{
					Id = prefab.Key.ToString();

					return true;
				}
			}
			SgtLogger.warning("could not find prefab with the name " + name);
			return false;

		}
	}
}
