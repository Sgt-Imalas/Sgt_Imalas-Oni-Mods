using HarmonyLib;
using OniRetroEdition.FX;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
	internal class MultiToolPatches
	{
		public static string DeconstructTool = "deconstruct";

		[HarmonyPatch(typeof(Deconstructable), nameof(Deconstructable.OnPrefabInit))]
		public static class SwitchToolType
		{
			public static void Postfix(Deconstructable __instance)
			{
				__instance.multitoolContext = DeconstructTool;
			}
		}

		[HarmonyPatch(typeof(Diggable), nameof(Diggable.UpdateColor))]
		public static class UpdateHitEffect
		{
			static bool HasOrIsTag(Element e, Tag tag)
			{
				return e.HasTag(tag) || e.GetMaterialCategoryTag() == tag;
			}
			public static void Postfix(Diggable __instance)
			{
				if (__instance.childRenderer == null)
					return;

				Material material = __instance.childRenderer.material;
				int targetCell = Grid.PosToCell(__instance.gameObject);
				Element element = Grid.Element[targetCell];


				if (Diggable.RequiresTool(element) || Diggable.Undiggable(element))
					return;

				if (element.HasTag(GameTags.IceOre))
				{
					__instance.multitoolHitEffectTag = DigIceEffect.ID;
				}
				else if (HasOrIsTag(element, GameTags.Metal) || HasOrIsTag(element, GameTags.RefinedMetal))
				{
					__instance.multitoolHitEffectTag = DigMetalEffect.ID;
				}
				else if (HasOrIsTag(element, GameTags.BuildableRaw))
				{
					__instance.multitoolHitEffectTag = DigRockEffect.ID;
				}
				else if (element.IsUnstable)
				{
					__instance.multitoolHitEffectTag = DigRubbleEffect.ID;
				}
			}
		}

		[HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.CreatePrefab))]
		public static class AddDeconstructGunOverrides
		{
			public static void Postfix(GameObject __result)
			{
				var snapon = __result.GetComponent<SnapOn>();
				if (snapon != null)
				{
					snapon.snapPoints.Add(new SnapOn.SnapPoint()
					{
						pointName = "dig",
						automatic = false,
						context = (HashedString)DeconstructTool,
						buildFile = Assets.GetAnim((HashedString)"constructor_gun_kanim"),
						overrideSymbol = (HashedString)"snapTo_rgtHand"
					});

				}
			}
		}


		[HarmonyPatch(typeof(MinionConfig), nameof(MinionConfig.SetupLaserEffects))]
		public static class RegisterDeconstructLaser
		{

			private static GameObject LaserEffectGO;
			private static readonly MethodInfo GetGameObjectMethod = AccessTools.Method(
					typeof(RegisterDeconstructLaser),
					nameof(RegisterDeconstructLaser.GetGameObject)
			   );
			public static GameObject GetGameObject(GameObject target)
			{
				LaserEffectGO = target;
				return target;
			}
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();

				var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldstr && ci.operand.ToString() == "LaserEffect");


				if (insertionIndex != -1)
				{
					insertionIndex++;
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, GetGameObjectMethod));
				}
				else
					SgtLogger.error("SetupLaserEffects TRANSPILER BROKE");
				// Debug.Log("DEBUGMETHOD: " + new CodeInstruction(OpCodes.Call, PacketSizeHelper));

				//SgtLogger.l("SetupLaserEffects TRANSPILER");
				//TranspilerHelper.PrintInstructions(code);
				return code;
			}


			public static void Postfix(GameObject prefab)
			{
				List<MinionConfig.LaserEffect> laserEffectArray = new()
				{
					new MinionConfig.LaserEffect()
					{
						id = "DeconstructEffect",
						animFile = "deconstruct_fx_kanim",
						anim = "beam",
						context = (HashedString)DeconstructTool
					},
				};


				if (LaserEffectGO != null)
				{

					var gameObject = LaserEffectGO;
					KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
					KBatchedAnimEventToggler animEventToggler = gameObject.GetComponent<KBatchedAnimEventToggler>();

					foreach (MinionConfig.LaserEffect laserEffect in laserEffectArray)
					{
						GameObject go = new GameObject(laserEffect.id);
						go.transform.parent = gameObject.transform;
						go.AddOrGet<KPrefabID>().PrefabTag = new Tag(laserEffect.id);
						KBatchedAnimTracker kbatchedAnimTracker = go.AddOrGet<KBatchedAnimTracker>();
						kbatchedAnimTracker.controller = component;
						kbatchedAnimTracker.symbol = new HashedString("snapTo_rgtHand");
						kbatchedAnimTracker.offset = new Vector3(195f, -35f, 0.0f);
						kbatchedAnimTracker.useTargetPoint = true;
						KBatchedAnimController kbatchedAnimController = go.AddOrGet<KBatchedAnimController>();
						kbatchedAnimController.AnimFiles = new KAnimFile[1]
									{
							Assets.GetAnim((HashedString) laserEffect.animFile)
									};
						KBatchedAnimEventToggler.Entry entry = new KBatchedAnimEventToggler.Entry()
						{
							anim = laserEffect.anim,
							context = laserEffect.context,
							controller = kbatchedAnimController
						};
						animEventToggler.entries.Add(entry);
						go.AddOrGet<LoopingSounds>();
					}
				}
			}
		}
	}
}
