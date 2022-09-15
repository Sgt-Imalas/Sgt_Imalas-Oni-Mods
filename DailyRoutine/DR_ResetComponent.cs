using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DailyRoutine
{
    class DR_ResetComponent : KMonoBehaviour, ISim200ms
    {
        [SerializeField]
        public ComplexFabricator fabricator;

        [SerializeField]
        public Dictionary<ComplexRecipe, int> StoredRecipes = new();

        [SerializeField]
        public float timeToReset = 0f;
        [SerializeField]
        public bool IsActive = false;
        [SerializeField]
        int LastCycle = 0;
        [SerializeField]
        public bool UseCustomTime = false; 

        public void Sim200ms(float dt)
        {
            if (IsActive)
                ResetRecipesFromList();
        }
        void ResetRecipesFromList()
        {
            int currentTime = (int)Math.Round(GameClock.Instance.GetTimeSinceStartOfCycle());
            int CurrentCycle = GameClock.Instance.GetCycle();
            int SetTime = (int)Math.Round(timeToReset);

            //Debug.Log("Current Seconds: " + currentTime + ", Time at reset: " + SetTime + ", Current Cycle: "+ CurrentCycle+", Last Cycle: "+LastCycle);
            if ((int)currentTime == SetTime && CurrentCycle != LastCycle)
            {
                Debug.Log("CHAAAAAAAAAAAAAAAAAAAAAAAAAAAANGE");
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
                foreach(var recipeCount in StoredRecipes) 
                {
                    fabricator.SetRecipeQueueCount(recipeCount.Key, recipeCount.Value);
                }
            }
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Debug.Log("Cmp Initialized");
            Debug.Log("Was stored active? " + IsActive);
        }

        internal string GetFormattedRecipes()
        {
            StringBuilder sb = new();
            if (StoredRecipes.Count > 0)
            {
                foreach (var recipeCount in StoredRecipes)
                {
                    sb.Append(TagManager.GetProperName(recipeCount.Key.FirstResult.Name)) ;sb.Append(": x");sb.AppendLine(recipeCount.Value.ToString());
                }
            }
            else
            {
                sb.Append("No recipes queued.");
            }
            return sb.ToString();
        }
    }
}
