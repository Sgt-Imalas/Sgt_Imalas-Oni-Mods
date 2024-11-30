using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KCompBuilder;

namespace UtilLibs
{
	public static class MinionAnimUtils
	{
		public static string[] symbolOverridesToRemove = new string[]
		{
			"snapTo_hat",
			"snapTo_hat_hair",
			"snapTo_neck",
			"snapTo_goggles",
			"snapTo_headfx",
			"snapTo_chest",
			"snapTo_pivot",
			"skirt",
			"necklace"
		};
		public static void ApplyNewAccessories(KBatchedAnimController kbac, SymbolOverrideController soc, List<KeyValuePair<string, string>> accessories)
		{

			soc.RemoveAllSymbolOverrides();
			foreach (string symbol in symbolOverridesToRemove)
			{
				soc.RemoveSymbolOverride(symbol);
				kbac.SetSymbolVisiblity(symbol, false);
			}

			var db = Db.Get();
			var dbAccessories = db.Accessories;
			var dbAccessorySlots = db.AccessorySlots;

			foreach (KeyValuePair<string, string> accessory in accessories)
			{
				if (dbAccessories.Exists(accessory.Value))
				{
					KAnim.Build.Symbol symbol = dbAccessories.Get(accessory.Value).symbol;
					AccessorySlot accessorySlot = dbAccessorySlots.Get(accessory.Key);
					soc.AddSymbolOverride(accessorySlot.targetSymbolId, symbol);
					kbac.SetSymbolVisiblity(accessory.Key, true);
				}
			}
			soc.ApplyOverrides();

		}
	}
}
