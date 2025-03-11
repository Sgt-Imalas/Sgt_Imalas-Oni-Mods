using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketPlatforms
{
	/// <summary>
	/// load order of buildings can lead to rocket platforms failing to detect the rocket above it because it hasnt been initialized yet.
	/// this causes the connection to fail-
	/// this class fixes this by forcing a delayed recheck
	/// </summary>
	internal class LandedStateFixer : KMonoBehaviour
	{
		[MyCmpGet] LaunchPad launchPad;
		public override void OnSpawn()
		{
			base.OnSpawn();
			StartCoroutine(ReCheckLandedState());
		}
		IEnumerator ReCheckLandedState()
		{
			yield return new WaitForSeconds(0.5f); //wait for half a second
			var lmd = gameObject.GetSMI<LaunchPadMaterialDistributor.Instance>();
			var rocketOnPad = launchPad.LandedRocket;
			if (rocketOnPad != null)
			{
				if (lmd.IsInsideState(lmd.sm.operational.noRocket)) //rocket is there, but registration failed
				{
					//force a recheck
					Trigger((int)GameHashes.ChainedNetworkChanged);
					SgtLogger.l("Forced a recheck for rockets on rocket platform" + this.GetProperName());
				}

			}
		}
	}
}
