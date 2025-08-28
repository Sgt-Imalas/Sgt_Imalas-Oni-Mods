using ElementUtilNamespace;
using HarmonyLib;
using PeterHan.PLib.Core;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class Reactor_Patches
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
		private static double ReplaceGermCountOnLightReactor(double germcount, Reactor instance)
		{
			if (instance is LightReactor lightReactor)
			{
				SgtLogger.l("Replacing germcount for ligth reactor: " + germcount);
				return lightReactor.GetRadGermMultiplierRads(germcount);
			}
			return germcount;
		}

		[HarmonyPatch(typeof(Reactor), nameof(Reactor.DumpSpentFuel))]
		public class Reactor_DumpSpentFuel_Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				// find injection point
				double originalGermCount = 50;

				var index = codes.FindIndex(ci => ci.LoadsConstant(originalGermCount));

				if (index == -1)
				{
					SgtLogger.l("TRANSPILER FAILED: Could not find original germ count in Reactor.DumpSpentFuel");
					return codes;
				}

				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(Reactor_Patches), nameof(ReplaceGermCountOnLightReactor));

				// inject right after the found index
				codes.InsertRange(index + 1,
				[
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, m_InjectedMethod)
						]);

				return codes;
			}
		}

		[HarmonyPatch]
		public class PatchInnerMeltdownLoop
		{

			[HarmonyPrepare]
			public static bool Prepare() => FindReactorMeltdownLoop() != null;
			[HarmonyTargetMethod]
			public static MethodBase FindReactorMeltdownLoop()
			{
				SgtLogger.l("Finding Reactor meltdown loop method...");
				var targetType = typeof(Reactor.States);
				foreach (var innerClass in targetType.GetNestedTypes(AccessTools.all))
				{
					if (!innerClass.AssemblyQualifiedName.Contains("Reactor+States+<>c"))
						continue;
					foreach (var method in innerClass.GetMethods(AccessTools.all))
					{
						MethodBody methodBody = method.GetMethodBody();
						if (methodBody == null)
							continue;
						if (methodBody.LocalVariables.Any(var => var.LocalType == typeof(Comet)))
						{
							SgtLogger.l("Found target method for reactor meltdown loop: " + method.Name);
							return method;
						}
					}
				}
				SgtLogger.error("TRANSPILER FAILED: failed to find target method for reactor meltdown loop");
				return null;
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();

				// find injection point
				double originalGermCount = 50;


				var index = codes.FindIndex(ci => ci.LoadsConstant(originalGermCount));

				if (index == -1)
				{
					SgtLogger.l("TRANSPILER FAILED: Could not find original germ count in eactorMeltdownLoop");
					TranspilerHelper.PrintInstructions(codes);
					return codes;
				}

				var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(Reactor_Patches), nameof(ReplaceGermCountOnLightReactor));

				// inject right after the found index
				codes.InsertRange(index + 1,
				[
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, m_InjectedMethod)
						]);

				return codes;
			}
		}


		/// <summary>
		/// Reduced radiaton on light reactor
		/// </summary>
		[HarmonyPatch(typeof(Reactor.States), nameof(Reactor.States.InitializeStates))]
		public class Reactor_States_InitializeStates_Patch
		{
			//these run after the vanilla states are set, so we can conditionally override them
			public static void Postfix(Reactor.States __instance)
			{
				__instance.root.EventHandler(GameHashes.OnStorageChange,(smi =>
				{
					if (smi.master is not LightReactor lightReactor)
						return;
						PrimaryElement storedCoolant = smi.master.GetStoredCoolant();
					if (!storedCoolant)
						smi.master.waterMeter.SetPositionPercent(0.0f);
					else
						smi.master.waterMeter.SetPositionPercent(storedCoolant.Mass / 30f);
				}));

				__instance.on
					.Enter(smi =>
					{
						if (smi.master is LightReactor lightReactor)
						{
							lightReactor.SetEmitRads(lightReactor.GetEmissionRads(false));
						}
					});

				__instance.meltdown.loop
					.Enter(smi =>
					{
						if (smi.master is LightReactor lightReactor)
						{
							lightReactor.SetEmitRads(lightReactor.GetEmissionRads(true));
						}
					});
				__instance.dead
					.Update((smi, dt) =>
					{
						if (smi.master is LightReactor lightReactor)
						{
							lightReactor.SetEmitRads(Mathf.Lerp(lightReactor.GetEmissionRads(true), 0f, smi.sm.timeSinceMeltdown.Get(smi) / 3000f));
						}
					});

				//__instance.meltdown.loop.updateActions

			}

			/// <summary>
			/// alternative to that transpiler above;
			/// replacing the entire update method, but thats a bit destructive...
			/// </summary>
			/// <param name="smi"></param>
			/// <param name="dt"></param>

			public static void ConfigurableMeltdownFunc(Reactor.StatesInstance smi, float dt)
			{

				double radGermsBaseMultiplier = 50f;
				if(smi.master is LightReactor lightReactor)
				{
					radGermsBaseMultiplier = lightReactor.GetRadGermMultiplierRads(radGermsBaseMultiplier);
				}

				smi.master.timeSinceMeltdownEmit += dt;
				float num = 0.5f;
				float b = 5f;
				if (smi.master.timeSinceMeltdownEmit > num && smi.sm.meltdownMassRemaining.Get(smi) > 0f)
				{
					smi.master.timeSinceMeltdownEmit -= num;
					float num2 = Mathf.Min(smi.sm.meltdownMassRemaining.Get(smi), b);
					smi.sm.meltdownMassRemaining.Delta(0f - num2, smi);
					for (int i = 0; i < 3; i++)
					{
						if (num2 >= NuclearWasteCometConfig.MASS)
						{
							GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(NuclearWasteCometConfig.ID), smi.master.transform.position + Vector3.up * 2f, Quaternion.identity);
							gameObject.SetActive(value: true);
							Comet component = gameObject.GetComponent<Comet>();
							component.ignoreObstacleForDamage.Set(smi.master.gameObject.GetComponent<KPrefabID>());
							component.addTiles = 1;
							int num3 = 270;
							while (num3 > 225 && num3 < 335)
							{
								num3 = UnityEngine.Random.Range(0, 360);
							}

							float f = (float)num3 * (float)Math.PI / 180f;
							component.Velocity = new Vector2((0f - Mathf.Cos(f)) * 20f, Mathf.Sin(f) * 20f);
							component.GetComponent<KBatchedAnimController>().Rotation = (float)(-num3) - 90f;
							num2 -= NuclearWasteCometConfig.MASS;
						}
					}

					for (int j = 0; j < 3; j++)
					{
						if (num2 >= 0.001f)
						{
							SimMessages.AddRemoveSubstance(Grid.PosToCell(smi.master.transform.position + Vector3.up * 3f + Vector3.right * j * 2f), SimHashes.NuclearWaste, CellEventLogger.Instance.ElementEmitted, num2 / 3f, 3000f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.RoundToInt((float)radGermsBaseMultiplier * (num2 / 3f)));
						}
					}
				}
			}
		}
	}
}
