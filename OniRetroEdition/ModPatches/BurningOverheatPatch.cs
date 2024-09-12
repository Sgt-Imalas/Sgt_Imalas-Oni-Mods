using HarmonyLib;
using OniRetroEdition.FX;
using System.Collections.Generic;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
	internal class BurningOverheatPatch
	{
		[HarmonyPatch(typeof(Overheatable.States), nameof(Overheatable.States.InitializeStates))]
		private static class AddFire
		{
			static Dictionary<Overheatable.StatesInstance, Tuple<float, GameObject>> ActiveFireEffects = new();
			public static void Postfix(Overheatable.States __instance)
			{
				__instance.overheated
					.Update((smi, dt) =>
					{
						var go = smi.master.gameObject;
						if (!go.TryGetComponent<BuildingHP>(out var hp) || hp.smi == null)
							return;
						float percentage = hp.smi.HealthPercent();



						Tuple<float, GameObject> effectInfo;
						if (!ActiveFireEffects.TryGetValue(smi, out effectInfo))
						{
							effectInfo = new(percentage, null);
							ActiveFireEffects.Add(smi, effectInfo);
						}

						//building fully healed
						if (Mathf.Approximately(percentage, 1))
						{
							if (effectInfo.second != null)
							{
								UnityEngine.Object.Destroy(effectInfo.second);
							}
							return;
						}

						//same effect
						if (Mathf.Approximately(percentage, effectInfo.first) && effectInfo.second != null)
							return;

						if (effectInfo.second != null)
						{
							UnityEngine.Object.Destroy(effectInfo.second);
						}
						string newEffect = BurnEffectLarge.ID;
						if (percentage > (1f / 3f))
							newEffect = BurnEffectMedium.ID;
						if (percentage > (2f / 3f))
							newEffect = BurnEffectSmall.ID;

						var Effect = AddEffect(smi.master.gameObject, newEffect);
						ActiveFireEffects[smi] = new Tuple<float, GameObject>(percentage, Effect);

					})
					.Exit(smi => Extinguish(smi));
			}

			static void Extinguish(Overheatable.StatesInstance smi)
			{
				if (ActiveFireEffects.TryGetValue(smi, out var effectInfo))
				{
					if (effectInfo.second != null)
					{
						UnityEngine.Object.Destroy(effectInfo.second);
					}
					effectInfo.first = 1;
				}
				AddEffect(smi.master.gameObject, BurnEffectExtinguish.ID);
			}

			static GameObject AddEffect(GameObject target, string effectId)
			{
				var collider = target.GetComponent<KCollider2D>();

				Vector3 targetPoint = collider != null ? new(collider.bounds.center.x, target.transform.GetPosition().y + 0.5f) : new Vector3(target.transform.GetPosition().x, target.transform.GetPosition().y + 0.5f);

				var hitEffect = GameUtil.KInstantiate(Assets.GetPrefab(effectId), targetPoint, Grid.SceneLayer.Ore);
				hitEffect.TryGetComponent<KBatchedAnimController>(out var component2);
				hitEffect.SetActive(true);
				component2.sceneLayer = Grid.SceneLayer.Ore;
				component2.enabled = false;
				component2.enabled = true;
				return hitEffect;
			}
		}
	}
}
