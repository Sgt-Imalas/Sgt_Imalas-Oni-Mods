using Dupes_Industrial_Overhaul.Chemical_Processing.Chemicals;
using HarmonyLib;
using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.HighPressureApplications;
using STRINGS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.BuildingPortUtils;
using static RonivansLegacy_ChemicalProcessing.STRINGS.BUILDINGS.PREFABS.AIO_FACILITYDOOR.FACADES;
using static RonivansLegacy_ChemicalProcessing.STRINGS.ELEMENTS;
using static STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.UI;
using static STRINGS.UI.NEWBUILDCATEGORIES;

namespace RonivansLegacy_ChemicalProcessing
{
	class STRINGS
	{
		public static LocString RONIVANSLEGACY_AIO_NAME = "Ronivan's Legacy - All In One Pack";
		public static LocString RONIVANSLEGACY_AIO_DESC = "All the mods created by Ronivan, now collected and remastered into one configurable modpack.";
		public class AIO_MODSOURCE
		{
			public static LocString CHEMICALPROCESSING_IO = "Chemical Processing - Industrial Overhaul";
			public static LocString CHEMICALPROCESSING_BIOCHEMISTRY = "Chemical Processing - Biochemistry";
			public static LocString MINERALPROCESSING_METALLURGY = "Mineral Processing - Metallurgy";
			public static LocString MINERALPROCESSING_MINING = "Mineral Processing - Mining";
			public static LocString NUCLEARPROCESSING = "Nuclear Processing";
			public static LocString DUPESMACHINERY = "Dupes Machinery";
			public static LocString DUPESENGINEERING = "Dupes Engineering";
			public static LocString DUPESLOGISTICS = "Dupes Logistics";
			public static LocString CUSTOMRESERVOIRS = "Custom Reservoirs";
			public static LocString DUPESREFRIGERATION = "Dupes Refrigeration";
			public static LocString CUSTOMGENERATORS = "Custom Generators";
			public static LocString HIGHPRESSUREAPPLICATIONS = "High Pressure Applications";
		}

		public class DUPLICANTS
		{
			//public class PROCESSING_AIO_RONIVAN
			//{
			//	public static LocString NAME = "Ronivan";
			//	public static LocString DESC = "{0}s are great artists who enjoy creating things, even during their free time.";
			//}
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
		public class BUILDINGS
		{
			public class FACADES_STANDALONE
			{
				public class PETROLEUMDISTILLERY
				{
					public static LocString NAME = FormatAsLink("Petroleum Distillery", nameof(PETROLEUMDISTILLERY));
					public static LocString DESC = "A distillation plant that uses induction to produce Petroleum from Crude Oil";
				}
				public class WOODENLADDER
				{
					public static LocString NAME = "Wooden Ladder";
					public static LocString DESC = "A pretty ladder made of wood.";
				}

				public class EXTERIORWALL
				{
					public class WOODENDRYWALL_B
					{
						public static LocString NAME = FormatAsLink("Parquet Drywall", nameof(WOODENDRYWALL_B));
						public static LocString DESC = "A dry wall covered with a pretty wooden panel in Parquet design.";
					}
					public class WOODENDRYWALL
					{
						public static LocString NAME = FormatAsLink("Wooden Panel Wall", nameof(WOODENDRYWALL));
						public static LocString DESC = "A dry wall covered with a pretty wooden panel.";
					}

					public class BRICKWALL
					{
						public static LocString NAME = FormatAsLink("Brick Wall", nameof(BRICKWALL));
						public static LocString DESC = "A pretty dry wall made with fine bricks.";
					}
				}
			}
			public class PREFABS
			{
				#region Biochemistry
				public class BIOCHEMISTRY_ALGAEGROWINGBASIN
				{
					public static LocString NAME = FormatAsLink("Algae Growing Basin", nameof(BIOCHEMISTRY_ALGAEGROWINGBASIN));
					public static LocString DESC = "A wide, automated biological tank design to grow organic masses of algae.";
					public static LocString EFFECT = string.Concat(
						[
							"Grows ",
							FormatAsLink("Algae", "ALGAE")," using ",
							"a constant supply of  ",
							FormatAsLink("Vegetable Oil", "LIQUIDVEGEOIL"),
							" that serves as serves as nourishment for the photosynthetic organisms within.",
							"\nRequires ",FormatAsLink("Light", "LIGHT")," and a strict control of ",
							FormatAsLink("Temperature", "HEAT"),
							" as to provide proper environment for the production."
						]);
				}
				public class BIOCHEMISTRY_ANAEROBICDIGESTER
				{
					public static LocString NAME = FormatAsLink("Anaerobic Digester", nameof(BIOCHEMISTRY_ANAEROBICDIGESTER));
					public static LocString DESC = "Anaerobic digestion is processes by which microorganisms break down biodegradable organic materials in the absence of oxygen.";
					public static LocString EFFECT = string.Concat(
						[
							"Breaks down most basic ",
							FormatAsLink("Food", "FOOD"),
							" stored within to  ",
							FormatAsLink("Natural Gas", "METHANE"),
							"\nThe amount produced depends of the ingredient choosen."
						]);
				}
				public class BIOCHEMISTRY_BIODIESELGENERATOR
				{
					public static LocString NAME = FormatAsLink("Biodiesel Generator", nameof(BIOCHEMISTRY_BIODIESELGENERATOR));
					public static LocString DESC = "An advanced power generator that uses biofuel. Has a built-in scrubber system that allows it to produce no gaseous exhaust.";
					public static LocString EFFECT = string.Concat(
						[
							"Converts ",
							FormatAsLink("Organic Fuels", "CHEMICALPROCESSING_BIODIESEL_COMPOSITION"),
							" into electrical ",
							FormatAsLink("Power", "POWER"),
							".\n\nProduces only ",
							FormatAsLink("Polluted Water", "DIRTYWATER"),
							"."
						]);
				}
				public class BIOCHEMISTRY_EXPELLERPRESS
				{
					public static LocString NAME = FormatAsLink("Expeller Press", nameof(BIOCHEMISTRY_EXPELLERPRESS));
					public static LocString DESC = "A mechanical device built to extract oil from raw materials. The raw materials are squeezed under high pressure through a caged barrel-like cavity.";
					public static LocString EFFECT = string.Concat(
						[
							"Extract ",
							FormatAsLink("Vegetable Oil", "LIQUIDVEGEOIL"),
							" from raw food ingredients. The amount of oil, and the solid waste in the form of ",
							FormatAsLink("Compressed Biomass", "SOLIDBIOMASS"),
							" depends of the ingredient choosen."
						]);
				}
				public class BIOCHEMISTRY_BIODIESELREFINERY
				{
					public static LocString NAME = FormatAsLink("Biodiesel Refinery", nameof(BIOCHEMISTRY_BIODIESELREFINERY));
					public static LocString DESC = "A refinery capable of transesterification of organic oils into a mix of mono-alkyl esters of long chain fatty acids.";
					public static LocString EFFECT = string.Concat(
						[
							"Transesterify ",
							FormatAsLink("Renewable Diesel", "LIQUIDBIODIESEL"),
							" from ",
							FormatAsLink("Organic Oils", "CHEMICALPROCESSING_BIOOIL_COMPOSITION"),
							" and ",
							FormatAsLink("Ethanol", "ETHANOL"),"."
						]);
				}
				public class BIOCHEMISTRY_BIOPLASTICPRINTER
				{
					public static LocString NAME = FormatAsLink("Bioplastic Printer", nameof(BIOCHEMISTRY_BIOPLASTICPRINTER));
					public static LocString DESC = "A device that uses organic oil and bacterial enzymes to print small sheets of lipid derived biopolymers which are compressed in to a solid block.";
					public static LocString EFFECT = string.Concat(
						[
							"Synthesizes ",
							FormatAsLink("Bioplastic", "BIOPLASTIC"),
							" from ",
							FormatAsLink("Organic Oils", "CHEMICALPROCESSING_BIOOIL_COMPOSITION"),
							" and ",
							FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),
							"\nThe enzymes requires a microbial medium in the form of ",
							FormatAsLink("Mush Bar", "MUSHBAR"),"."
						]);
				}
				#endregion
				#region ChemicalProcessing
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
				public class CHEMICAL_AMMONIABREAKER
				{
					public static LocString NAME = FormatAsLink("Ammonia Breaker", nameof(CHEMICAL_AMMONIABREAKER));
					public static LocString DESC = "An advanced catalytic cracking furnace which the ammonia synthesis reaction is reversed at elevated temperatures.";
					public static LocString EFFECT = "Break down " + FormatAsLink("Ammonia", "AMMONIAGAS") + " into " + FormatAsLink("Hydrogen", "HYDROGEN")
						+ " and " + FormatAsLink("Nitrogen", "NITROGENGAS") + " using " + FormatAsLink("Iron", "IRON") + " as catalyst. " +
						"\nThe cracking process exudes a lot of heat.\n\nIMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate.";
				}
				public class CHEMICAL_AMMONIACOMPRESSOR
				{
					public static LocString NAME = FormatAsLink("Ammonia Compressor", nameof(CHEMICAL_AMMONIACOMPRESSOR));
					public static LocString DESC = "An industrial grade rotatory compressor unit that criticaly increase the pressure of a gas by reducing its volume, while cooling it down until liquid state is reached.";
					public static LocString EFFECT = "Compresses Ammonia gas and cool it down to liquid Ammonia. This device is also capable of storing liquid with complete insulation.";
				}
				public class CHEMICAL_BALLCRUSHERMILL
				{
					public static LocString NAME = FormatAsLink("Ball Crusher Mill (Chemical Washing)", nameof(CHEMICAL_BALLCRUSHERMILL));
					public static LocString DESC = "A large sized industrial mill that crushes raw ores using steel balls and special mixture of acids. Capable to process much more than the standard mill, as well more efficient in the extraction of valuable minerals from the raw more sludge.";
					public static LocString EFFECT = string.Concat("Crush down ", FormatAsLink("Raw Minerals", "RAWMINERAL"), " in to useful materials and industrial ingredients.");
				}

				public class CHEMICAL_CARBONDIOXIDECOMPRESSOR
				{
					public static LocString NAME = FormatAsLink("Carbon Dioxide Compressor", nameof(CHEMICAL_CARBONDIOXIDECOMPRESSOR));
					public static LocString DESC = "An industrial grade rotatory compressor unit that criticaly increase the pressure of a gas by reducing its volume, while cooling it down until liquid state is reached.";
					public static LocString EFFECT = "Compresses " + FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " gas and cool it down to" + FormatAsLink("Liquid Carbon Dioxide", "LIQUIDCARBONDIOXIDE") + ". This device is also capable of storing liquid with complete insulation.";
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
						FormatAsLink("Fullerene", "FULLERENE"), " and " +
						FormatAsLink("Rust", "RUST"), " as waste product.");

