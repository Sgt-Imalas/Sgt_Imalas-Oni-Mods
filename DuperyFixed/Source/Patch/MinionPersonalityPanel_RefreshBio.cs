using HarmonyLib;
using UnityEngine;

namespace Dupery.Patch
{
	[HarmonyPatch(typeof(MinionPersonalityPanel))]
	[HarmonyPatch(nameof(MinionPersonalityPanel.RefreshBioPanel))]
	internal class MinionPersonalityPanel_RefreshBio
	{
		[HarmonyPostfix]
		static void RefreshBio(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
		{
			if (!targetEntity.TryGetComponent<MinionIdentity>(out var minionIdentity))
				return;
			targetPanel.SetLabel("personality",
				string.Format(DuperyPatches.PersonalityManager.FindDescription(minionIdentity.nameStringKey), minionIdentity.name),
				string.Format(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", minionIdentity.nameStringKey.ToUpper())), minionIdentity.name));
		}
	}
}
