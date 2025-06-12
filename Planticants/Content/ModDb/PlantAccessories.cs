using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Planticants.Content.ModDb
{
    class PlantAccessories
    {
		public static void Register(Accessories accessories, AccessorySlots slots)
		{
			
		}

		public static void AddAllCustomAccessories(KAnimFile anim_file, ResourceSet parent, AccessorySlots slots)
		{
			if (anim_file == null)
				return;

			KAnim.Build build = anim_file.GetData().build;
			for (int i = 0; i < build.symbols.Length; i++)
			{
				string symbol_name = HashCache.Get().Get(build.symbols[i].hash);
				var accessorySlot = slots.resources.Find((AccessorySlot slot) => symbol_name.IndexOf(slot.Id, 0, StringComparison.OrdinalIgnoreCase) != -1);
				if (accessorySlot != null)
				{
					var accessory = new Accessory(symbol_name, parent, accessorySlot, anim_file.batchTag, build.symbols[i], anim_file);
					accessorySlot.accessories.Add(accessory);
					HashCache.Get().Add(accessory.IdHash.HashValue, accessory.Id);
					SgtLogger.l("added accessory: " + accessory.Id);
				}
			}
		}
		public static void AddCustomAccessoryForSlot(KAnimFile anim_file, AccessorySlot slot, ResourceSet parent)
		{
			SgtLogger.l(slot.Id);

			var build = anim_file.GetData().build;
			var id = slot.Id.ToLower();

			for (var i = 0; i < build.symbols.Length; i++)
			{
				var symbolName = HashCache.Get().Get(build.symbols[i].hash);
				SgtLogger.l(symbolName);

				if (symbolName.StartsWith(id))
				{
					var accessory = new Accessory(symbolName, parent, slot, anim_file.batchTag, build.symbols[i]);
					slot.accessories.Add(accessory);
					HashCache.Get().Add(accessory.IdHash.HashValue, accessory.Id);

					SgtLogger.l("Added accessory: " + accessory.Id);
				}
				else
				{
					SgtLogger.l($"Symbol {symbolName} in file {anim_file.name} is not starting with {id}");
				}
			}
		}
	}
}
