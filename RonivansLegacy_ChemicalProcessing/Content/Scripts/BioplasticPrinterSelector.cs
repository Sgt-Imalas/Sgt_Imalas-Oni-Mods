using Biochemistry.Buildings;
using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.Gaskets;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using static ElementConverter;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class BioplasticPrinterSelector : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen, ISim200ms
	{
		[MyCmpReq]
		public ElementConverter converter;
		[MyCmpReq]
		Polymerizer printer;
		[MyCmpReq]
		KSelectable selectable;
		[MyCmpReq]
		Storage storage;



		public BioplasticPrinterSelector() { }

		[Serialize]
		public Tag product = Tag.Invalid;
		[Serialize]
		private bool _doInternalConversion = false;

		private string _lastProductName;
		private float _productThresholdInternal = 0;
		private SymbolOverrideController _meterSoc;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
		}

		public override void OnSpawn()
		{
			base.OnSpawn();

			SetupMeterSymbolOverrideController();
			if (product == Tag.Invalid)
				product = ModElements.BioPlastic_Solid.Tag;

			LoadBioplasticRecipe(product);
		}
		void SetupMeterSymbolOverrideController()
		{
			var meterController = printer.plasticMeter;
			meterController.meterController.usingNewSymbolOverrideSystem = true;
			_meterSoc = meterController.gameObject.AddOrGet<SymbolOverrideController>();
			foreach(var anim_file in meterController.meterController.animFiles)
				_meterSoc.AddBuildOverride(anim_file.GetData(), -1);
		}


		private static Dictionary<Tag, Tuple<float, float>> _bioPlasticProducts = null;
		public static Dictionary<Tag, Tuple<float, float>> BioplasticProducts
		{
			get
			{
				if (_bioPlasticProducts == null)
				{
					InitOptions();
				}
				return _bioPlasticProducts;
			}
		}

		public static void InitOptions()
		{
			_bioPlasticProducts = new()
			{
				//custom internal converter amount, dropper threshold
				{ ModElements.BioPlastic_Solid.Tag, new(0, 25f)},
				{ BioPlasticGasketConfig.ID, new(50 , 1)}, //1 gasket is 50kg bioplastic
			};
		}

		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions()
		{
			var list = new List<FewOptionSideScreen.IFewOptionSideScreen.Option>();

			foreach (var entry in BioplasticProducts)
			{
				var name = Assets.TryGetPrefab(entry.Key)?.GetProperName() ?? entry.Key.name;
				var tooltip = string.Format(global::STRINGS.CODEX.FORMAT_STRINGS.TRANSITION_LABEL_TO_ONE_ELEMENT, ElementLoader.GetElement(ModElements.VegetableOil_Liquid.Tag).name, name);
				list.Add(new(entry.Key, name, Def.GetUISprite(entry.Key), tooltip));
			}
			return list.ToArray();
		}

		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
		{
			var newProduct = option.tag;
			if (newProduct == product)
				return;
			LoadBioplasticRecipe(option.tag);
		}

		public void LoadBioplasticRecipe(Tag target)
		{
			selectable.RemoveStatusItem(StatusItemsDatabase.Biplastic_PrintingStatus);
			_meterSoc.RemoveSymbolOverride("meter_plastic_chunk");

			if (!BioplasticProducts.TryGetValue(target, out var rates))
			{
				SgtLogger.warning(target + " was an invalid tag in bioplastic printer!");
				return;
			}
			product = target;
			_doInternalConversion = (rates.first > 0);
			_productThresholdInternal = rates.first;
			//bioplasticDropper.emitTag = target;
			//bioplasticDropper.emitMass = rates.second;

			float emitMass = Biochemistry_BioplasticPrinterConfig.EMITMASS;
			///reuse the meter going up but block bioplastic actually dropping
			printer.emitMass = _doInternalConversion ? _productThresholdInternal + 1f : emitMass;
			var targetPrefab = Assets.TryGetPrefab(target);

			if (_doInternalConversion && targetPrefab)
			{
				var build = targetPrefab.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build;
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
					_meterSoc.AddSymbolOverride("meter_plastic_chunk", symbol);
			}

			_lastProductName = targetPrefab?.GetProperName() ?? target.name;
			selectable.AddStatusItem(StatusItemsDatabase.Biplastic_PrintingStatus, this);
		}

		public Tag GetSelectedOption() => product;

		public void Sim200ms(float dt)
		{
			SpawnCustomProduct();
		}

		void SpawnCustomProduct()
		{
			if (!_doInternalConversion)
				return;
			if (storage.GetAmountAvailable(ModElements.BioPlastic_Solid.Tag) < _productThresholdInternal)
				return;

			storage.ConsumeAndGetDisease(ModElements.BioPlastic_Solid.Tag, _productThresholdInternal, out _, out var diseaseInfo, out float temp);

			GameObject productPrefab = Assets.GetPrefab(product);

			GameObject spawnedProduct = GameUtil.KInstantiate(productPrefab, Grid.SceneLayer.Ore);
			if (spawnedProduct.TryGetComponent<PrimaryElement>(out var element))
			{
				element.SetTemperature(temp);
				element.AddDisease(diseaseInfo.idx, diseaseInfo.count, "spawned from input");
			}
			spawnedProduct.SetActive(true);
			storage.Store(spawnedProduct);
		}


		internal string GetCurrentItemName() => _lastProductName.IsNullOrWhiteSpace() ? product.name : _lastProductName;

		internal string GetPercentageInfoText()
		{
			if (!_doInternalConversion)
				return string.Empty;

			float percentageComplete = Mathf.Clamp01(storage.GetAmountAvailable(ModElements.BioPlastic_Solid.Tag) / _productThresholdInternal);
			return string.Format($"({GameUtil.GetFormattedPercent(percentageComplete * 100f)})");
		}

		internal static Func<string, object, string> HandleStatusItem()
		{
			return delegate (string str, object obj)
			{
				if (obj is not BioplasticPrinterSelector ps)
					return str;

				return str
					.Replace("{ITEM}", ps.GetCurrentItemName())
					.Replace("{PERCENTAGE}", ps.GetPercentageInfoText());
			};
		}
	}
}