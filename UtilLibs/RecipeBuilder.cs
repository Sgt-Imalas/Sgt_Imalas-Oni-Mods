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
		private int sortOrder = 0;

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
		public RecipeBuilder Description(string desc)
		{
			if (desc != null)
			{
				description = desc;
			}
			return this;
		}
		public RecipeBuilder DescriptionFunc(System.Func<RecipeElement[], RecipeElement[], string> descriptionAction)
		{
			if (descriptionAction != null)
			{
				description = descriptionAction(inputs.ToArray(), outputs.ToArray());
			}
			return this;
		}

		public RecipeBuilder Description(string ToFormat, int inputCount,int outputCount)
		{
			if (inputs.Count < inputCount || outputs.Count < outputCount)
			{
				throw new InvalidOperationException($"Recipe must have at least {inputCount} inputs and {outputCount} outputs to use Description.");
			}
			description = string.Format(ToFormat, GetFormatArgs(inputCount, outputCount));
			return this;
		}
		public string[] GetFormatArgs(int inputCount, int outputCount)
		{
			if (inputCount > inputs.Count || outputCount > outputs.Count)
			{
				throw new InvalidOperationException($"Recipe must have at least {inputCount} inputs and {outputCount} outputs to use GetFormatArgs.");
			}
			var result = new string[inputCount + outputCount];

			for (int i = 0; i < inputCount; i++)
			{
				var input = inputs[i];
				if (input == null)
					throw new InvalidOperationException($"Input {i} is null in GetFormatArgs.");
				var tag = input.material;
				var item = Assets.GetPrefab(tag);
				if (item != null)
				{
					result[i] = item.GetProperName();
				}
				else
				{
					result[i] = tag.ProperName();
				}
			}
			for (int i = 0; i < outputCount; i++)
			{
				var output = outputs[i];
				if (output == null)
					throw new InvalidOperationException($"Output {i} is null in GetFormatArgs.");
				var tag = output.material;
				var item = Assets.GetPrefab(tag);
				if (item != null)
				{
					result[inputCount + i] = item.GetProperName();
				}
				else
				{
					result[inputCount + i] = tag.ProperName();
				}
			}
			return result;
		}
		public RecipeBuilder Description1I1O(string ToFormat) => Description(ToFormat, 1, 1);
		public RecipeBuilder Description1I2O(string ToFormat) => Description(ToFormat, 1, 2);
		public RecipeBuilder Description1I3O(string ToFormat) => Description(ToFormat, 1, 3);
		public RecipeBuilder Description1I4O(string ToFormat) => Description(ToFormat, 1, 4);
		public RecipeBuilder Description2I1O(string ToFormat) => Description(ToFormat, 2, 1);
		public RecipeBuilder Description2I2O(string ToFormat) => Description(ToFormat, 2, 2);
		public RecipeBuilder Description3I2O(string ToFormat) => Description(ToFormat, 3, 2);

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
		public RecipeBuilder Input(IEnumerable<Tag> tags, float amount)
		{
			inputs.Add(new RecipeElement(tags.ToArray(), amount));
			return this;
		}
		public RecipeBuilder Input(IEnumerable<SimHashes> tags, float amount)
		{
			inputs.Add(new RecipeElement(tags.Select(simhash=> simhash.CreateTag()).ToArray(), amount));
			return this;
		}
		public RecipeBuilder InputSO(SimHashes simHashes, float amount, bool inheritElement = true)
		{
			if (DlcManager.IsExpansion1Active())
				return Input(simHashes, amount, inheritElement);
			else
				return this;
		}
		public RecipeBuilder InputBase(SimHashes simHashes, float amount, bool inheritElement = true)
		{
			if (DlcManager.IsPureVanilla())
				return Input(simHashes, amount, inheritElement);
			else
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

			var recipe = new ComplexRecipe(recipeID, i, o)
			{
				time = time,
				description = description,
				customName = name,
				nameDisplay = nameDisplay,
				fabricators = new List<Tag> { fabricator }
			};
			if (this.sortOrder > 0)
			{
				recipe.sortOrder = this.sortOrder;
			}
			return recipe;
		}
	}
}
