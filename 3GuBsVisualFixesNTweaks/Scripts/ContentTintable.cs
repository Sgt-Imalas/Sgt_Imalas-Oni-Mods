using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	class ContentTintable : KMonoBehaviour
	{
		[MyCmpReq] Storage ContentStorage;
		[MyCmpGet] Operational operational;
		[MyCmpGet] KBatchedAnimController kbac;
		KBatchedAnimController kbacFG, kbacMeter;

		[SerializeField] public Tag TintTag = null;
		[SerializeField] public bool TintGeneratorMeter = false;
		[SerializeField] public bool TintPolymerizer = false;

		private static readonly EventSystem.IntraObjectHandler<ContentTintable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ContentTintable>((tintable, data) => tintable.UpdateTint());
		//private static readonly EventSystem.IntraObjectHandler<ContentTintable> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ContentTintable>((tintable, data) => tintable.ClearTint());
		HashSet<string> ExistingTintSymbols = [];
		HashSet<string> ExistingFGTintSymbols = [];
		HashSet<string> ExistingMeterTintSymbols = [];
		bool hasMeter, hasFg;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Subscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
			//Subscribe((int)GameHashes.ActiveChanged, OnActiveChangedDelegate);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (kbacFG == null)
			{
				if (kbac.layering?.foregroundController is KBatchedAnimController kbac2)
				{
					kbacFG = kbac2;
				}
			}
			if (TintGeneratorMeter && TryGetComponent<EnergyGenerator>(out var generator) && generator.hasMeter)
			{
				kbacMeter = generator.meter.meterController;
			}
			else if (TintPolymerizer && TryGetComponent<Polymerizer>(out var polymerizer))
			{
				kbacMeter = polymerizer.oilMeter.meterController;
			}
			AssignTintables();
			UpdateTint();
		}
		void AssignTintables()
		{
			ExistingTintSymbols.Clear();
			ExistingFGTintSymbols.Clear();
			ExistingMeterTintSymbols.Clear();

			bool HasSymbol(KBatchedAnimController kbac, string symbol_name)
			{
				KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(kbac.GetBatchGroupID()).GetSymbol(symbol_name);
				return symbol != null;
			}
			hasFg = kbacFG != null;
			hasMeter = kbacMeter != null;

			foreach (var symbol in ModAssets.PossibleTintSymbols)
			{
				if (HasSymbol(kbac, symbol))
				{
					ExistingTintSymbols.Add(symbol);
				}
				if (hasFg && HasSymbol(kbacFG, symbol))
				{
					ExistingFGTintSymbols.Add(symbol);
				}
				if (hasMeter && HasSymbol(kbacMeter, symbol))
				{
					ExistingMeterTintSymbols.Add(symbol);
				}
			}
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
			//Unsubscribe((int)GameHashes.ActiveChanged, OnActiveChangedDelegate);
		}

		public void UpdateTint()
		{
			foreach (var element in ContentStorage.GetItems())
			{
				if (!element.TryGetComponent<PrimaryElement>(out var liquidPrimaryElement) || liquidPrimaryElement.Mass <= 0)
				{
					continue;
				}

				if (TintTag != null && !element.HasTag(TintTag) && liquidPrimaryElement.ElementID.CreateTag() != TintTag)
				{
					continue;
				}

				var tintColor = liquidPrimaryElement.Element.substance.conduitColour;
				tintColor.a = 255;
				//SgtLogger.l("Tinting " + UI.StripLinkFormatting(gameObject.GetProperName()) + " with color from element " + UI.StripLinkFormatting(element.GetProperName()) + " with color: " + tintColor.ToString());



				TintAll(tintColor);
				break;
			}
		}
		void TintAll(Color color)
		{
			SetSymbolTint(kbac, ExistingTintSymbols, color);
			if (hasFg)
				SetSymbolTint(kbacFG, ExistingFGTintSymbols, color);
			if (hasMeter)
				SetSymbolTint(kbacMeter, ExistingMeterTintSymbols, color);
		}
		static void SetSymbolTint(KBatchedAnimController kbac, HashSet<string> symbols, Color color)
		{
			if (kbac == null) return;
			if (!symbols.Any()) return;
			foreach (var symbol in symbols)
			{
				kbac.SetSymbolTint(symbol, color);
			}
		}

		void ClearTint()
		{
			if (!operational.IsActive)
			{
				TintAll(Color.clear);
			}
		}
	}
}
