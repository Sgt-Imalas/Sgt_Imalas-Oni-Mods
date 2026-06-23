using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Db
	{
		public static void Init(Db db)
		{
			Aq_Effects.Register(db);
			Aq_Amounts.RegisterAmounts(db.Amounts);
			Aq_Accessories.Register(db.Accessories, db.AccessorySlots);
			Aq_Personalities.RegisterPersonalities(db.Personalities);
			Aq_StatusItems.InitStatusitems(db);
			AQ_TUNING.RegisterType();
			Aq_SkillGroups.Register(db.SkillGroups);
			Aq_SkillPerks.Register(db.SkillPerks);
			Aq_Skills.Register(db.Skills);
		}
	}
}
