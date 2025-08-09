//using HarmonyLib;
//using Klei.AI;
//using KSerialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using static Klei.SimUtil;

//namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
//{
//	class MultiRecipeCoffeeMaker : KMonoBehaviour, FewOptionSideScreen.IFewOptionSideScreen, IGameObjectEffectDescriptor
//	{
//		#region descriptor

//		public List<Descriptor> GetDescriptors(GameObject go)
//		{
			
//			if (!HasCustomRecipeSelected(out var drinkOption))
//			{
//				return [];
//			}
//			Descriptor rec = default(Descriptor);
//			rec.SetupDescriptor(global::STRINGS.UI.BUILDINGEFFECTS.RECREATION, global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
//			List<Descriptor> requirements = [rec];

//			Effect.AddModifierDescriptions(base.gameObject, requirements, drinkOption.DrinkEffect, increase_indent: true);
//			requirements.Add(AddRequirementDesc(drinkOption.InputLiquid, drinkOption.LiquidAmount));
//			requirements.Add(AddRequirementDesc(drinkOption.InputSolid, drinkOption.LiquidAmount));
//			return requirements;
//		}
//		private Descriptor AddRequirementDesc(Tag tag, float mass)
//		{
//			string str = tag.ProperName();
//			Descriptor descriptor = new Descriptor();
//			descriptor.SetupDescriptor(string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object)str, (object)GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), string.Format((string)global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object)str, (object)GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), Descriptor.DescriptorType.Requirement);
//			return descriptor;
//		}
//		#endregion

//		#region options
//		/// <summary>
//		/// placeholder tags as keys for the sidescreen and option dictionary
//		/// </summary>
//		public static readonly Tag
//			NormalEspresso = new Tag("Espresso"),
//			LatteMacchiato = new Tag("LatteMacchiato"),
//			OilySpresso = new Tag("OilySpresso")
//			;

//		public class DrinkOption
//		{

//			public string Name, Tooltip;

//			public Tag InputLiquid = SimHashes.Water.CreateTag();
//			public float LiquidAmount = 1f;
//			public Tag InputSolid = "SpiceNut";
//			public float SolidAmount = 1f;
//			public string DrinkEffect = "Espresso";
//			public Tag? LimitedToMinionModel = null; // Optional, if set, only minions of this model can use this drink
//		}
//		public static Dictionary<Tag, DrinkOption> DrinkOptions;

//		public static void AddDrinkConfig(Tag drinkTag, string drinkName, string tooltip, SimHashes inputLiquid, float drinkAmount = 1, string inputSolid = "SpiceNut", float solidAmount = 1, string drinkEffect = "Espresso", Tag? minionLimit = null)
//		{
//			if (DrinkOptions == null)
//				DrinkOptions = new Dictionary<Tag, DrinkOption>();
//			DrinkOptions[drinkTag] = new DrinkOption
//			{
//				InputLiquid = inputLiquid.CreateTag(),
//				LiquidAmount = drinkAmount,
//				InputSolid = inputSolid,
//				SolidAmount = solidAmount,
//				DrinkEffect = drinkEffect,
//				Name = drinkName,
//				Tooltip = tooltip,
//				LimitedToMinionModel = minionLimit
//			};
//		}
//		#endregion

//		[Serialize]
//		public Tag SelectedOption = NormalEspresso;
//		[MyCmpReq] ConduitConsumer liquidInput;
//		[MyCmpReq] ManualDeliveryKG solidInput;
//		[MyCmpReq] Storage storage;
//		public Storage Storage => storage;
//		public MultiRecipeCoffeeMaker()
//		{
//			AddDrinkConfig(NormalEspresso, "Espresso", "good o'l espresso, brewed from pincha pepper nuts with water.",SimHashes.Water, 1f, "SpiceNut", 1, "Espresso");
//			AddDrinkConfig(LatteMacchiato, "Latte Macchiato", "a milky, yet slightly salty brew, made from brackene and pincha pepper",SimHashes.Milk, 1f, "SpiceNut", 1, "Espresso", GameTags.Minions.Models.Standard);
//			AddDrinkConfig(OilySpresso, "Oily Espresso", "oily coffee brewed from crude oil and pincha pepper nuts.\nBionic duplicants love this drink.",SimHashes.CrudeOil, 1f, "SpiceNut", 1, "Espresso",GameTags.Minions.Models.Bionic);
//		}

