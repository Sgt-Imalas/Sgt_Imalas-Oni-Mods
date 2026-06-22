using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using UtilLibs;
using static Beached_ModAPI.Beached_API;

namespace SetStartDupes.API_IO
{
	internal class FoodOverhaul_API
	{
		static bool initialized = false;

		public static GetFavouriteFoodDelegate GetFavouriteFood;

		public delegate string GetFavouriteFoodDelegate(MinionStartingStats minionStartingStats);

		public static SetFavouriteFoodDelegate SetFavouriteFood;

		public delegate void SetFavouriteFoodDelegate(MinionStartingStats minionStartingStats, string foodId);

		static List<TUNING.DUPLICANTSTATS.TraitVal> FF_TraitValsRaw;

		public static List<TUNING.DUPLICANTSTATS.TraitVal> GetRawTraitVals() => FF_TraitValsRaw;

		public static Trait GetFavouriteFoodTrait(MinionStartingStats minionStartingStats)
		{
			if (!initialized)
				return null;
			var traitId = "FF_"+GetFavouriteFood(minionStartingStats);
			SgtLogger.l(minionStartingStats.Name + " has fav food trait: " + traitId);
			var trait = Db.Get().traits.TryGet(traitId);
			return trait;
		}
		public static void SetFavouriteFoodTrait(MinionStartingStats minionStartingStats, Trait trait)
		{
			string foodId = trait.Id.Replace("FF_", string.Empty);
			if (!initialized)
				return;
			SetFavouriteFood(minionStartingStats, foodId);
		}


		private static bool TryInitialize(bool logWarnings = true)
		{
			var type = Type.GetType("FoodOverhaul.FavoriteFoodConfig, FoodOverhaul");

			if (type == null)
			{
				if (logWarnings)
					SgtLogger.warning("FoodOverhaul does not exist.");
				return false;
			}

			var m_favoriteTraitValList = AccessTools.Field(type, "favoriteTrait");

			if (m_favoriteTraitValList == null)
			{
				if (logWarnings)
					SgtLogger.warning("favoriteTrait is not a an existing field.");
				return false;
			}
			try
			{
				FF_TraitValsRaw = m_favoriteTraitValList.GetValue(null) as List<TUNING.DUPLICANTSTATS.TraitVal>;
			}
			catch (Exception ex)
			{
				SgtLogger.warning("ERROR while trying to get favourite foods traitval list:\n" + ex.Message);
				//return false;
			}

			type = AccessTools.TypeByName("FavoriteFoodTraitPatch");
			var m_GetCurrentFavFood = AccessTools.Method(type, "Get", [typeof(MinionStartingStats)]);

			if (m_GetCurrentFavFood == null)
			{
				if (logWarnings) Debug.LogWarning("m_GetCurrentFavFood is not a method.");
				return false;
			}
			GetFavouriteFood = (GetFavouriteFoodDelegate)Delegate.CreateDelegate(typeof(GetFavouriteFoodDelegate), m_GetCurrentFavFood);
			var m_SetCurrentFavFood = AccessTools.Method(type, "Set", [typeof(MinionStartingStats), typeof(string)]);

			if (m_SetCurrentFavFood == null)
			{
				if (logWarnings) Debug.LogWarning("m_SetCurrentFavFood is not a method.");
				return false;
			}
			SetFavouriteFood = (SetFavouriteFoodDelegate)Delegate.CreateDelegate(typeof(SetFavouriteFoodDelegate), m_SetCurrentFavFood);

			initialized = true;
			return true;
		}
		public static void InitFoodOverhaulAPI()
		{
			if (FoodOverhaul_API.TryInitialize())
			{
				ModAssets.InitFoodOverhaul();
			}
			else
				SgtLogger.l("Food Overhaul mod not found, API is resting now, gn...zzz");
		}
	}
}
