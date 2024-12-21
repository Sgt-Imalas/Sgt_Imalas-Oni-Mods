using Dupery;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UtilLibs;
using static STRINGS.DUPLICANTS;

namespace DuperyFixed.Source.Patch
{
	internal class MinionIdentity_CheekFix
	{
		public class MinionIdentityPatch
		{
			/// <summary>
			/// remapping the anim name for the cheek replacement when its in a custom anim
			/// </summary>
			[HarmonyPatch(typeof(MinionIdentity), nameof(MinionIdentity.OnSpawn))]
			public class MinionIdentity_OnSpawn_Patch
			{

				//public static bool Prefix(MinionIdentity __instance)
				//{
				//	if (__instance.addToIdentityList)
				//	{
				//		__instance.ValidateProxy();
				//		__instance.CleanupLimboMinions();
				//	}

				//	PathProber component = __instance.GetComponent<PathProber>();
				//	if (component != null)
				//	{
				//		component.SetGroupProber(MinionGroupProber.Get());
				//	}

				//	__instance.SetName(__instance.name);
				//	if (__instance.nameStringKey == null)
				//	{
				//		__instance.nameStringKey = __instance.name;
				//	}

				//	__instance.SetGender(__instance.gender);
				//	if (__instance.genderStringKey == null)
				//	{
				//		__instance.genderStringKey = "NB";
				//	}

				//	if (__instance.personalityResourceId == HashedString.Invalid)
				//	{
				//		Personality personalityFromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(__instance.nameStringKey);
				//		if (personalityFromNameStringKey != null)
				//		{
				//			__instance.personalityResourceId = personalityFromNameStringKey.Id;
				//		}
				//	}

				//	if (!__instance.model.IsValid)
				//	{
				//		Personality personalityFromNameStringKey2 = Db.Get().Personalities.GetPersonalityFromNameStringKey(__instance.nameStringKey);
				//		if (personalityFromNameStringKey2 != null)
				//		{
				//			__instance.model = personalityFromNameStringKey2.model;
				//		}
				//	}

				//	if (__instance.addToIdentityList)
				//	{
				//		Components.MinionIdentities.Add(__instance);
				//		if (!Components.MinionIdentitiesByModel.ContainsKey(__instance.model))
				//		{
				//			Components.MinionIdentitiesByModel[__instance.model] = new Components.Cmps<MinionIdentity>();
				//		}

				//		Components.MinionIdentitiesByModel[__instance.model].Add(__instance);
				//		if (!__instance.gameObject.HasTag(GameTags.Dead))
				//		{
				//			Components.LiveMinionIdentities.Add(__instance);
				//			if (!Components.LiveMinionIdentitiesByModel.ContainsKey(__instance.model))
				//			{
				//				Components.LiveMinionIdentitiesByModel[__instance.model] = new Components.Cmps<MinionIdentity>();
				//			}

				//			Components.LiveMinionIdentitiesByModel[__instance.model].Add(__instance);
				//			Game.Instance.Trigger(2144209314, __instance);
				//		}
				//	}

				//	SgtLogger.l("Pre-symbolz");
				//	SymbolOverrideController component2 = __instance.GetComponent<SymbolOverrideController>();
				//	if (component2 != null)
				//	{
				//		Accessorizer component3 = __instance.gameObject.GetComponent<Accessorizer>();
				//		if (component3 != null)
				//		{
				//			string text = HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.Mouth).symbol.hash).Replace("mouth", "cheek");
				//			SgtLogger.l("Cheek");
				//			component2.AddSymbolOverride("snapto_cheek", Assets.GetAnim("head_swap_kanim").GetData().build.GetSymbol(text), 1);
				//			SgtLogger.l("hair");
				//			component2.AddSymbolOverride("snapto_hair_always", component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
				//			SgtLogger.l("hat_hair");
				//			component2.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component3.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
				//		}
				//	}
				//	SgtLogger.l("symbolz");

				//	__instance.voiceId = (__instance.voiceIdx + 1).ToString("D2");
				//	Prioritizable component4 = __instance.GetComponent<Prioritizable>();
				//	if (component4 != null)
				//	{
				//		component4.showIcon = false;
				//	}

				//	Pickupable component5 = __instance.GetComponent<Pickupable>();
				//	if (component5 != null)
				//	{
				//		component5.carryAnimOverride = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				//	}
				//	SgtLogger.l("End");
				//	__instance.ApplyCustomGameSettings();
				//	return false;
				//}

				public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
				{
					var m_RemapSymbolName = AccessTools.Method(
						typeof(MinionIdentity_OnSpawn_Patch),
						"RemapAnimFileName",
						new[]
						{
						typeof(string),
						typeof(MinionIdentity)
						});

					var codes = orig.ToList();
					var index = codes.FindIndex(c => c.opcode == OpCodes.Ldstr && c.operand is string str && str == "head_swap_kanim");

					if (index == -1)
					{
						SgtLogger.error("DUPERY CHEEK TRANSPILER FAILED");
						return codes;
					}

					codes.InsertRange(index + 1, new[]
					{
					// string on stack
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Call, m_RemapSymbolName)
				});

					return codes;
				}

				private static string RemapAnimFileName(string originalKanimFile, MinionIdentity identity)
				{
					if (identity != null
						&& DuperyPatches.AccessoryManager.TryGetCheekGetterAnimOverride(identity, out var anim)
						&& anim != null)
					{

						SgtLogger.l("custom cheek anim found: " + anim);
						if (!identity.TryGetComponent<Accessorizer>(out var accessorizer))
						{
							SgtLogger.warning("no accessorizer was found!");
							return originalKanimFile; 
						}

						var customAnim = Assets.GetAnim(anim);
						if (customAnim == null)
						{
							SgtLogger.warning("custom anim was not found!");
							return originalKanimFile;
						}

						string cheek_symbol_name = HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Mouth).symbol.hash).Replace("mouth", "cheek");

						var symbol = customAnim.GetData().build.GetSymbol(cheek_symbol_name);

						SgtLogger.l("custom head_swap anim on "+ identity.name+" for cheeks: "+ cheek_symbol_name+ " in anim: " + anim);

						if (symbol == null)
						{
							SgtLogger.error("No cheek override symbol found for " + identity.name + " in anim: " + anim + " with the name " + cheek_symbol_name);
							return originalKanimFile;
						}
						SgtLogger.l("custom cheek override successfully registered");
							return anim;
					}

					return originalKanimFile;
				}
			}
		}
	}
}