//		public FewOptionSideScreen.IFewOptionSideScreen.Option[] GetOptions() => DrinkOptions.Select(o =>
//			new FewOptionSideScreen.IFewOptionSideScreen.Option(o.Key, o.Value.Name, Def.GetUISprite(o.Value.InputLiquid), o.Value.Tooltip)).ToArray();
//		public Tag GetSelectedOption() => SelectedOption;
//		public void OnOptionSelected(FewOptionSideScreen.IFewOptionSideScreen.Option option)
//		{
//			if (!DrinkOptions.TryGetValue(option.tag, out DrinkOption drinkOption))
//			{
//				Debug.LogError($"MultiRecipeCoffeeMaker: Option {option.tag} not found in DrinkOptions.");
//				return;
//			}
//			SelectedOption = option.tag;
//			ApplyDrinkOption(drinkOption);
//		}

//		void ApplyDrinkOption(DrinkOption drinkOption, bool drop = true)
//		{
//			if (drinkOption == null)
//			{
//				Debug.LogError("MultiRecipeCoffeeMaker: DrinkOption is null.");
//				return;
//			}
//			Tag oldLiquid = liquidInput.capacityTag;

//			liquidInput.capacityTag = drinkOption.InputLiquid;
//			solidInput.RequestedItemTag = drinkOption.InputSolid;

//			if(drinkOption.InputLiquid != oldLiquid && drop)
//			{
//				storage.DropAll();
//			}
//		}

//		public override void OnSpawn()
//		{
//			base.OnSpawn();

//			ApplyDrinkOption(DrinkOptions[SelectedOption], false);
//		}
//		public bool RecipeIngredientsReady()
//		{
//			HasCustomRecipeSelected(out var DrinkOption);

//			var liquid = storage.FindFirstWithMass(DrinkOption.InputLiquid, DrinkOption.LiquidAmount);
//			var solid = storage.FindFirstWithMass(DrinkOption.InputSolid, DrinkOption.SolidAmount);

//			return liquid != null && solid != null;
//		}

//		public bool HasCustomRecipeSelected() => HasCustomRecipeSelected(out _);
//		public bool HasCustomRecipeSelected(out DrinkOption customDrink)
//		{
//			customDrink = null;

//			if (SelectedOption == null || SelectedOption == NormalEspresso)
//				return false;

//			if (!DrinkOptions.TryGetValue(SelectedOption, out DrinkOption drinkOption))
//			{
//				Debug.LogError($"MultiRecipeCoffeeMaker: Selected option {SelectedOption} not found in DrinkOptions.");
//				return false;
//			}
//			customDrink = drinkOption;
//			return true;
//		}

//		internal void ApplyChoreLimitationsForCurrentRecipe(Chore drinkChore)
//		{
//			///no petrol for bionics
//			if(!HasCustomRecipeSelected(out var drinkOption))
//			{
//				return;
//			}
//			if (drinkOption.LimitedToMinionModel.HasValue)
//			{
//				var minionModel = drinkOption.LimitedToMinionModel.Value;

//				if (minionModel == GameTags.Minions.Models.Bionic)
//					drinkChore.AddPrecondition(ChorePreconditions.instance.IsBionic);
//				if (minionModel == GameTags.Minions.Models.Standard)
//					drinkChore.AddPrecondition(ChorePreconditions.instance.IsNotABionic);
//			}
//		}
//	}

//	/// <summary>
//	/// use custom descriptors from component instead for custom recipe
//	/// interface method is implemented explicitly, so nameof doesnt work here
//	/// </summary>
//	[HarmonyPatch(typeof(EspressoMachine), "IGameObjectEffectDescriptor.GetDescriptors")]
//	public class EspressoMachine_GetDescriptors_Patch
//	{
//		public static bool Prefix(EspressoMachine __instance, ref List<Descriptor> __result)	
//		{
//			if (!__instance.TryGetComponent(out MultiRecipeCoffeeMaker coffeeMaker))
//				return true;
//			if (!coffeeMaker.HasCustomRecipeSelected(out var drinkOption))
//				return true;
//			__result = new List<Descriptor>();
//			return false;
//		}
//	}

//	/// <summary>
//	/// add custom cmp
//	/// </summary>
//	[HarmonyPatch(typeof(EspressoMachineConfig), nameof(EspressoMachineConfig.ConfigureBuildingTemplate))]
//	public class EspressoMachineConfig_ConfigureBuildingTemplate_Patch
//	{
//		public static void Postfix(GameObject go)
//		{
//			go.AddOrGet<MultiRecipeCoffeeMaker>();
//		}
//	}

