using STRINGS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.CODEX;
using static STRINGS.UI;

namespace RonivansLegacy_ChemicalProcessing
{
	class STRINGS
	{
		public class ELEMENTS
		{
			//===== [ Zinc ] ================================
			public class SOLIDZINC
			{
				public static LocString NAME = FormatAsLink("Zinc", nameof(SOLIDZINC));
				public static LocString DESC = "(Zn) Zinc is a bluish-white, lustrous, diamagnetic metal, slightly brittle metal.";
			}
			public class MOLTENZINC
			{
				public static LocString NAME = FormatAsLink("Molten Zinc", nameof(MOLTENZINC));
				public static LocString DESC = "(Zn) Zinc is a bluish-white, lustrous, diamagnetic metal in its molten state.";
			}
			public class ZINCGAS
			{
				public static LocString NAME = FormatAsLink("Zinc Gas", nameof(ZINCGAS));
				public static LocString DESC = "(Zn) Zinc is a bluish-white, lustrous, diamagnetic metal in its gaseous state.";
			}
			//===== [ Silver ] ==============================
			public class SOLIDSILVER
			{
				public static LocString NAME = FormatAsLink("Silver", nameof(SOLIDSILVER));
				public static LocString DESC = "(Ag) Silver is soft, white, lustrous transition metal, that exhibits a high electrical and thermal conductivity.";
			}
			public class MOLTENSILVER
			{
				public static LocString NAME = FormatAsLink("Molten Silver", nameof(MOLTENSILVER));
				public static LocString DESC = "(Ag) Silver is soft, white, lustrous transition metal, in its molten state.";
			}
			public class SILVERGAS
			{
				public static LocString NAME = FormatAsLink("Silver Gas", nameof(SILVERGAS));
				public static LocString DESC = "(Ag) Silver is soft, white, lustrous transition metal, in its gaseous state.";
			}
			//===== [ Ammonia ] =============================
			public class SOLIDAMMONIA
			{
				public static LocString NAME = FormatAsLink("Ammonia Snow", nameof(SOLIDAMMONIA));
				public static LocString DESC = "(NH<sub>3</sub>) Ammonia is an inorganic compound of nitrogen and hydrogen, currently its its cold, solid state.";
			}
			public class LIQUIDAMMONIA
			{
				public static LocString NAME = FormatAsLink("Liquid Ammonia", nameof(LIQUIDAMMONIA));
				public static LocString DESC = "(NH<sub>3</sub>) Ammonia is an inorganic compound of nitrogen and hydrogen, currently in its cold, liquid state.";
			}
			public class AMMONIAGAS
			{
				public static LocString NAME = FormatAsLink("Ammonia", nameof(AMMONIAGAS));
				public static LocString DESC = "(NH<sub>3</sub>) Ammonia is an inorganic compound of nitrogen and hydrogen. A stable binary hydride, and the simplest pnictogen hydride, ammonia is a gas with a distinct pungent smell.";
			}
			//===== [ Toxic Waste ] ==========================
			public class TOXICCLAY
			{
				public static LocString NAME = FormatAsLink("Toxic Clay", nameof(TOXICCLAY));
				public static LocString DESC = "A sick looking, brittle clay produced as waste product from industrial operations, composed of many different chemical substances.";
			}
			public class TOXICSLURRY
			{
				public static LocString NAME = FormatAsLink("Toxic Slurry", nameof(TOXICSLURRY));
				public static LocString DESC = "A thick, toxic slurry produced as waste product from industrial operations, composed of many different chemical substances.";
			}
			public class TOXICGAS
			{
				public static LocString NAME = FormatAsLink("Toxic Gas", nameof(TOXICGAS));
				public static LocString DESC = "A heavy, foul smelling gas produced as waste product from industrial operations, composed of many different chemical substances.";
			}
			//===== [ Alloy ] ================================
			public class SOLIDBRASS
			{
				public static LocString NAME = FormatAsLink("Brass", nameof(SOLIDBRASS));
				public static LocString DESC = "Brass is an alloy of " + FormatAsLink("Copper", "COPPER") + " and " + FormatAsLink("Zinc", "SOLIDZINC") + ", widely used to make utensils due to properties such as having a low melting point, high workability, durability, and electrical and thermal conductivity.";
			}
			public class PHOSPHORBRONZE
			{
				public static LocString NAME = FormatAsLink("Phosphor Bronze", nameof(PHOSPHORBRONZE));
				public static LocString DESC = "An alloy composed of " + FormatAsLink("Copper", "COPPER") + ", " + FormatAsLink("Lead", "LEAD") + " and " + FormatAsLink("Phosphorus", "PHOSPHORUS") + ". Among copper based alloys it is remarkable tough, and has a relative low thermal conductivity.";
			}
			public class PLASTEEL
			{
				public static LocString NAME = FormatAsLink("Plasteel", nameof(PLASTEEL));
				public static LocString DESC = "A composite of " + FormatAsLink("Plastic", "POLYPROPYLENE") + " stabilized fibers grown into a " + FormatAsLink("Steel", "Steel") + " crystal structure. It is much more stronger and lighter than standard metals alloys, and has a very low thermal conductivity.";
			}
			//===== [ Special ] ==============================

