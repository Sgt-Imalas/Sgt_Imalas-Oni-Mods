using HarmonyLib;
using System;

namespace Dupery
{
	[HarmonyPatch(typeof(UIDupeRandomizer), nameof(UIDupeRandomizer.Apply))]
	internal class UIDupeRandomizer_Apply
	{
		static void Postfix(KBatchedAnimController dupe, Personality personality)
		{
			KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(personality);

			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Hair, personality.nameStringKey, bodyData.hair);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.HeadShape, personality.nameStringKey, bodyData.headShape);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Mouth, personality.nameStringKey, bodyData.mouth);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Neck, personality.nameStringKey, bodyData.neck);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Body, personality.nameStringKey, bodyData.body);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Arm, personality.nameStringKey, bodyData.arms);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.ArmLower, personality.nameStringKey, bodyData.armslower);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.ArmLowerSkin, personality.nameStringKey, bodyData.armLowerSkin);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.ArmUpperSkin, personality.nameStringKey, bodyData.armUpperSkin);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.LegSkin, personality.nameStringKey, bodyData.legSkin);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Belt, personality.nameStringKey, bodyData.belt);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Pelvis, personality.nameStringKey, bodyData.pelvis);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Leg, personality.nameStringKey, bodyData.legs);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Foot, personality.nameStringKey, bodyData.foot);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Hand, personality.nameStringKey, bodyData.hand);
			AddAccessoryIfMissing(dupe, Db.Get().AccessorySlots.Cuff, personality.nameStringKey, bodyData.cuff);
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
