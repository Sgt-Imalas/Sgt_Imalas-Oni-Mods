using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace TrainMod.Content.Scripts.PathSystem
{
	public class TrackStation : TrackPiece, ISidescreenButtonControl
	{
		public string SidescreenButtonText => "Find Path!";

		public string SidescreenButtonTooltip => "throw new NotImplementedException()";

		public int ButtonSideScreenSortOrder() => 0;
		public int HorizontalGroupID() => -1;

		public void OnSidescreenButtonPressed()
		{
			foreach (var piece in TrackManager.TrackPieces)
				piece.Tint(null);

			var target = TrackManager.TrackStations.FirstOrDefault(station => station != this);
			if (target != null)
			{
				var trackpieces = TrackManager.Pathfind(this, target, true);
				var trackpieces2 = TrackManager.Pathfind(this, target, false);

				if (trackpieces != null)
				{
					StartCoroutine(TintBuildings(trackpieces));
				}
				else if (trackpieces2 != null)
				{

					StartCoroutine(TintBuildings(trackpieces2));
				}
				else
					SgtLogger.error("Pathfinder Failed!!!!");
			}
		}

		IEnumerator TintBuildings(List<TrackPiece> remaining)
		{
			foreach (var piece in remaining)
			{
				yield return (object)new WaitForSecondsRealtime(1);
				piece.Tint(Color.green);
			}
		}


		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
		}

		public bool SidescreenButtonInteractable() => TrackManager.TrackStations.Count > 1;

		public bool SidescreenEnabled() => true;
	}
}
