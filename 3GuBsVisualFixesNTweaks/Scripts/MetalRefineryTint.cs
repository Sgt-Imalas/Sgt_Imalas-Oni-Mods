using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.UI.TOOLS.FILTERLAYERS;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
	class MetalRefineryTint : KMonoBehaviour
	{
		[SerializeField] public Storage ProductStorage;

		[MyCmpReq] LiquidCooledRefinery refinery;
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] SymbolOverrideController soc;
		KBatchedAnimController kbacMeter;
		public override void OnSpawn()
		{
			Subscribe((int)GameHashes.OnStorageChange, UpdateTint);
			Subscribe(ModAssets.OnRefineryAnimPlayed, DropAllProducts);
			base.OnSpawn();
			UpdateTint(null);
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.OnStorageChange, UpdateTint);
			Unsubscribe(ModAssets.OnRefineryAnimPlayed, DropAllProducts);
			base.OnCleanUp();
		}
		void UpdateTint(object _)
		{
			UpdateCoolantTint();
			UpdateMetalTint();
		}
		void DropAllProducts(object _)
		{
			var cellRight = Grid.CellRight(Grid.CellRight(Grid.PosToCell(refinery.gameObject)));
			var cellRightBottom = Grid.CellBelow(cellRight);
			if (Grid.IsSolidCell(cellRight) || (!Grid.IsSolidCell(cellRightBottom)))
				ProductStorage.DropAll(offset: new(1f, 0));
			else
				ProductStorage.DropAll(offset: new(1.5f, 0));
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