			public class ISOPROPANEGAS
			{
				public static LocString NAME = FormatAsLink("Isopropane", nameof(ISOPROPANEGAS));
				public static LocString DESC = "(HC(CH<sub>3</sub>)<sub>3</sub>) Isopropane is a petrochemical refrigerant gas suitable for a variety of purposes. Degrades to " + FormatAsLink("Propane", "PROPANE") + " at higher temperatures.";
			}
			public class SOLIDSLAG
			{
				public static LocString NAME = FormatAsLink("Slag", nameof(SOLIDSLAG));
				public static LocString DESC = "Slag is a by-product of smelting (pyrometallurgical) ores and used metals. Despite being a waste product, it has many applications, such as aggregate in " + FormatAsLink("Concrete Blocks", "CONCRETEBLOCK") + ".";
			}
			public class MOLTENSLAG
			{
				public static LocString NAME = FormatAsLink("Molten Slag", nameof(MOLTENSLAG));
				public static LocString DESC = "Molten Slag is a by-product of smelting (pyrometallurgical) ores and used metals. Present in molten state, this waste materail needs to be cooled down to solid " + FormatAsLink("Slag", "SOLIDSLAG") + " before can be used.";
			}
			public class SOLIDBORAX
			{
				public static LocString NAME = FormatAsLink("Borax", nameof(SOLIDBORAX));
				public static LocString DESC = "Borax, also known as sodium borate, is an important boron compound, mainly used in the manufacture of " + FormatAsLink("Fiberglass", "SOLIDFIBERGLASS") + ", and as a flux in metallurgy.";
			}
			public class SOLIDOILSHALE
			{
				public static LocString NAME = FormatAsLink("Oil Shale", nameof(SOLIDOILSHALE));
				public static LocString DESC = "Oil shale is an organic-rich fine-grained sedimentary rock containing heavy crude oil, sulfur compounds and heavy metals.";
			}
			public class SOLIDFIBERGLASS
			{
				public static LocString NAME = FormatAsLink("Fiberglass", nameof(SOLIDFIBERGLASS));
				public static LocString DESC = "Fiberglass is a thermoset polymer matrix made by a poltrusion of boronsilicate " + FormatAsLink("Glass", "GLASS") + " and " + FormatAsLink("Plastic", "POLYPROPYLENE") + ". Although the fiber is weak in compression, this composite material has moderate insulating properties, and can be easily used in many different applications due to its relative flexibility.";
			}
			public class AMMONIUMWATER
			{
				public static LocString NAME = FormatAsLink("Ammonium Water", nameof(AMMONIUMWATER));
				public static LocString DESC = "(NH<sub>4</sub>OH) Ammonium hydroxide is a solution of " + FormatAsLink("Ammonia", "AMMONIAGAS") + " and " + FormatAsLink("Salt Water", "SALTWATER") + ".";
			}
			public class AMMONIUMSALT
			{
				public static LocString NAME = FormatAsLink("Nitrate Nodules", nameof(AMMONIUMSALT));
				public static LocString DESC = "(NH<sub>4</sub>NO<sub>3</sub>) Nodules of dirt containing high concentrations of Ammonium Nitrate.";
			}
			public class RAWNATURALGAS
			{
				public static LocString NAME = FormatAsLink("Raw Natural Gas", nameof(RAWNATURALGAS));
				public static LocString DESC = "A fossil gas consisting of gaseous hydrocarbons and other impurites. The majority of the gas mass is composed of " + FormatAsLink("Methane", "METHANE") + ", " + FormatAsLink("Propane", "PROPANE") + " and " + FormatAsLink("Sour Gas", "SOURGAS") + ".";
			}
			public class CONCRETEBLOCK
			{
				public static LocString NAME = FormatAsLink("Concrete Block", nameof(CONCRETEBLOCK));
				public static LocString DESC = "Concrete blocks are standard-size rectangular blocks used in building construction. A versatile component made from different aggregates that are often considered waste products.";
			}
			public class CARBONFIBER
			{
				public static LocString NAME = FormatAsLink("Carbon Composite", nameof(CARBONFIBER));
				public static LocString DESC = "Carbon fiber-reinforced polymers blocks are extremely strong and light fiber-reinforced plastics that contain carbon fibers. Used on wherever high strength-to-weight ratio and stiffness (rigidity) are required, such as aerospace. Its composition of allotropes of carbon make it extremely resistant to heat.";
			}
			public class SOURWATER
			{
				public static LocString NAME = FormatAsLink("Sour Water", nameof(SOURWATER));
				public static LocString DESC = "An aqueous solution of Hydrogen Sulfide (H<sub>2</sub>S>) and Ammonia (NH<sub>3</sub>). May occur naturally from aquifers exposed to hydrogen sulfide sources, but it is more common as a wastewater from industrial processes.";
			}
			//===== [ Metallic Sands ] =======================
			public class LOWGRADESAND
			{
				public static LocString NAME = FormatAsLink("Low-Grade Metallic Sand", nameof(LOWGRADESAND));
				public static LocString DESC = "A sandy material composed mostly of low quality metallic grains, mixed with other finer mineral particles.";
			}
			public class BASEGRADESAND
			{
				public static LocString NAME = FormatAsLink("Base-Grade Metallic Sand", nameof(BASEGRADESAND));
				public static LocString DESC = "A heavy sandy material composed mostly of common metallic grains, mixed with other finer mineral particles.";
			}
			public class HIGHGRADESAND
			{
				public static LocString NAME = FormatAsLink("High-Grade Metallic Sand", nameof(HIGHGRADESAND));
				public static LocString DESC = "A glimmering sandy material composed mostly of high quality metallic grains, mixed with other finer mineral particles.";
			}
			//===== [ Acids ] ================================
			public class LIQUIDSULFURIC
			{
				public static LocString NAME = FormatAsLink("Sulfuric Acid", nameof(LIQUIDSULFURIC));
				public static LocString DESC = "(H<sub>2</sub>SO<sub>4</sub>) A mineral acid composed of the elements sulfur, oxygen and hydrogen. Presented in its liquid state, it is a very dangerous chemical for its corrosive nature.";
			}
			public class SULFURICGAS
			{
				public static LocString NAME = FormatAsLink("Sulfuric Gas", nameof(SULFURICGAS));
				public static LocString DESC = "(H<sub>2</sub>SO<sub>4</sub>) An acidic gas composed of the elements sulfur, oxygen and hydrogen. Presented in its gaseous state, it is a very dangerous chemical for its corrosive nature.";
			}
			public class LIQUIDNITRIC
			{
				public static LocString NAME = FormatAsLink("Nitric Acid", nameof(LIQUIDNITRIC));
				public static LocString DESC = "(HNO<sub>3</sub>) An inorganic mineral acid composed of the elements nitrogen, oxygen and hydrogen. Presented in its liquid state, is the primary reagent used for nitration – the addition of a nitro group, typically to an organic molecule.";
			}
			//===== [ Raw Minerals ] =========================
			public class ARGENTITEORE
			{
				public static LocString NAME = FormatAsLink("Silver Ore", nameof(ARGENTITEORE));
				public static LocString DESC = "(Ag<sub>2</sub>S) Argentite is a cubic silver sulfide is a conductive metal, and the main source of refined " + FormatAsLink("Silver", "SOLIDSILVER") + " metal.";
			}
			public class AURICHALCITEORE
			{
				public static LocString NAME = FormatAsLink("Zinc Ore", nameof(AURICHALCITEORE));
				public static LocString DESC = "((Zn,Cu)<sub>5</sub>(CO<sub>3</sub>)<sub>2</sub>(OH)<sub>6</sub>) Aurichalcite is a carbonate mineral, and the main source of refined " + FormatAsLink("Zinc", "SOLIDZINC") + " metal.";
			}
			public class GALENA
			{
				public static LocString NAME = FormatAsLink("Galena", nameof(GALENA));
				public static LocString DESC = "Galena is the natural mineral form of lead(II) sulfide (PbS).It is the most important ore of " + FormatAsLink("Lead", "LEAD") + " and an important source of " + FormatAsLink("Silver", "SOLIDSILVER") + ".";
			}
			public class CHLOROSCHIST
			{
				public static LocString NAME = FormatAsLink("Chloroschist", nameof(CHLOROSCHIST));
				public static LocString DESC = "A dense medium-grained metamorphic rock showing pronounced schistosity. This sample has a high content of chloride minerals within its compacted layers.";
			}
			public class METEORORE
			{
				public static LocString NAME = FormatAsLink("Meteor Ore", nameof(METEORORE));
				public static LocString DESC = "A dense stony mass formed when various types of dust and small grains in the early Solar System accreted to form primitive asteroids. Despite their stony nature, these collision remnants contain traces of rare metals.";
			}
		}
		public class MISC
		{
			public class TAGS
			{
				public static LocString CHEMICALPROCESSING_RANDOMSAND = "Metallic Sand";
				public static LocString CHEMICALPROCESSING_RANDOMSAND_DESC = "Sandy materials composed of a various number of metallic grains";
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class CHEMICAL_ADVANCEDKILN
				{
					public static LocString NAME = FormatAsLink("Advanced Kiln", nameof(CHEMICAL_ADVANCEDKILN));
					public static LocString DESC = "Adress a wide range of industrial and chemical applications.";
					public static LocString EFFECT = "An advanced insulated electrical kiln in which the heat is applied by induction heating of metal.\nThe advantage of the induction kiln is a clean, energy-efficient and well-controllable heating process compared to most other means of fuel heating.";
				}
				public class CHEMICAL_ADVANCEDMETALREFINERY
				{
					public static LocString NAME = FormatAsLink("Advanced Metal Refinery", nameof(CHEMICAL_ADVANCEDMETALREFINERY));
					public static LocString DESC = "An improved refinery capable of bulk smelting process, as well exclusive metal production.";
					public static LocString EFFECT = string.Concat(
					"An advanced method for production of ",
					FormatAsLink("Refined Metals", "REFINEDMETAL"),
					" from raw ",
					FormatAsLink("Metal Ore", "RAWMETAL"),
					".\n\nSignificantly ",
					FormatAsLink("Heats", "HEAT"),
					" and exclusively uses ",
					FormatAsLink("Super Coolant", "SUPERCOOLANT"),
					" piped into it.\n\nDuplicants will not fabricate items unless recipes are queued."
					);
				}
				public class CHEMICAL_BALLCRUSHERMILL
				{
					public static LocString NAME = FormatAsLink("Ball Crusher Mill", nameof(CHEMICAL_BALLCRUSHERMILL));
					public static LocString DESC = "A large sized industrial mill that crushes raw ores using steel balls and special mixture of acids. Capable to process much more than the standar mill, as well more efficient in the extraction of valuable minerals from the raw more sludge.";
					public static LocString EFFECT = string.Concat("Crush down ", FormatAsLink("Raw Minerals", "RAWMINERAL"), " in to useful materials and industrial ingredients.");
				}
				public class CHEMICAL_CO2PUMP
				{
					public static LocString NAME = FormatAsLink("CO2 Filter", nameof(CHEMICAL_CO2PUMP));
					public static LocString DESC = "A fancy pump capable to detects Carbon Dioxide and pump it.";
					public static LocString EFFECT = string.Concat(
						"Automatically detects trace of ",
						FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
						" and extract it out of the surroudings."
							);

				}
				public class CHEMICAL_CO2RECYCLER
				{
					public static LocString NAME = FormatAsLink("Carbon Recycling Unit", nameof(CHEMICAL_CO2RECYCLER));
					public static LocString DESC = "An autonomous chemical device capable of executing both Bosch and Sabatier Reactions based on the input conditions.";
					public static LocString EFFECT = string.Concat(
					"Sabatier Reaction: \n " +
						FormatAsLink("Liquid Carbon Dioxide", "LIQUIDCARBONDIOXIDE"), ", " +
						FormatAsLink("Hydrogen", "HYDROGEN"), " and " +
						FormatAsLink("Iron", "IRON"), " as catalyst. The Sabatier Reaction " +
						"produce ", FormatAsLink("Water", "WATER"), ", " +
						FormatAsLink("Natural Gas", "METHANE"), " and " +
						FormatAsLink("Rust", "RUST"), " as waste product.\n\n" +

						"Bosch Reaction: \n" +
						FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"), ", " +
						FormatAsLink("Hydrogen", "HYDROGEN"), " and " +
						FormatAsLink("Iron", "IRON"), " as catalyst. The Bosch Reaction " +
						"produce ", FormatAsLink("Steam", "STEAM"), ", " +
						FormatAsLink("Refined Coal", "REFINEDCARBON"), " and " +
						FormatAsLink("Rust", "RUST"), " as waste product.");
				}
				public class CHEMICAL_CO2RECYCLERDLC1
				{
					public static LocString NAME = FormatAsLink("Carbon Recycling Unit", nameof(CHEMICAL_CO2RECYCLERDLC1));
					public static LocString DESC = "An autonomous chemical device capable of executing both Bosch and Sabatier Reactions based on the input conditions.";
					public static LocString EFFECT = string.Concat(
					"Sabatier Reaction: \n " +
						FormatAsLink("Liquid Carbon Dioxide", "LIQUIDCARBONDIOXIDE"), ", " +
						FormatAsLink("Hydrogen", "HYDROGEN"), " and " +
						FormatAsLink("Iron", "IRON"), " as catalyst. The Sabatier Reaction " +
						"produce ", FormatAsLink("Water", "WATER"), ", " +
						FormatAsLink("Natural Gas", "METHANE"), " and " +
						FormatAsLink("Rust", "RUST"), " as waste product.\n\n" +

						"Bosch Reaction: \n" +
						FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"), ", " +
						FormatAsLink("Hydrogen", "HYDROGEN"), " and " +
						FormatAsLink("Iron", "IRON"), " as catalyst. The Bosch Reaction " +
						"produce ", FormatAsLink("Steam", "STEAM"), ", " +
						FormatAsLink("Graphite", "GRAPHITE"), " and " +
						FormatAsLink("Rust", "RUST"), " as waste product.");
				}
				public class CHEMICAL_COAL_BOILER
				{
					public static LocString NAME = FormatAsLink("Coal-fueled Steam Boiler", nameof(CHEMICAL_COAL_BOILER));
					public static LocString DESC = "An industrial grade boiler that generates thermal energy by burning Coal.";
					public static LocString EFFECT = string.Concat("Boils ", FormatAsLink("Water", "WATER"), " to ", FormatAsLink("Steam", "STEAM"), " at 200 °C.\nThis particular boiler uses ", FormatAsLink("Coal", "CARBON"), " as fuel.");
				}
				public class CHEMICAL_CRUDEOILREFINERY
				{
					public static LocString NAME = FormatAsLink("Crude Oil Refinery", nameof(CHEMICAL_CRUDEOILREFINERY));
					public static LocString DESC = "An industrial process plant responsible for refining raw oil extracted from wells.";
					public static LocString EFFECT = string.Concat("This refinement plant is capable of the following production from "
						, FormatAsLink("Crude Oil", "CRUDEOIL"), ": \n" +
						"- 50% ", FormatAsLink("Petroleum", "PETROLEUM"), "\n" +
						"- 25% ", FormatAsLink("Naphtha", "NAPHTHA"), "\n" +
						"- 10% ", FormatAsLink("Natural Gas", "METHANE"), "\n" +
						"- 10% ", FormatAsLink("Sour Water", "SOURWATER"), " waste.\n" +
						"- 5% ", FormatAsLink("Bitumen", "BITUMEN"), " waste.\n\n" +
						"The process require ", FormatAsLink("Steam", "STEAM"), " for the operation.\n\n" +
						"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate.");

				}
				public class CHEMICAL_ELECTRICBOILER
				{
					public static LocString NAME = FormatAsLink("Electric Steam Boiler", nameof(CHEMICAL_ELECTRICBOILER));
					public static LocString DESC = "A small sized eletric boiler.";
					public static LocString EFFECT = string.Concat("Boils ", FormatAsLink("Water", "WATER"), " to ", FormatAsLink("Steam", "STEAM"), " at 106 °C. This particular boiler uses electricity.");
				}
				public class CHEMICAL_ENDOTHERMICUNIT
				{
					public static LocString NAME = FormatAsLink("Endothermic Unit", nameof(CHEMICAL_ENDOTHERMICUNIT));
					public static LocString DESC = "A device that uses endothermical chemical reaction to cool itself and its surroundings.";
					public static LocString EFFECT = string.Concat(new string[] { "Creates an endothermical reaction from a mixture of ", FormatAsLink("Nitrate Nodules", "AMMONIUMSALT"), " and ", FormatAsLink("Water", "WATER"), ". \nOutputs ", FormatAsLink("Ammonium Water", "AMMONIUMWATER"), " as result." });
				}
				public class CHEMICAL_FLOCCULATIONSIEVE
				{
					public static LocString NAME = FormatAsLink("Flocculation Sieve", nameof(CHEMICAL_FLOCCULATIONSIEVE));
					public static LocString DESC = "A dedicated industrial sieve that flocculates colloidal particles out of suspension to sediment under the form of floc. Strong filter media further improves the cleaning process of liquids.";
					public static LocString EFFECT = string.Concat(new string[]
					{
						"Treat ",
						FormatAsLink("Polluted Water", "DIRTYWATER"), " or " +
						"Toxic Slurry using special filter and chemicals. " +
						"Sieve process also completely removes", FormatAsLink("Germs", "DISEASE"),"."
					});
				}
				public class CHEMICAL_GAS_BOILER
				{
					public static LocString NAME = FormatAsLink("Gas-fueled Steam Boiler", nameof(CHEMICAL_GAS_BOILER));
					public static LocString DESC = "An industrial grade boiler that generates thermal energy by burning Combustible Gases.";
					public static LocString EFFECT = string.Concat(new string[] { "Boils ", FormatAsLink("Water", "WATER"), " to ", FormatAsLink("Steam", "STEAM"), " at 200 °C. This particular boiler uses ", FormatAsLink("Natural Gas", "METHANE"), " as fuel, but may as well work with other combustible gases." });
				}
				public class CHEMICAL_GLASSFOUNDRY
				{
					public static LocString NAME = FormatAsLink("Glass Foundry", nameof(CHEMICAL_GLASSFOUNDRY));
					public static LocString DESC = "A plasma arc furnace uses low-temperature plasma flow created by an electric arc heater (plasmatron).";
					public static LocString EFFECT = string.Concat(new string[]
						{
							"This techlogical advanced glass foundry is capable more than melt ",
							FormatAsLink("Sand", "SAND"),
							" in to ",
							FormatAsLink("Molten Glass", "MOLTENGLASS"),
							", but a wide range of other applications."
						});
				}
				public class CHEMICAL_SMALLCRUSHERMILL
				{
					public static LocString NAME = FormatAsLink("Jaw Crusher Mill", nameof(CHEMICAL_SMALLCRUSHERMILL));
					public static LocString DESC = "A jaw crusher uses compressive force for breaking of stone and other raw minerals.";
					public static LocString EFFECT = string.Concat(new string[]
						{
						"Crush down ",
						FormatAsLink("Raw Minerals", "RAWMINERAL"),
						" in to useful materials and industrial ingredients."
						});
				}
				public class CHEMICAL_NAPHTHAREFORMER
				{
					public static LocString NAME = FormatAsLink("Naphtha Reformer", nameof(CHEMICAL_NAPHTHAREFORMER));
					public static LocString DESC = "An industrial petrochemical plant responsible for rearranging hydrocarbon molecules of Naphtha in to Petroleum.";
					public static LocString EFFECT = string.Concat(new string[]
						{
							"Second Stage refinement plant is capable of furter refine ",FormatAsLink("Naphtha", "NAPHTHA"), ":\n "+
							"- 45% ",FormatAsLink("Petroleum", "PETROLEUM"), "\n" +
							"- 10% ",FormatAsLink("Natural Gas", "METHANE"), "\n" +
							"- 45% ",FormatAsLink("Bitumen", "BITUMEN"),".\n\n" +
							
							"The process requires ", FormatAsLink("Hydrogen", "HYDROGEN")," to buffer the reaction." +
							"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate."
						});
				}

			}
		}

