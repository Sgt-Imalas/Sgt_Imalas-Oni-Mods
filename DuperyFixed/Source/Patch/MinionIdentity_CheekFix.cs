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