					public static LocString SPACEDOUT_RECIPE = string.Concat(
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

					public static LocString EFFECT = DlcManager.IsExpansion1Active() ? SPACEDOUT_RECIPE : BASEGAME_RECIPE;
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
					public static LocString NAME = FormatAsLink("Carbon fueled Steam Boiler", nameof(CHEMICAL_COAL_BOILER));
					public static LocString DESC = "An industrial grade boiler that generates thermal energy by burning solid fossil fuels.";
					public static LocString EFFECT = string.Concat("Boils ", FormatAsLink("Water", "WATER"), " to ", FormatAsLink("Steam", "STEAM"), " at 200 °C.\nThis particular boiler uses ", FormatAsLink("Combustustable Solids", "COMBUSTIBLESOLID"), " as fuel.");
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
						"The process requires ", FormatAsLink("Steam", "STEAM"), " for the operation.\n\n" +
						"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate.");

				}
				public class CHEMICAL_CRUDEOILREFINERYSTAGED
				{
					public static LocString NAME = FormatAsLink("Multi-Stage Crude Oil Refinery", nameof(CHEMICAL_CRUDEOILREFINERYSTAGED));
					public static LocString DESC = "An industrial process plant responsible for refining raw oil extracted from wells.";
					public static LocString EFFECT = "The refinery has two stages: \n\n" +
						"First Stage: " + FormatAsLink("Crude Oil", "CRUDEOIL") + " is first refined to:\n" +
						"- 50% " + FormatAsLink("Petroleum", "PETROLEUM") + "\n" +
						"- 25% " + FormatAsLink("Naphtha", "NAPHTHA") + "\n" +
						"- 10% " + FormatAsLink("Natural Gas", "METHANE") + "\n" +
						"- 15% " + FormatAsLink("Bitumen", "BITUMEN") + ".\n\n" +
						"Second Stage: " + FormatAsLink("Naphtha", "NAPHTHA") + " is furter refined:\n" +
						"- 45% " + FormatAsLink("Petroleum", "PETROLEUM") + "\n" +
						"- 10% " + FormatAsLink("Natural Gas", "METHANE") + "\n" +
						"- 45% " + FormatAsLink("Bitumen", "BITUMEN") + ".\n\n" +
						"The first stage uses " + FormatAsLink("Steam", "STEAM") + " for the distillation process, while the second Stage uses " + FormatAsLink("Hydrogen", "HYDROGEN") + " to buffer the reaction.";

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
				public class CHEMICAL_GAS_BOILER
				{
					public static LocString NAME = FormatAsLink("Gas-fueled Steam Boiler", nameof(CHEMICAL_GAS_BOILER));
					public static LocString DESC = "An industrial grade boiler that generates thermal energy by burning Combustible Gases.";
					public static LocString EFFECT = string.Concat(["Boils ", FormatAsLink("Water", "WATER"), " to ", FormatAsLink("Steam", "STEAM"), " at 200 °C. This particular boiler uses ", FormatAsLink("Combustible Gases", "COMBUSTIBLEGAS"), " as fuel, but may as well work with other combustible gases."]);
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

