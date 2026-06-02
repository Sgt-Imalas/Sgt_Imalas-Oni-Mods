using UnityEngine;
using static STRINGS.LORE.BUILDINGS;

namespace ClusterTraitGenerationManager.GeyserExperiments
{
	public class GeyserDataEntry
	{
		public string ID;
		public string Name;
		public string Description;
		public Sprite Sprite;
		public bool Generic;
		public GeyserDataEntry(string id, string name, string description, string anim, bool genericGeyser)
		{
			ID = id;
			Name = name;
			Description = description;
			Generic = genericGeyser;
			if (!Generic)
				Description += "\n\n" + STRINGS.UI.INFOTOOLTIPS.NONGENERICGEYSER;
			else
				Description += "\n\n" + STRINGS.UI.INFOTOOLTIPS.GENERICGEYSER;
			var kanim = Assets.GetAnim(anim);
			if (kanim != null)
			{
				Sprite = Def.GetUISpriteFromMultiObjectAnim(kanim);
			}
		}
	}
}
