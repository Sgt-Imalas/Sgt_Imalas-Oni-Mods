using ElementUtilNamespace;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets_TinyYetBig.Elements
{
	public class ModElements
	{
		public static ElementInfo
			UnobtaniumDust = ElementInfo.Solid("UnobtaniumDust", Color.black),
			SpaceStationForceField  = ElementInfo.Solid("UnobtaniumAlloy", Color.grey),
			UnobtaniumAlloy = ElementInfo.Solid("SpaceStationForceField", Color.blue);

		public static void RegisterSubstances(List<Substance> list)
		{
			var glass = list.Find(e => e.elementID == SimHashes.Diamond).material;
			var newElements = new HashSet<Substance>()
			{
				UnobtaniumDust.CreateSubstance(),
				SpaceStationForceField.CreateSubstance(true, glass),
				UnobtaniumAlloy.CreateSubstance(true, glass)
			};
			list.AddRange(newElements);
			//SgtLogger.debuglog("2," + list + ", " + list.Count);

		}
	}
}
