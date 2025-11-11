using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplexRecipe.RecipeElement;
using static ComplexRecipe;
using UtilLibs.MarkdownExport;

namespace UtilLibs
{
	public class RecipeBuilder //Source: Aki
	{
		private string techRequirement;
		private string fabricator;
		private float time;
		private RecipeNameDisplay nameDisplay;
		private string description;
		private string name;
		private string spritePrefabId;
		private int sortOrder = 0;
		private int hepConsumed = 0;
		private int hepProduced = 0;

		private List<RecipeElement> inputs;
		private List<RecipeElement> outputs;
		private Dictionary<RecipeElement, Tag> GroupDescriptors = [];

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
		public RecipeBuilder AppendExtraDescription(string extraDescription)
		{
			description += extraDescription;
			return this;
		}
		public RecipeBuilder Description(string ToFormat, int inputCount, int outputCount)
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
			List<string> result = new();

			for (int i = 0; i < inputCount; i++)
			{
				var input = inputs[i];
				if (input == null)
					throw new InvalidOperationException($"Input {i} is null in GetFormatArgs.");

				var tag = input.material;
				if (GroupDescriptors.TryGetValue(input, out var descriptorTag))
				{
					tag = descriptorTag;
				}
				var item = Assets.TryGetPrefab(tag);
				string itemName = item?.GetProperName() ?? tag.ProperName();
				result.Add(itemName);
			}
			for (int i = 0; i < outputCount; i++)
			{
				var output = outputs[i];
				if (output == null)
					throw new InvalidOperationException($"Output {i} is null in GetFormatArgs.");
				var tag = output.material;
				if (GroupDescriptors.TryGetValue(output, out var descriptorTag))
				{
					tag = descriptorTag;
				}
				var item = Assets.TryGetPrefab(tag);
				string itemName = item?.GetProperName() ?? tag.ProperName();
				result.Add(itemName);
			}
			return result.ToArray();
		}
		public RecipeBuilder Description1I1O(string ToFormat) => Description(ToFormat, 1, 1);
		public RecipeBuilder Description1I2O(string ToFormat) => Description(ToFormat, 1, 2);
		public RecipeBuilder Description1I3O(string ToFormat) => Description(ToFormat, 1, 3);
		public RecipeBuilder Description1I4O(string ToFormat) => Description(ToFormat, 1, 4);
		public RecipeBuilder Description2I1O(string ToFormat) => Description(ToFormat, 2, 1);
		public RecipeBuilder Description2I2O(string ToFormat) => Description(ToFormat, 2, 2);
		public RecipeBuilder Description3I2O(string ToFormat) => Description(ToFormat, 3, 2);

		public RecipeBuilder RequiresTech(string techId)
		{
			techRequirement = techId;
			return this;
		}

		public RecipeBuilder NameDisplay(RecipeNameDisplay nameDisplay)
		{
			this.nameDisplay = nameDisplay;
			return this;
		}
		public RecipeBuilder InputHEP(int hep)
		{
			this.hepConsumed = hep;
			return this;
		}
		public RecipeBuilder OutputHEP(int hep)
		{
			this.hepProduced = hep;
			return this;
		}
		public RecipeBuilder NameOverride(string name)
		{
			this.name = name;
			return this;
		}
		public RecipeBuilder NameOverrideFormat(string name, object f1)
		{
			this.name = string.Format(name,f1);
			return this;
		}
		public RecipeBuilder NameOverrideFormatIngredient(string name, int ingredientIndex = 0)
		{
			var entries = GetFormatArgs(inputs.Count(), outputs.Count());
			if (ingredientIndex < 0 || ingredientIndex >= entries.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(ingredientIndex), "Ingredient index is out of range.");
			}


