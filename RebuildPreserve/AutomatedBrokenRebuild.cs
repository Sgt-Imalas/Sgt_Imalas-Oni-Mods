using KSerialization;
using System;
using UnityEngine;

namespace RebuildPreserve
{
	internal class AutomatedBrokenRebuild : KMonoBehaviour
	{
		[MyCmpGet]
		Reconstructable reconstructable;
		[MyCmpGet]
		PrimaryElement primaryElement;
		[MyCmpGet]
		BuildingHP hp;

		[Serialize]
		public bool RebuildOnBreaking = false;

		private static readonly Action<object, object> OnCopySettingsDelegate = (component, data) => ((AutomatedBrokenRebuild)component).OnCopySettings(data);
		public bool IsValidForRebuilding => CanRebuild();

		bool CanRebuild()
		{
			if (reconstructable == null)
				return false;

			if (reconstructable.building.Def.Invincible)
				return false;

			if (!reconstructable.deconstructable.allowDeconstruction)
				return false;

			if (hp == null || hp.destroyOnDamaged)
				return false;

			return true;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (reconstructable == null)
			{
				this.enabled = false;
				return;
			}
			if (CanRebuild())
				this.Sub((int)GameHashes.BuildingBroken, OnBuildingBroken);
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			if (CanRebuild())
				this.Sub((int)GameHashes.CopySettings, OnCopySettingsDelegate);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			if (!CanRebuild())
			{
				return;
			}
			this.UnsubAll();
		}

		public void OnBuildingBroken(object data)
		{
			if (RebuildOnBreaking && reconstructable != null)
			{
				reconstructable.RequestReconstruct(primaryElement.Element.tag);
			}
		}

		public void OnCopySettings(object data)
		{
			if (data is GameObject sauceGameObject && sauceGameObject.TryGetComponent<AutomatedBrokenRebuild>(out var addon))
			{
				this.RebuildOnBreaking = addon.RebuildOnBreaking;
			}
		}
	}
}
