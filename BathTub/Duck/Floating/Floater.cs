using HarmonyLib;
using KSerialization;
using System.Reflection;
using UnityEngine;

namespace BathTub.Duck.Floating
{
	public class Floater : KMonoBehaviour, ISim4000ms
	{
		[MyCmpGet] Pickupable pickupable;
		[Serialize] public bool isFlipped;
		[MyCmpGet] KBatchedAnimController kbac;


		public float extendOffsetY = 0f;
		public void Sim4000ms(float dt)
		{
			if (Helpers.ShouldFloat(transform, out _))
			{
				AddFaller();
				TryMergeWithOthers();
			}
		}

		void AddFaller()
		{
			if (!GameComps.Fallers.Has(gameObject))
			{
				GameComps.Fallers.Add(gameObject, Vector2.zero);
			}

			MethodInfo addGravity = AccessTools.Method(typeof(FallerComponents), "AddGravity");
			addGravity.Invoke(null, new object[] { transform, Vector2.zero });
		}

		private void TryMergeWithOthers()
		{
			if (!pickupable.absorbable) return;

			Vector3 thisPosition = transform.GetPosition();

			int cell = Helpers.PosToCell(transform.position);
			pickupable.objectLayerListItem.Update(cell);
			for (ObjectLayerListItem i = pickupable.objectLayerListItem.nextItem; i != null; i = i.nextItem)
			{
				Pickupable target = i.gameObject.GetComponent<Pickupable>();
				if (target?.TryAbsorb(pickupable, false) == true)
				{
					// Offset the position to make it look less like one of the objects are vanishing
					target.transform.SetPosition((thisPosition + target.transform.GetPosition()) / 2);
					break;
				}
			}
		}

		internal Vector2 GetFloatingPosition(bool invert = false)
		{
			var pos = transform.GetPosition();
			if (invert)
				pos.y += extendOffsetY;
			else
				pos.y -= extendOffsetY;
			return pos;
		}

		internal void UpdateDirection(bool ShouldBeFlipped)
		{
			if (isFlipped == ShouldBeFlipped)
				return;

			isFlipped = ShouldBeFlipped;
			kbac.FlipX = isFlipped;
		}
	}
}
