using BionicBoostersPlus.Content.Defs.Buildings;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;

namespace BionicBoostersPlus.Content.ModDb
{
	public class BB_Db
	{
		public static void Init(Db db)
		{
			BB_Buildings.Register();

			//BB_Urges.Register(db.Urges);
			//BB_Amounts.RegisterAmounts(db.Amounts);
			BB_Effects.Register(db);
			//BB_Accessories.Register(db.Accessories, db.AccessorySlots);
			//BB_Personalities.RegisterPersonalities(db.Personalities);
			BB_StatusItems.InitStatusitems(db);
			//BB_TUNING.RegisterType();
			//BB_SkillGroups.Register(db.SkillGroups);
			BB_SkillPerks.Register(db.SkillPerks);
			BB_Skills.Register(db.Skills);
			//BB_ChoreTypes.Register(db.ChoreTypes);
			//BB_Traits.Register(db);
		}
	}
}
