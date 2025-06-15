using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
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
					public static LocString BASEGAME_RECIPE =
						string.Concat(
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
					public static LocString SPACEDOUT_RECIPE = string.Concat(
						"THIS BUILDING HAS BEEN DEPRECATED AND CANNOT BE BUILT. INSTEAD, USE THE REGULAR " + CHEMICAL_CO2RECYCLER.NAME + "\n\n",

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

					public static LocString EFFECT => DlcManager.IsExpansion1Active() ? SPACEDOUT_RECIPE : BASEGAME_RECIPE;
				}
				public class CHEMICAL_CO2RECYCLERDLC1
				{
					public static LocString NAME = FormatAsLink("[DEPRECATED] SpacedOut Carbon Recycling Unit", nameof(CHEMICAL_CO2RECYCLERDLC1));
					public static LocString DESC = "An autonomous chemical device capable of executing both Bosch and Sabatier Reactions based on the input conditions.";
					public static LocString EFFECT = string.Concat(
						"THIS BUILDING HAS BEEN DEPRECATED AND CANNOT BE BUILT. INSTEAD, USE THE REGULAR " + CHEMICAL_CO2RECYCLER.NAME + "\n\n",

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
					public static LocString EFFECT = string.Concat(["Creates an endothermical reaction from a mixture of ", FormatAsLink("Nitrate Nodules", "AMMONIUMSALT"), " and ", FormatAsLink("Water", "WATER"), ". \nOutputs ", FormatAsLink("Ammonium Water", "AMMONIUMWATER"), " as result."]);
				}
				public class CHEMICAL_FLOCCULATIONSIEVE
				{
					public static LocString NAME = FormatAsLink("Flocculation Sieve", nameof(CHEMICAL_FLOCCULATIONSIEVE));
					public static LocString DESC = "A dedicated industrial sieve that flocculates colloidal particles out of suspension to sediment under the form of floc. Strong filter media further improves the cleaning process of liquids.";
					public static LocString EFFECT = string.Concat(
					[
						"Treat ",
						FormatAsLink("Polluted Water", "DIRTYWATER"), " or " +
						"Toxic Slurry using special filter and chemicals. " +
						"Sieve process also completely removes", FormatAsLink("Germs", "DISEASE"),"."
					]);
				}
				public class CHEMICAL_GAS_BOILER
				{
					public static LocString NAME = FormatAsLink("Gas-fueled Steam Boiler", nameof(CHEMICAL_GAS_BOILER));
					public static LocString DESC = "An industrial grade boiler that generates thermal energy by burning Combustible Gases.";
					public static LocString EFFECT = string.Concat(["Boils ", FormatAsLink("Water", "WATER"), " to ", FormatAsLink("Steam", "STEAM"), " at 200 °C. This particular boiler uses ", FormatAsLink("Natural Gas", "METHANE"), " as fuel, but may as well work with other combustible gases."]);
				}
				public class CHEMICAL_GLASSFOUNDRY
				{
					public static LocString NAME = FormatAsLink("Glass Foundry", nameof(CHEMICAL_GLASSFOUNDRY));
					public static LocString DESC = "A plasma arc furnace uses low-temperature plasma flow created by an electric arc heater (plasmatron).";
					public static LocString EFFECT = string.Concat(
						[
							"This techlogical advanced glass foundry is capable more than melt ",
							FormatAsLink("Sand", "SAND"),
							" in to ",
							FormatAsLink("Molten Glass", "MOLTENGLASS"),
							", but a wide range of other applications."
						]);
				}
				public class CHEMICAL_SMALLCRUSHERMILL
				{
					public static LocString NAME = FormatAsLink("Jaw Crusher Mill", nameof(CHEMICAL_SMALLCRUSHERMILL));
					public static LocString DESC = "A jaw crusher uses compressive force for breaking of stone and other raw minerals.";
					public static LocString EFFECT = string.Concat(
						[
						"Crush down ",
						FormatAsLink("Raw Minerals", "RAWMINERAL"),
						" in to useful materials and industrial ingredients."
						]);
				}
				public class CHEMICAL_NAPHTHAREFORMER
				{
					public static LocString NAME = FormatAsLink("Naphtha Reformer", nameof(CHEMICAL_NAPHTHAREFORMER));
					public static LocString DESC = "An industrial petrochemical plant responsible for rearranging hydrocarbon molecules of Naphtha in to Petroleum.";
					public static LocString EFFECT = string.Concat(
						[
							"Second Stage refinement plant is capable of furter refine ",FormatAsLink("Naphtha", "NAPHTHA"), ":\n "+
							"- 45% ",FormatAsLink("Petroleum", "PETROLEUM"), "\n" +
							"- 10% ",FormatAsLink("Natural Gas", "METHANE"), "\n" +
							"- 45% ",FormatAsLink("Bitumen", "BITUMEN"),".\n\n" +

							"The process requires ", FormatAsLink("Hydrogen", "HYDROGEN")," to buffer the reaction." +
							"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate."
						]);
				}
				public class CHEMICAL_PROPANEREFORMER
				{
					public static LocString NAME = FormatAsLink("Propane Reformer", nameof(CHEMICAL_PROPANEREFORMER));
					public static LocString DESC = "An industrial petrochemical plant responsible for oxidative steam reforming process of Propane to Hydrogen.";
					public static LocString EFFECT = string.Concat(
						[
							"Reforming process of ",FormatAsLink("Propane", "PROPANE"), " in to:\n "+
							"- 60% ",FormatAsLink("Hydrogen", "HYDROGEN"), "\n" +
							"- 30% ",FormatAsLink("Polluted Water", "DIRTYWATER"), " waste\n" +
							"- 10% ",FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE")," waste\n" +

							"The process require ",FormatAsLink("Steam", "STEAM")," for the operation.\n\n" +
							"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate."
						]);
				}
				public class CHEMICAL_PYROLYSISKILN
				{
					public static LocString NAME = FormatAsLink("Pyrolysis Kiln", nameof(CHEMICAL_PYROLYSISKILN));
					public static LocString DESC = "A basic kiln that uses pyrolysis process to convert woodlogs to usable coal.";
					public static LocString EFFECT = string.Concat(
						[
							"Cook ",
							FormatAsLink("Woodlog", "WOODLOG"),
							" to ",
							FormatAsLink("Coal", "CARBON"),
							"."
						]);
				}
				public class CHEMICAL_RAWGASREFINERY
				{
					public static LocString NAME = FormatAsLink("Raw Gas Refinery", nameof(CHEMICAL_RAWGASREFINERY));
					public static LocString DESC = "An industrial process plant responsible for refining the impure raw natural gas extracted from wells.";
					public static LocString EFFECT = string.Concat(
						[
							"This refinement plant is capable of the following production: \n " +
							"- 50% ",FormatAsLink("Natural Gas", "METHANE"), "\n" +
							"- 35% ",FormatAsLink("Propane", "PROPANE"), "\n" +
							"- 15% ",FormatAsLink("Sour Water", "SOURWATER")," waste\n" +
							"The process require ",FormatAsLink("Steam", "STEAM")," for the operation.\n\n" +

							"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate."

						]);
				}
				public class CHEMICAL_RAYONLOOM
				{
					public static LocString NAME = FormatAsLink("Rayon Loom", nameof(CHEMICAL_RAYONLOOM));
					public static LocString DESC = "A chemical loom capable of producing celulose fibers with Viscose process.";
					public static LocString EFFECT = string.Concat(
						[
							"Produces ",
							FormatAsLink("Rayon Fiber", RayonFabricConfig.TAG.ProperName()),
							" from ",
							global::STRINGS.ELEMENTS.WOODLOG.NAME,
							" pulp through a complex chemical reaction. Requires ",
							FormatAsLink("Synthetic Gas", "SYNGAS"),
							" and constantly outputs ",
							FormatAsLink("Steam", "STEAM"),
							" while operational."
						]);
				}
				public class CHEMICAL_SELECTIVEARCFURNACE
				{
					public static LocString NAME = FormatAsLink("Selective Arc-Furnace", nameof(CHEMICAL_SELECTIVEARCFURNACE));
					public static LocString DESC = "A specialized furnace that heats material by means of an electric arc. Its delicate heat control structure allows mixture of metal alloys, as well separating metals from an homogeneous mixture of scraps. Since the furnace is air-cooled, it releases a lot of heat into its surroundings.";
					public static LocString EFFECT = string.Concat(
						[
							"Special works with ",
							FormatAsLink("Refined Metals", "REFINEDMETAL"),
							" and in the manufacture of metal alloys."
						]);
				}
				public class CHEMICAL_SOILMIXER
				{
					public static LocString NAME = FormatAsLink("Soil Mixer", nameof(CHEMICAL_SOILMIXER));
					public static LocString DESC = "A mixer designed for uniform blending of a wide variety of solid materials, while treating them with chemicals.";
					public static LocString EFFECT = string.Concat(
					[
						"A solid material mixer designed for agricultural purpose, mainly for the production of ",
						FormatAsLink("Dirt", "DIRT"),
						" and ",
						FormatAsLink("Fertilizer", "FERTILIZER"),
						"." ]);

				}
				public class CHEMICAL_SOURWATERSTRIPPER
				{
					public static LocString NAME = FormatAsLink("Sour Water Stripper", nameof(CHEMICAL_SOURWATERSTRIPPER));
					public static LocString DESC = "A heavy industrial device that uses a combination of pH control and heat, direct injection of steam to drives off the ammonia and hydrogen sulfide from the water. Also fitted with a filter mechanism to ensure the quality of the stripped water.";
					public static LocString EFFECT = string.Concat(
						[
						"Separate clean ",FormatAsLink("Water", "WATER")," from "
						,FormatAsLink("Sour Water", "SOURWATER"), " using hot "
						,FormatAsLink("Steam", "STEAM"), ".\n " +
						"The stripping process also produces "
						,FormatAsLink("Sour Gas", "SOURGAS"), " from the separation.\n"
						,FormatAsLink("Sand", "SAND"), " is required to further filter the water from any other contaminants it may still have, which is then released as  "
						,FormatAsLink("Polluted Dirty", "TOXICSAND"), " afterwards.\n" +
						"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate."
						]);
				}
				public class CHEMICAL_SYNGASREFINERY
				{
					public static LocString NAME = FormatAsLink("Syngas Refinery", nameof(CHEMICAL_SYNGASREFINERY));
					public static LocString DESC = "A refinery capable of catalytic partial oxidation reactions to produce Syngas.";
					public static LocString EFFECT = "Produce Synthetic Gas from a variety of Organic and Mineral materials.";

				}
				public class CHEMICAL_SYNTHESIZERNITRIC
				{
					public static LocString NAME = FormatAsLink("Nitric Acid Synthesizer", nameof(CHEMICAL_SYNTHESIZERNITRIC));
					public static LocString DESC = "A chemical synthesizer device capable of producing Nitric Acid.";
					public static LocString EFFECT = string.Concat(new string[]
						{
							"Produces industrial grade ",
							FormatAsLink("Nitric Acid", "LIQUIDNITRIC"), " using provided " +
							FormatAsLink("Sulfuric Acid", "LIQUIDSULFURIC")," and " +
							FormatAsLink("Ammonia", "AMMONIAGAS"),"."
						});
				}
				public class CHEMICAL_SYNTHESIZERSALTWATER
				{
					public static LocString NAME = FormatAsLink("Salt Water Mixer", nameof(CHEMICAL_SYNTHESIZERSALTWATER));
					public static LocString DESC = "A simple device capable of producing high-quality salt water.";
					public static LocString EFFECT =
						"Produces " + FormatAsLink("Salt Water", "SALTWATER") + " using provided " +
						FormatAsLink("Water", "WATER") + " and " +
						FormatAsLink("Salt", "SALT") + ".";
				}
				public class CHEMICAL_SYNTHESIZERSULFURIC
				{
					public static LocString NAME = FormatAsLink("Sulfuric Acid Synthesizer", nameof(CHEMICAL_SYNTHESIZERSULFURIC));
					public static LocString DESC = "A chemical synthesizer device capable of producing Sulfuric Acid.";
					public static LocString EFFECT = string.Concat(new string[]
						{
							"Produces industrial grade ",
							FormatAsLink("Sulfuric Acid", "LIQUIDSULFURIC"), " using provided " +
							FormatAsLink("Steam", "STEAM")," and " +
							FormatAsLink("Sulfur", "SULFUR"), "."
						});
				}
				public class CHEMICAL_THERMALDESALINATOR
				{
					public static LocString NAME = FormatAsLink("Thermal Desalinator", nameof(CHEMICAL_THERMALDESALINATOR));
					public static LocString DESC = "A basic desalinator that uses Vapor-compression evaporation to archive the separation of clean, usable water from the solution it is currently mixed.";
					public static LocString EFFECT = string.Concat(new string[]
						{
							"Uses ",
							FormatAsLink("Steam", "STEAM"), " as buffer for the Desalination process of either " +
							FormatAsLink("Salt Water", "SALTWATER")," or Ammonium Water, releasing fresh " +
							FormatAsLink("Water", "WATER"), " and concentrated ",FormatAsLink("Brine", "BRINE"),
							" in return."
						});
				}
				public class CHEMICAL_WOODEN_BOILER
				{
					public static LocString NAME = FormatAsLink("Wood-fueled Steam Boiler", nameof(CHEMICAL_WOODEN_BOILER));
					public static LocString DESC = "An industrial grade boiler that generates thermal energy by burning wood.";
					public static LocString EFFECT = "Boils " + FormatAsLink("Water", "WATER") + " to " + FormatAsLink("Steam", "STEAM") + " at 200 °C. This particular boiler uses " + FormatAsLink("Wood", "WOODLOG") + " as fuel.";
				}
				public class CUSTOM_OILWELLCAP
				{
					public static LocString ANY_WATER_DESC = "Extracts " + FormatAsLink("Crude Oil", "CRUDEOIL") + " using any form of water.\n\nMust be built atop an " + FormatAsLink("Oil Reservoir", "OIL_WELL") + ".";
					public static LocString NAME = FormatAsLink("[DEPRECATED] Custom Oil Well Pump", nameof(CUSTOM_OILWELLCAP));
					public static LocString DESC = "A large device capable to extract oil from the attached well using liquid pumped in. Liquid pumped into an oil reservoir cannot be recovered. ";
					public static LocString EFFECT = "THIS BUILDING HAS BEEN DEPRECATED AND CANNOT BE BUILT. INSTEAD, USE THE REGULAR " + FormatAsLink("Oil Well", "OILWELLCAP") + "\n\n" + ANY_WATER_DESC;
				}
				public class CUSTOM_POLYMERIZER
				{
					public static LocString NAME = FormatAsLink("Ethanol Polymer Press", nameof(CUSTOM_POLYMERIZER));
					public static LocString DESC = "A custom polymerization press capable of producing polymer from Ethanol.";
					public static LocString EFFECT = string.Concat(new string[]
						{
							"Special modifications allows the polymerization of ",
							FormatAsLink("Ethanol", "ETHANOL"),
							" into raw ",
							FormatAsLink("Plastic", "POLYPROPYLENE"),
							"with the addition of ",
							FormatAsLink("Chlorine Gas", "CHLORINE"),"."

						});
				}
				public class CUSTOM_METALREFINERY
				{
					public static LocString NAME = FormatAsLink("[DEPRECATED] Custom Metal Refinery", nameof(CUSTOM_METALREFINERY));
					public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.DESC;
					public static LocString EFFECT = string.Concat(new string[]
						{
							"THIS BUILDING HAS BEEN DEPRECATED AND CANNOT BE BUILT. INSTEAD, USE THE REGULAR " + FormatAsLink("METAL REFINERY", "METALREFINERY"),"\n\n",
							global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.EFFECT

						});
				}
			}
		}
		public class CREATURES
		{
			public class SPECIES
			{
				public class GEYSER
				{
					public class AMMONIAGEYSER
					{
						public static LocString NAME = FormatAsLink("Ammonia Geyser", "GeyserGeneric_" + nameof(AMMONIAGEYSER));
						public static LocString DESC = $"A highly pressurized cryogeyser that periodically erupts with cold liquid {FormatAsLink("Ammonia", "LIQUIDAMMONIA")}.";
					}
					public class AMMONIUMWATERGEYSER
					{
						public static LocString NAME = FormatAsLink("Ammonium Water Geyser", "GeyserGeneric_" + nameof(AMMONIUMWATERGEYSER));
						public static LocString DESC = $"A highly pressurized cryogeyser that periodically erupts with cold {FormatAsLink("Ammonium Water", "AMMONIUMWATER")}.";
					}
					public class HOTMERCURYGASVENT
					{
						public static LocString NAME = FormatAsLink("Hot Mercury Gas Vent", "GeyserGeneric_" + nameof(HOTMERCURYGASVENT));
						public static LocString DESC = $"A highly pressurized geothermal vent that periodically erupts with hot {FormatAsLink("Mercury Gas", "MERCURYGAS")}.";
					}
					public class MOLTENSALTGEYSER
					{
						public static LocString NAME = FormatAsLink("Molten Salt Geyser", "GeyserGeneric_" + nameof(MOLTENSALTGEYSER));
						public static LocString DESC = $"A highly pressurized hot fissure that periodically erupts with {FormatAsLink("Molten Salt", "MOLTENSALT")}.";
					}
					public class PHOSPHORUSGEYSER
					{
						public static LocString NAME = FormatAsLink("Liquid Phosphorus Geyser", "GeyserGeneric_" + nameof(PHOSPHORUSGEYSER));
						public static LocString DESC = $"A highly pressurized hot fissure that periodically erupts with {FormatAsLink("Liquid Phosphorus", "LIQUIDPHOSPHORUS")}.";
					}
					public class RAWGASVENT
					{
						public static LocString NAME = FormatAsLink("Hot Raw Natural Gas Vent", "GeyserGeneric_" + nameof(RAWGASVENT));
						public static LocString DESC = $"A highly pressurized geothermal vent that periodically erupts with hot {FormatAsLink("Raw Natural Gas", "RAWNATURALGAS")}.";
					}
					public class SOURWATERGEYSER
					{
						public static LocString NAME = FormatAsLink("Sour Water Geyser", "GeyserGeneric_" + nameof(SOURWATERGEYSER));
						public static LocString DESC = $"A highly pressurized geyser that periodically erupts with cold {FormatAsLink("Sour Water", "SOURWATER")}.";
					}
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

			//===== [ Vegetable Oil ] ================================
			public class SOLIDVEGEOIL
			{
				public static LocString NAME = FormatAsLink("Frozen Vegetable Oil", nameof(SOLIDVEGEOIL));
				public static LocString DESC = "Frozen solid oil extracted from vegetable biomass, composed of triglycerides and monounsaturated fatty acids.";
			}
			public class LIQUIDVEGEOIL
			{
				public static LocString NAME = FormatAsLink("Vegetable Oil", nameof(LIQUIDVEGEOIL));
				public static LocString DESC = "Liquid oil extracted from vegetable biomass, composed of triglycerides and monounsaturated fatty acids.";
			}
			public class VEGEOILGAS
			{
				public static LocString NAME = FormatAsLink("Oily Gas", nameof(VEGEOILGAS));
				public static LocString DESC = "Gaseous oil extracted from vegetable biomass, composed of triglycerides and monounsaturated fatty acids.";
			}

			//===== [ Biodiesel ] ========================================
			public class SOLIDBIODIESEL
			{
				public static LocString NAME = FormatAsLink("Frozen Renewable Diesel", nameof(SOLIDBIODIESEL));
				public static LocString DESC = "Biodiesel is a renewable biofuel from biological sources like vegetable oils. Frozen solid on its present form.";
			}
			public class LIQUIDBIODIESEL
			{
				public static LocString NAME = FormatAsLink("Renewable Diesel", nameof(LIQUIDBIODIESEL));
				public static LocString DESC = "Renewable diesel is a renewable biofuel from biological sources like vegetable oils. Used as an alternative to fossil fuels.";
			}

			//===== [ Bioplastic ] ========================================
			public class BIOPLASTIC
			{
				public static LocString NAME = FormatAsLink("Bioplastic", nameof(BIOPLASTIC));
				public static LocString DESC = "A synthetic biopolymer produced from renewable biomass sources, such as vegetable fats and oils, and enzymes from bacterial biosynthesis. Unlike traditional plastics, which are derived from fossil fuels, bioplastics are obtained from renewable resources.";
			}

			//===== [ Biomass ] ==========================================
			public class SOLIDBIOMASS
			{
				public static LocString NAME = FormatAsLink("Compressed Biomass", nameof(SOLIDBIOMASS));
				public static LocString DESC = "A dried, hard pressed clump of organic mass from vegetable origin. Has almost no moinsture and can be used as burnable fuel or can be turned to dirt through composting.";
			}
		}
		public class MISC
		{
			public class TAGS
			{
				public static LocString CHEMICALPROCESSING_RANDOMSAND = FormatAsLink("Metallic Sand", nameof(CHEMICALPROCESSING_RANDOMSAND));
				public static LocString CHEMICALPROCESSING_RANDOMSAND_DESC = "Sandy materials composed of a various number of metallic grains";
				public static LocString CHEMICALPROCESSING_BIODIESEL_COMPOSITION = FormatAsLink("Organic Fuel", nameof(CHEMICALPROCESSING_BIODIESEL_COMPOSITION));
				public static LocString CHEMICALPROCESSING_BIODIESEL_COMPOSITION_DESC = "Organic fuels serve as a renewable alternative to petrochemicals.";
				public static LocString CHEMICALPROCESSING_BIOOIL_COMPOSITION = FormatAsLink("Organic Oil", nameof(CHEMICALPROCESSING_BIOOIL_COMPOSITION));
				public static LocString CHEMICALPROCESSING_BIOOIL_COMPOSITION_DESC = "Organic Oils are extracted from renewable biomass.";
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
				public static LocString JAWCRUSHERMILL_MILLING_1_1_BREAK = "Break down {0} into {1}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_2 = "Break down {0} into {1} and {2}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_4 = "Break down {0} into:\n• {1}\n• {2}\n• {3}\n• {4}";
				public static LocString CRUSHEDROCK_FROM_RAW_MINERAL_NAME = FormatAsLink("Raw Mineral", "BUILDABLERAW") + " to " + global::STRINGS.ELEMENTS.CRUSHEDROCK.NAME;
				public static LocString CRUSHEDROCK_FROM_RAW_MINERAL_DESCRIPTION = "Crushes " + FormatAsLink("Raw Minerals", "BUILDABLERAW") + " into " + global::STRINGS.ELEMENTS.CRUSHEDROCK.NAME;
				public static LocString RAYON_LOOM_FIBER = "Produces {0} from the pulp of {1}.";

				public static LocString ARCFURNACE_SMELT_2_1 = "Smelt {0} and {1} to produce {2}";
				public static LocString ARCFURNACE_SMELT_3_1 = "Smelt {0} and {1} with addition of {2} to produce {3}";
				public static LocString ARCFURNACE_STEEL_1 = "Refine {0} to {3} with a mixture of of {1} and {2} ";
				public static LocString ARCFURNACE_STEEL_2 = "Refine {0} to {4} with a mixture of of {1}, {2} and {3}";
				public static LocString ARCFURNACE_RANDOM_RECIPE =
					"Smelt {0} into random products.\n" +
					"The Furnace system will separate the mixture in different portions based on each layer composition.\n" +
					"The smelting process has a random chance of yelding the following materials: {1}\n" +
					"\nProduces {2} as waste.";
				public static LocString ARCFURNACE_NIOBIUM = "Smelt down {0} alloy to basic {1} metal.";

				public static LocString SOILMIXER_3_1 = "Address a mixture of {0}, {1} and {2}, to produce {3}.";
				public static LocString SOILMIXER_4_1 = "Address a mixture of {0}, {1}, {2} and {3}, to produce {4}.";

				public static LocString SYNGASREFINERY_1_1_1 = "Refine {0} to {1}. Produces {2} as waste product.";
				public static LocString SYNGASREFINERY_1_1_2 = "Refine {0} to {1}. Produces {2} and {3} as by-product.";


				public static LocString METALREFINERY_2_1_1 = "Smelt a mixture of {0} and {1}, to produce {2}.\nProduces {3} as waste.";
				public static LocString METALREFINERY_2_2_1 = "Smelt a mixture of {0} and {1}, to produce {2} and {3}.\nProduces {4} as waste.";


				public static LocString SUPERMATERIALREFINERY_3_1 = "Molecularly reassemble a mixture of {0} and {1} with addition of {2} to produce {3}.";

				public class RANDOMRECIPERESULT
				{
					public static LocString NAME = "Random Composition: {0}";
					public static LocString DESC = "This recipe yields {0} of random amounts of the following elements:";
					public static LocString DESC_MAX_COUNT = "This recipe yields {0} of random amounts of {1} of the following elements:";
					public static LocString DESC_RANGE = "{0} - {1}";
					public static LocString COMPOSITION_ENTRY = "• {0}, {1} - {2}";
					public static LocString COMPOSITION_ENTRY_CHANCE = "• {0}: {1} - {2}, {3} Chance";
				}
			}
			public class SPACEDESTINATIONS
			{
				public class COMETS
				{
					public class HEAVYCOMET
					{
						public static LocString NAME = "Heavy Comet";
					}
				}
			}
		}
	}
}