		public class ITEMS
		{
			public class INGREDIENTS
			{
				public class RAYONFIBER
				{
					public static LocString NAME = FormatAsLink("Rayon Fiber", nameof(RAYONFIBER));
					public static LocString NAME_PLURAL = FormatAsLink("Rayon Fibers", nameof(RAYONFIBER));
					public static LocString DESC = "Rayon is a synthetic fiber, chemically made from regenerated cellulose extracted from Lumber.";
				}
			}
		}
		public class DUPLICANTS
		{
			public class STATUSITEMS
			{
				public class ACIDBURNS
				{
					public static LocString NOTIFICATION_NAME = "Chemical Burns";
					public static LocString NOTIFICATION_TOOLTIP = "Duplicants have been exposed to acids.";
					public static LocString NAME = "Acid Burns";
					public static LocString TOOLTIP = "This duplicant has been injured from their recent exposure to corrosive acids.";
				}
			}
		}
		public class UI
		{
			public class CHEMICAL_COMPLEXFABRICATOR_STRINGS
			{
				public static LocString HEAT_REFINE = "Refine {0} with intense heat, producing {1}.";
				public static LocString HEAT_COOK = "Cook {0} at high temperature, and produces {1}.";
				public static LocString THREE_MIXTURE_COMPRESS_COOKING = "Compress a mixture of {0}, {1} and {2}.\nCooking the mixture at high temperature produces {3}";
				public static LocString THREE_MIXTURE_FUSE = "Fuse together a mixture of {0} and {1}, with addition of {2} to produce {3}.";
				public static LocString THREE_MIXTURE_TWO_PRODUCTS_SMELT_WASTE = "Smelt a mixture of {0}, {1} and {2}, to produce {3} and {4}. Produces {5} as waste.";
				public static LocString THREE_MIXTURE_SMELT_WASTE = "Smelt a mixture of {0}, {1} and {2}, to produce {3}. Produces {4} as waste.";

