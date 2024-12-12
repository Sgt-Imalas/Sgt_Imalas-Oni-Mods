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

   //     [HarmonyPatch(typeof(BionicUpgradesMonitor), nameof(BionicUpgradesMonitor.SpawnAndInstallInitialUpgrade))]
   //     public class BionicUpgradesMonitor_SpawnAndInstallInitialUpgrade_Patch
   //     {
   //         public static bool Prefix(BionicUpgradesMonitor.Instance smi)
   //         {
   //             var traits = smi.GetComponent<Traits>();
   //             var traitIds = traits.GetTraitIds();

   //             HashSet<string> bionicTraits = DUPLICANTSTATS.BIONICUPGRADETRAITS.Select(entry => entry.id).ToHashSet();

   //             foreach (var traitID in traitIds)
   //             {
   //                 if (!bionicTraits.Contains(traitID)) //not a bionic trait
   //                     continue;

			//		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(BionicUpgradeComponentConfig.GetBionicUpgradePrefabIDWithTraitID(traitID)), smi.master.transform.position);
			//		gameObject.SetActive(true);
			//		IAssignableIdentity component1 = smi.GetComponent<IAssignableIdentity>();
			//		BionicUpgradeComponent component2 = gameObject.GetComponent<BionicUpgradeComponent>();
			//		component2.Assign(component1);
			//		smi.InstallUpgrade(component2);
			//	}
			//	smi.sm.InitialUpgradeSpawned.Set(true, smi);
			//	return false;
			//}
   //     }
	}
}
