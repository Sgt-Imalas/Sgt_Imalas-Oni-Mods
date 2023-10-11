using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DupeStations
{
    public class PillDispenserWorkable : WorkableReactable
    {
        [MyCmpGet]
        Storage pillStorage;


        public PillDispenserWorkable(Workable workable, ChoreType chore_type, AllowedDirection allowed_direction = AllowedDirection.Any) : base(workable, "TakeMedicine", chore_type, allowed_direction)
        {

        }

        public bool CanBegin()
        {
            return pillStorage.FindFirstWithMass(BasicRadPillConfig.ID, 1);
        }

        public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
        {
            if (base.InternalCanBegin(new_reactor, transition))
            {//replace with current filterable
                if (CanBegin())
                {

                }
            }
            return false;
        }

        public class Work : Workable
        {

        }
    }
}