				public static LocString BALLCRUSHER_MILLING_2INGREDIENTS = "Mill down {0} with a mixture of {1} and {2}.\nThe milling process has a random chance of yielding random amounts of the following materials:\n{3}\n\nProduces large amounts of {4} as waste product.";
				public static LocString BALLCRUSHER_MILLING_3INGREDIENTS = "Mill down {0} with a mixture of {1}, {2} and {3}.\nThe milling process has a random chance of yielding random amounts of the following materials:\n{4}\n\nProduces large amounts of {5} as waste product.";
				public static LocString ARCFURNACE_SMELT = "Smelt {0} to produce {1}.";
				public static LocString ARCFURNACE_MELT = "Melt {0} to produce {1}.";

				public static LocString JAWCRUSHERMILL_MILLING_1_1_2 = "Grind down {0} and extract small amounts of {1}.\nProduces {2} and {3} as waste products.";
				public static LocString JAWCRUSHERMILL_MILLING_1_1 = "Grind down {0} to {1}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_1_BREAK = "Break down {0} to {1}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_2 = "Break down {0} to {1} and {2}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_4 = "Break down {0} to:\n• {1}\n• {2}\n• {3}\n• {4}";
				public static LocString CRUSHEDROCK_FROM_RAW_MINERAL_NAME = FormatAsLink("Raw Mineral", "BUILDABLERAW") + " to " + global::STRINGS.ELEMENTS.CRUSHEDROCK.NAME;
				public static LocString CRUSHEDROCK_FROM_RAW_MINERAL_DESCRIPTION = "Crushes " + FormatAsLink("Raw Minerals", "BUILDABLERAW") + " into " + global::STRINGS.ELEMENTS.CRUSHEDROCK.DESC;

				public class RANDOMRECIPERESULT
				{
					public static LocString NAME = "Random Composition: {0}";
					public static LocString DESC = "This recipe yields {0} of random amounts of the following elements:";
					public static LocString DESC_MAX_COUNT = "This recipe yields {0} of random amounts of {1} of the following elements:";
					public static LocString COMPOSITION_ENTRY = "• {0}, {1} - {2}";
					public static LocString COMPOSITION_ENTRY_CHANCE = "• {0}: {1} - {2}, {3} Chance";
				}
			}
		}
	}
}
