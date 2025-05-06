using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;
using UtilLibs;
using ComplexFabricatorRibbonController.UI.Components;
using ComplexFabricatorRibbonController.Scripts.Buildings;

namespace ComplexFabricatorRibbonController.UI
{
    public class RibbonRecipeController_SecondarySidescreen : KScreen
	{
		ComplexFabricatorRecipeControlAttachment Target;
		public Action<ComplexRecipe> OnConfirm;

		GameObject groupsContainer;
		RecipeEntry groupEntryPrefab;
		Dictionary<string, RecipeEntry> GroupEntries = new();
		int OpenedFromBit;

		RecipeEntry NoneUIEntry;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}

		void Init()
		{
			SgtLogger.l("RibbonRecipeController_SecondarySidescreen OnPrefabInit");
			var recipeEntryPrefabGO = transform.Find("ScrollArea/Content/GroupEntryPrefab").gameObject;
			groupsContainer = recipeEntryPrefabGO.transform.parent.gameObject;
			recipeEntryPrefabGO.SetActive(false);
			groupEntryPrefab = recipeEntryPrefabGO.AddOrGet<RecipeEntry>();
		}
		void SetEmptyRow()
		{
			if (NoneUIEntry == null)
			{
				NoneUIEntry = AddOrGetGroupEntry(null);
				NoneUIEntry.RecipeLabel.SetText(global::STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION);
			}
		}
		internal void SetOpenedFrom(ComplexFabricatorRecipeControlAttachment targetComponent, int bit)
		{
			OpenedFromBit = bit;
			Target = targetComponent;
			SetEmptyRow();
			//CurrentTagEntries = Target.GetCurrentlyExistingGroups();
			Refresh();
		}
		void Refresh()
		{
			if (!Target)
				return;

			//Target.TryGetGroupName(out var groupName);

			foreach (var entry in GroupEntries)
			{
				entry.Value.gameObject.SetActive(false);
			}
			foreach (var recipe in Target.GetAvailableRecipes())
			{
				AddOrGetGroupEntry(recipe);
			}
			var currentRecipe = Target.GetRecipe(OpenedFromBit);
			if(currentRecipe != null)
			{
				AddOrGetGroupEntry(currentRecipe);
			}
		}
		void SelectComplexRecipe(ComplexRecipe current)
		{
			OnConfirm?.Invoke(current);
		}

		RecipeEntry AddOrGetGroupEntry(ComplexRecipe recipe)
		{
			string recipeId = string.Empty;
			if(recipe != null)
				recipeId = recipe.id;


			if (GroupEntries.TryGetValue(recipeId, out var entry))
			{
				entry.gameObject.SetActive(true);
				return entry;
			}


			var newEntry = Util.KInstantiateUI<RecipeEntry>(groupEntryPrefab.gameObject, groupsContainer);
			newEntry.targetRecipe = recipe;
			newEntry.SelectRecipe = SelectComplexRecipe;
			newEntry.gameObject.SetActive(true);
			newEntry.UpdateUI();
			if (!recipeId.IsNullOrWhiteSpace())
			{
				GroupEntries[recipeId] = newEntry;
			}
			return newEntry;
		}
	}
}
