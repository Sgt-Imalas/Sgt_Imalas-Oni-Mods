using Database;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class Db_Patches
	{
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Prefix()
			{
				SkinDatabase.AddSkins();
			}
			public static void Postfix(Db __instance)
			{
				BuildingManager.AddBuildingsToTechs();
				ModElements.OverrideDebrisAnims();
				SpaceMiningAdditions.AddExtraPOIElements();
				StatusItemsDatabase.CreateStatusItems();

				__instance.Skills.Add(new Skill(
					"ARCHEOLOGY_ID",
					"Secret Beached Skill",
					"filler",
					2,
					"hat_role_mining1", 
					"skillbadge_role_mining1",
					Db.Get().SkillGroups.Mining.Id,
					new List<SkillPerk> { Db.Get().SkillPerks.CanDigRadioactiveMaterials }, 
					new List<string> {
						__instance.Skills.Mining2.Id,
						__instance.Skills.Researching2.Id,
					}));
			}
		}
	}
}
