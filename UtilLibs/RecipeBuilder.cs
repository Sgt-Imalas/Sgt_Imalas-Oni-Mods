using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe.RecipeElement;
using static ComplexRecipe;

namespace UtilLibs 
{
	public class RecipeBuilder //Source: Aki
	{
		private string fabricator;
		private float time;
		private RecipeNameDisplay nameDisplay;
		private string description;
		private string name;
		private int sortOrder;

		private List<RecipeElement> inputs;
		private List<RecipeElement> outputs;

		public static RecipeBuilder Create(string fabricatorID, string description, float time)
		{
			var builder = new RecipeBuilder
			{
				fabricator = fabricatorID,
				description = description,
				time = time,
				nameDisplay = RecipeNameDisplay.IngredientToResult,
				inputs = new List<RecipeElement>(),
				outputs = new List<RecipeElement>()
			};

			return builder;
		}
		public static RecipeBuilder Create(string fabricatorID, float time)
		{
			var builder = new RecipeBuilder
			{
				fabricator = fabricatorID,
				time = time,
				nameDisplay = RecipeNameDisplay.IngredientToResult,
				inputs = new List<RecipeElement>(),
				outputs = new List<RecipeElement>()
			};

			return builder;
		}
		public RecipeBuilder Description(System.Func<RecipeElement[], RecipeElement[], string> descriptionAction)
		{
			if (descriptionAction != null)
			{
				description = descriptionAction(inputs.ToArray(), outputs.ToArray());
			}
			return this;
		}


		public RecipeBuilder NameDisplay(RecipeNameDisplay nameDisplay)
		{
			this.nameDisplay = nameDisplay;
			return this;
		}

		public RecipeBuilder NameOverride(string name)
		{
			this.name = name;
			return this;
		}

		public RecipeBuilder SortOrder(int sortOrder)
		{
			this.sortOrder = sortOrder;
			return this;
		}

		public RecipeBuilder Input(Tag tag, float amount, bool inheritElement = true)
		{
			inputs.Add(new RecipeElement(tag, amount, inheritElement));
			return this;
		}
		public RecipeBuilder Input(SimHashes simhash, float amount, bool inheritElement = true)
		{
			inputs.Add(new RecipeElement(simhash.CreateTag(), amount, inheritElement));
			return this;
		}

		public RecipeBuilder Output(Tag tag, float amount, TemperatureOperation tempOp = TemperatureOperation.AverageTemperature, bool storeElement = false)
		{
			outputs.Add(new RecipeElement(tag, amount, tempOp, storeElement));
			return this;
		}
		public RecipeBuilder Output(SimHashes simhash, float amount, TemperatureOperation tempOp = TemperatureOperation.AverageTemperature, bool storeElement = false)
		{
			outputs.Add(new RecipeElement(simhash.CreateTag(), amount, tempOp, storeElement));
			return this;
		}

		public RecipeBuilder FacadeOutput(Tag tag, float amount, string facadeID = "", bool storeElement = false, TemperatureOperation tempOp = TemperatureOperation.AverageTemperature)
		{
			outputs.Add(new RecipeElement(tag, amount, tempOp, facadeID, storeElement));
			return this;
		}

		public ComplexRecipe Build(string facadeID = "")
		{
			var i = inputs.ToArray();
			var o = outputs.ToArray();

			string recipeID = facadeID.IsNullOrWhiteSpace() ? ComplexRecipeManager.MakeRecipeID(fabricator, i, o) : ComplexRecipeManager.MakeRecipeID(fabricator, i, o, facadeID);

			return new ComplexRecipe(recipeID, i, o)
			{
				time = time,
				description = description,
				customName = name,
				nameDisplay = nameDisplay,
				fabricators = new List<Tag> { fabricator }
			};
		}
	}
}