				public class CHEMICAL_MIXINGUNIT
				{
					public static LocString NAME = FormatAsLink("Chemical Mixing Unit", nameof(CHEMICAL_MIXINGUNIT));
					public static LocString DESC = "This chemical fabricator has several functions in petrochemical industry.";
					public static LocString EFFECT = "An industrial aparatus capable to address several chemical reactions. Its large array of pipes allows safe handling of dangerous liquids and gases.";
				}
				public class CHEMICAL_NAPHTHAREFORMER
				{
					public static LocString NAME = FormatAsLink("Naphtha Reformer", nameof(CHEMICAL_NAPHTHAREFORMER));
					public static LocString DESC = "An industrial petrochemical plant responsible for rearranging hydrocarbon molecules of Naphtha in to Petroleum.";
					public static LocString EFFECT = string.Concat(
						[
							"This second stage refinement plant is capable of furter refining ",FormatAsLink("Naphtha", "NAPHTHA"), ":\n "+
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
							"The process requires ",FormatAsLink("Steam", "STEAM")," for the operation.\n\n" +

							"IMPORTANT: The building require all pipes ports to be connected with their respective pipes in order for it to operate."

						]);
				}
				public class CHEMICAL_RAWGASREFINERYSTAGED
				{
					public static LocString NAME = FormatAsLink("Multi-Stage Raw Gas Refinery", nameof(CHEMICAL_RAWGASREFINERYSTAGED));
					public static LocString DESC = "An industrial process plant responsible for refining the impure raw natural gas extracted from wells.";
					public static LocString EFFECT = "The refinery has three stages:\n\n" +
						"First Stage: Raw Natural Gas is first refined to: " +
						"\n- 50% " + FormatAsLink("Natural Gas", "PETROLEUM") +
						"\n- 40% " + FormatAsLink("Propane", "PROPANE") +
						"\n- 10% " + FormatAsLink("Sour Gas", "SOURGAS") +
						"\nSecond Stage: " + FormatAsLink("Propane", "PROPANE") + " is mixed with " + FormatAsLink("Hydrogen", "Hydrogen") + " resulting in a complete conversion to " + FormatAsLink("Methane", "METHANE") +
						"\nThird Stage reacts the remaining " + FormatAsLink("Sour Gas", "SOURGAS") + " with Nitric Acid, producing Ammonia Gas.";

				}
				public class CHEMICAL_RAYONLOOM
				{
					public static LocString NAME = FormatAsLink("Rayon Loom", nameof(CHEMICAL_RAYONLOOM));
					public static LocString DESC = "A chemical loom capable of producing celulose fibers with Viscose process.";
					public static LocString EFFECT = string.Concat(
						[
							"Produces ",
							FormatAsLink("Rayon Fiber", "RAYONFIBER"),
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
					public static LocString EFFECT = string.Concat(
						[
							"Produces industrial grade ",
							FormatAsLink("Nitric Acid", "LIQUIDNITRIC"), " using provided " +
							FormatAsLink("Sulfuric Acid", "LIQUIDSULFURIC")," and " +
							FormatAsLink("Ammonia", "AMMONIAGAS"),"."
						]);
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
					public static LocString EFFECT = string.Concat(
						[
							"Produces industrial grade ",
							FormatAsLink("Sulfuric Acid", "LIQUIDSULFURIC"), " using provided " +
							FormatAsLink("Steam", "STEAM")," and " +
							FormatAsLink("Sulfur", "SULFUR"), "."
						]);
				}
				public class CHEMICAL_THERMALDESALINATOR
				{
					public static LocString NAME = FormatAsLink("Thermal Desalinator", nameof(CHEMICAL_THERMALDESALINATOR));
					public static LocString DESC = "A basic desalinator that uses Vapor-compression evaporation to archive the separation of clean, usable water from the solution it is currently mixed.";
					public static LocString EFFECT = string.Concat(
						[
							"Uses ",
							FormatAsLink("Steam", "STEAM"), " as buffer for the Desalination process of either " +
							FormatAsLink("Salt Water", "SALTWATER")," or Ammonium Water, releasing fresh " +
							FormatAsLink("Water", "WATER"), " and concentrated ",FormatAsLink("Brine", "BRINE"),
							" in return."
						]);
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
					public static LocString EFFECT = string.Concat(
						[
							"Special modifications allows the polymerization of ",
							FormatAsLink("Ethanol", "ETHANOL"),
							" into raw ",
							FormatAsLink("Plastic", "POLYPROPYLENE"),
							"with the addition of ",
							FormatAsLink("Chlorine Gas", "CHLORINE"),"."

						]);
				}
				public class CUSTOM_METALREFINERY
				{
					public static LocString NAME = FormatAsLink("[DEPRECATED] Custom Metal Refinery", nameof(CUSTOM_METALREFINERY));
					public static LocString DESC = global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.DESC;
					public static LocString EFFECT = string.Concat(
						[
							"THIS BUILDING HAS BEEN DEPRECATED AND CANNOT BE BUILT. INSTEAD, USE THE REGULAR " + FormatAsLink("METAL REFINERY", "METALREFINERY"),"\n\n",
							global::STRINGS.BUILDINGS.PREFABS.METALREFINERY.EFFECT

						]);
				}
				#endregion
				#region Metallurgy
				public class METALLURGY_PLASMAFURNACE
				{
					public static LocString NAME = FormatAsLink("Plasma Furnace", nameof(METALLURGY_PLASMAFURNACE));
					public static LocString DESC = "An advanced pyrometallurgical furnace that uses an extremely hot thermal plasma generated by a carrier gas jet. The high energy consumption is compensated by the quality and the yield of the refining process.";
					public static LocString EFFECT = string.Concat(
						[
							"An advanced method for the refinement of ",
							FormatAsLink("Metal Ores", "RAWMETAL"),
							" and other ",
							FormatAsLink("Raw Minerals", "RAWMINERAL"),
							".\n\nSignificantly ",
							FormatAsLink("Heats", "HEAT"),
							" and exclusively consumes ",
							FormatAsLink("Hydrogen", "HYDROGEN"),
							" as carrier gas.\n\nMain products are dispensed in molten state directly in the floor below the building while the liquid waste is released in a separated port."
						]);
				}
				public class METALLURGY_BALLCRUSHERMILL
				{
					public static LocString NAME = FormatAsLink("Ball Crusher Mill (Mechanical Separation)", nameof(METALLURGY_BALLCRUSHERMILL));
					public static LocString DESC = "A large sized industrial mill that crushes raw ores using steel balls and special vibrating separation device to sort out possible useful resources.";
					public static LocString EFFECT = string.Concat("Crush down ", FormatAsLink("Raw Minerals", "RAWMINERAL"), " in to useful materials and industrial ingredients.");
				}
				public class METALLURGY_BASICOILREFINERY
				{
					public static LocString NAME = FormatAsLink("Basic Oil Refinery", nameof(METALLURGY_BASICOILREFINERY));
					public static LocString DESC = "A basic oil refinery that uses burning solids as heat source.";
					public static LocString EFFECT = string.Concat(
						[
						"Refines "
						,FormatAsLink("Crude Oil", "CRUDEOIL")," to "
						,FormatAsLink("Petroleum", "PETROLEUM"), " using "
						,FormatAsLink("Coal", "CARBON"), " as heat source. \nThe refinement process produces "
						,FormatAsLink("Natural Gas", "METHANE"), " and "
						,FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE"),".\n\n" +
						"IMPORTANT: The Gases output ports piping is optional. If no Gas Pipe is attached to them, the output gases will be released directly on the environment."
						]);
				}
				public class METALLURGY_PYROLYSISKILN
				{
					public static LocString NAME = FormatAsLink("Pyrolysis Kiln", nameof(METALLURGY_PYROLYSISKILN));
					public static LocString DESC = "A basic kiln that uses pyrolysis process to convert woodlogs to usable coal.";
					public static LocString EFFECT = string.Concat(
						[
							"Cook ",
							global::STRINGS.ELEMENTS.WOODLOG.NAME,
							" to ",
							FormatAsLink("Coal", "CARBON"),
							"."
						]);
				}
				#endregion
				#region Mining
				public class MINING_CNCMACHINE
				{
					public static LocString NAME = FormatAsLink("CNC Machining Station", nameof(MINING_CNCMACHINE));
					public static LocString DESC = "An advanced CNC (Computer Numerical Control) Machining production station. Capable of managing different tools using specific directives by graphical computer-aided design software.";
					public static LocString EFFECT = "Capable of producing specialty gadgets and other advanced tools. Requires the Mechatronics Engineering knowledge skill to be operated.";
				}
				public class MINING_AUGERDRILL
				{
					public static LocString NAME = FormatAsLink("Asteroid Drill Rig Mk-II", nameof(MINING_AUGERDRILL));
					public static LocString DESC = "A giant machinery engineered in the old world for asteroid mining. Has a self propelled drilling head fitted with a sensorial system and actuators that helps it to travel through the asteroid crust in search for resources of interest.  ";
					public static LocString EFFECT = string.Concat(
						[
							"This rig uses specialized Drillbits to mine useful resources from the asteroid crust. Retrived resources are released upon the Drill Head return.\n"+
							"Require a constant supply of ",FormatAsLink("Combustable Fuel", "COMBUSTIBLELIQUID")
							+" to function.\n"+
							"Results: resources that are delivered upon Drillhead return.\n"+
							"Occurrence: resources that are generated during Drillhead operation."
						]);
				}
				public class MINING_MINERALDRILL
				{
					public static LocString NAME = FormatAsLink("Asteroid Auger Drill", nameof(MINING_MINERALDRILL));
					public static LocString DESC = "Drills into Deep Mineral Deposits to extract solid resources.";
					public static LocString EFFECT = "A strong drilling apparatus engineered with self guidance sensors for mineral ores, and retrieval of resources while operational.\nThe type of the drill head determines the dept of the drilling, retriving resources from other parts of the Asteroid that are inaccessible by normal means.\nThe drill sensors have a limited range, and retrieval of useful ores are not guaranteed.";
				}

				#endregion
				#region DupesMachinery
				/// <summary>
				/// Dupes machinery is not namespaced
				/// </summary>
				public class FLOCCULATIONSIEVE
				{
					public static LocString NAME = FormatAsLink("Flocculation Sieve", nameof(FLOCCULATIONSIEVE));
					public static LocString DESC = "A dedicated industrial sieve that flocculates colloidal particles out of suspension to sediment under the form of floc. Strong filter media further improves the cleaning process of liquids.";
					public static LocString EFFECT = string.Concat(
					[
						"Treat ",
						FormatAsLink("Polluted Water", "DIRTYWATER"), " or " +FormatAsLink("Toxic Slurry", "TOXICSLURRY"),
						" using special filter and chemicals. " +
						"Sieve process also completely removes", FormatAsLink("Germs", "DISEASE"),"."
					]);
				}
				public class SLIMEVAT
				{
					public static LocString NAME = FormatAsLink("Slime Vat", nameof(SLIMEVAT));
					public static LocString DESC = "An advanced biological vat that grows a culture of mucopolysaccharides and multicelular fungi mould. This unit also uses advanced filter to extract Polluted Oxygen from its surroundings, albeit its not necessery for it to function. ";
					public static LocString EFFECT = string.Concat(["Slime Vat needs ", FormatAsLink("Water", "WATER"), " and ", FormatAsLink("Mush Bar", "MUSHBAR"), " to grow, producing ", FormatAsLink("Slime", "SLIMEMOLD"), " outgrowth that is collected from the botton. Due to its nature, the slime its produce will be contaminated with Slimelung."]);

				}
				public class CORALVAT
				{
					public static LocString NAME = FormatAsLink("Coral Vat", nameof(CORALVAT));
					public static LocString DESC = "An advanced biological vat that grows a special colony of marine invertebrates, engineered from the Earth species of the class Anthozoa. This unit also uses advanced filter to extract Chlorine Gas from its surroundings, albeit its not necessery for it to function.";
					public static LocString EFFECT = string.Concat(["Coral colony needs ", FormatAsLink("Salt Water", "SALTWATER"), " or ", FormatAsLink("Brine", "BRINE"), " to grow, producing a fair amount of clean ", FormatAsLink("Water", "WATER"), " as result of its biological functions. The coral colony will also excreate tiny particles of ", FormatAsLink("Bleach Stone", "BLEACHSTONE"), ", which are filtered from the water and later released as a solid mass."]);

				}
				public class ALGAEVAT
				{
					public static LocString NAME = FormatAsLink("Algae Vat", nameof(ALGAEVAT));
					public static LocString DESC = "An advanced biological vat that grows algae. Due to the controlled atmospheric condition, this algae formation produces oxygen more efficiently than the standard terrarium. This unit also uses advanced filter to extract Carbon Dioxide from its surroundings, albeit its not necessery for it to function. ";
					public static LocString EFFECT = string.Concat(["Algae patch needs ", FormatAsLink("Water", "WATER"), " to grow, producing a fair amount of clean ", FormatAsLink("Oxygen", "OXYGEN"), " as result of its biological functions. Excess water is expelled in the form of ", FormatAsLink("Polluted Water", "DIRTYWATER"), "."]);

				}
				public class ETHANOLSTILL
				{
					public static LocString NAME = FormatAsLink("Ethanol Stil", nameof(ETHANOLSTILL));
					public static LocString DESC = "A solid metal still capable of distillation by selective temperature.";
					public static LocString EFFECT = string.Concat(["Distills ", FormatAsLink("Ethanol", "ETHANOL"), " from a fermented mixture of ", FormatAsLink("Sucrose", "SUCROSE"), " and ", FormatAsLink("Water", "WATER"), ". The fermenting process occurs with ", FormatAsLink("Slime", "SLIMEMOLD"), " bacterias by the degradation of organic nutrients anaerobically."]);
				}
				#endregion
				#region NuclearProcessing
				/// <summary>
				/// nuclear processing is not namespaced
				/// </summary>
				public class LIGHTREACTOR
				{
					public static LocString NAME = FormatAsLink("ABWR Reactor", nameof(LIGHTREACTOR));
					public static LocString DESC = "The advanced boiling water reactor (ABWR) uses non-pressurized coolant for steam production. Its comparative small size favors for a quicker heating of the coolant, however, it also increases the odds of a Meltdown in the absence of cooling. Releases Steam in the area directly below its central tile segment.";
					public static LocString EFFECT = string.Concat(
					[
						"Uses ",
						FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM"),
						" to produce ",
						FormatAsLink("Steam", "STEAM"),
						" for electrical purpose."
						]);
				}
				public class HEPPROJECTOR
				{
					public static LocString NAME = FormatAsLink("Rad Projector", nameof(HEPPROJECTOR));
					public static LocString DESC = "A radiactive source containing radionuclids which emits ionizing radiation.";
					public static LocString EFFECT = string.Concat(
					[
							"This device uses " + FormatAsLink("Uranium Ore", "URANIUMORE"),
							". As the fuel decays, it turns to liquid " + FormatAsLink("Nuclear Waste", "NUCLEARWASTE"),
							", which is piped out of the source."
						]);
				}
				public class HEPCENTRIFUGE
				{
					public static LocString NAME = FormatAsLink("Zippe-Type Centrifuge", nameof(HEPCENTRIFUGE));
					public static LocString DESC = "A gas centrifuge is a device that performs isotope separation of gases. A centrifuge relies on the principles of centripetal force accelerating molecules so that particles of different masses are physically separated in a gradient along the radius of a rotating container.";
					public static LocString EFFECT = string.Concat(
					[
						"The Zippe-type centrifuge is a gas device designed to enrich the rare fissile " + FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM"),
						" from the mixture of isotopes found in the manufactured ",
						FormatAsLink("Yellow Cake", "YELLOWCAKE"),
						". The separation process releases small amounts of ",
						FormatAsLink("Depleted Uranium", "DEPLETEDURANIUM"),
						"as waste product."
						]);
				}
				public class HEPCALCINATOR
				{
					public static LocString NAME = FormatAsLink("Voloxidation Calcinator", nameof(HEPCALCINATOR));
					public static LocString DESC = "Voloxidation process can separate uranium oxides from nuclear waste, along with other heavy elements. Produces waves of contaminating radiation while operational.";
					public static LocString EFFECT = "An advanced kiln capable of volumetric oxidation with heavy nuclear waste by alternating different stages of oxidation and reduction.";
				}
				#endregion
				#region DupesEngineering
				public class AIO_FACILITYDOOR
				{
					public static LocString NAME = FormatAsLink("Facility Door", nameof(AIO_FACILITYDOOR));
					public static LocString DESC = "A light-weight door with intricate designs that suggests it bellongs to a industrial facility.";
					public static LocString EFFECT = "A high-tech light door";
					public class FACADES
					{
						public class FACILITYDOORWHITE
						{
							public static LocString NAME = FormatAsLink("White Facility Door", nameof(FACILITYDOORWHITE));
							public static LocString DESC = "A high-tech light door with white tint.";
						}
						public class FACILITYDOORYELLOW
						{
							public static LocString NAME = FormatAsLink("Yellow Facility Door", nameof(FACILITYDOORYELLOW));
							public static LocString DESC = "A high-tech light door with yellow tint.";
						}
						public class FACILITYDOORRED
						{
							public static LocString NAME = FormatAsLink("Red Facility Door", nameof(FACILITYDOORRED));
							public static LocString DESC = "A high-tech light door with red tint.";
						}
					}
				}
				public class LOGICALERTLIGHT
				{
					public static LocString NAME = FormatAsLink("Alert LED", nameof(LOGICALERTLIGHT));
					public static LocString DESC = "A white colored alert light.";
					public static LocString EFFECT = "A led light that servers as alert. Produces no significant luminosity.";
					public class FACADES
					{
						public class LOGICALERTLIGHTRED
						{
							public static LocString NAME = FormatAsLink("Red Alert LED", nameof(LOGICALERTLIGHTRED));
							public static LocString DESC = "A red colored alert light.";
						}
						public class LOGICALERTLIGHTYELLOW
						{
							public static LocString NAME = FormatAsLink("Yellow Alert LED", nameof(LOGICALERTLIGHTYELLOW));
							public static LocString DESC = "A yellow colored alert light.";
						}
						public class LOGICALERTLIGHTGREEN
						{
							public static LocString NAME = FormatAsLink("Green Alert LED", nameof(LOGICALERTLIGHTGREEN));
							public static LocString DESC = "A green colored alert light.";
						}
					}
				}
				public class GLASSDOORCOMPLEX
				{
					public static LocString NAME = FormatAsLink("Security Glass Door", nameof(GLASSDOORCOMPLEX));
					public static LocString DESC = "Functions as a Manual Airlock when no Power is available.";
					public static LocString EFFECT = "A mechanized airlock door made with " + FormatAsLink("Glass", "GLASS") + " panels. Blocks Liquid and Gas flow, maintaining pressure between areas. Sets Duplicant Access Permissions for area restriction.";
				}
				public class GLASSDOORSIMPLE
				{
					public static LocString NAME = FormatAsLink("Simple Glass Door", nameof(GLASSDOORSIMPLE));
					public static LocString DESC = "Wild Critters cannot pass through doors. Door controls can be used to prevent Duplicants from entering restricted areas.";
					public static LocString EFFECT = "A simple door made with " + FormatAsLink("Glass", "GLASS") + " panels. Encloses areas without blocking Liquid or Gas flow. Sets Duplicant Access Permissions for area restriction.";
				}
				public class WOODENDOOR
				{
					public static LocString NAME = FormatAsLink("Wooden Door", nameof(WOODENDOOR));
					public static LocString DESC = "Be careful with splinters!";
					public static LocString EFFECT = "A pretty wooden door that encloses areas without blocking Liquid or Gas flow. Sets Duplicant Access Permissions for area restriction.";
				}
				public class CEMENTMIXER
				{
					public static LocString NAME = FormatAsLink("Cement Mixer", nameof(CEMENTMIXER));
					public static LocString DESC = "Cement is a quite old building material, but still pretty much useful.";
					public static LocString EFFECT = "A device that can homogeneously combine several solid and liquid ingredients used in the production of cement.";
				}
				public class AIO_MOSAICTILE
				{
					public static LocString NAME = FormatAsLink("Mosaic Tile", nameof(AIO_MOSAICTILE));
					public static LocString DESC = "A fine tile made from glazed stones.";
					public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nSignificantly increases Duplicant runspeed.";
				}
				public class MARBLETILESTRINGS
				{
					public static LocString NAME = FormatAsLink("Marble Tile", nameof(MOULDINGTILE));
					public static LocString DESC = "A fine tile made from polished quality stones.";
					public static LocString EFFECT = "Used as floor and wall tile to build rooms.\n\nSignificantly increases Duplicant runspeed.";
				}
				public class MONOELEMENTTILE
				{
					public static LocString NAME = FormatAsLink("Compacted Tile", nameof(MONOELEMENTTILE));
					public static LocString DESC = "A fine tile made from compacted stones.";
					public static LocString EFFECT = "A tile composed out of compacted minerals\n\nIncreases Duplicant runspeed.";
				}
				public class CUSTOMGRANITETILE
				{
					public static LocString NAME = FormatAsLink("Compacted Granite Tile", nameof(CUSTOMGRANITETILE));
					public static LocString DESC = "Granite tiles are aesthetically pleasing while remaining a good insulator.";
					public static LocString EFFECT = "A smooth tile made with several granite stones carefully placed together.";
				}
				public class CUSTOMIGNEOUSROCKTILE
				{
					public static LocString NAME = FormatAsLink("Compacted Igneous Tile", nameof(CUSTOMIGNEOUSROCKTILE));
					public static LocString DESC = "A notable tile made exclusively with Igneous Rock.";
					public static LocString EFFECT = "Igneous tiles enhance not only the environmental beauty, but also increases duplicants walking speed.";
				}
				public class CUSTOMOBSIDIANTILE
				{
					public static LocString NAME = FormatAsLink("Compacted Obsidian Tile", nameof(CUSTOMOBSIDIANTILE));
					public static LocString DESC = "A notable tile made exclusively with Obsidian, a form of volcanic glass.";
					public static LocString EFFECT = "Obsidian tiles enhance not only the environmental beauty, but also increases duplicants walking speed.";
				}
				public class CUSTOMSANDSTONETILE
				{
					public static LocString NAME = FormatAsLink("Compacted Sandstone Tile", nameof(CUSTOMSANDSTONETILE));
					public static LocString DESC = "A notable tile made exclusively with Sandstone.";
					public static LocString EFFECT = "Sandstone tiles enhance not only the environmental beauty, but also increases duplicants walking speed.";
				}
				public class CUSTOMBRICKTILE
				{
					public static LocString NAME = FormatAsLink("Compacted Brick Tile", nameof(CUSTOMBRICKTILE));
					public static LocString DESC = "A notable tile made exclusively out of Bricks.";
					public static LocString EFFECT = "A smooth tile made with several bricks carefully placed together.";
				}
				public class REINFORCEDCONCRETETILE
				{
					public static LocString NAME = FormatAsLink("Reinforced Concrete Tile", nameof(REINFORCEDCONCRETETILE));
					public static LocString DESC = "Concrete tiles are not aesthetically pleasing, but they make up for strength and thermal resistance.";
					public static LocString EFFECT = "A rock hard composite tile made with coarse aggregate bonded together with a fluid cement.\nThe concrete's relatively low tensile strength and ductility are compensated for by the inclusion of a internal rebar reinforcement having higher tensile strength or ductility.";
				}
				public class STRUCTURETILE
				{
					public static LocString NAME = FormatAsLink("Structure Tile", nameof(REINFORCEDCONCRETETILE));
					public static LocString DESC = "A solid structural tile wrought from refined metal. Use to build the walls and floors of rooms.";
					public static LocString EFFECT = "This steel structure is commonly used as a simple, yet strong tile for buildings. The frame structure will not hold any gas or liquid.";
				}
				public class WOODENCOMPOSITIONTILE
				{
					public static LocString NAME = FormatAsLink("Wood Composite Tile", nameof(WOODENCOMPOSITIONTILE));
					public static LocString DESC = "Tiles composed of minerals and wooden adornments.";
					public static LocString EFFECT = "Used to build the walls and floors of rooms. Increases Decor, contributing to Morale.\nIts composition gives it excellent insulation.";
				}
				public class WOODENGASTILE
				{
					public static LocString NAME = FormatAsLink("Wooden Airflow Tile", nameof(WOODENGASTILE));
					public static LocString DESC = "Building with wooden airflow tiles promotes better gas circulation within a colony.";
					public static LocString EFFECT = "A semipermeable wooden tile, used to build walls and floors of rooms. \n\nBlocks Liquid flow without obstructing Gas.";
				}
				public class WOODENMESHTILE
				{
					public static LocString NAME = FormatAsLink("Wooden Mesh Tile", nameof(WOODENMESHTILE));
					public static LocString DESC = "Building with hollow wooden tiles promotes better gas circulation within a colony.";
					public static LocString EFFECT = "A permeable wooden tile, used to build walls and floors of rooms. \n\nAllows the flow of both liquid and Gas.";
				}
				public class SPACERTILESOLID
				{
					public static LocString NAME = FormatAsLink("Solid Spacer Tile", nameof(SPACERTILESOLID));
					public static LocString DESC = "A durable tile made for vacuum-exposed environments.";
					public static LocString EFFECT = "This solid tile is specially engineered to withstand the harsh conditions of space.\n\nBlocks Gas and Liquid while offering minimal thermal insulation.";
				}
				public class SPACERTILEWINDOW
				{
					public static LocString NAME = FormatAsLink("Transparent Spacer Tile", nameof(SPACERTILEWINDOW));
					public static LocString DESC = "A transparent tile designed for external observation in vacuum-exposed environments.";
					public static LocString EFFECT = "This windowed tile allows light and visibility through while maintaining a strong barrier against the vacuum of space.\n\nBlocks Gas and Liquid while offering minimal thermal insulation.";
				}
				public class INSULATIONCOMPOSITIONTILE
				{
					public static LocString NAME = FormatAsLink("Insulation Composite Tile", nameof(INSULATIONCOMPOSITIONTILE));
					public static LocString DESC = "Used to build the walls and floors of rooms. Reduces heat transfer between walls, retaining ambient heat in an area.";
					public static LocString EFFECT = "A solid tile assembled in a range of materials and radiative pattern that reduce the heat transfer.";
				}
				public class WOODENCEILING
				{
					public static LocString NAME = "Wooden Ceiling";
					public static LocString DESC = "Wood work used to decorate the ceilings of rooms. Increases Decor, contributing to Morale.";
					public static LocString EFFECT = "This ceiling is a beautiful masonry and wood work, but serves only as a decorative purpose.";
				}

				public class WOODENCORNERARCH
				{
					public static LocString NAME = "Wooden Corner Arch";
					public static LocString DESC = "A wooden arch used to decorate the ceiling corners of rooms. Increases Decor, contributing to Morale.";
					public static LocString EFFECT = "This corner ceiling arch is beautiful masonry and wood work, but serves only as a decorative purpose.";
				}

				public class SPACERWALL
				{
					public static LocString NAME = FormatAsLink("Spacer Wall", nameof(SPACERWALL));
					public static LocString DESC = "A small sized spacer wall.";
					public static LocString EFFECT = "A solid wall panel wrought from steel. Hermetically sealed joints prevent gas leakage into space.";
					public class FACADES
					{
						public class SPACERDANGER
						{
							public static LocString NAME = FormatAsLink("Spacer Perimeter Wall", nameof(SPACERDANGER));
							public static LocString DESC = "This spacer wall is fitted with a perimeter stripped demarcation. Can be flipped around.";
						}
						public class SPACERDANGERCORNER
						{
							public static LocString NAME = FormatAsLink("Spacer Perimeter Wall Corner", nameof(SPACERDANGERCORNER));
							public static LocString DESC = "This spacer wall is fitted with a corner perimeter stripped demarcation destined to corners. Can be flipped around.";
						}
						public class SPACERPANEL
						{
							public static LocString NAME = FormatAsLink("Spacer Panel", nameof(SPACERPANEL));
							public static LocString DESC = "A small sized spacer panel.";
						}
					}
				}
				public class SPACERWINDOWWALL
				{
					public static LocString NAME = FormatAsLink("Spacer Window", nameof(SPACERWINDOWWALL));
					public static LocString DESC = "A small sized spacer window..";
					public static LocString EFFECT = "A solid wall wrought from steel and fitted with thick layers of glass. Hermetically sealed joints prevent gas leakage into space.";
					public class FACADES
					{
						public class SPACERWINDOW_B
						{
							public static LocString NAME = FormatAsLink("Spacer Window Alt.", nameof(SPACERWINDOW_B));
							public static LocString DESC = "A small sized spacer window. This is the B model.";
						}
					}
				}
				public class SPACERWALLLARGE
				{
					public static LocString NAME = FormatAsLink("Large Spacer Panel", nameof(SPACERWALLLARGE));
					public static LocString DESC = "A large sized spacer panel.";
					public static LocString EFFECT = "A large solid wall wrought from steel. Hermetically sealed joints prevent gas leakage into space.";
				}
				public class SPACERWINDOWLARGE
				{
					public static LocString NAME = FormatAsLink("Large Spacer Window", nameof(SPACERWINDOWLARGE));
					public static LocString DESC = "A large sized spacer window.";
					public static LocString EFFECT = "A solid window wrought from steel and fitted with thick layers of glass. Hermetically sealed joints prevent gas leakage into space.";
				}

				#endregion
				#region CustomReservoirs
				public class SMALLGASRESERVOIRDEFAULT
				{
					public static LocString NAME = FormatAsLink("Small Gas Reservoir", nameof(SMALLGASRESERVOIRDEFAULT));
					public static LocString DESC = "A small sized reservoir. Reservoirs cannot receive manually delivered resources.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
				}
				public class SMALLGASRESERVOIR
				{
					public static LocString NAME = FormatAsLink("Small Gas Reservoir B", nameof(SMALLGASRESERVOIR));
					public static LocString DESC = "A small sized reservoir. This variant has top to bottom flow.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
				}
				public class INVERTEDSMALLGASRESERVOIR
				{
					public static LocString NAME = FormatAsLink("Small Gas Reservoir A", nameof(INVERTEDSMALLGASRESERVOIR));
					public static LocString DESC = "A small sized reservoir. This variant has bottom to top flow.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
				}
				public class WALLGASTANK
				{
					public static LocString NAME = FormatAsLink("Wall Gas Reservoir", nameof(WALLGASTANK));
					public static LocString DESC = "A small sized reservoir designed for all placements, which does not require any foundation.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
				}
				public class SMALLLIQUIDRESERVOIRDEFAULT
				{
					public static LocString NAME = FormatAsLink("Small Liquid Reservoir", nameof(SMALLLIQUIDRESERVOIRDEFAULT));
					public static LocString DESC = "A small sized reservoir. Reservoirs cannot receive manually delivered resources.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
				}
				public class SMALLLIQUIDRESERVOIR
				{
					public static LocString NAME = FormatAsLink("Small Liquid Reservoir B", nameof(SMALLLIQUIDRESERVOIR));
					public static LocString DESC = "A small sized reservoir. This variant has top to bottom flow";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
				}
				public class INVERTEDSMALLLIQUIDRESERVOIR
				{
					public static LocString NAME = FormatAsLink("Small Liquid Reservoir A", nameof(INVERTEDSMALLLIQUIDRESERVOIR));
					public static LocString DESC = "A small sized reservoir. This variant has bottom to top flow";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
				}
				public class WALLLIQUIDTANK
				{
					public static LocString NAME = FormatAsLink("Wall Liquid Reservoir", nameof(WALLLIQUIDTANK));
					public static LocString DESC = "A small sized reservoir designed for all placements, which does not require any foundation.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
				}
				public class MEDGASRESERVOIR
				{
					public static LocString NAME = FormatAsLink("Medium Gas Reservoir", nameof(MEDGASRESERVOIR));
					public static LocString DESC = "A medium sized reservoir with double amount of input and output ports.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
				}
				public class MEDLIQUIDRESERVOIR
				{
					public static LocString NAME = FormatAsLink("Medium Liquid Reservoir", nameof(MEDLIQUIDRESERVOIR));
					public static LocString DESC = "A medium sized reservoir with double amount of input and output ports.";
					public static LocString EFFECT = "Stores any " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
				}
				#endregion
				#region DupesLogistics
				public class CABINETFROZEN
				{
					public static LocString NAME = FormatAsLink("Refrigerated Storage Cabinet", nameof(CABINETFROZEN));
					public static LocString DESC = "Store solids and other industrial materials at a constant temperature moderation.";
					public static LocString EFFECT = "Store the " +
						 FormatAsLink("Solid Materials", "ELEMENTS_SOLID") +
						 " of your choosing.\n\nConsumes " +
						 FormatAsLink("Power", "POWER") +
						 " to cool the contents down to 24°C.\n\nCannot store" +
						 " any liquefiable solids.\nTakes items from conveyor rails based on its filters.";
				}
				public class CABINETNORMAL
				{
					public static LocString NAME = FormatAsLink("Insulated Storage Cabinet", nameof(CABINETNORMAL));
					public static LocString DESC = "Store solids and other industrial materials at a constant temperature moderation.";
					public static LocString EFFECT = "Store the "+FormatAsLink("Solid Materials", "ELEMENTS_SOLID")+ " of your choosing.\nThe cabinet prevents temperature exchange with the environment, as well gas leakage.\nTakes items from conveyor rails based on its filters.";
				}
				public class STORAGEPOD
				{
					public static LocString NAME = FormatAsLink("Storage Pod", nameof(STORAGEPOD));
					public static LocString DESC = "Store solids and other industrial materials.";
					public static LocString EFFECT = "A versatile and convenient storage pod that can be built on walls. Store the " + FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " of your choosing.\nThe pod prevents temperature exchange with the environment, as well gas leakage.";
					public class FACADES
					{
						public class STORAGEPOD_A
						{
							public static LocString NAME = FormatAsLink("Green Storage Pod", nameof(STORAGEPOD_A));
							public static LocString DESC = "A green colored storage pod.";
						}
						public class STORAGEPOD_B
						{
							public static LocString NAME = FormatAsLink("Brown Storage Pod", nameof(STORAGEPOD_B));
							public static LocString DESC = "A brown colored storage pod.";
						}
						public class STORAGEPOD_C
						{
							public static LocString NAME = FormatAsLink("Yellow Storage Pod", nameof(STORAGEPOD_C));
							public static LocString DESC = "A yellow colored storage pod.";
						}
					}
				}
				public class LOGISTICBRIDGE
				{
					public static LocString NAME = FormatAsLink("Logistic Solid Bridge", nameof(LOGISTICBRIDGE));
					public static LocString DESC = "Separating rail systems helps ensure materials go to the intended destinations.";
					public static LocString EFFECT = "Runs one " + FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
				}
				public class LOGISTICFILTER
				{
					public static LocString NAME = FormatAsLink("Logistic Solid Filter", nameof(LOGISTICFILTER));
					public static LocString DESC = "All solids are sent into the building's output conveyor, except the solid chosen for filtering.";
					public static LocString EFFECT = "Separates one " + FormatAsLink("Solid Material", "ELEMENTS_SOLID") + " from the conveyor, sending it into a dedicated solid conduit.";
				}
				public class LOGISTICLOADER
				{
					public static LocString NAME = FormatAsLink("Logistic Loader", nameof(LOGISTICLOADER));
					public static LocString DESC = "Material filters can be used to determine what resources are sent down the rail.";
					public static LocString EFFECT = "Loads " +
							FormatAsLink("Solid Materials", "ELEMENTS_SOLID") +
							" onto " +
							FormatAsLink("Logistic Conveyor Rails", "LOGISTICRAIL") +
							" for transport.\n\nOnly loads the resources of your choosing.";
				}
				public class LOGISTICOUTBOX
				{
					public static LocString NAME = FormatAsLink("Logistic Receptacle", nameof(LOGISTICOUTBOX));
					public static LocString DESC = "When materials reach the end of a rail they enter a receptacle to be used by Duplicants.";
					public static LocString EFFECT =
							"Unloads " +
							FormatAsLink("Solid Materials", "ELEMENTS_SOLID")+
							" from a "+
							FormatAsLink("Logistic Rail", "LOGISTICRAIL") +
							" into storage.";
				}
				public class LOGISTICRAIL
				{
					public static LocString NAME = FormatAsLink("Logistic Rail", nameof(LOGISTICRAIL));
					public static LocString DESC = "Rails move materials where they'll be needed most, saving Duplicants the walk.";
					public static LocString EFFECT = string.Concat(
						[
							"Transports small amounts of ",
							FormatAsLink("Solid Materials", "ELEMENTS_SOLID"),
							" on a track between ",
							FormatAsLink("Logistic Loader", "LOGISTICLOADER"),
							" and ",
							FormatAsLink("Logistic Receptacle", "LOGISTICOUTBOX"),
							".\n\nCan be run through wall and floor tile."
						]);
				}
				public class LOGISTICTRANSFERARM
				{
					public static LocString NAME = FormatAsLink("Logistic Auto-Sweeper", nameof(LOGISTICTRANSFERARM));
					public static LocString DESC = "An auto-sweeper's range can be viewed at any time by clicking on the building.";
					public static LocString EFFECT = string.Concat(
						[
							"Automates ",
							FormatAsLink("Sweeping", "CHORES"),
							" and ",
							FormatAsLink("Supplying", "CHORES"),
							" errands by sucking up all nearby ",
							FormatAsLink("Debris", "DECOR"),
							".\n\nMaterials are automatically delivered to any ",
							FormatAsLink("Logistic Loader", "LOGISTICLOADER"),
							", ",
							FormatAsLink("Logistic Receptacle", "LOGISTICOUTBOX"),
							", storage, or buildings within range."
						]);
				}
				public class LOGISTICVENT
				{
					public static LocString NAME = FormatAsLink("Logistic Chute", nameof(LOGISTICVENT));
					public static LocString DESC = "When materials reach the end of a rail, they are dropped back into the world.";
					public static LocString EFFECT = string.Concat(
						[
							"Unloads ",
							FormatAsLink("Solid Materials", "ELEMENTS_SOLID"),
							" from a ",
							FormatAsLink("Logistic Rail", "LOGISTICRAIL"),
							" onto the floor."
						]);
				}

				public class LOGISTICRAILVALVE
				{
					public static LocString NAME = FormatAsLink("Logistic Reduction Valve", nameof(LOGISTICRAILVALVE));
					public static LocString DESC = "A mechanical valve capable of reducing the flow of mass from a conveyor rail to to a logistic rail, to avoid dropping items.";
					public static LocString EFFECT = "Allows " +
					FormatAsLink("Solid Materials", "ELEMENTS_SOLID") +
					" to be transfered from normal " +
					FormatAsLink("Conveyor Rails", "SOLIDCONDUIT") +
					" to " +
					FormatAsLink("Logistic Rails", "LOGISTICRAIL") +
					".";
				}
				#endregion
				#region HighPressureApplications

				public class DECOMPRESSIONGASVALVE
				{
					public static LocString NAME = FormatAsLink("Decompression Gas Valve", nameof(DECOMPRESSIONGASVALVE));
					public static LocString DESC = "A mechanical valve capable of reducing the flow of gas from a pressurized pipe to a normal pipe, avoid it to break.";
					public static LocString EFFECT = "Allows " +
					FormatAsLink("Gases", "ELEMENTS_GAS") +
					" to be transfered from " +
					FormatAsLink("High Pressure Gas Pipe", HighPressureGasConduitConfig.ID) +
					" to normal " +
					FormatAsLink("Pipes", "GASCONDUIT") +
					".";
				}
				public class DECOMPRESSIONLIQUIDVALVE
				{
					public static LocString NAME = FormatAsLink("Decompression Liquid Valve", nameof(DECOMPRESSIONLIQUIDVALVE));
					public static LocString DESC = "A mechanical valve capable of reducing the flow of liquid from a pressurized pipe to a normal pipe, avoid it to break.";
					public static LocString EFFECT = string.Concat(
						[
							"Allows ",
							FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
							" to be transfered from ",
							FormatAsLink("High Pressure Liquid Pipe", HighPressureLiquidConduitConfig.ID),
							" to normal ",
							FormatAsLink("Pipes", "LIQUIDCONDUIT"),
							"."
						]);
				}
				public class HIGHPRESSUREGASCONDUITBRIDGE
				{
					public static LocString NAME = FormatAsLink("High Pressure Gas Conduit Bridge", nameof(HIGHPRESSUREGASCONDUITBRIDGE));
					public static LocString DESC = "A reinforced gas pipe bridge capable of handling high pressure flow.\nComposite nature of the pipe prevents gas contents from significantly changing temperature in transit.";
					public static LocString EFFECT = "Runs one High Pressure Gas Pipe section over another without joining them.\n\nCan be run through wall and floor tile.";
				}
				public class HIGHPRESSUREGASCONDUIT
				{
					public static LocString NAME = FormatAsLink("High Pressure Gas Conduit", nameof(HIGHPRESSUREGASCONDUIT));
					public static LocString DESC = "A reinforced gas pipe capable of handling high pressure flow.\nComposite nature of the pipe prevents gas contents from significantly changing temperature in transit.";
					public static LocString EFFECT = string.Concat(
						[
							"Carries a maximum of " +
							Config.Instance.HPA_Capacity_Gas,
							"kg of ",
							FormatAsLink("Gas", "ELEMENTS_GAS"),
							" with minimal change in ",
							FormatAsLink("Temperature", "HEAT"),
							".\n\nCan be run through wall and floor tile."
						]);
				}
				public class HIGHPRESSURELIQUIDCONDUITBRIDGE
				{
					public static LocString NAME = FormatAsLink("High Pressure Liquid Conduit Bridge", nameof(HIGHPRESSURELIQUIDCONDUITBRIDGE));
					public static LocString DESC = "A reinforced liquid pipe bridge capable of handling high pressure flow. Composite nature of the pipe prevents liquid contents from significantly changing temperature in transit.";
					public static LocString EFFECT = "Runs one High Pressure Liquid Pipe section over another without joining them.\n\nCan be run through wall and floor tile.";
				}
				public class HIGHPRESSURELIQUIDCONDUIT
				{
					public static LocString NAME = FormatAsLink("High Pressure Liquid Pipe", nameof(HIGHPRESSURELIQUIDCONDUIT));
					public static LocString DESC = "A reinforced liquid pipe capable of handling high pressure flow. Composite nature of the pipe prevents liquid contents from significantly changing temperature in transit.";
					public static LocString EFFECT = string.Concat(
						[
							"Carries a maximum of " +
							Config.Instance.HPA_Capacity_Liquid,
							"kg of ",
							FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
							" with minimal change in ",
							FormatAsLink("Temperature", "HEAT"),
							".\n\nCan be run through wall and floor tile."
						]);
				}
				public class PRESSUREGASPUMP
				{
					public static LocString NAME = FormatAsLink("High Pressure Gas Pump", nameof(PRESSUREGASPUMP));
					public static LocString DESC = "An advanced pump that perform mechanical work to compress and move gases. More powerful than the standard pump, this one is capable of moving large amounts of gases, although this is only archived through the "+ FormatAsLink("High Pressure Gas Pipe", HighPressureGasConduitConfig.ID)+ ".";
					public static LocString EFFECT = string.Concat(
						[
							"Draws in ",
							FormatAsLink("Gas", "ELEMENTS_GAS"),
							" and runs it through ",
							FormatAsLink("High Pressure Gas Pipe", HighPressureGasConduitConfig.ID),
							".\n\nMust be submerged in ",
							FormatAsLink("Gas", "ELEMENTS_GAS"),
							"."
						]);
				}
				public class PRESSURELIQUIDPUMP
				{
					public static LocString NAME = FormatAsLink("High Pressure Liquid Pump", nameof(PRESSURELIQUIDPUMP));
					public static LocString DESC = "An advanced pump that perform mechanical work to compress and move fluids. More powerful than the standard pump, this one is capable of moving large amounts of liquids, although this is only archived through the "+FormatAsLink("High Pressure Liquid Pipe", HighPressureLiquidConduitConfig.ID)+ "." ;
					public static LocString EFFECT = string.Concat(
						[
							"Draws in ",
							FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
							" and runs it through ",
							FormatAsLink("High Pressure Liquid Pipe", HighPressureLiquidConduitConfig.ID),
							".\n\nMust be submerged in ",
							FormatAsLink("Liquid", "ELEMENTS_LIQUID"),
							"."
						]);
				}
				public class HPAVENTLIQUID
				{
					public static LocString NAME = FormatAsLink("Compressor Liquid Vent", nameof(HPAVENTLIQUID));
					public static LocString DESC = "A reinforced liquid vent with a built in compression pump, capable of dispensing liquids even in high pressure environments.";
					public static LocString EFFECT = string.Concat(
						[
							"Dispenses ",
							FormatAsLink("Liquids", "ELEMENTS_LIQUID"),
							" in high pressure environments.\n\nMust be connected to a ",
							FormatAsLink("High Pressure Liquid Pipe", HighPressureLiquidConduitConfig.ID),
							".\n\nRequires power to function."
						]);
				}
				public class HPAVENTGAS
				{
					public static LocString NAME = FormatAsLink("Compressor Gas Vent", nameof(HPAVENTGAS));
					public static LocString DESC = "A reinforced gas vent with a built in compression pump, capable of dispensing gases even in high pressure environments.";
					public static LocString EFFECT = string.Concat(
						[
							"Dispenses ",
							FormatAsLink("Gases", "ELEMENTS_GAS"),
							" in high pressure environments.\n\nMust be connected to a ",
							FormatAsLink("High Pressure Gas Pipe", HighPressureGasConduitConfig.ID),
							".\n\nRequires power to function."
						]);
				}

				public class HPA_SOLIDRAIL
				{
					public static LocString NAME = FormatAsLink("Heavy-Duty Rail", nameof(HPA_SOLIDRAIL));
					public static LocString DESC = "Rails move materials where they'll be needed most, saving Duplicants the walk.";
					public static LocString EFFECT = string.Concat(
						[
							"Transports large quantities of ",FormatAsLink("Solid Materials", "ELEMENTS_SOLID")," on a track between ",
							FormatAsLink("Heavy-Duty Loaders", "HPA_INBOX"),
							" and ",
							FormatAsLink("Heavy-Duty Receptacles", "HPA_OUTBOX"),
							".\n\nCan not be run through wall and floor tiles."
						]);
				}
				public class HPA_SOLIDRAIL_INSULATED
				{
					public static LocString NAME = FormatAsLink("Insulated Heavy-Duty Rail", nameof(HPA_SOLIDRAIL_INSULATED));
					public static LocString DESC = "Rails move materials where they'll be needed most, saving Duplicants the walk.";
					public static LocString EFFECT = string.Concat(
						[
							"Transports large quantities of ",FormatAsLink("Solid Materials", "ELEMENTS_SOLID"),
							".\n\nCan not be run through wall and floor tiles.\n\nBeing held in a vaccuum, transported items are fully insulated."
						]);
				}
				public class HPA_SOLIDRAILBRIDGETILE
				{
					public static LocString NAME = FormatAsLink("Heavy-Duty Joint Plate", nameof(HPA_SOLIDRAILBRIDGETILE));
					public static LocString DESC = "Joint plates can run Heavy-Duty rails through walls without leaking gas or liquid.";
					public static LocString EFFECT = "Allows " + HPA_SOLIDRAIL.NAME+" and "+ HPA_SOLIDRAIL_INSULATED.NAME + " to be run through wall and floor tile.\n\nFunctions as regular insulated tile.";
				}
				public class HPA_SOLIDRAILBRIDGE
				{
					public static LocString NAME = FormatAsLink("Heavy-Duty Bridge", nameof(HPA_SOLIDRAILBRIDGE));
					public static LocString DESC = "Separating rail systems helps ensure materials go to the intended destinations.";
					public static LocString EFFECT = "Allows " + HPA_SOLIDRAIL.NAME + " and " + HPA_SOLIDRAIL_INSULATED.NAME + " to run one section over another without joining them.\n\nCanot be run through wall and floor tile.";
				}
				public class HPA_INBOX
				{
					public static LocString NAME = FormatAsLink("Heavy-Duty Loader", nameof(HPA_INBOX));
					public static LocString DESC = "";
					public static LocString EFFECT = "";
				}
				public class HPA_OUTBOX
				{
					public static LocString NAME = FormatAsLink("Heavy-Duty Receptacle", nameof(HPA_OUTBOX));
					public static LocString DESC = "";
					public static LocString EFFECT = "";
				}
				public class HPA_SOLIDRAILVALVE
				{
					public static LocString NAME = FormatAsLink("Heavy-Duty Reduction Valve", nameof(HPA_SOLIDRAILVALVE));
					public static LocString DESC = "A mechanical valve capable of reducing the flow of mass from a Heavy-Duty rail to to a conveyor rail, to avoid dropping items.";
					public static LocString EFFECT = "Allows " +
					FormatAsLink("Solid Materials", "ELEMENTS_SOLID") +
					" to be transfered from " +
					FormatAsLink("Heavy-Duty Rails", "HPA_SOLIDRAIL") +
					" to normal" +
					FormatAsLink("Conveyor Rails", "SOLIDCONDUIT") +
					".";
				}

				public class HPA_TRANSFERARM
				{
					public static LocString NAME = FormatAsLink("Heavy-Duty Auto-Sweeper", nameof(HPA_TRANSFERARM));
					public static LocString DESC = "An auto-sweeper's range can be viewed at any time by clicking on the building.";
					public static LocString EFFECT = string.Concat(
						[
							"Automates ",
							FormatAsLink("Sweeping", "CHORES"),
							" and ",
							FormatAsLink("Supplying", "CHORES"),
							" errands by sucking up all nearby ",
							FormatAsLink("Debris", "DECOR"),
							".\n\nMaterials are automatically delivered to any ",
							FormatAsLink("Heavy-Duty Loader", "HPA_INBOX"),
							", ",
							FormatAsLink("Heavy-Duty Receptacle", "HPA_OUTBOX"),
							", storage, or buildings within range.\n\nComes with increased range, capacity and rotation speed."
						]);
				}

				#endregion
				#region DupesRefrigeration

				public class AIO_FRIDGELARGE
				{
					public static LocString NAME = FormatAsLink("Household Fridge", nameof(AIO_FRIDGELARGE));
					public static LocString DESC = "A quality fridge commonly seen in the old world days. Still, this vintage building does the same job as a modern variant, which is the preservation of food.";
					public static LocString EFFECT = "The sealed storage of this fridge keeps food very cold, but not in sterile atmospheric conditions.";
					public class FACADES
					{
						public class FRIDGELARGE_STICKERS
						{
							public static LocString NAME = FormatAsLink("Stickered Household Fridge", nameof(FRIDGELARGE_STICKERS));
							public static LocString DESC = "A variant of the Household Fridge that has been plastered with a large variety of stickers";
						}
						public class FRIDGELARGE_BLUE
						{
							public static LocString NAME = FormatAsLink("Blue Household Fridge", nameof(FRIDGELARGE_BLUE));
							public static LocString DESC = "A blue variant of the Household Fridge";
						}
						public class FRIDGELARGE_RED
						{
							public static LocString NAME = FormatAsLink("Red Household Fridge", nameof(FRIDGELARGE_RED));
							public static LocString DESC = "A red variant of the Household Fridge";
						}
						public class FRIDGELARGE_YELLOW
						{
							public static LocString NAME = FormatAsLink("Yellow Household Fridge", nameof(FRIDGELARGE_YELLOW));
							public static LocString DESC = "A yellow variant of the Household Fridge";
						}
					}
				}
				public class FRIDGEPOD
				{
					public static LocString NAME = FormatAsLink("Fridge Pod", nameof(FRIDGEPOD));
					public static LocString DESC = "A tiny wall pod capable of preserving food at low temperatures.";
					public static LocString EFFECT = "The sealed storage of this pod keeps food very cold, but not in sterile atmospheric conditions.";
				}
				public class SIMPLEFRIDGE
				{
					public static LocString NAME = FormatAsLink("Home Fridge", nameof(SIMPLEFRIDGE));
					public static LocString DESC = "A plain old-style home fridge. Still, this vintage building does the same job as a modern variant, which is preservation the of food.";
					public static LocString EFFECT = "The sealed storage of this fridge keeps food very cold, but not in sterile atmospheric conditions.";
					public class FACADES
					{
						public class SIMPLEFRIDGE_STICKERS
						{
							public static LocString NAME = FormatAsLink("Stickered Home Fridge", nameof(SIMPLEFRIDGE_STICKERS));
							public static LocString DESC = "A variant of the Home Fridge that has been plastered with a large variety of stickers";
						}
					}
				}
				public class SPACEBOX
				{
					public static LocString NAME = FormatAsLink("Spacefaring Food Box", nameof(SPACEBOX));
					public static LocString DESC = "A high-grade polymer box used for storing food inside spacecraft. Requires placement inside Space Module.";
					public static LocString EFFECT = "The sealed storage compartment of this box allows complete vacuum through connection with the spacecraft hulk, thus completely preserving the food within.";
				}
				public class HIGHTECHSMALLFRIDGE
				{
					public static LocString NAME = FormatAsLink("Hightech Fridge", nameof(HIGHTECHSMALLFRIDGE));
					public static LocString DESC = "A fancy fridge with a sci-fi design";
					public static LocString EFFECT = "The sealed storage of this fridge keeps food very cold, but not in sterile atmospheric conditions.\nTakes food items from conveyor rails based on its filters.";
				}
				public class HIGHTECHBIGFRIDGE
				{
					public static LocString NAME = FormatAsLink("Large Hightech Refrigerator", nameof(HIGHTECHBIGFRIDGE));
					public static LocString DESC = "A fancy refrigerator with a sci-fi design.";
					public static LocString EFFECT = "The sealed storage of this fridge keeps food very cold, but not in sterile atmospheric conditions\nProvides water bottles as long there a pipe linked to it.\nTakes food items from conveyor rails based on its filters.";
				}
				#endregion
				#region CustomGenerators
				public class CUSTOMPETROLEUMGENERATOR
				{
					public static LocString NAME = FormatAsLink("Custom Combustion Generator", nameof(CUSTOMPETROLEUMGENERATOR));
					public static LocString DESC = "A small, custom Combustible Liquid generator. This rather versatile generator has piped outputs.";
					public static LocString EFFECT = "Converts "+ FormatAsLink("Combustable Fuel", "COMBUSTIBLELIQUID")+ " into Power.\nThe waste products are either released into the world or inserted into pipes.";
				}
				public class CUSTOMMETHANEGENERATOR
				{
					public static LocString NAME = FormatAsLink("Custom Gas Generator", nameof(CUSTOMMETHANEGENERATOR));
					public static LocString DESC = "A small, custom Combustible Gas generator. This rather versatile generator has piped outputs.";
					public static LocString EFFECT = "Converts " + FormatAsLink("Combustible Gas", "COMBUSTIBLEGAS") + " into Power.\nThe waste products are either released into the world or inserted into pipes.";
				}
				public class CUSTOMSOLARPANEL
				{
					public static LocString NAME = FormatAsLink("Custom Solar Panel", nameof(CUSTOMSOLARPANEL));
					public static LocString DESC = "Solar panels convert high intensity sunlight into power and produce zero waste.";
					public static LocString EFFECT = "Converts " + FormatAsLink("Sunlight", "LIGHT") + " into electrical " + FormatAsLink("Power", "POWER") + ".\n\nCan be rotated.";
				}
				public class CUSTOMSTEAMGENERATOR
				{
					public static LocString NAME = FormatAsLink("Custom Steam Turbine", nameof(CUSTOMSTEAMGENERATOR));
					public static LocString DESC = "Useful for converting the geothermal energy into usable power.";
					public static LocString EFFECT = "Draws in " + FormatAsLink("Steam", "STEAM") + " from the tiles directly below the machine's foundation and uses it to generate electrical " + FormatAsLink("Power", "POWER") + ".\n\nOutputs " + FormatAsLink("Water", "WATER") + ".";
				}
				public class CUSTOMSOLIDGENERATOR
				{
					public static LocString NAME = FormatAsLink("Custom Carbon Generator", nameof(CUSTOMSOLIDGENERATOR));
					public static LocString DESC = "A small, custom Refined Carbon generator. This rather versatile generator has piped outputs.";
					public static LocString EFFECT = "Converts " + FormatAsLink("Refined Carbon", "REFINEDCARBON") + " into Power.\nThe waste products are either released into the world or inserted into pipes.";
				}
				#endregion
			}
		}

