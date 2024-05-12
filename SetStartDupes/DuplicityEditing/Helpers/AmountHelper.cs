using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes.DuplicityEditing.Helpers
{
    public static class AmountHelper
    {
        public static List<Amount> GetEditableAmounts()
        {

            var amounts = Db.Get().Amounts;
            var output = new List<Amount>
            {
                amounts.HitPoints,
                amounts.Stamina,
                amounts.Calories,
                amounts.Breath,
                amounts.Bladder,
                amounts.Stress,
                //amounts.Decor,
            };
            return output;
        }
    }
}
