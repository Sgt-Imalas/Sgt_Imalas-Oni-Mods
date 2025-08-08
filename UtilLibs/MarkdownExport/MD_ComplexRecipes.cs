using ClipperLib;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameUtil;

namespace UtilLibs.MarkdownExport
{
	public class MD_ComplexRecipes : IMD_Entry
	{
		public MD_ComplexRecipes(List<ComplexRecipe> recipes)
		{
			this.recipes = recipes;
		}

		static StringBuilder sb = new StringBuilder();
		private List<ComplexRecipe> recipes;
		HashSet<string> processedRecipes = [];

		public string FormatAsMarkdown()
		{
			if (recipes == null || !recipes.Any())
				return string.Empty;
			var buildingID = recipes.First().fabricators[0].ToString();
			bool hasRandomResults = MD_Exporter.Instance.RandomRecipeResults.TryGetValue(buildingID, out var randomResultsPerInput);
			bool hasRandomOccurences = MD_Exporter.Instance.RandomRecipeOccurences.TryGetValue(buildingID, out var randomOccurencesPerInput);


			sb.Clear();
			if (hasRandomResults)
				sb.Append("|Ingredients:| Time: |Randomized Products:");
			else
				sb.Append("|Ingredients:| Time: |Products:");

			if (hasRandomOccurences)
				sb.Append("|Occurence:");
			sb.AppendLine("|");


			if (hasRandomOccurences)
				sb.AppendLine("|-|-|-|-|");
			else
				sb.AppendLine("|-|-|-|");

			foreach (var recipe in recipes)
			{
				//multi material recipes appear multiple times
				if (processedRecipes.Contains(recipe.id))
					continue;

				sb.Append("|");
				foreach (var input in recipe.ingredients)
				{
					sb.Append(MarkdownUtil.GetTagName(input.material));
					sb.Append(" (");
					sb.Append(GameUtil.GetFormattedMass(input.amount));
					sb.Append(")");
					sb.Append("<br>");
				}
				sb.Append("|");
				sb.Append(GameUtil.GetFormattedTime(recipe.time));
				sb.Append("|");

				if (!hasRandomResults)
				{
					foreach (var output in recipe.results)
					{
						sb.Append(MarkdownUtil.GetTagName(output.material));
						sb.Append(" (");
						sb.Append(GameUtil.GetFormattedMass(output.amount));
						sb.Append(")");
						sb.Append("<br>");
					}
				}
				else if (randomResultsPerInput.TryGetValue(recipe.ingredients[0].material, out var randomResults))
				{
					foreach (var output in randomResults)
					{
						sb.Append(MarkdownUtil.GetTagName(output));
						sb.Append("<br>");
					}
				}
				if (hasRandomOccurences && randomOccurencesPerInput.TryGetValue(recipe.ingredients[0].material, out var randomOccurences))
				{
					sb.Append("|");
					foreach (var output in randomOccurences)
					{
						sb.Append(MarkdownUtil.GetTagName(output));
						sb.Append("<br>");
					}
				}				
				sb.AppendLine("|");

				processedRecipes.Add(recipe.id);
			}
			sb.AppendLine();
			return sb.ToString();
		}
	}
}
