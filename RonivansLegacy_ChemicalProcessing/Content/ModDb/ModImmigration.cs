using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb
{
	internal class ModImmigration
	{
		public static bool CycleCondition(int cycle)
		{
			return GameClock.Instance.GetCycle() >= cycle;
		}

		public static bool DiscoveredCondition(Tag tag)
		{
			return DiscoveredResources.Instance.IsDiscovered(tag);
		}

		public static void AddModdedCarePackages(Immigration i)
		{
			void AddPackage(string id, float amount, Func<bool> condition)
			{
				i.carePackages.Add(new CarePackageInfo(id, amount, condition));
			}

			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				AddPackage(ModElements.VegetableOil_Liquid.Tag.ToString(),100,() => DiscoveredCondition(ModElements.VegetableOil_Liquid.CreateTag()) && CycleCondition(32));
				AddPackage(ModElements.BioPlastic_Solid.Tag.ToString(), 500, () => DiscoveredCondition(ModElements.BioPlastic_Solid.CreateTag()) && CycleCondition(48));
			}
			if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
			{
				AddPackage(ModElements.Aurichalcite_Solid.Tag.ToString(), 2000, () => DiscoveredCondition(ModElements.Aurichalcite_Solid.CreateTag()) && CycleCondition(12));
				AddPackage(ModElements.Argentite_Solid.Tag.ToString(), 2000, () => DiscoveredCondition(ModElements.Argentite_Solid.CreateTag()) && CycleCondition(12));
				AddPackage(SimHashes.Electrum.CreateTag().ToString(), 2000, () => DiscoveredCondition(SimHashes.Electrum.CreateTag()) && CycleCondition(24));
				AddPackage(ModElements.Galena_Solid.Tag.ToString(), 2000, () => DiscoveredCondition(ModElements.Galena_Solid.CreateTag()) && CycleCondition(24));

				AddPackage(ModElements.MeteorOre_Solid.Tag.ToString(), 400, () => DiscoveredCondition(ModElements.MeteorOre_Solid.CreateTag()) && CycleCondition(48));

				AddPackage(ModElements.Zinc_Solid.Tag.ToString(), 400, () => DiscoveredCondition(ModElements.Zinc_Solid.CreateTag()) && CycleCondition(48));
				AddPackage(ModElements.Silver_Solid.Tag.ToString(), 400, () => DiscoveredCondition(ModElements.Silver_Solid.CreateTag()) && CycleCondition(48));

				AddPackage(ModElements.Brass_Solid.Tag.ToString(), 100, () => DiscoveredCondition(ModElements.Brass_Solid.CreateTag()) && CycleCondition(48));
				AddPackage(ModElements.PhosphorBronze.Tag.ToString(), 100, () => DiscoveredCondition(ModElements.PhosphorBronze.CreateTag()) && CycleCondition(48));

				AddPackage(ModElements.Borax_Solid.Tag.ToString(), 150, () => DiscoveredCondition(ModElements.Borax_Solid.CreateTag()) && CycleCondition(48));

				AddPackage(ModElements.SulphuricAcid_Liquid.Tag.ToString(), 1000, () => DiscoveredCondition(ModElements.SulphuricAcid_Liquid.CreateTag()) && CycleCondition(96));
				AddPackage(ModElements.NitricAcid_Liquid.Tag.ToString(), 1000, () => DiscoveredCondition(ModElements.NitricAcid_Liquid.CreateTag()) && CycleCondition(96));

				AddPackage(ModElements.AmmoniumWater_Liquid.Tag.ToString(), 2000, () => DiscoveredCondition(ModElements.AmmoniumWater_Liquid.CreateTag()) && CycleCondition(24));
				AddPackage(ModElements.AmmoniumSalt_Solid.Tag.ToString(), 1000, () => DiscoveredCondition(ModElements.AmmoniumWater_Liquid.CreateTag()) && CycleCondition(24));
			}
		}
	}
}
