using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
    class ModTechs
    {
        public static readonly string HPA_Rails_Research_ID = "HPA_Rails_Research";
        public static readonly string Biochemistry_RenewableFuel_ID = "Biochemistry_RenewableFuel";

		public static Tech HPA_Rails_Research, Biochemistry_RenewableFuel;

		public static void RegisterTechs(Database.Techs instance)
        {
            if (Config.Instance.HPA_Rails_Enabled)
            {
                HPA_Rails_Research = new Tech(HPA_Rails_Research_ID, [], instance);
            }
            if(Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				Biochemistry_RenewableFuel = new Tech(Biochemistry_RenewableFuel_ID, [], instance);
			}
		}
        public static void RegisterTechCards(ResourceTreeLoader<ResourceTreeNode> instance)
		{
			if (Config.Instance.HPA_Rails_Enabled)
			{
                TechUtils.AddNode(instance,
					HPA_Rails_Research_ID,
					DlcManager.IsExpansion1Active() ? GameStrings.Technology.SolidMaterial.SolidTransport : GameStrings.Technology.SolidMaterial.SolidControl,
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
		}
    }
}
