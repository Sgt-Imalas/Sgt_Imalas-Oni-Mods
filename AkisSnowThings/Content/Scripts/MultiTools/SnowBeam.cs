using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace AkisSnowThings.Content.Scripts.MultiTools
{
	public class SnowBeam
	{
		public const string CONTEXT = "SnowSculptures_SnowmanBuilding";
		public const string LASER_EFFECT = "SnowSculptures_SnowManEffect";

		public static void AddSnapOn(GameObject gameObject)
		{
			gameObject.AddOrGet<SnapOn>().snapPoints.Add(new SnapOn.SnapPoint
			{
				pointName = "dig",
				automatic = false,
				context = CONTEXT,
				buildFile = Assets.GetAnim("water_gun_kanim"),
				overrideSymbol = "snapTo_rgtHand"
			});
		}

		internal static void AddLaserEffect(GameObject minionPrefab)
		{
			var laserEffects = minionPrefab.transform.Find("LaserEffect").gameObject;
			var kbatchedAnimEventToggler = laserEffects.GetComponent<KBatchedAnimEventToggler>();
			var kbac = minionPrefab.GetComponent<KBatchedAnimController>();
			InjectionMethods.AddLaserEffect(LASER_EFFECT, CONTEXT, kbatchedAnimEventToggler, kbac, "sm_snowbeam_kanim", "loop");
		}
	}
}
