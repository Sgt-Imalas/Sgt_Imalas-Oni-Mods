using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class FilteredSolidConduitConsumer : KMonoBehaviour
	{
		private int _inputCell = -1;

		[MyCmpReq]
		private TreeFilterable treeFilterable;

		[MyCmpReq]
		private Operational operational;
		[MyCmpReq]
		private Building building;
		[MyCmpReq]
		private Storage storage;


		public override void OnSpawn()
		{
			base.OnSpawn();

			_inputCell = building.GetUtilityInputCell();
			SolidConduit.GetFlowManager().AddConduitUpdater(ConduitUpdate);
		}

		public override void OnCleanUp()
		{
			SolidConduit.GetFlowManager().RemoveConduitUpdater(ConduitUpdate);
			base.OnCleanUp();
		}

		private void ConduitUpdate(float dt)
		{
			if (!operational.IsOperational) return;

			var flowManager = SolidConduit.GetFlowManager();
			if (!flowManager.HasConduit(_inputCell) || flowManager.IsConduitEmpty(_inputCell))
				return;

			var acceptedTags = treeFilterable.AcceptedTags;

			var pickupable = flowManager.RemovePickupable(_inputCell);
			if (!(bool)pickupable)
				return;

			foreach (var acceptedTag in acceptedTags)
			{
				float capacity = storage.RemainingCapacity();
				if (pickupable.HasTag(acceptedTag) && pickupable.TotalAmount <= capacity)
				{
					var toStore = pickupable.Take(capacity);
					storage.Store(toStore.gameObject, true);
					return;
				}
			}

			flowManager.AddPickupable(_inputCell, pickupable);
		}
	}
}
