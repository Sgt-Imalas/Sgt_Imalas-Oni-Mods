//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Cryopod.PassengersScenario
//{
//    public class baseAger
//    {
//        private bool instantiated = false;
//        private static readonly Lazy<baseAger> lazy =
//           new Lazy<baseAger>(() => new baseAger());

//        public static baseAger Instance { get { return lazy.Value; } }


//        public void AgeBaseP()
//        {
//            if(!instantiated)
//            {
//                Instance.AgeBase();
//                instantiated = true;
//            }
//        }
//        private void AgeBase()
//        {
//            Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, Camera.main.transform.GetPosition().z));
//            //camera pan to dark


//            //do Stuff
//            foreach(var building in Components.Breakables.Items)
//            {
//                building.GetComponent< BuildingHP>().DoDamage(int.MaxValue);
//            }

//            foreach (Battery building in Components.Batteries.Items)
//            {
//                building.ConsumeEnergy(float.PositiveInfinity,false);
//            }
//            //wake up one dupe

//            //camera turn back on.
//            Wait();

//            var gamestatemanager = GameFlowManager.Instance.smi;
//            gamestatemanager.GoTo(gamestatemanager.GetStateMachine().GetDefaultState());
//        }

//        private static IEnumerator Wait()
//        {
//            yield return new WaitForSecondsRealtime(15);
//        }
//    }    
//}
