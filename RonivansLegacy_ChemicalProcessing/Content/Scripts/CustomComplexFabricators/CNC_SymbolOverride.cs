using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators
{
	internal class CNC_SymbolOverride : KMonoBehaviour
	{
		[MyCmpReq] SymbolOverrideController soc;
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] ComplexFabricator cf;


		int _handle = -1;
		public override void OnSpawn()
		{
			_handle = Subscribe((int)GameHashes.FabricatorOrderStarted, UpdateOverride);
			base.OnSpawn();
		}

		void UpdateOverride(object data)
		{
			//SgtLogger.l("OnRecipeStarted");
			soc.RemoveSymbolOverride("metal");
			if (data is not ComplexRecipe recipe)
				return;
			//SgtLogger.l("Recipe: " + recipe.id);

			var firstIngredient = recipe.ingredients.FirstOrDefault();
			if (firstIngredient == null)
				return;
			//SgtLogger.l("First ingredient: " + firstIngredient.material);

			var firstIngredientItem = cf.buildStorage.FindFirst(firstIngredient.material);
			if (firstIngredientItem == null)
				firstIngredientItem = cf.inStorage.FindFirst(firstIngredient.material);
			if (firstIngredientItem == null)
				return;
			//SgtLogger.l("found ingredient: " + firstIngredientItem.name);
			if (firstIngredientItem.TryGetComponent<PrimaryElement>(out var prim))
				kbac.SetSymbolTint("spinning", prim.Element.substance.colour);
			else
				kbac.SetSymbolTint("spinning", Color.white);



				KAnim.Build build = firstIngredientItem.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build;
			HashedString ui = new HashedString("ui");
			KAnim.Build.Symbol symbol = build.GetSymbol(build.name);
			if (symbol == null)
			{
				foreach (var sym in build.symbols)
				{
					if (sym.hash != ui)
					{
						symbol = sym;
						break;
					}
				}
			}
			if (symbol != null)
			{
				//SgtLogger.l("found symbol: " + build.name);
				soc.AddSymbolOverride("metal", symbol);
			}
		}

		public override void OnCleanUp()
		{
			Unsubscribe(_handle);
			base.OnCleanUp();
		}
	}
}
