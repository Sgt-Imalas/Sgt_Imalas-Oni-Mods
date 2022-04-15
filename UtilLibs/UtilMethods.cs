using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public static class UtilMethods
    {
        public static bool IsCellInSpaceAndVacuum(int _cell)
        {
            return (Grid.IsCellOpenToSpace(_cell) && Grid.Mass[_cell] == 0);
        }
    }
}
