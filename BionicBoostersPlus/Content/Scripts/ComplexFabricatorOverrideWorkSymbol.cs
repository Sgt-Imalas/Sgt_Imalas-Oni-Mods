using System.Linq;

namespace BionicBoostersPlus.Content.Scripts
{
	internal class ComplexFabricatorOverrideWorkSymbol : ComplexFabricator
	{
		[MyCmpReq] SymbolOverrideController soc;
		public override void TransferCurrentRecipeIngredientsForBuild()
		{
			base.TransferCurrentRecipeIngredientsForBuild();
			ComplexRecipe.RecipeElement[] ingredients = recipe_list[workingOrderIdx].ingredients;
			if (!ingredients.Any())
				return;
			var booster = buildStorage.FindFirst(GameTags.BionicUpgrade);
			if (booster == null)
				return;

			if (!booster.TryGetComponent<KBatchedAnimController>(out var kbac))
				return;

			string animStateName = BionicUpgradeComponentConfig.UpgradesData[booster.PrefabID()].animStateName;
			KAnim.Build.Symbol symbol = kbac.AnimFiles[0].GetData().build.GetSymbol(animStateName);

			soc.RemoveSymbolOverride("snapto_object");
			soc.AddSymbolOverride("snapto_object", symbol);
		}
	}
}
