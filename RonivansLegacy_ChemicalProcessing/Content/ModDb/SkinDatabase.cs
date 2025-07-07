using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InventoryOrganization;
using static UtilLibs.SupplyClosetUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	class SkinDatabase
	{
		internal static void AddSkins()
		{
			SkinCollection.Create("FacilityDoor")
				.AddToExisting(PermitSubcategories.BUILDINGS_STORAGE)
				.Skin("FacilityDoorYellow","Yellow Facility Door", "A light-weight door with intricate designs that suggests it bellongs to a industrial facility.", "facility_door_yellow_kanim")
				;


			SkinCollection.RegisterAllSkins();
		}
	}
}
