namespace Planticants.Content.ModDb
{
    class PlantDb
    {
        public static void Init(Db db)
        {
            PlantAccessories.Register(db.Accessories, db.AccessorySlots);
			PlantPersonalities.RegisterPersonalities(db.Personalities);

            PLANT_TUNING.RegisterType();
		}
    }
}
