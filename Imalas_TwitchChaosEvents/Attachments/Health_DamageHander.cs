using Imalas_TwitchChaosEvents.Elements;
using System.Collections.Generic;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Attachments
{
	internal class Health_DamageHander : KMonoBehaviour, ISim1000ms
	{
		[MyCmpReq]
		Health _health;

		[MyCmpGet]
		SuitEquipper suitEquipper;

		[MyCmpGet]
		KSelectable selectable;
		[MyCmpGet]
		KPrefabID kPrefabID;
		[MyCmpGet]
		MinionIdentity minionIdentiy;

		float lastDamaged = 0;
		float accumulatedDamage = 0;

		public void Sim1000ms(float dt)
		{
			DamageChecker(dt);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
		}

		public static Dictionary<SimHashes, DamagingMaterial> damagingMaterials = new Dictionary<SimHashes, DamagingMaterial>()
		{
			{
				ModElements.Creeper.SimHash,
				new DamagingMaterial()
				{
					criticalMass = 0.1f,
					damagePerSecond = 100f/60f,
					//damagePerSecondSuited= 100f/600f,
				}
			},
			{
				ModElements.VoidLiquid.SimHash,
				new DamagingMaterial()
				{
					criticalMass = 0.1f,
					damagePerSecond = 100f/300f,
					damagePerSecondSuited= 100f/600f,
					suitDurabilityDamage = 1f/60f,
					isVoidDamage = true
				}
			}
		};
		public float lastBurnTime;

		void DealHealthDamage(DamagingMaterial damagingElement, float dt, out bool suited)
		{
			suited = false;
			float damage = damagingElement.damagePerSecond * dt;
			if (suitEquipper != null && suitEquipper.IsWearingAirtightSuit())
			{
				var suit = suitEquipper.IsWearingAirtightSuit();

				suited = true;
				damage = damagingElement.damagePerSecondSuited * dt;

				//damage suits when there is suit damage
				if(suit.TryGetComponent<Durability> (out var durability) && damagingElement.suitDurabilityDamage > 0)
				{
					durability.durability = Mathf.Clamp01(durability.durability - damagingElement.suitDurabilityDamage);
				}
			}
			if(damagingElement.isVoidDamage && minionIdentiy != null && minionIdentiy == RandomTickManager.Instance.Target)
			{
				damage *= 2f;
			}
			accumulatedDamage += damage;

			if (Time.time - lastDamaged < 3.0)
				return;

			_health.Damage(accumulatedDamage);
			accumulatedDamage = 0;
			lastDamaged = Time.time;
		}
		public bool IsMinion()
		{
			return kPrefabID.HasTag(GameTags.BaseMinion);
		}

		public void DamageChecker(float dt)
		{
			DamagingMaterial damagingElement = this.DamagingElementSearch();
			if (damagingElement != null)
			{
				DealHealthDamage(damagingElement, dt, out bool suited); 
				if (IsMinion() && !suited)
				{
					this.lastBurnTime = Time.time;
					selectable.AddStatusItem(damagingElement.isVoidDamage ? ModAssets.StatusItems.VoidBurns : ModAssets.StatusItems.CreeperBurns, this);
				}
			}
			else if(IsMinion())
			{
				if (Time.time - lastBurnTime > 5.0)
				{
					selectable.RemoveStatusItem(ModAssets.StatusItems.VoidBurns, (bool)this);
					selectable.RemoveStatusItem(ModAssets.StatusItems.CreeperBurns, (bool)this);
				}
			}
		}

		public bool IsDamagingElement(SimHashes chemical, float mass) => damagingMaterials.ContainsKey(chemical) && mass > damagingMaterials[chemical].criticalMass;

		public DamagingMaterial DamagingElementSearch()
		{
			if (!this.gameObject.HasTag(GameTags.Dead) && !this.gameObject.HasTag(GameTags.Robot))
			{
				int cell = Grid.PosToCell(this.gameObject);
				if (!Grid.IsValidCell(cell))
					return (DamagingMaterial)null;

				Element element1 = Grid.Element[cell];
				float mass1 = Grid.Mass[cell];

				if (this.IsDamagingElement(element1.id, mass1))
					return damagingMaterials[element1.id];
				if (suitEquipper != null)
				{
					int i = Grid.CellAbove(Grid.PosToCell(this.gameObject));
					if (!Grid.IsValidCell(i))
						return (DamagingMaterial)null;

					Element element2 = Grid.Element[i];
					float mass2 = Grid.Mass[i];
					if (this.IsDamagingElement(element2.id, mass2))
						return damagingMaterials[element2.id];
				}

			}
			return (DamagingMaterial)null;
		}

		public class DamagingMaterial
		{
			public float criticalMass = 0.0f;
			public float damagePerSecond = 10f;
			public float damagePerSecondSuited = 0f;
			public float suitDurabilityDamage = 0f;
			public bool isVoidDamage = false;
		}
	}
}
