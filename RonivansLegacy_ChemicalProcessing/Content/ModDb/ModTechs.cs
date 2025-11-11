using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.Mining_DrillMk2_Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static RonivansLegacy_ChemicalProcessing.STRINGS.RESEARCH.TECHS;
using static UtilLibs.GameStrings;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class ModTechs
	{
		public static readonly string HPA_Rails_Research_ID = "HPA_Rails_Research";
		public static readonly string Biochemistry_RenewableFuel_ID = "Biochemistry_RenewableFuel";
		public static readonly string Mining_Mk2DrillTech_ID = "Mining_Mk2DrillTech";

		public static Tech HPA_Rails_Research, Biochemistry_RenewableFuel, Mining_Mk2DrillTech;

		public static void RegisterTechs(Database.Techs instance)
		{
			if (Config.Instance.HPA_Rails_Mod_Enabled)
			{
				HPA_Rails_Research = new Tech(HPA_Rails_Research_ID, [], instance);
			}
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				Biochemistry_RenewableFuel = new Tech(Biochemistry_RenewableFuel_ID, [], instance);
			}
			if (Config.Instance.MineralProcessing_Mining_Enabled)
			{
				Mining_Mk2DrillTech = new Tech(Mining_Mk2DrillTech_ID, [], instance);
			}
		}

		public static void RegisterCustomEntries()
		{
			if (Config.Instance.MineralProcessing_Mining_Enabled)
			{
				InjectionMethods.AddItemToTechnologyKanim(					
					SimpleDrillbits_Config.TECHITEM_ID,
					Technology.SolidMaterial.SolidControl,
					MINING_SIMPLEDRILLBIT_CRAFTING.NAME,
				MINING_SIMPLEDRILLBIT_CRAFTING.DESC,
				"drillbits_simple_kanim"
					);
			}
		}

		public static void RegisterTechCards(ResourceTreeLoader<ResourceTreeNode> instance)
		{
			if (Config.Instance.HPA_Rails_Mod_Enabled)
			{
				TechUtils.AddNode(instance,
					HPA_Rails_Research_ID,
					DlcManager.IsExpansion1Active() ? Technology.SolidMaterial.SolidTransport : Technology.SolidMaterial.SolidControl,
					xDiff: 1,
					yDiff: 1);
			}
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				TechUtils.AddNode(instance,
					Biochemistry_RenewableFuel_ID,
					DlcManager.IsExpansion1Active() ? GameStrings.Technology.Power.AdvancedCombustion : GameStrings.Technology.Power.FossilFuels,
					xDiff: 1,
					yDiff: DlcManager.IsExpansion1Active() ? 0 : 1);
			}
			if (Config.Instance.MineralProcessing_Mining_Enabled)
			{
				TechUtils.AddNode(instance,
					Mining_Mk2DrillTech_ID,
					Technology.SolidMaterial.SolidManagement,
					xDiff: 1,
					yDiff: 0);
			}
		}
	}
}
