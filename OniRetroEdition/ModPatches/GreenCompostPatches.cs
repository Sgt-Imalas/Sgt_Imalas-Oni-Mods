using HarmonyLib;
using System;

namespace OniRetroEdition.ModPatches
{
	internal class GreenCompostPatches
	{
		[HarmonyPatch(typeof(Compost.States))]
		[HarmonyPatch("InitializeStates")]
		private static class MakeGreen
		{
			public static void Postfix(Compost.States __instance)
			{
				bool green = false;
				__instance.insufficientMass.PlayAnims((smi =>
				{
					return green ? new HashedString[2]
						{
							"composting_pst",
							"idle_half"
						} : new HashedString[1]
						{
							"idle_half"
						};
				}))
					.Toggle("no green", (smi => { }), (smi => green = false));
				__instance.composting.PlayAnim("composting_pre").QueueAnim("composting_loop", true)
					.Toggle("green", (smi => { }), (smi => green = true));
				__instance.inert
					.PlayAnims((smi =>
					{
						return (!green) ? new HashedString[1] { "on" } : new HashedString[2] { (HashedString)"composting_pst", (HashedString)"composting" };
					}));
				__instance.disabled.PlayAnims((Func<Compost.StatesInstance, HashedString[]>)(smi =>
				{
					return green ? new HashedString[2]
						{
							"composting_pst",
							"on"
						} : new HashedString[1]
						{
							"on"
						};
				})).Toggle("no green", (StateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State.Callback)(smi => { }), (StateMachine<Compost.States, Compost.StatesInstance, Compost, object>.State.Callback)(smi => green = false));
			}
		}
	}
}