		public class BUILDING
		{
			public class EFFECTS
			{
				public class TRANSFER_CAPACITY_LIMIT
				{
					public static LocString NAME = "Maximum Pipe Capacity: {0}";
					public static LocString TOOLTIP = "This pipe can transport up to {0} per second.";
				}
				public class SOLID_TRANSFER_CAPACITY_LIMIT
				{
					public static LocString NAME = "Maximum Item Weight {0}";
					public static LocString TOOLTIP = "This conveyor rail can transport items up to {0}.";
				}
			}
			public class STATUSITEMS
			{
				public class HPA_NEEDGASIN
				{
					public static LocString NAME ="No High Pressure Gas Intake";
					public static LocString TOOLTIP =("This building's " + PRE_KEYWORD + "High Pressure Gas Intake" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.HIGHPRESSUREGASCONDUIT.NAME + " connected");
				}

				public class HPA_NEEDGASOUT
				{
					public static LocString NAME ="No High Pressure Gas Output";
					public static LocString TOOLTIP =("This building's " + PRE_KEYWORD + "High Pressure Gas Output" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.HIGHPRESSUREGASCONDUIT.NAME + " connected");
				}

				public class HPA_NEEDLIQUIDIN
				{
					public static LocString NAME ="No High Pressure Liquid Intake";
					public static LocString TOOLTIP =("This building's " + PRE_KEYWORD + "High Pressure Liquid Intake" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.HIGHPRESSURELIQUIDCONDUIT.NAME + " connected");
				}

