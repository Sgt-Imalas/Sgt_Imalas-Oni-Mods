using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace CrittersShedFurOnBrush
{
	public class ModAssets
	{
		/// <summary>
		/// Creature + Shed amount / cycle
		/// </summary>
		public static Dictionary<Tag, FloofCarrier> SheddableCritters = new Dictionary<Tag, FloofCarrier>();
		public static void InitSheddables()
		{
			if (Config.Instance.Drecko)
				AddFluffyCritter((Tag)DreckoConfig.ID, 1f / 7f, UIUtils.rgb(220, 217, 204));

			if (Config.Instance.CuddlePip)
				AddFluffyCritter((Tag)SquirrelHugConfig.ID, 1f / 5f, UIUtils.rgb(255, 192, 174));

			if (Config.Instance.SageHatch)
				AddFluffyCritter((Tag)(Tag)HatchVeggieConfig.ID, 1f / 10f, UIUtils.rgb(76, 129, 103));

			if (Config.Instance.OilFloaterFur)
				AddFluffyCritter((Tag)OilFloaterDecorConfig.ID, 1f / 6f, UIUtils.rgb(86, 102, 208));

			if (Config.Instance.Pufts)
			{
				AddFluffyCritter((Tag)PuftConfig.ID, 1f / 10f, UIUtils.rgb(255, 203, 100));
				AddFluffyCritter((Tag)PuftAlphaConfig.ID, 1f / 10f, UIUtils.rgb(225, 225, 109));
				AddFluffyCritter((Tag)PuftBleachstoneConfig.ID, 1f / 5f, UIUtils.rgb(105, 211, 78));
			}

			if (Config.Instance.PlugSlug)
				AddFluffyCritter((Tag)StaterpillarConfig.ID, 1f / 6f, UIUtils.rgb(31, 113, 121));


			if (Config.Instance.Flox)
			{
				AddFluffyCritter((Tag)WoodDeerConfig.ID, 1f / 7f, UIUtils.rgb(162, 223, 205));
				AddFluffyCritter("GlassDeer", 1f / 7f, UIUtils.rgb(169, 115, 224));
			}
			if (Config.Instance.Bammoth)
			{
				AddFluffyCritter("IceBelly", 1f / 6f, Color.white); //bammoth
				AddFluffyCritter("GoldBelly", 1f / 5f, UIUtils.rgb(81, 54, 129)); //regal bammoth
			}
			if (Config.Instance.HuskyMoo)
			{
				AddFluffyCritter("DieselMoo", 1f / 7f, UIUtils.rgb(36, 36, 38)); //husky moo
				
			}
		}
		public static void AddFluffyCritter(Tag critterId, float floofPerCycle) => AddFluffyCritter(critterId, floofPerCycle, Color.white);
		public static void AddFluffyCritter(Tag critterId, float floofPerCycle, Color floofColour)
		{
			var FloofInfo = new FloofCarrier(critterId, floofPerCycle, floofColour);
			SheddableCritters[critterId] = FloofInfo;

		}
		public struct FloofCarrier
		{
			public Tag ID;
			public float FloofPerCycle;
			public Color FloofColour;
			public FloofCarrier(Tag _id, float _floofPerCycle, Color _floofColor)
			{
				ID = _id;
				FloofPerCycle = _floofPerCycle;
				FloofColour = _floofColor;
			}
		}
	}
}
