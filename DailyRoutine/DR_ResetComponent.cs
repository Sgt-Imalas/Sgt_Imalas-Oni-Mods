using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using static DailyRoutine.STRINGS.UISTRINGS;

namespace DailyRoutine
{
	class DR_ResetComponent : KMonoBehaviour, ISim200ms
	{
		[Serialize]
		public ComplexFabricator fabricator;

		[Serialize]
		public Dictionary<ComplexRecipe, int> StoredRecipes = new();

		[Serialize]
		public float timeToReset = 0f;
		[Serialize]
		public bool IsActive = false;
		[Serialize]
		int LastCycle = 0;
		[Serialize]
		public bool UseCustomTime = false;
		[Serialize]
		public bool QueueRecipes = false;

		public void Sim200ms(float dt)
		{
			if (IsActive)
				ResetRecipesFromList();
		}
		void ResetRecipesFromList()
		{
			int currentTime = (int)Math.Round(GameClock.Instance.GetTimeSinceStartOfCycle());
			int CurrentCycle = GameClock.Instance.GetCycle();
			int SetTime = this.UseCustomTime ? (int)Math.Round(timeToReset) : 0;

			//Debug.Log("Current Seconds: " + currentTime + ", Time at reset: " + SetTime + ", Current Cycle: "+ CurrentCycle+", Last Cycle: "+LastCycle);
			if ((int)currentTime == SetTime && CurrentCycle != LastCycle)
			{
				OverrideRecipeCount();
				LastCycle = CurrentCycle;
			}
		}

		public void ChangeStoredRecipes()
		{
			if (IsActive)
			{
				StoredRecipes.Clear();

				var allRecipes = fabricator.GetRecipes();
				//var values = (Dictionary<string, int>)reflection.GetValue(fabricator);
				foreach (var recipe in allRecipes)
				{
					int Count = fabricator.GetRecipeQueueCount(recipe);
					if (Count > 0)
					{
						StoredRecipes.Add(recipe, Count);
					}
				}
			}
			else
			{
				StoredRecipes.Clear();
			}
		}

		void OverrideRecipeCount()
		{
			if (StoredRecipes.Count > 0)
			{
				foreach (var recipeCount in StoredRecipes)
				{
					if (QueueRecipes)
					{
						for (int i = recipeCount.Value; i > 0; i--)
							fabricator.IncrementRecipeQueueCount(recipeCount.Key);
					}
					else
					{
						fabricator.SetRecipeQueueCount(recipeCount.Key, recipeCount.Value);
					}
				}
			}
		}

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			// Debug.Log("Cmp Initialized");
			// Debug.Log("Was stored active? " + IsActive);
		}




		internal string GetFormattedCount(bool isTooltip = false)
		{
			StringBuilder sb = new();
			if (StoredRecipes.Count > 0)
			{
				int amount = 0, types = 0;
				foreach (var recipeCount in StoredRecipes)
				{
					if (recipeCount.Value > 0)
					{
						amount += recipeCount.Value;
						++types;
					}
				}
				if (isTooltip)
					sb.Append(string.Format(FORMATTEDCOUNTEXT.TOOLTIP, amount, types));
				else
					sb.Append(string.Format(FORMATTEDCOUNTEXT.LABEL, amount));
			}
			else
			{
				sb.Append(FORMATTEDCOUNTEXT.NONE);
			}
			return sb.ToString();
		}

		internal string GetFormattedRecipes()
		{
			StringBuilder sb = new();
			if (StoredRecipes.Count > 0)
			{
				sb.AppendLine(FORMATTEDRECIPETEXT.LABEL);
				foreach (var recipeCount in StoredRecipes)
				{
					sb.Append(TagManager.GetProperName(recipeCount.Key.FirstResult.Name)); sb.Append(" x"); sb.AppendLine(recipeCount.Value.ToString());
				}
			}
			else
			{
				sb.Append(FORMATTEDRECIPETEXT.NONE);
			}
			return sb.ToString();
		}
	}
}