				public class HPA_NEEDLIQUIDOUT
				{
					public static LocString NAME ="No High Pressure Liquid Output";
					public static LocString TOOLTIP =("This building's " + PRE_KEYWORD + "High Pressure Liquid Output" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.HIGHPRESSURELIQUIDCONDUIT.NAME + " connected");
				}
				public class HPA_NEEDSOLIDIN
				{
					public static LocString NAME ="No Heavy-Duty Solid Intake";
					public static LocString TOOLTIP =("This building's " + PRE_KEYWORD + "Heavy-Duty Solid Intake" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.HPA_SOLIDRAIL.NAME + " connected");
				}
				public class HPA_NEEDLSOLIDOUT
				{
					public static LocString NAME ="No Heavy-Duty Solid Output";
					public static LocString TOOLTIP =("This building's " + PRE_KEYWORD + "Heavy-Duty Solid Output" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.HPA_SOLIDRAIL.NAME + " connected");
				}
				public class LOGISTIC_NEEDSOLIDIN
				{
					public static LocString NAME = "No Logistic Rail Intake";
					public static LocString TOOLTIP = ("This building's " + PRE_KEYWORD + "Logistic Solid Intake" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.LOGISTICRAIL.NAME + " connected");
				}
				public class LOGISTIC_NEEDSOLIDOUT
				{
					public static LocString NAME = "No Logistic Rail Output";
					public static LocString TOOLTIP = ("This building's " + PRE_KEYWORD + "Logistic Solid Output" + PST_KEYWORD + " does not have a " + (string)BUILDINGS.PREFABS.LOGISTICRAIL.NAME + " connected");
				}
				public class HPA_SOLIDCONDUITITEMDROPPED
				{
					public static LocString NAME = "Rail Capacity Exceeded!";
					public static LocString TOOLTIP = "This conveyor rail failed at transporting a debris chunk that was too heavy for it\nThe incoming item was {0} kg, but it could only hold {1} kg of that.\nExcess mass was dropped (WORK IN PROGRESS TEXT!)";
				}
				public class HPA_CONDUITOVERPRESSURIZED
				{
					public static LocString NAME = "Pipe Overpressurized!";
					public static LocString TOOLTIP = "This pipe tried to take in too high pressure!\nThe incoming package was {0} kg, but it could only contain {1} kg!";
				}
				public class ALGAEGROWER_LIGHTEFFICIENCY
				{
					public static LocString NAME = "Growth Efficiency {0}";
					public static LocString TOOLTIP = "The algae growth in this building is currently at {0} efficiency.\nThis is dependent on the light the building receives.\nRequires at minimum {1}.";
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
					public class NITROGENVENT
					{
						public static LocString NAME = FormatAsLink("Nitrogen Vent", "GeyserGeneric_" + nameof(NITROGENVENT));
						public static LocString DESC = $"A highly pressurized vent that periodically erupts with {FormatAsLink("Nitrogen", nameof(NITROGENGAS))}.";
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
			//===== [ Nitrogen ] =============================
			public class SOLIDNITROGEN
			{
				public static LocString NAME = FormatAsLink("Solid Nitrogen", nameof(SOLIDNITROGEN));
				public static LocString DESC = "(N<sub>2</sub>) Nitrogen is a nonmetal and the lightest member of group 15 of the periodic table, currently its its cold, solid state.";
			}
			public class LIQUIDNITROGEN
			{
				public static LocString NAME = FormatAsLink("Liquid Nitrogen", nameof(LIQUIDNITROGEN));
				public static LocString DESC = "(N<sub>2</sub>) Nitrogen is a nonmetal and the lightest member of group 15 of the periodic table, currently in its cold, liquid state.";
			}
			public class NITROGENGAS
			{
				public static LocString NAME = FormatAsLink("Nitrogen", nameof(NITROGENGAS));
				public static LocString DESC = "(N<sub>2</sub>) Nitrogen is a nonmetal and the lightest member of group 15 of the periodic table.";
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
				public static LocString DESC = "A dried, hard pressed clump of organic mass from vegetable origin. Has almost no moisture and can be used as burnable fuel or can be turned to dirt through composting.";
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

