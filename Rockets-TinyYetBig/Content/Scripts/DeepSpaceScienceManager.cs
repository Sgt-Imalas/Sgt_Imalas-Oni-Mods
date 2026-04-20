using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Science
{
	public class DeepSpaceScienceManager : KMonoBehaviour, ISaveLoadable
	{
		public static DeepSpaceScienceManager Instance;


		[Serialize]
		float CurrentScienceValue = 0;

		void ApplySciencePoints()
		{
			for (float i = CurrentScienceValue; i >= 0; i--)
			{
				//++deepscienceresearch
				if (GetCurrentDeepSpaceReserach(out var target))
				{
					var techInstance = Research.Instance.Get(target);
					if (techInstance != null)
					{
						int pointsToAdd = (int)Math.Min((techInstance.tech.costsByResearchTypeID[ModAssets.DeepSpaceScienceID] - techInstance.progressInventory.PointsByTypeID[ModAssets.DeepSpaceScienceID]), i);
						techInstance.progressInventory.AddResearchPoints(ModAssets.DeepSpaceScienceID, pointsToAdd);
						if (Mathf.Approximately(1, techInstance.GetTotalPercentageComplete()))
						{
							if (!techInstance.IsComplete())
							{
								techInstance.Purchased();
								Game.Instance.Trigger((int)GameHashes.ResearchComplete, techInstance.tech);
							}
						}
						CurrentScienceValue -= pointsToAdd;
					}
				}
			}
		}
		List<string> DeepSpaceTechs = new List<string>()
		{
			ModAssets.Techs.SpaceScienceTechID,
			ModAssets.Techs.SpaceStationTechID,
			ModAssets.Techs.SpaceStationTechMediumID,
			ModAssets.Techs.SpaceStationTechLargeID
		};

		bool GetCurrentDeepSpaceReserach(out Tech Target)
		{
			Target = null;
			var Techs = Db.Get().Techs;

			List<Tech> potentialTechs = new List<Tech>();
			foreach (var dst in DeepSpaceTechs)
			{
				var tech = Techs.TryGet(dst);
				if (tech != null)
					potentialTechs.Add(tech);
			}
			potentialTechs.Sort( (a, b) => a.costsByResearchTypeID[ModAssets.DeepSpaceScienceID].CompareTo(b.costsByResearchTypeID[ModAssets.DeepSpaceScienceID]));
			potentialTechs.RemoveAll(tech => tech.IsComplete() || !tech.RequiresResearchType(ModAssets.DeepSpaceScienceID) || Research.Instance.Get(tech).PercentageCompleteResearchType(ModAssets.DeepSpaceScienceID) >= 1f);

			if(!potentialTechs.Any())
				return false;
			
			Target = potentialTechs.First();
			return true;
		}

		public void ArtifactResearched(bool terrestial)
		{
			if (terrestial)
				CurrentScienceValue += 2;
			else
				CurrentScienceValue += 1;
			ApplySciencePoints();
		}

		public void ScienceResearched(string researchType)
		{
			if (DeepSpaceScienceUnlocked)
			{
				switch (researchType)
				{
					case ResearchTypes.ID.BASIC:
						CurrentScienceValue += 0.1f;
						break;
					case ResearchTypes.ID.ADVANCED:
						CurrentScienceValue += 0.2f;
						break;
					case ResearchTypes.ID.NUCLEAR:
						CurrentScienceValue += 0.4f;
						break;
					case ResearchTypes.ID.ORBITAL:
						CurrentScienceValue += 0.4f;
						break;
				}
				ApplySciencePoints();
			}
		}
		public bool DeepSpaceScienceUnlocked => ModAssets.Techs.SpaceStationTech.IsComplete();
	}
}
