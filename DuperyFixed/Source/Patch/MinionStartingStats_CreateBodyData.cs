using HarmonyLib;
using System.Collections.Generic;
using UtilLibs;

namespace Dupery
{
	[HarmonyPatch(typeof(MinionStartingStats), nameof(MinionStartingStats.CreateBodyData))]
	internal class MinionStartingStats_CreateBodyData
	{

		[HarmonyPostfix]
		static void Postfix(ref KCompBuilder.BodyData __result, Personality p)
		{
			var slots = Db.Get().AccessorySlots;
			foreach (var slot in Db.Get().AccessorySlots.resources)
			{
				HashedString replacementId = FindNewId(slot, p.nameStringKey);
				if (replacementId == null)
					continue;

				if (slot == slots.HeadShape)
					__result.headShape = replacementId;
				else if(slot == slots.Mouth)
					__result.mouth = replacementId;
				else if (slot == slots.Neck)
					__result.neck = replacementId;
				else if (slot == slots.Eyes)
					__result.eyes = replacementId;
				else if (slot == slots.Hair)
					__result.hair = replacementId;
				else if (slot == slots.Body)
				{
					// body defines both the upper body and the arm clothes
					__result.body = replacementId;
					__result.arms = replacementId;
					__result.armslower = replacementId;
				}
				else if (slot == slots.ArmLowerSkin)
					__result.armLowerSkin = replacementId;
				else if (slot == slots.ArmUpperSkin)
					__result.armUpperSkin = replacementId;
				else if (slot == slots.LegSkin)
					__result.legSkin = replacementId;
				else if (slot == slots.Belt)
					__result.belt = replacementId;
				else if (slot == slots.Pelvis)
				{
					// pelvis defines both the pelvis and the leg clothes
					__result.pelvis = replacementId;
					__result.legs = replacementId;
				}
				else if (slot == slots.Foot)
					__result.foot = replacementId;
				else if (slot == slots.Cuff)
					__result.cuff = replacementId;
				else if (slot == slots.Hand)
					__result.hand = replacementId;
			}			
		}

		private static HashedString FindNewId(AccessorySlot slot, string duplicantId)
		{
			string id = DuperyPatches.PersonalityManager.FindOwnedAccessory(duplicantId, slot.Id);
			if (id != null)
				return HashCache.Get().Add(id);
			else
				return null;
		}
	}
}
