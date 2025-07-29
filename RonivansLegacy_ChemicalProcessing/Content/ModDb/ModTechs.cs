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

        public static Tech HPA_Rails_Research;

		public static void RegisterTechs(Database.Techs instance)
        {
            if (Config.Instance.HPA_Rails_Enabled)
            {
                HPA_Rails_Research = new Tech(HPA_Rails_Research_ID, [], instance);
            }
        }
        public static void RegisterTechCards(ResourceTreeLoader<ResourceTreeNode> instance)
		{
			if (Config.Instance.HPA_Rails_Enabled)
			{
                TechUtils.AddNode(instance,
					HPA_Rails_Research_ID,
					GameStrings.Technology.SolidMaterial.SolidTransport,
					xDiff: 1,
					yDiff: 1);
			}
		}
    }
}
