using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UtilLibs;
using static STRINGS.UI.CODEX;
using static UtilLibs.RocketryUtils;

namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations
{
	public class RTB_BuildingCategoryType
	{
		public static int GetNameHash([CallerMemberName] string name = "" ) => name.GetHashCode();

		public static readonly int A_ROCKETRYPLUS = GetNameHash();
		public static readonly int B_MININGSHIPPING = GetNameHash();
		public static readonly int C_FUELLOGISTICS = GetNameHash();
		public static readonly int D_POWERUTILITY = GetNameHash();
		public static readonly int E_SPACEEXPANSION = GetNameHash();

		public static readonly int RocketModuleUpgrade = GetNameHash();

		public static readonly List<int> FilterCategories =
			[
			A_ROCKETRYPLUS,
			B_MININGSHIPPING,
			C_FUELLOGISTICS,
			D_POWERUTILITY,
			E_SPACEEXPANSION,
			];

		static Dictionary<int, string> _strings = null;
		public static bool TryGetCategoryName(int categoryId, out string name)
		{
			if(_strings == null)
			{
				_strings = new Dictionary<int, string>
				{
					[A_ROCKETRYPLUS] = STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS,
					[B_MININGSHIPPING] = STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING,
					[C_FUELLOGISTICS] = STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS,
					[D_POWERUTILITY] = STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.D_POWERUTILITY,
					[E_SPACEEXPANSION] = STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.E_SPACEEXPANSION,
				};
				foreach( var entry in ModAssets.GetTooltips())
				{
					string CategoryName = ((RocketCategory)entry.Key).ToString().ToUpperInvariant();
					_strings[entry.Key] = Strings.Get("STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTITLES." + CategoryName);
				}
			}
			return _strings.TryGetValue(categoryId, out name);
		}
	}
}
