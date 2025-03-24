using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks
{
	class ContentTintable : KMonoBehaviour
	{
		[MyCmpReq] Storage ContentStorage;
		[MyCmpGet] Operational operational;
		[MyCmpGet] KBatchedAnimController kbac;

		private static readonly EventSystem.IntraObjectHandler<ContentTintable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ContentTintable>((tintable, data) => tintable.UpdateTint());
		private static readonly EventSystem.IntraObjectHandler<ContentTintable> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ContentTintable>((tintable, data) => tintable.ClearTint());
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Subscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
			Subscribe((int)GameHashes.ActiveChanged, OnActiveChangedDelegate);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
			Unsubscribe((int)GameHashes.ActiveChanged, OnActiveChangedDelegate);
		}

		void UpdateTint()
		{
			foreach (var liquid in ContentStorage.GetItems())
			{
				if (!liquid.TryGetComponent<PrimaryElement>(out var liquidPrimaryElement) || liquidPrimaryElement.Mass <= 0)
					continue;

				var tintColor = liquidPrimaryElement.Element.substance.conduitColour;
				kbac.SetSymbolTint("tint", tintColor);
				break;
			}
		}
		void ClearTint()
		{
			if(!operational.IsActive)
				kbac.SetSymbolTint("tint",Color.clear);
		}
	}
}
