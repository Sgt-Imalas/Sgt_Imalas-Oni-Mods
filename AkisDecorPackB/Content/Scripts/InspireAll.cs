using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Scripts
{
	public class InspireAll : KMonoBehaviour
	{
		[MyCmpReq] public Operational operational;
		[SerializeField] public string effectId;

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)GameHashes.NewDay, _ => ApplyEffectToDupes(true));
			Subscribe((int)GameHashes.OperationalChanged, _ => ApplyEffectToDupes(true));
			ApplyEffectToDupes(true);
		}

		private void ApplyEffectToDupes(bool emote)
		{
			if (!operational.IsFunctional)
				return;

			var worldId = this.GetMyWorldId();

			foreach (var identity in Components.LiveMinionIdentities.GetWorldItems(worldId, true))
			{
				if (identity is null)
					continue;

				if (emote)
					identity.GetComponent<Facing>().Face(transform.position.x);

				identity.TryGetComponent(out Effects effects);
				{
					if (effects.HasEffect(effectId))
						effects.Remove(effectId);

					effects.Add(effectId, true);
				}
			}
		}
	}
}
