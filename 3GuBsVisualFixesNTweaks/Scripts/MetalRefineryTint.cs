using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI.TOOLS.FILTERLAYERS;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	class MetalRefineryTint : KMonoBehaviour
	{
		[MyCmpReq] LiquidCooledRefinery refinery;
		[MyCmpReq] KBatchedAnimController kbac;
		KBatchedAnimController kbacMeter;
		public override void OnSpawn()
		{
			Subscribe((int)GameHashes.OnStorageChange, UpdateTint);
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.OnStorageChange, UpdateTint);
			base.OnCleanUp();
		}
		void UpdateTint(object _)
		{
			UpdateCoolantTint();
			UpdateMetalTint();
		}
		void UpdateCoolantTint()
		{
			var liquid = refinery.inStorage.FindFirstWithMass(refinery.coolantTag);
			if (liquid == null)
				liquid = refinery.buildStorage.FindFirstWithMass(refinery.coolantTag);
			if (liquid == null)
				liquid = refinery.outStorage.FindFirstWithMass(refinery.coolantTag);

			if (liquid == null)
				return;
			kbac.SetSymbolTint("tint", ModAssets.GetElementColor(liquid.ElementID));
		}
		void UpdateMetalTint()
		{
			if (kbacMeter == null)
			{
				kbacMeter = refinery.meter_metal.meterController;
			}

			if (kbacMeter == null)
				return;

			if (!refinery.buildStorage.items.Any()) return;

			var targetMetal = refinery.buildStorage.FindFirst(GameTags.Metal);
			if (targetMetal == null)
				targetMetal = refinery.buildStorage.FindFirst(GameTags.RefinedMetal);

			if (targetMetal == null || !targetMetal.TryGetComponent<PrimaryElement>(out var oreElement))
				return;


			kbacMeter.SetSymbolTint("meter_target_metal", ModAssets.GetElementColor(oreElement.ElementID));
		}
	}
}