				public static LocString MINERALPROCESSING_GUIDANCEUNIT = FormatAsLink("Guidance Device", nameof(MINERALPROCESSING_GUIDANCEUNIT));
				public static LocString MINERALPROCESSING_GUIDANCEUNIT_DESC = "Guidance Devices allow the the mining drillhead to be programmed to target mine specific asteroid sectors.";

				public static LocString MINERALPROCESSING_DRILLBIT = FormatAsLink("Drillbit", nameof(MINERALPROCESSING_DRILLBIT));
				public static LocString MINERALPROCESSING_DRILLBIT_DESC = "Self-Propelled Drillbits that are used by the Asteroid Drill Rig to extract resources from otherwise unreachable sectors of the asteroid.";
				

				public static LocString RANDOMRECIPEINGREDIENT_DESTROYONCANCEL = FormatAsLink("Non-refundable Ingredient", nameof(RANDOMRECIPEINGREDIENT_DESTROYONCANCEL));
				public static LocString RANDOMRECIPEINGREDIENT_DESTROYONCANCEL_DESC = "This ingredient gets used up during its use, if a recipe with it gets canceled, it is lost.";
				public static LocString AIO_HARDENEDALLOY = FormatAsLink("Hardened Alloy", nameof(AIO_HARDENEDALLOY));
				public static LocString AIO_HARDENEDALLOY_DESC = "Hardened Alloys are a fusion of two or more materials.\nTheir high material strength allows them to be used as a substitute to " + global::STRINGS.ELEMENTS.STEEL.NAME;
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
					public static LocString RECIPE_DESC = "Produces " + NAME_PLURAL + " from the pulp of {0}.";
				}
			}
			public class INDUSTRIAL_PRODUCTS
			{
				public class MINING_DRILLBITS_TUNGSTEN_ITEM
				{
					public static LocString NAME = FormatAsLink("Tungsten Drillbits", nameof(MINING_DRILLBITS_TUNGSTEN_ITEM));
					public static LocString DESC = "A set of sturdy drill bits made for extremely hard rock mining operations.\nHas no own guidance system and can drill through deep, very hard rocks stratum even at high temperature.";
					public static LocString RECIPE_DESC = "Instruct the computer to produce Tungsten Drillbits.";

				}
				public class MINING_DRILLBITS_STEEL_ITEM
				{
					public static LocString NAME = FormatAsLink("Steel Drillbits", nameof(MINING_DRILLBITS_STEEL_ITEM));
					public static LocString DESC = "A set of sturdy drill bits made for hard rock mining operations.\nHas no own guidance system and can drill through hard rocks stratum.";
					public static LocString RECIPE_DESC = "Instruct the computer to produce Steel Drillbits.";
				}
				public class MINING_DRILLBITS_BASIC_ITEM
				{
					public static LocString NAME = FormatAsLink("Basic Drillbits", nameof(MINING_DRILLBITS_BASIC_ITEM));
					public static LocString DESC = "A set of sturdy drill bits made for basic mining operations.\nHas no own guidance system and can drill through soft rocks stratum.";
					public static LocString RECIPE_DESC = "Instruct the computer to produce Basic Drillbits.\nThis instruction is meant for Copper variation.";
				}
				public class MINING_DRILLBITS_GUIDANCEDEVICE_ITEM
				{
					public static LocString NAME = FormatAsLink("Guidance Device (Unprogrammed)", "MINERALPROCESSING_GUIDANCEUNIT");
					public static LocString NAME_PROGRAMMED = FormatAsLink("Guidance Device (Target: {0})", "MINERALPROCESSING_GUIDANCEUNIT");
					public static LocString DESC = "A sophisticated electronic module that aids the Mining Drillhead to navigate while operating.\nIn general, the guidance system computes the instructions for the Drillhead control system, which comprises its actuators, increasing the performance and detecting element resources from its surroundings.\n\nIt can be programmed by a skilled Duplicant or by the CNC machine.";
					public static LocString DESC_PROGRAMMED = "A sophisticated electronic module that aids the Mining Drillhead to navigate while operating.\nIn general, the guidance system computes the instructions for the Drillhead control system, which comprises its actuators, increasing the performance and detecting element resources from its surroundings.\n\nThis guidance device is programmed to target the {0}.";
					public static LocString RECIPE_DESC = "Instruct the computer to produce a Guidance Device.";
					public static LocString RECIPE_DESC_PROGRAM = "Instruct the computer to load the target program for the {0} into the Guidance Device";
				}
			}
		}
		public class UI
		{
			public class TOOLTIPS
			{
				public static LocString HELP_BUILDLOCATION_HPA_RAIL = "Obstructed by Heavy-Duty Rail";
			}
			public class KLEI_INVENTORY_SCREEN
			{
				public class SUBCATEGORIES
				{
					public static LocString RONIVAN_AIO_SKINS = "Ronivans Legacy - All In One";
				}
			}
			public class BUILDINGEDITOR
			{
				public static LocString TITLE = "Building Configuration Editor";

