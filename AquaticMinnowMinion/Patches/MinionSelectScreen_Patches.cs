using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Patches
{
	internal class MinionSelectScreen_Patches
	{

        [HarmonyPatch(typeof(MinionSelectScreen), nameof(MinionSelectScreen.EnsureSwimmingSkill))]
        public class MinionSelectScreen_EnsureSwimmingSkill_Patch
        {
            public static bool Prefix(CharacterContainer container)
            {
                //skip for aquatics
                if (container != null && container.Stats.personality.model == ModAssets.Tags.AquaticMinion)
                    return false;
                return true;

            }
        }
	}
}
