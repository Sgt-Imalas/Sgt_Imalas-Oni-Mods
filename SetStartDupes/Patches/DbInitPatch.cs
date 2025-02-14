using Beached_ModAPI;
using HarmonyLib;
using SetStartDupes.API_IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;

namespace SetStartDupes.Patches
{
	internal class DbInitPatch
	{

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
			[HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix(Db __instance)
            {
				RainbowFarts_API.InitRainbowFartsAPI();
                Beached_API.InitBeachedAPI();
				FixGeneShufflerTraits();
			}
            static void FixGeneShufflerTraits()
            {
				// gene shuffler traits were marked as negative for some reason. Possibly an oversight.
				var traits = Db.Get().traits;
				foreach (var trait in DUPLICANTSTATS.GENESHUFFLERTRAITS)
				{
					var traitDef = traits.Get(trait.id);
					if(traitDef != null)
					{
						traitDef.PositiveTrait = true;
					}
				}
			}
        }
	}
}
