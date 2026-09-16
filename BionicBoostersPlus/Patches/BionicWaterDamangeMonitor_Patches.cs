using BionicBoostersPlus.Content.ModDb;
using HarmonyLib;
using Klei.AI;

namespace BionicBoostersPlus.Patches
{
	internal class BionicWaterDamangeMonitor_Patches
	{

        [HarmonyPatch(typeof(BionicWaterDamageMonitor), nameof(BionicWaterDamageMonitor.InitializeStates))]
        public class BionicWaterDamageMonitor_InitializeStates_Patch
        {
            public static void Postfix(BionicWaterDamageMonitor __instance)
            {
				__instance.suffering
					.Exit(RemoveWaterproofEffect)
					.Enter(AddWaterpoofEffectIfValid)
					.EventHandler(GameHashes.BionicUpgradeChanged,RefreshWaterproofEffect);
            }
        }

		public static void RefreshWaterproofEffect(BionicWaterDamageMonitor .Instance smi)
		{
			if (!smi.master.gameObject.TryGetComponent<MinionResume>(out var resume) || !smi.master.gameObject.TryGetComponent<Effects>(out var effects))
				return;

			bool hasPerk = resume.HasPerk(BB_SkillPerks.BB_ReducedWaterStress);
			if (hasPerk == effects.HasEffect(BB_Effects.BB_WaterproofedStressReduction))
				return;

			if(hasPerk)
				effects.Add(BB_Effects.BB_WaterproofedStressReduction, true);
			else
				effects.Remove(BB_Effects.BB_WaterproofedStressReduction);
		}

		public static void RemoveWaterproofEffect(BionicWaterDamageMonitor.Instance smi)
		{
			if (smi.master.gameObject.TryGetComponent<Effects>(out var effects))
				effects.Remove(BB_Effects.BB_WaterproofedStressReduction);
		}
		public static void AddWaterpoofEffectIfValid(BionicWaterDamageMonitor.Instance smi)
		{
			if (smi.master.gameObject.TryGetComponent<MinionResume>(out var resume)
				&& resume.HasPerk(BB_SkillPerks.BB_ReducedWaterStress)
				&& smi.master.gameObject.TryGetComponent<Effects>(out var effects))
			{
				effects.Add(BB_Effects.BB_WaterproofedStressReduction, true);
			}
		}
	}
}