			this.name = string.Format(name, entries[ingredientIndex]);
			return this;
		}
		public RecipeBuilder IconPrefabIngredient(int index)
		{
			if(index < 0 || index >= inputs.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for inputs.");
			}
			var tag = inputs[index].material;
			return IconPrefabOverride(tag);
		}
		public RecipeBuilder IconPrefabResult(int index)
		{
			if (index < 0 || index >= outputs.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for outputs.");
			}
			var tag = outputs[index].material;
			return IconPrefabOverride(tag);
		}
		public RecipeBuilder IconPrefabOverride(Tag prefabId) => IconPrefabOverride(prefabId.ToString());
		public RecipeBuilder IconPrefabOverride(string prefabId)
		{
			this.spritePrefabId = prefabId;
			return this;
		}

		public RecipeBuilder SortOrder(int sortOrder)
		{
			this.sortOrder = sortOrder;
			return this;
		}

		public RecipeBuilder Input(Tag tag, float amount, bool inheritElement = false, bool doNotConsume = false)
		{
			var element = new RecipeElement(tag, amount, inheritElement);
			element.doNotConsume = doNotConsume;
			inputs.Add(element);
			return this;
		}
		public RecipeBuilder Input(IEnumerable<Tag> tags, float amount, Tag descriptor = default)
		{
			tags = tags.Where(tag => Assets.GetPrefab(tag) != null);
			RecipeElement recipeElement = new RecipeElement(tags.ToArray(), amount);
			inputs.Add(recipeElement);
			if(descriptor != default)
				GroupDescriptors.Add(recipeElement,descriptor);
			return this;
		}
		public RecipeBuilder Input(IEnumerable<SimHashes> tags, float amount, Tag descriptor = default)
		{
			var recipeElement = new RecipeElement(tags.Select(simhash => simhash.CreateTag()).ToArray(), amount);
			inputs.Add(recipeElement);
			if (descriptor != default)
				GroupDescriptors.Add(recipeElement, descriptor);
			return this;
		}
		public RecipeBuilder Input(IEnumerable<SimHashes> tags, float[] amounts, Tag descriptor = default)
		{
			var recipeElement = new RecipeElement(tags.Select(simhash => simhash.CreateTag()).ToArray(), amounts);
			inputs.Add(recipeElement);
			if (descriptor != default)
				GroupDescriptors.Add(recipeElement, descriptor);
			return this;
		}
		public RecipeBuilder InputSO(SimHashes simHashes, float amount, bool inheritElement = false) => InputConditional(simHashes, amount, DlcManager.IsExpansion1Active, inheritElement);
		public RecipeBuilder InputBase(SimHashes simHashes, float amount, bool inheritElement = false) => InputConditional(simHashes, amount, DlcManager.IsPureVanilla, inheritElement);

		public RecipeBuilder InputDlcDependent(SimHashes basegame, SimHashes spacedout, float amount, bool inheritElement = false) => Input(DlcManager.IsPureVanilla() ? basegame : spacedout, amount, inheritElement);

		public RecipeBuilder InputConditional(SimHashes simhash, float amount, Func<bool> condition, bool inheritElement = false) => InputConditional(simhash.CreateTag(), amount, condition(), inheritElement);
		public RecipeBuilder InputConditional(SimHashes simhash, float amount, bool condition, bool inheritElement = false) => InputConditional(simhash.CreateTag(), amount, condition, inheritElement);
		public RecipeBuilder InputConditional(Tag tag, float amount, bool condition, bool inheritElement = false)
		{
			if (condition)
				return Input(tag, amount, inheritElement);
			else
				return this;
		}
		public RecipeBuilder InputConditional(IEnumerable<SimHashes> hashes, float amount, Func<bool> condition) => InputConditional(hashes, amount, condition());
		public RecipeBuilder InputConditional(IEnumerable<SimHashes> hashes, float amount, bool condition)
		{
			if (condition)
				return Input(hashes, amount);
			else
				return this;
		}
		public RecipeBuilder AltInput(SimHashes simHashes, float amount) => AltInput(simHashes.CreateTag(), amount);
		public RecipeBuilder AltInput(Tag tag, float amount)
		{
			var lastInput = inputs.Last();
			if(lastInput == null)
			{
				throw new InvalidOperationException("Cannot add alt input when there is no previous ingredient!");
			}
			inputs.Remove(lastInput);
			var previousTags = lastInput.possibleMaterials;
			var previousAmounts = lastInput.amount > 0 ? [lastInput.amount] : lastInput.possibleMaterialAmounts;

			previousTags = previousTags.Concat([tag]);
			previousAmounts = previousAmounts.Concat([amount]);
			var composite = new RecipeElement(previousTags, previousAmounts);
			composite.inheritElement = lastInput.inheritElement;
			return this;

		}
		public RecipeBuilder Input(SimHashes simhash, float amount, bool inheritElement = false)
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
		public RecipeBuilder OutputConditional(SimHashes hashes, float amount, Func<bool> condition, TemperatureOperation tempOp = TemperatureOperation.AverageTemperature, bool storeElement = false) => OutputConditional(hashes, amount, condition(), tempOp,storeElement);
		public RecipeBuilder OutputConditional(SimHashes hashes, float amount, bool condition, TemperatureOperation tempOp = TemperatureOperation.AverageTemperature, bool storeElement = false)
		{
			if (condition)
				return Output(hashes, amount);
			else
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

			var recipe = new ComplexRecipe(recipeID, i, o,hepConsumed,hepProduced)
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
			if(!spritePrefabId.IsNullOrWhiteSpace())
				recipe.customSpritePrefabID = spritePrefabId;
			if(!techRequirement.IsNullOrWhiteSpace())
				recipe.requiredTech = techRequirement;

			return recipe;
		}
	}
}
