using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	class ContentTintable : KMonoBehaviour
	{
		[MyCmpReq] Storage ContentStorage;
		[MyCmpGet] Operational operational;
		[MyCmpGet] KBatchedAnimController kbac;
		KBatchedAnimController kbacFG, kbacMeter;

		[SerializeField] public Tag TintTag = null;
		[SerializeField] public bool TintGeneratorMeter = false;

		private static readonly EventSystem.IntraObjectHandler<ContentTintable> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ContentTintable>((tintable, data) => tintable.UpdateTint());
		//private static readonly EventSystem.IntraObjectHandler<ContentTintable> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<ContentTintable>((tintable, data) => tintable.ClearTint());
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Subscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
			//Subscribe((int)GameHashes.ActiveChanged, OnActiveChangedDelegate);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (kbacFG == null)
			{
				if (kbac.layering?.foregroundController is KBatchedAnimController kbac2)
				{
					kbacFG = kbac2;
				}
			}
			if (TintGeneratorMeter && TryGetComponent<EnergyGenerator>(out var generator) && generator.hasMeter)
			{
				kbacMeter = generator.meter.meterController;
			}
			else
				SgtLogger.l("no tintable meter on " + gameObject.name);

			UpdateTint();
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe((int)GameHashes.OnStorageChange, OnStorageChangeDelegate);
			//Unsubscribe((int)GameHashes.ActiveChanged, OnActiveChangedDelegate);
		}

		public void UpdateTint()
		{
			foreach (var element in ContentStorage.GetItems())
			{
				if (!element.TryGetComponent<PrimaryElement>(out var liquidPrimaryElement) || liquidPrimaryElement.Mass <= 0)
				{
					continue;
				}

				if (TintTag != null && !element.HasTag(TintTag) && liquidPrimaryElement.ElementID.CreateTag() != TintTag)
				{
					continue;
				}

				var tintColor = liquidPrimaryElement.Element.substance.conduitColour;
				tintColor.a = 255;
				//SgtLogger.l("Tinting " + UI.StripLinkFormatting(gameObject.GetProperName()) + " with color from element " + UI.StripLinkFormatting(element.GetProperName()) + " with color: " + tintColor.ToString());

				kbac.SetSymbolTint("tint", tintColor);
				kbac.SetSymbolTint("tint_dark", tintColor);
				kbacMeter?.SetSymbolTint("tint", tintColor);
				kbacFG?.SetSymbolTint("tint_fg", tintColor);
				break;
			}
		}
		void ClearTint()
		{
			if (!operational.IsActive)
			{
				kbac.SetSymbolTint("tint", Color.clear);
				kbac.SetSymbolTint("tint_dark", Color.clear);
				kbacFG?.SetSymbolTint("tint_fg", Color.clear);
			}
		}
	}
}
