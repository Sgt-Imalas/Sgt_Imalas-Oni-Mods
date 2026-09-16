using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;

namespace Planticants.Patches
{
    class MinionVitalsPanel_Patches
    {
		[HarmonyPatch(typeof(MinionVitalsPanel), nameof(MinionVitalsPanel.Init))]
		public class MinionVitalsPanel_Init
		{
			public static void Postfix(MinionVitalsPanel __instance)
			{
				foreach(var amount in Aq_Amounts.GetAmounts())
				 __instance.AddAmountLine(amount);
			}
		}
	}
}
