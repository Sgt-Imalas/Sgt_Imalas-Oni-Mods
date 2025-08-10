using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace SetStartDupes.Patches
{
	internal class BionicUpgradesMonitorPatches
	{
		/// <summary>
		/// allow multiple bionic boosters to be spawned from the start
		/// </summary>
		[HarmonyPatch(typeof(BionicUpgradesMonitor), nameof(BionicUpgradesMonitor.SpawnAndInstallInitialUpgrade))]
		public class BionicUpgradesMonitor_SpawnAndInstallInitialUpgrade_Patch
		{
			public static bool Prefix(BionicUpgradesMonitor.Instance smi)
			{
				var traits = smi.GetComponent<Traits>();
				var traitIds = traits.GetTraitIds();
				var assignableEntity = smi.GetComponent<IAssignableIdentity>();

				HashSet<string> bionicTraits = DUPLICANTSTATS.BIONICUPGRADETRAITS.Select(entry => entry.id).ToHashSet();

				foreach (var traitID in traitIds)
				{
					if (!bionicTraits.Contains(traitID)) //not a bionic trait
						continue;

					var upgradePrefab = Assets.TryGetPrefab(BionicUpgradeComponentConfig.GetBionicUpgradePrefabIDWithTraitID(traitID));

					GameObject gameObject = Util.KInstantiate(upgradePrefab, smi.master.transform.position);
					gameObject.SetActive(true);
					BionicUpgradeComponent component2 = gameObject.GetComponent<BionicUpgradeComponent>();
					component2.Assign(assignableEntity);
					smi.InstallUpgrade(component2);
				}
				smi.sm.InitialUpgradeSpawned.Set(true, smi);
				smi.GoTo(smi.sm.inactive);
				return false;
			}
		}
	}
}
