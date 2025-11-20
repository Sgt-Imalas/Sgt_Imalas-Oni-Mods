using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// Stagger research entries instead of having them in a single line
	/// </summary>
	public sealed class ResearchNotificationMessageFix : PForwardedComponent
	{
		public static void Register()
		{
			new ResearchNotificationMessageFix().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod = AccessTools.Method(typeof(ResearchCompleteMessage), nameof(ResearchCompleteMessage.GetMessageBody));
				var transpiler = AccessTools.Method(typeof(ResearchNotificationMessageFix), nameof(LinebreakTranspiler));
				plibInstance.Patch(targetMethod, transpiler: new(transpiler));
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		private static IEnumerable<CodeInstruction> LinebreakTranspiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
		{
			var codes = orig.ToList();

			// find injection point
			var index = codes.FindIndex(ci => ci.LoadsConstant(", "));

			if (index == -1)
			{
				Console.WriteLine("TRANSPILER FAILED: ResearchCompleteMessage");
				return codes;
			}
			codes[index].operand = "\n  • ";
			return codes;
		}
	}
}
