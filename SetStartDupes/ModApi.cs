using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SetStartDupes
{
    public class ModApi
    {
        public static Dictionary<string, System.Action<GameObject>> ActionsOnTraitRemoval = new Dictionary<string, Action<GameObject>>();

        /// <summary>
        /// Traits that add more than a component with the same name as the trait id leave these components on the duplicant when initialising a stat selectable cryopod dupe or hermit. 
        /// Please register an action here that takes the duplicants gameobject as param and removes any additional components the trait added to the duplicant
        /// </summary>
        /// <param name="TraitID">The id of the trait</param>
        /// <param name="action">the action that should be called on removing a trait from a duplicant. provides the duplicant gameobject as param</param>
        public static void AddTraitRemovalAction(string TraitID, Action<GameObject> action)
        {
            if (ActionsOnTraitRemoval.ContainsKey(TraitID))
            {
                ActionsOnTraitRemoval[TraitID] += action;
            }
            else
            {
                ActionsOnTraitRemoval[TraitID] = action;
            }
        }
    }
}