				public static LocString PARENT_MOD_DISABLED = "Building disabled by category.\nThe parent mod this building is from is turned off in the config.";
				public static LocString MOD_ORIGIN_TEXT = "This building is part of:";
				public static LocString BUILDINGCONFIGURABLE = "This building has settings that can be modified.";
				public class RESETALLCHANGES
				{
					public static LocString TITLE = "Reset Changes";
					public static LocString TEXT = "Are you sure you want to reset all modifications you have done to the building configuration?";
				}



				public class HORIZONTALLAYOUT
				{
					public class OBJECTLIST
					{
						public class SEARCHBAR
						{
							public static LocString CLEARTOOLTIP = "Clear search bar";
							public class INPUT
							{
								public class TEXTAREA
								{
									public static LocString PLACEHOLDER = "Filter buildings...";
									public static LocString TEXT = "";
								}
							}
						}
					}
					public class ITEMINFO
					{
						public class SCROLLAREA
						{
							public class CONTENT
							{
								public static LocString ENABLEBUILDING = "Enable Building:";
								public static LocString CAPACITYSETTINGS = "Storage Capacity:";
								public static LocString WATTAGESETTINGS = "Power Consumption:";
							}
						}

					}

				}
				public class BUTTONS
				{
					public class CLOSEBUTTON
					{
						public static LocString TEXT = "Return";
						public static LocString TOOLTIP = "Close window";
					}
					public class RESETBUTTON
					{
						public static LocString TEXT = "Reset All Changes";
						public static LocString TOOLTIP = "Reset all changes you have made";
					}
				}

			}
			public class MININGGUIDANCEDEVICEPROGRAMSELECTORSIDESCREEN
			{
				public static LocString TITLE = "Reprogram guidance device";
				public static LocString APPLY = "Reprogram";
			}
			public class MINING_SMART_DRILL_LOCATIONS
			{
				public static LocString MANTLE = "Deep Mantle";
				public static LocString SOFT_STRATUM = "Soft Stratum";
				public static LocString HARD_STRATUM = "Hard Stratum";
				public static LocString AQUIFER = "Aquifer";
				public static LocString OIL_RESERVES = "Oil Reserves";
				public static LocString CRYOSPHERE = "Cryosphere";
			}
			public class MINING_AUGUR_DRILL
			{
				public static LocString BASIC_DRILLING = "Basic Drilling";
				public static LocString STEEL_DRILLING = "Steel Drilling";
				public static LocString TUNGSTEN_DRILLING = "Tungsten Drilling";
				public static LocString SMART_DRILLING = "Smart Drilling: {0}";
				public static LocString TARGET_DESC = "Direct the drill towards specific areas of the asteroid, targeting the {0}.";

