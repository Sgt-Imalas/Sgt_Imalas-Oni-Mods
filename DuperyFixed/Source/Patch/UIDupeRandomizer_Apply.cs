using HarmonyLib;
using System;

namespace Dupery
{
	[HarmonyPatch(typeof(UIDupeRandomizer))]
	[HarmonyPatch("Apply")]
	internal class UIDupeRandomizer_Apply
	{
		[HarmonyPostfix]
		static void Apply(Database.AccessorySlots ___slots, KBatchedAnimController dupe, Personality personality)
		{
			KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Hair, personality.nameStringKey, bodyData.hair);
		}

		private static void AddAccessoryIfMissing(KBatchedAnimController dupe, AccessorySlot slot, string duplicantId, HashedString accessoryId)
		{
			string customAccessoryId = DuperyPatches.PersonalityManager.FindOwnedAccessory(duplicantId, slot.Id);
			if (customAccessoryId != null)
			{
				Accessory accessory = slot.accessories.Find((Predicate<Accessory>)(a => a.IdHash == accessoryId));
				if (accessory == null)
					accessory = slot.accessories[0];

				UIDupeRandomizer.AddAccessory(dupe, accessory);
			}
		}
	}
}
