using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SatisfyingPowerShards.Components
{
	internal class PowerShardGrowthMonitor:KMonoBehaviour, ISim4000ms
	{
		[Serialize][SerializeField]
		public float ShardGrowth = 0;

		[Serialize][SerializeField]
		public float ShardGrowthPerDay;


		AttributeInstance metabolism;

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.metabolism = this.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Metabolism);
		}

		public void Sim4000ms(float dt)
		{
			
		}
	}
}