//	/// <summary>
//	/// ingredient check for custom recipes
//	/// </summary>
//	[HarmonyPatch(typeof(EspressoMachine.States), nameof(EspressoMachine.States.IsReady))]
//	public class EspressoMachine_States_IsReady_Patch
//	{
//		public static void Postfix(EspressoMachine.StatesInstance smi, ref bool __result)
//		{
//			if (smi?.master?.TryGetComponent(out MultiRecipeCoffeeMaker coffeeMaker) ?? false)
//			{
//				if (!coffeeMaker.HasCustomRecipeSelected())
//					return; // Default behavior for normal espresso
//				__result = coffeeMaker.RecipeIngredientsReady();				
//			}
//		}
//	}

//	/// <summary>
//	/// chore precondition that limit the espresso machine to bionic or standard minions, if the recipe is duplicant specific
//	/// </summary>
//	[HarmonyPatch(typeof(EspressoMachine.States), nameof(EspressoMachine.States.CreateChore))]
//	public class EspressoMachine_States_CreateChore_Patch
//	{
//		public static void Postfix(EspressoMachine.StatesInstance smi, Chore __result)
//		{
//			if (smi?.master?.TryGetComponent(out MultiRecipeCoffeeMaker coffeeMaker) ?? false)
//			{
//				if (!coffeeMaker.HasCustomRecipeSelected())
//					return; // Default behavior for normal espresso
//				coffeeMaker.ApplyChoreLimitationsForCurrentRecipe(__result);
//			}
//		}
//	}

//	/// <summary>
//	/// consume custom materials & apply custom effects when a custom drink is selected
//	/// </summary>
//	[HarmonyPatch(typeof(EspressoMachineWorkable), nameof(EspressoMachineWorkable.OnCompleteWork))]
//	public class EspressoMachineWorkable_OnCompleteWork_Patch
//	{
//		public static bool Prefix(EspressoMachineWorkable __instance, WorkerBase worker)
//		{
//			if(!__instance.TryGetComponent(out MultiRecipeCoffeeMaker coffeeMaker))			
//				return true;
//			if (!coffeeMaker.HasCustomRecipeSelected(out var drinkOption))
//				return true;

//			var consumeFromStorage = coffeeMaker.Storage;

//			consumeFromStorage.ConsumeAndGetDisease(drinkOption.InputLiquid, drinkOption.LiquidAmount, out _, out DiseaseInfo liquidDisease, out _);
//			consumeFromStorage.ConsumeAndGetDisease(drinkOption.InputSolid, drinkOption.SolidAmount, out _, out DiseaseInfo solidDisease, out _);

//			GermExposureMonitor.Instance smi = worker.GetSMI<GermExposureMonitor.Instance>();
//			if (smi != null)
//			{
//				smi.TryInjectDisease(liquidDisease.idx, liquidDisease.count, drinkOption.InputLiquid, Sickness.InfectionVector.Digestion);
//				smi.TryInjectDisease(solidDisease.idx, solidDisease.count, drinkOption.InputSolid, Sickness.InfectionVector.Digestion);
//			}

//			if(worker.TryGetComponent<Effects>(out var effects))
//			{
//				if(!string.IsNullOrEmpty(drinkOption.DrinkEffect))
//					effects.Add(drinkOption.DrinkEffect, true);
//				if (!string.IsNullOrEmpty(EspressoMachineWorkable.trackingEffect))
//					effects.Add(EspressoMachineWorkable.trackingEffect, true);
//			}
//			return false;
//		}
//	}

//	/// <summary>
//	/// the bionic specific anim for coffee is smelling it, then pouring it away
//	/// this patch overrides that to use the standard anim for the espresso machine for drinks especially made for bionic minions, like the oily espresso
//	/// </summary>
//	[HarmonyPatch(typeof(EspressoMachineWorkable), nameof(EspressoMachineWorkable.GetAnim))]
//	public class EspressoMachineWorkable_GetAnim_Patch
//	{
//		public static void Postfix(EspressoMachineWorkable __instance, ref Workable.AnimInfo __result)
//		{
//			if (!__instance.TryGetComponent(out MultiRecipeCoffeeMaker coffeeMaker))
//				return;
//			if (!coffeeMaker.HasCustomRecipeSelected(out var drinkOption))
//				return;
//			if(drinkOption.LimitedToMinionModel.HasValue && drinkOption.LimitedToMinionModel == GameTags.Minions.Models.Bionic)
//			{
//				__result.overrideAnims = __instance.workerTypeOverrideAnims[GameTags.Minions.Models.Standard];
//			}
//		}
//	}
//}
