using UnityEngine;

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
			var kanim = Assets.GetAnim(anim);
			if (kanim != null)
			{
				Sprite = Def.GetUISpriteFromMultiObjectAnim(kanim);
			}
			Generic = genericGeyser;
		}
	}
}
