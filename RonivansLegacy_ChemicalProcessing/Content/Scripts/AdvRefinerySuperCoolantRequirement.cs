using PeterHan.PLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.CustomComplexFabricators;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Descriptor;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class AdvRefinerySuperCoolantRequirement : KMonoBehaviour, IModifiesEfficiencyMultiplier, IGameObjectEffectDescriptor
	{
		[MyCmpReq]
		ComplexFabricatorWorkable workable;
		[MyCmpReq]
		LiquidCooledRefinery fabricator;
		[MyCmpReq]
		KSelectable selectable;

		const float RefinerySlowDown = 1f / 2f;
		Tag SUPERCOOLANT;
		readonly List<int> handles = [];
		bool SuperCoolantInRefinery;

		public bool Multiplicative => true;

		public override void OnSpawn()
		{
			base.OnSpawn();
			SUPERCOOLANT = SimHashes.SuperCoolant.CreateTag();
			handles.Add(Subscribe((int)GameHashes.FabricatorOrderCompleted, OnRecipeCompletedAction));
			handles.Add(Subscribe(ModAssets.FabricatorOrderCanceled, OnRecipeCompletedAction));
			handles.Add(Subscribe((int)GameHashes.FabricatorOrderStarted, OnRecipeStartedAction));
		}
		public override void OnCleanUp()
		{
			foreach (var handle in handles)
				Unsubscribe(handle);
			handles.Clear();
			base.OnCleanUp();
		}

		bool IsCurrentlySuperCooled()
		{
			foreach (var ingredient in fabricator.buildStorage.items)
				if (ingredient.HasTag(SUPERCOOLANT) || ingredient.TryGetComponent<PrimaryElement>(out var ele) && ele.Element.IsLiquid && ele.Element.specificHeatCapacity >= 8f) //allow any coolants with similar or better performance, like i.e. Hypercoolant from that one mod 
					return true;
			return false;
		}
		void ToggleStatusItem(bool on)
		{
			this.selectable.ToggleStatusItem(StatusItemsDatabase.AdvRefinery_NoSupercoolant, on);
		}
		public void OnRecipeStartedAction(object data)
		{
			SuperCoolantInRefinery = IsCurrentlySuperCooled();
			ToggleStatusItem(!SuperCoolantInRefinery);
		}

		public void OnRecipeCompletedAction(object data)
		{
			ToggleStatusItem(false);
		}

		public float ApplyEfficiencyModifierChanges(float modifier)
		{
			if (SuperCoolantInRefinery)
				return modifier;
			return modifier * RefinerySlowDown;
		}

		public string FormatDescriptor(string text, float wattage, float maxWattage = -1)
		{
			text = text.Replace("{UsedPower}", GameUtil.GetFormattedWattage(wattage));
			if (maxWattage > 0)
			{
				text = text.Replace("{MaxPower}", GameUtil.GetFormattedWattage(maxWattage));
			}
			return text;
		}
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			var supercoolant = ElementLoader.FindElementByHash(SimHashes.SuperCoolant);
			return [new(
				string.Format(STRINGS.BUILDING.EFFECTS.PREFERRED_COOLANT.NAME, supercoolant.name),
				string.Format(STRINGS.BUILDING.EFFECTS.PREFERRED_COOLANT.TOOLTIP, supercoolant.name),
				DescriptorType.Requirement)];
		}
	}
}
