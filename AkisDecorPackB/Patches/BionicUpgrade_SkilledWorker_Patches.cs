using AkisDecorPackB.Content.ModDb;
using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Patches
{
	internal class BionicUpgrade_SkilledWorker_Patches
	{

        [HarmonyPatch(typeof(BionicUpgrade_SkilledWorker.Def), MethodType.Constructor, [typeof(string), typeof(string), typeof(AttributeModifier[]), typeof(SkillPerk[]), typeof(string[])])]
        public class BionicUpgrade_SkilledWorker_Def_TargetMethod_Patch
        {
            public static void Prefix(ref SkillPerk[] skillPerks)   
            {
                if (skillPerks == null || !skillPerks.Any())
                    return;

                skillPerks = ModSkillPerks.AddExtraPerks(skillPerks.ToList()).ToArray();

			}
        }
	}
}
