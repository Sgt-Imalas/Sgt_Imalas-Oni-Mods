using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;
using static ComplexRecipe.RecipeElement;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	public static class Rephysicalized
	{
		public const string PRegistryKeyMetalDictionary = "Rephysicalized.MetalDictionary";
		private static Dictionary<string, List<Tuple<string, float>>> _metalOresAdditionalOutputs = null;
		private static bool _initialized = false;

		public static bool TryGetRephysicalizedProducts(Tag ore, out List<Tuple<string, float>> products)
		{
			products = null;
			if (!_initialized)
				InitDictionary();
			return _metalOresAdditionalOutputs.TryGetValue(ore.ToString(), out products);
		}
		static void InitDictionary()
		{
			_metalOresAdditionalOutputs = PRegistry.GetData<Dictionary<string, List<Tuple<string, float>>>>(PRegistryKeyMetalDictionary);
			if (_metalOresAdditionalOutputs == null)
				_metalOresAdditionalOutputs = [];
			_initialized = true;
		}
		public static RecipeBuilder RephysicalizedOutput(this RecipeBuilder builder, SimHashes hash, float amount, TemperatureOperation tempOp = TemperatureOperation.AverageTemperature, bool storeElement = false) => RephysicalizedOutput(builder, hash.CreateTag(), amount, tempOp, storeElement);
		public static RecipeBuilder RephysicalizedOutput(this RecipeBuilder builder, Tag tag, float amount, TemperatureOperation tempOp = TemperatureOperation.AverageTemperature, bool storeElement = false)
		{
			if (!TryGetRephysicalizedProducts(builder.FirstIngredient().material, out var products))
				builder.Output(tag, amount, tempOp, storeElement);
			else
			{
				for (int i = 0; i < products.Count; i++)
				{
					var output = products[i];
					builder.Output(output.first, amount * output.second, tempOp, storeElement);
				}
			}
			return builder;
		}

	}
}
