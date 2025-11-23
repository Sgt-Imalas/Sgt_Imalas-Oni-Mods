using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SandboxToolParameterMenu.SelectorValue;

namespace UtilLibs
{
	/// <summary>
	/// Credit: Akis FUtility
	/// </summary>
	public class SandboxUtil
	{
		// Creates a filter based on an entity icon, and a tag check
		// SandboxToolParameterMenu.instance does not exist when this is most sensible to be called, so don't use that here
		public static SearchFilter CreateSimpleTagFilter(SandboxToolParameterMenu menu, string name, Tag tag, string prefabIDToUseForIcon, string parentFilterID = null)
		{
			var sprite = Def.GetUISprite(global::Assets.GetPrefab(prefabIDToUseForIcon));
			var parent = FindParent(menu, parentFilterID);

			return new SearchFilter(name, obj => obj is KPrefabID id && id.HasTag(tag), parent, sprite);
		}

		public static SearchFilter AddModMenu(SandboxToolParameterMenu menu, string name, Sprite icon, Func<object, bool> condition)
		{
			return AddModMenu(menu, name, new Tuple<Sprite, Color>(icon, Color.white), condition);
		}

		public static SearchFilter AddModMenu(SandboxToolParameterMenu menu, string name, Tuple<Sprite, Color> icon, Func<object, bool> condition)
		{
			var parent = AddOrGetModsMenu(menu);
			var filter = new SearchFilter(name, condition, parent, icon);
			menu.entitySelector.filters = menu.entitySelector.filters.AddToArray(filter);

			return filter;
		}

		public static SearchFilter AddOrGetModsMenu(SandboxToolParameterMenu menu)
		{
			var color = new Color32(254, 254, 254, 255);
			var idx = menu.entitySelector.filters
				.ToList()
				.FindIndex(x => x.icon != null && x.icon.first?.name == "mod_machinery" && x.icon.second.Equals(color));

			var filters = new List<SearchFilter>();

			if (idx == -1)
			{
				var modsSprite = global::Assets.GetSprite("mod_machinery");
				var newFilter = new SearchFilter(STRINGS.UI.FRONTEND.MODS.TITLE, _ => false, null, new Tuple<Sprite, Color>(modsSprite, color));

				menu.entitySelector.filters = menu.entitySelector.filters.AddToArray(newFilter);

				return newFilter;
			}
			else
			{
				return menu.entitySelector.filters[idx];
			}
		}

		public static void AddFilters(SandboxToolParameterMenu menu, params SearchFilter[] newFilters)
		{
			var filters = menu.entitySelector.filters;

			if (filters == null)
			{
				SgtLogger.warning("Filters are null");
				return;
			}

			var f = new List<SearchFilter>(filters);
			f.AddRange(newFilters);
			menu.entitySelector.filters = f.ToArray();

			// UpdateOptions(menu);
		}

		public static void UpdateOptions(SandboxToolParameterMenu menu)
		{
			var filters = menu.entitySelector.filters;

			if (filters == null) return;

			var options = ListPool<object, SandboxToolParameterMenu>.Allocate();

			foreach (var prefab in global::Assets.Prefabs)
			{
				foreach (var filter in filters)
				{
					if (filter.condition(prefab))
					{
						options.Add(prefab);
						break;
					}
				}
			}

			menu.entitySelector.options = options.ToArray();
			options.Recycle();
		}

		private static SearchFilter FindParent(SandboxToolParameterMenu menu, string parentFilterID)
		{
			return parentFilterID != null ? menu.entitySelector.filters.First(x => x.Name == parentFilterID) : null;
		}
	}
}
