using System;
using System.Collections.Generic;
using System.Text;

namespace AquaticMinnowMinion.Content.ModDb
{
	internal class Aq_Db
	{
		public static void Init(Db db)
		{
			Aq_Accessories.Register(db.Accessories, db.AccessorySlots);
			Aq_Personalities.RegisterPersonalities(db.Personalities);
			Aq_StatusItems.InitStatusitems(db);
			AQ_TUNING.RegisterType();
			Aq_SkillPerks.Register(db.SkillPerks);
			Aq_Skills.Register(db.Skills);
			Aq_Effects.Register(db);
		}
	}
}
