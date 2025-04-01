using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class Db_Patches
	{       /// <summary>
			/// add proper cold air effect immunity to WarmTouch and WarmTouchFood so the tooltips actually reflect that
			/// </summary>
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				FixWarmTouchTooltips();
			}

			public static void FixWarmTouchTooltips()
			{
				var effects = Db.Get().effects;
				Effect frostImmunityEffect = effects.Get("WarmTouch");
				frostImmunityEffect.immunityEffectsNames = frostImmunityEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();

				Effect frostImmunityFoodEffect = effects.Get("WarmTouchFood");
				frostImmunityFoodEffect.immunityEffectsNames = frostImmunityFoodEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();
			}
		}
	}
}
