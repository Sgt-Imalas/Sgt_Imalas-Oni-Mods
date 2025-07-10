using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces
{
    interface IHasConfigurableStorageCapacity
	{
		float GetStorageCapacity();
		void SetStorageCapacity(float mass);
	}
}
