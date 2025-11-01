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

			var target = TrackManager.TrackStations.FirstOrDefault(station => station != this && TrackManager.Pathfind(this, station, true) != null);
			if (target != null)
			{

				var trackpieces = TrackManager.Pathfind(this, target, true);

				if (trackpieces != null)
				{
					StartCoroutine(TintBuildings(trackpieces));
				}
				else
				{
					SgtLogger.error("Pathfinder Failed!!!!");
					Tint(Color.red);
				}
			}
			else
			{
				SgtLogger.error("Pathfinder Failed!!!!");
				Tint(Color.red);
			}
		}

		IEnumerator TintBuildings(List<TrackPiece> remaining)
		{
			foreach (var piece in remaining)
			{
				yield return new WaitForSecondsRealtime(0.1f);
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
