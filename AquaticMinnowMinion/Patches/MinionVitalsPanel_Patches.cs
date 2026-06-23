using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
