using HarmonyLib;
using Klei.AI;
using System.Collections.Generic;
using UtilLibs;
using static BawoonFwiend.ModAssets;

namespace BawoonFwiend
{
	internal class Patches
	{
		/// <summary>
		/// add buildings to plan screen
		/// </summary>
		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{

			public static void Prefix()
			{
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, BawoonBuildingConfig.ID, EspressoMachineConfig.ID);
			}
		}
		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch(nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.HomeLuxuries, BawoonBuildingConfig.ID);

				Effect BalloonBuddyEffect = Db.Get().effects.Get("HasBalloon");
				ModAssets.JustAMachine = new Effect(ModAssets.JustAMachineId, STRINGS.EFFECTS.NOTATRUEFRIEND.NAME, STRINGS.EFFECTS.NOTATRUEFRIEND.DESC, BalloonBuddyEffect.duration, true, false, false);
				foreach (AttributeModifier attributeModifier in BalloonBuddyEffect.SelfModifiers)
				{
					var newOne = attributeModifier.Clone();
					newOne.Value = (-8 + Config.Instance.MachineGivenBalloonBuff);
					newOne.Description = STRINGS.EFFECTS.NOTATRUEFRIEND.NAME;
					JustAMachine.Add(newOne);
				}
				JustAMachine.showInUI = false;
				Db.Get().effects.Add(ModAssets.JustAMachine);
			}
		}


		//[HarmonyPatch(typeof(EquippableBalloonConfig))]
		//[HarmonyPatch(nameof(EquippableBalloonConfig.OnEquipBalloon))]
		//public class AddNotTrueFriendEffect
		//{
		//    public static void Postfix(Equippable eq)
		//    {
		//        Ownables soleOwner = eq.assignee.GetSoleOwner();
		//        if (soleOwner.IsNullOrDestroyed())
		//            return;
		//        KMonoBehaviour target = (KMonoBehaviour)soleOwner.GetComponent<MinionAssignablesProxy>().target;
		//        if (target.TryGetComponent<Effects>(out var component))
		//        {
		//            component.Add(JustAMachine, false);
		//        }
		//    }
		//}
		[HarmonyPatch(typeof(EquippableBalloonConfig))]
		[HarmonyPatch(nameof(EquippableBalloonConfig.OnUnequipBalloon))]
		public class RemoveNotTrueFriendEffect
		{
			public static void Prefix(Equippable eq)
			{
				if (!eq.IsNullOrDestroyed() && !eq.assignee.IsNullOrDestroyed())
				{
					Ownables soleOwner = eq.assignee.GetSoleOwner();
					if (soleOwner.IsNullOrDestroyed())
						return;
					KMonoBehaviour target = (KMonoBehaviour)soleOwner.GetComponent<MinionAssignablesProxy>().target;
					if (target.IsNullOrDestroyed())
						return;
					if (target.TryGetComponent<Effects>(out var component) && component.HasEffect(JustAMachine))
					{
						component.Remove(JustAMachine);
					}
				}
			}
		}


		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}

		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.Load))]
		public static class Patch_ElementLoader_Load
		{
			public static void Postfix()
			{
				var uran = ElementLoader.GetElement(SimHashes.Hydrogen.CreateTag());
				if (uran.oreTags is null)
				{
					uran.oreTags = new Tag[] { };
				}
				uran.oreTags = uran.oreTags.AddToArray(ModAssets.Tags.BalloonGas);

				var lead = ElementLoader.GetElement(SimHashes.Helium.CreateTag());
				if (lead.oreTags is null)
				{
					lead.oreTags = new Tag[] { };
				}
				lead.oreTags = lead.oreTags.AddToArray(ModAssets.Tags.BalloonGas);
			}
		}

		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "icon_balloon_toggle_random_icon");
			}
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class CustomSideScreenPatch_SatelliteCarrier
		{
			public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
			{
				UIUtils.AddClonedSideScreen<BalloonStationSkinSelectorSidescreen>("BalloonStationSkinSelectorSidescreen", "ArtableSelectionSideScreen", typeof(ArtableSelectionSideScreen));
			}
		}

	}
}
