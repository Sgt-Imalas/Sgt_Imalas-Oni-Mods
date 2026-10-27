using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace UtilLibs
{
	public static class KanimUtils
	{

		/// <summary>
		/// Makes the kbac render above liquids for buildings that are meant to show up in front of them, like conveyortiles
		/// </summary>
		/// <param name="go"></param>
		public static void RenderAboveLiquids(GameObject go, bool mainLayerOnly = false)
		{
			//mirrored from Fake tile buildings like storage tile and farm tiles
			if (go.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				//main kbac (non-foreground) of the building
				kbac.initialBlendParameters = 4;
			}
			else
				SgtLogger.error("KBatchedAnimController not found on " + go.name + ", cannot mark to render above liquids!");
			
			if (mainLayerOnly)
				return;
			RenderSubKanimsAboveLiquids(go);
		}
		///sets all other kbacs (bloom, foreground, etc) to render above liquids as well
		public static void RenderSubKanimsAboveLiquids(GameObject go)
		{
			if (go.TryGetComponent<KPrefabID>(out var kPrefabID))
			{
				kPrefabID.prefabInitFn += new KPrefabID.PrefabFn(SetSecondaryKBACsAboveWater);
			}
		}
		private static void SetSecondaryKBACsAboveWater(GameObject instance)
		{
			foreach (KBatchedAnimController kbatchedAnimController in instance.GetComponentsInChildrenOnly<KBatchedAnimController>())
			{
				kbatchedAnimController.SetBlendValue(KBatchedAnimInstanceData.BlendActiveOptions.LiquidVisibilityLayer, false);
				kbatchedAnimController.SetBlendValue(KBatchedAnimInstanceData.BlendActiveOptions.WaterProof, true);
			}
		}


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


		/// <summary>
		/// Apply list of accessories to a duplicant
		/// </summary>
		/// <param name="kbac"></param>
		/// <param name="soc"></param>
		/// <param name="accessories"></param>
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
