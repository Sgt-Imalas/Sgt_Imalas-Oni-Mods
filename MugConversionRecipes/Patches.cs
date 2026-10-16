using HarmonyLib;
using MugConversionRecipes.Content.Defs;
using System.Collections.Generic;
using UtilLibs;

namespace MugConversionRecipes
{
	internal class Patches
	{

        [HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.LoadEntities))]
        public class ArtifactConfig_Init_Patch
		{
            public static void Postfix()
            {
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Refinement, ArtifactRecombinatorConfig.ID,SupermaterialRefineryConfig.ID);
                InjectionMethods.AddBuildingToTechnology(GameStrings.Technology.Gases.Catalytics, ArtifactRecombinatorConfig.ID);


                string id = ArtifactRecombinatorConfig.ID;
                //string mugId = "artifact_officemug";

                var allArtifacts = new List<string>();
                foreach(var artifactTypeList in ArtifactConfig.artifactItems)
                    allArtifacts.AddRange(artifactTypeList.Value);

				foreach (var artifactTypeList in ArtifactConfig.artifactItems)
                {
                    foreach(var artifact in artifactTypeList.Value)
                    {
                        //if (mugId == artifact)
                        //    continue;
                        List<Tag> otherArtifacts = [.. allArtifacts];
                        otherArtifacts.Remove(artifact);


                        //SgtLogger.l("adding mug recipe for " + artifact);

                        RecipeBuilder.Create(id, 150)
                            .Input(otherArtifacts, 1)
                            //.Input(SimHashes.Niobium, 1)
                            //.Input(SimHashes.Fullerene, 1)
                            //.Input(SimHashes.Isoresin, 1)
                            .Output(artifact, 1)
                            .Description(STRINGS.BUILDINGS.PREFABS.MCR_RECOMBINATOR.RECIPE_DESC, 0, 1)
                            .NameDisplay(ComplexRecipe.RecipeNameDisplay.Result)
                            .Build();
					}

                }
            }
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
			{
				UtilLibs.LocalisationUtil.Translate(typeof(STRINGS), true);
			}
        }
	}
}
