using AkisSnowThings.Content.Defs.Buildings;
using AkisSnowThings.Content.Scripts;
using AkisSnowThings.Patches.Buildings;
using HarmonyLib;
using UtilLibs;

namespace AkisSnowThings.Patches
{
    public class DbPatch
	{
		[HarmonyPatch(typeof(Db), "Initialize")]
		public class Db_Initialize_Patch
		{
			public static void Prefix()
			{
				BuildingFacadesPatches.PrefixPatch();
			}

			public static void Postfix(Db __instance)
			{
				ModAssets.PresentSlot =  __instance.AssignableSlots.Add(new(ModAssets.PresentSlotId, STRINGS.ENTITIES.PREFABS.SNOWSCULPTURES_FESTIVEPRESENT.NAME, false));

				SnowStatusItems.CreateStatusItems();

				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, SnowSculptureConfig.ID, IceSculptureConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, GlassCaseConfig.ID,IceMachineConfig.ID );
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, SnowMachineConfig.ID,PixelPackConfig.ID);
				InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Furniture, ChristmasTreeAttachmentConfig.ID,BeachChairConfig.ID);

				InjectionMethods.AddBuildingToTechnology( GameStrings.Technology.Decor.ArtisticExpression, SnowSculptureConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.GlassBlowing,GlassCaseConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.InteriorDecor,SnowMachineConfig.ID);
				InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Decor.EnvironmentalAppreciation, ChristmasTreeAttachmentConfig.ID);

				ConfigureRecipes();

				ModAssets.LoadAssets();
			}

			private static void ConfigureRecipes()
			{
				var desc = string.Format(
					global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION,
					SimHashes.Ice.CreateTag().ProperName(),
					SimHashes.Snow.CreateTag().ProperName());

				RecipeBuilder.Create(RockCrusherConfig.ID, desc, 20f)
					.Input(SimHashes.Ice.CreateTag(), 100f)
					.Output(SimHashes.Snow.CreateTag(), 100f)
					.Build();
			}
		}
	}
}
