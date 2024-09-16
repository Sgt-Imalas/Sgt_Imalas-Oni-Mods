using HarmonyLib;
using Klei.AI;
using System.Linq;
using UtilLibs;

namespace TinyFixes
{
	internal class Patches
	{
		/// <summary>
		/// Fix the reactor meter by removing that obsolete frame scale hack thing from an earlier reactor implementation
		/// </summary>
		[HarmonyPatch(typeof(Reactor), nameof(Reactor.OnSpawn))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Prefix()
			{
				Reactor.meterFrameScaleHack = 1;
			}
		}

		/// <summary>
		/// fix that check to actually check for immunities instead of hardcoding for the WarmTouch effect (which breaks the for WarmTouchFood)
		/// </summary>
		[HarmonyPatch(typeof(ColdImmunityMonitor), nameof(ColdImmunityMonitor.HasImmunityEffect))]
		public static class ColdImmunityMonitor_HasImmunityEffect
		{
			static Effect ColdAir;
			public static void Postfix(ref bool __result, ColdImmunityMonitor.Instance smi)
			{
				if (__result)
					return;
				if (ColdAir == null)
					ColdAir = Db.Get().effects.Get("ColdAir");

				var effects = smi.GetComponent<Effects>();
				if (effects.HasImmunityTo(ColdAir))
					__result = true;
			}
		}
		/// <summary>
		/// add proper cold air effect immunity to WarmTouch and WarmTouchFood so the tooltips actually reflect that
		/// </summary>
		[HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
		public class Db_Initialize_Patch
		{
			public static void Postfix()
			{
				Effect frostImmunityEffect = Db.Get().effects.Get("WarmTouch");
				frostImmunityEffect.immunityEffectsNames = frostImmunityEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();

				Effect frostImmunityFoodEffect = Db.Get().effects.Get("WarmTouchFood");
				frostImmunityFoodEffect.immunityEffectsNames = frostImmunityFoodEffect.immunityEffectsNames.AddItem("ColdAir").ToArray();
			}
		}
	}
}
