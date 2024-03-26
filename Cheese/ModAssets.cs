using Cheese.Entities;
using Cheese.Foods;
using Cheese.ModElements;
using Cheese.Traits;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Cheese
{
    internal class ModAssets
    {
        public static void LoadAll()
        {
            SOUNDS.LoadAll();
        }
        public static void ClearCheeseTables()=>CheeseTableTargets.Clear();

        public static HashSet<CheeseTable> CheeseTableTargets = new HashSet<CheeseTable>();

        public static Color CheeseColor = UIUtils.rgb(240, 180, 0);

        public class Foods
        {
            public static Dictionary<Tag,Tag> CheeseConversions = new Dictionary<Tag, Tag>()
            {
                {
                    BurgerConfig.ID, CheeseBurgerConfig.ID
                }
            }
            ;


            public const float CHEESE_KCAL_PER_KG = 20f; //200KCal cheese per day per cow, cows make 50kg milk per day and the chosen conversion ratio is 5->1 
            public const float CHEESEBURGER_KCAL_PER_KG = 6400f; //200KCal cheese per day per cow, cows make 50kg milk per day and the chosen conversion ratio is 5->1 

            public static EdiblesManager.FoodInfo CheeseEdible = new(
                ModElements.ModElementRegistration.Cheese.ToString(),
                DlcManager.VANILLA_ID,
                CHEESE_KCAL_PER_KG * 1000f,
                2,
                255.15f,
                277.15f,
                4800f,
                false);

            public static EdiblesManager.FoodInfo CheeseBurger = 
                new EdiblesManager.FoodInfo(CheeseBurgerConfig.ID,
                    DlcManager.VANILLA_ID,
                    CHEESEBURGER_KCAL_PER_KG * 1000f,
                    6, 
                    255.15f,
                    277.15f, 2800f, 
                    can_rot: true)
                .AddEffects(new List<string> { "GoodEats" }, DlcManager.AVAILABLE_ALL_VERSIONS)
                .AddEffects(new List<string> { "SeafoodRadiationResistance" }, DlcManager.AVAILABLE_EXPANSION1_ONLY);


        }
        public static class SOUNDS
        {
            public const string
                CHEESE = "CHEESE_JAMESMAY"
                ;
            public static void LoadAll()
            {
               SoundUtils.LoadSound(SOUNDS.CHEESE, "JamesCheese.wav");
            }
        }


        public class Tags
        {
            /// <summary>
            /// Add to brackene products for the bractose intolerance trait
            /// </summary>
            public static Tag BrackeneProduct = TagManager.Create("CheeseMod_BrackeneProduct");
            public static Tag CheeseMaterial = TagManager.Create("CheeseMod_CheeseMaterial");

            
        }
    }
}
