using HarmonyLib;
using System;

namespace BionicBoostersPlus.Patches
{
	internal class BaseMinionConfig_Patches
	{

		[HarmonyPatch(typeof(BaseMinionConfig), nameof(BaseMinionConfig.BaseOnSpawn))]
		public class BaseMinionConfig_BaseOnSpawn_Patch
		{
			public static void Prefix(Tag duplicantModel, ref Func<RationalAi.Instance, StateMachine.Instance>[] rationalAiSM)
			{
				if (duplicantModel == GameTags.Minions.Models.Bionic)
					rationalAiSM = [.. rationalAiSM, smi => new Dreamer.Instance(smi.master)];
			}
		}
	}
}
