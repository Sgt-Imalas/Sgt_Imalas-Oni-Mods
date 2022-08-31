using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStartDupes
{
    class DupeSettings : KScreen
    {
        MinionStartingStats Edit;
        public DupeSettings(ref MinionStartingStats referenceToEdit)
        {
            Edit = referenceToEdit;
        }
    }
}
