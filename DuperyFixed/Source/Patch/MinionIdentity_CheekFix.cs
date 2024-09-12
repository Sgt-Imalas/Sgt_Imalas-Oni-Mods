using Dupery;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace DuperyFixed.Source.Patch
{
	internal class MinionIdentity_CheekFix
	{
		public class MinionIdentityPatch
		{
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
						return codes;

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
						&& DuperyPatches.AccessoryManager.TryGetHeadAnimOverride(identity, out var anim)
						&& anim != null)
						return anim;

					return originalKanimFile;
				}
			}
		}
	}
}