				public static LocString RECIPE_1I = "Engage a drilling operation using {0} as drillbit.";
				public static LocString RECIPE_3I = "Engage a guided {0} drilling operation using {1} and {2}.";
				public static LocString RECIPE_LIQUID = "This drilling has a much higher chance to produce occurances in form of liquids.";
				public static LocString RECIPE_LIQUID_DANGER = "This DANGEROUS drilling has a much higher chance to produce occurences in form of high temperature liquids.";
				public static LocString RECIPE_RESULTS = "Standard Results:";
				public static LocString RECIPE_OCCURENCES = "Occurences during mining:";

			}

			public class CHEMICAL_COMPLEXFABRICATOR_STRINGS
			{
				public static LocString HEAT_REFINE = "Refine {0} with intense heat, producing {1}.";
				public static LocString HEAT_COOK = "Cook {0} at high temperature, and produces {1}.";
				public static LocString THREE_MIXTURE_COMPRESS_COOKING = "Compress a mixture of {0}, {1} and {2}.\nCooking the mixture at high temperature produces {3}";
				public static LocString THREE_MIXTURE_FUSE = "Fuse together a mixture of {0} and {1}, with addition of {2} to produce {3}.";
				public static LocString THREE_MIXTURE_TWO_PRODUCTS_SMELT_WASTE = "Smelt a mixture of {0}, {1} and {2}, to produce {3} and {4}. Produces {5} as waste.";
				public static LocString THREE_MIXTURE_SMELT_WASTE = "Smelt a mixture of {0}, {1} and {2}, to produce {3}. Produces {4} as waste.";

				public static LocString BALLCRUSHER_MILLING_1INGREDIENT = "Mill down {0}.\nThe milling process will yield {1} of random amounts of the following materials:\n{2}";
				public static LocString BALLCRUSHER_MILLING_2INGREDIENTS = "Mill down {0} with a mixture of {1} and {2}.\nThe milling process will yield {3} of random amounts of the following materials:\n{4}\n\nProduces large amounts of {5} as waste product.";
				public static LocString BALLCRUSHER_MILLING_3INGREDIENTS = "Mill down {0} with a mixture of {1}, {2} and {3}.\nThe milling process will yield {4} of random amounts of the following materials:\n{5}\n\nProduces large amounts of {6} as waste product.";
				public static LocString ARCFURNACE_SMELT = "Smelt {0} to produce {1}.";
				public static LocString ARCFURNACE_MELT = "Melt {0} to produce {1}.";

				public static LocString JAWCRUSHERMILL_MILLING_1_1_2 = "Grind down {0} and extract small amounts of {1}.\nProduces {2} and {3} as waste products.";
				public static LocString JAWCRUSHERMILL_MILLING_1_1 = "Grind down {0} into {1}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_1_BREAK = "Break down {0} into {1}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_2 = "Break down {0} into {1} and {2}.";
				public static LocString JAWCRUSHERMILL_MILLING_1_4 = "Break down {0} into:\n• {1}\n• {2}\n• {3}\n• {4}";
				public static LocString CRUSHEDROCK_FROM_RAW_MINERAL_NAME = FormatAsLink("Raw Mineral", "BUILDABLERAW") + " to " + global::STRINGS.ELEMENTS.CRUSHEDROCK.NAME;
				public static LocString CRUSHEDROCK_FROM_RAW_MINERAL_DESCRIPTION = "Crushes " + FormatAsLink("Raw Minerals", "BUILDABLERAW") + " into " + global::STRINGS.ELEMENTS.CRUSHEDROCK.NAME;

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
				public static LocString ANAEROBIC_DIGESTER_1_2 = "Break down {0}, producing {1} and {2}.";
				public static LocString EXPELLER_PRESS_1_2 = "Press down {0} and extract {1}. Produces {2} as waste.";
				public static LocString EXPELLER_PRESS_SEEDTOOIL = FormatAsLink("Seeds", "SEED") + " to " + ELEMENTS.LIQUIDVEGEOIL.NAME;
				public static LocString EXPELLER_PRESS_FOODTOOIL = "{0} to " + ELEMENTS.LIQUIDVEGEOIL.NAME;

				public static LocString PLASMAFURNACE_1_1 = "Smelts raw {0} to produce high purity {1}.";
				public static LocString PLASMAFURNACE_2_1 = "Smelt an uniform mixture of {0} and {1} to produce high purity {2}.";
				public static LocString PLASMAFURNACE_2_1_1 = "Smelt an uniform mixture of {0} and {1} to produce high purity {2}.\nProduces {3} as waste.";
				public static LocString PLASMAFURNACE_2_2_1 = "Smelt an uniform mixture of {0} and {1} to produce high purity {2} and {3}.\nProduces {4} as waste.";
				public static LocString PLASMAFURNACE_1_1_1 = "Smelt raw {0} to produce high purity {1}.\nProduces {2} as waste.";
				public static LocString PLASMAFURNACE_1_2 = "Smelt raw {0} to produce high purity {1} and {2}.";
				public static LocString PLASMAFURNACE_STEEL = "Smelt a mixture of {0} and {1}, with {2} as flux, to produce high purity {3}.";

				public static LocString CALCINATOR_1_1 = "Submit {0} to high temperature oxidation reduction to produce {1}.";
				public static LocString CALCINATOR_2_1 = "Submit a mixture of {0} and {1} to high temperature oxidation degradation to produce {2}.";

				public static LocString CHEMICAL_MIXINGUNIT_2_1 = "Mix {0} and {1} to produce {2}.";
				public static LocString CHEMICAL_MIXINGUNIT_3_1 = "React a mixture of {0} and {1} with the addition of {2} to produce {3}.";
				public static LocString CHEMICAL_MIXINGUNIT_FERTILIZER = "Treat a portion of {0} with {1}, {1} and {2} additives, producing {3}.";

				public static LocString CEMENT_MIXER_CEMENT_3 = "Produce {3} from a mixture of\n{0}, {1} and {2}.";
				public static LocString OILSHALE_CEMENT = "Oil Shale Cement";
				public static LocString SLAG_CEMENT = "Slag Cement";
				public static LocString CRUSHEDROCK_CEMENT = " Limestone Cement";

				public class RANDOMRECIPERESULT
				{
					public static LocString NAME = "Random Composition: {0}";
					public static LocString NAME_OCCURENCE_FORMAT = "Random Occurence: {0}";
					public static LocString OCCURENCE_RANDOM_AMOUNT = "{0}/{1} s";
					public static LocString DESC = "This recipe yields {0} of random amounts of the following elements:";
					public static LocString DESC_MAX_COUNT = "This recipe yields {0} of random amounts of {1} of the following elements:";
					public static LocString DESC_OCCURENCE = "During production, the machine will generate random amounts of the following byproducts every {0} seconds:";
					public static LocString DESC_RANGE = "~{0}";
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

		public class RONIVAN_AIO_MODCONFIG
		{
			public class ENABLEMOD
			{
				public static LocString NAME = "Enable Mod Content";
				public static LocString TOOLTIP = "The mod is enabled, this includes buildings, research, recipes and other mod specific mechanics.\nYou can still disable individual buildings of the mod in the building editor, separate from this setting.";
				public static LocString TOOLTIP_ELEMENTS = "The mod is enabled, this includes buildings, research, recipes, elements, comets and other mod specific mechanics.\nYou can still disable individual buildings of the mod in the building editor, separate from this setting.\nWarning: when this mod is enabled, worldgen will be altered by new elements";
				
			}
			public class GEYSERS
			{
				public static LocString NAME = "Generic Mod Geysers";
				public static LocString TOOLTIP = "New Geysers are added to the pool of randomly spawned geysers.\nThis will affect worldgen if active (different random geysers are chosen compared to vanilla)\nTurning it off will prevent those geysers from showing up from worldgen naturally unless you use a different mod to add them manually (e.g. customize geyser or CGM)";
			}
			public class MODELEMENTSWORLDGEN
			{
				public static LocString NAME = "Modified Biome Composition";
				public static LocString TOOLTIP = "New mod elements are embedded into the biomes during worldgen.";
			}
			public class BUILDINGEDITOR
			{
				public static LocString NAME = "Building Editor";
				public static LocString TOOLTIP = "Open the building editor to individually toggle all buildings added by the mod and change settings on some.\nAll changes done here require a restart!";
			}
			public class LOGISTIC_RAIL_CAPACITY
			{
				public static LocString NAME = "Logistic Rail Capacity";
				public static LocString TOOLTIP = "Logistic Rails serve as an early game version to conveyor rails, lacking the mechatronic requirements and unlocking earlier at the cost of lower maximum throughput.";
			}
			public class LOGISTIC_SWEEPER_RANGE
			{
				public static LocString NAME = "Logistic Auto-Sweeper Range";
				public static LocString TOOLTIP = "The Logistic Auto-Sweeper serves as an early game version of the autosweeper, trading reduced carrying capacity for a lack of a mechatronics requirement.";
			}
			public class HP_GAS_CAPACITY
			{
				public static LocString NAME = "High Pressure Gas Capacity";
				public static LocString TOOLTIP = "High pressure gas pipes allow pumping gases at much higher pipe throughput than regular pipes, at the cost of more expensive building materials.";
			}
			public class HP_GAS_PUMPCOST
			{
				public static LocString NAME = "Base Wattage: High Pressure Gas Pump";
				public static LocString TOOLTIP = "The total pump wattage is calculated by multiplying the base wattage value with the respective pipe capacity multiplier (compared to regular pipes)";
			}
			public class HP_LIQUID_CAPACITY
			{
				public static LocString NAME = "High Pressure Liquid Capacity";
				public static LocString TOOLTIP = "High pressure liquid pipes allow pumping liquids at much higher pipe throughput than regular pipes, at the cost of more expensive building materials.";
			}
			public class HP_LIQUID_PUMPCOST
			{
				public static LocString NAME = "Base Wattage: High Pressure Liquid Pump";
				public static LocString TOOLTIP = "The total pump wattage is calculated by multiplying the base wattage value with the respective pipe capacity multiplier (compared to regular pipes)";
			}
			public class HP_SOLID_ENABLE
			{
				public static LocString NAME = "Enable Heavy-Duty Rails";
				public static LocString TOOLTIP = "Heavy-Duty Rails serve as an late game upgrade to conveyor rails, having a much higher throughput at the cost of more complex build requirements.";
			}
			public class HP_SOLID_CAPACITY
			{
				public static LocString NAME = "Heavy-Duty Rail Capacity";
				public static LocString TOOLTIP = "Heavy-Duty Rails serve as an late game upgrade to conveyor rails, having a much higher throughput at the cost of more complex build requirements.";
			}
			public class HP_SOLID_ARMRANGE
			{
				public static LocString NAME = "Heavy-Duty Auto-Sweeper Range";
				public static LocString TOOLTIP = "The Heavy-Duty Auto-Sweeper serves as a late game version of the autosweeper, having higher range and throughput at the cost of more complex build requirements";
			}
		}
	}
}
