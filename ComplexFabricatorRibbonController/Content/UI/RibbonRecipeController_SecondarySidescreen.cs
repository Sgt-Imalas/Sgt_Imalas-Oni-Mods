using ComplexFabricatorRibbonController.Content.Scripts.Buildings;
using ComplexFabricatorRibbonController.Content.UI.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using UtilLibs.UIcmp;
using static ModInfo;

namespace ComplexFabricatorRibbonController.Content.UI
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
		FInputField2 BlueprintSearchbar;
		FButton ClearBlueprintSearchbar;

		Dictionary<string, RecipeEntry> CurrentFabricatorRecipeIDs = [];


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


			BlueprintSearchbar = transform.Find("SearchBar/Input").FindOrAddComponent<FInputField2>();
			BlueprintSearchbar.OnValueChanged.AddListener(ApplyBlueprintFilter);
			BlueprintSearchbar.Text = string.Empty;
			ClearBlueprintSearchbar = transform.Find("SearchBar/DeleteButton").FindOrAddComponent<FButton>();
			ClearBlueprintSearchbar.OnClick += () => BlueprintSearchbar.Text = string.Empty;
			UIUtils.AddSimpleTooltipToObject(ClearBlueprintSearchbar.gameObject, STRINGS.UI.LISTSELECTION_SECONDARYSIDESCREEN.SEARCHBAR.CLEARTOOLTIP);
		}
		public void ApplyBlueprintFilter(string filterstring = "")
		{
			ApplyTextFilterVisibility();
		}
		void SetEmptyRow()
		{
			if (NoneUIEntry == null)
			{
				NoneUIEntry = AddOrGetGroupEntry(null);
				NoneUIEntry.RecipeLabel.SetText(STRINGS.UI.RFRC_NO_RECIPE);
			}
		}
		internal void SetOpenedFrom(ComplexFabricatorRecipeControlAttachment targetComponent, int bit)
		{
			OpenedFromBit = bit;
			Target = targetComponent;
			SetEmptyRow();
			//CurrentTagEntries = Target.GetCurrentlyExistingGroups();
			Refresh();
			BlueprintSearchbar.SetTextFromData(string.Empty);
		}
		void ApplyTextFilterVisibility()
		{
			foreach (var currentRecipe in CurrentFabricatorRecipeIDs)
			{
				var recipe = currentRecipe.Value.targetRecipe;
				if (recipe == null)
					continue;
				currentRecipe.Value.gameObject.SetActive(RecipeWithinFilters(recipe));
			}
		}

		static StringBuilder sb = new StringBuilder();
		static Dictionary<ComplexRecipe, string> CachedSearchableStrings = [];
		bool RecipeWithinFilters(ComplexRecipe recipe)
		{
			if (recipe == null || !BlueprintSearchbar.Text.Any())
				return true;
			if (!CachedSearchableStrings.TryGetValue(recipe, out var searchableString))
			{
				sb.Clear();
				sb.Append(recipe.id);
				sb.Append(recipe.description);
				foreach (var ingredient in recipe.ingredients)
				{
					if (ingredient.material == null)
						continue;
					var item = Assets.TryGetPrefab(ingredient.material);
					sb.Append(ingredient.material);
					if (item == null)
						continue;
					sb.Append((int)ingredient.amount);
					sb.Append(global::STRINGS.UI.StripLinkFormatting(item.GetProperName()));
				}
				foreach (var result in recipe.results)
				{
					if (result.material == null)
						continue;
					var item = Assets.TryGetPrefab(result.material);
					sb.Append(result.material);
					sb.Append((int)result.amount);
					if (item == null)
						continue;
					sb.Append(global::STRINGS.UI.StripLinkFormatting(item.GetProperName()));
				}
				CachedSearchableStrings[recipe] = sb.ToString();
				searchableString = sb.ToString();
			}
			return CultureInfo.InvariantCulture.CompareInfo.IndexOf(searchableString, BlueprintSearchbar.Text, CompareOptions.IgnoreCase) >= 0;
		}

		void Refresh()
		{
			if (!Target)
				return;
			CurrentFabricatorRecipeIDs.Clear();
			//Target.TryGetGroupName(out var groupName);

			foreach (var entry in GroupEntries)
			{
				entry.Value.gameObject.SetActive(false);
			}
			var currentRecipe = Target.GetRecipe(OpenedFromBit);
			if (Target.TryGetAvailableRecipes(out var fabricatorRecipes))
			{
				foreach (var recipeInfo in fabricatorRecipes)
				{
					var recipe = recipeInfo.first;
					int currentlySelectedOnBit = recipeInfo.second;
					bool currentlySelectedOnOwnComponent = recipeInfo.third;

					bool currentlySelectedRecipe = recipe == currentRecipe;
					var entry = AddOrGetGroupEntry(recipe);
					entry.UpdateInUseState(currentlySelectedOnBit, currentlySelectedOnOwnComponent, currentlySelectedRecipe);

					if (recipe != null)
						CurrentFabricatorRecipeIDs[recipe.id] = entry;
				}
			}
			NoneUIEntry.UpdateInUseState(-1, false, (currentRecipe == null));
			ApplyTextFilterVisibility();
		}
		void SelectComplexRecipe(ComplexRecipe current)
		{
			OnConfirm?.Invoke(current);
		}

		RecipeEntry AddOrGetGroupEntry(ComplexRecipe recipe)
		{
			string recipeId = string.Empty;
			if (recipe != null)
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
