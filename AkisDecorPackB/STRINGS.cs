using AkisDecorPackB.Content.Defs.Buildings;
using AkisDecorPackB.Content.Defs.Items;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.UI;

namespace AkisDecorPackB
{
	internal class STRINGS
	{
		public static LocString DESIGN_BY_DECORPACKB = "\n\n<color=#ec9c03><b>Design added by Decor Pack II.</b></color>";

		public class ITEMS
		{
			public class DECORPACKB_FOSSILNODULE
			{
				public static LocString NAME = "Fossil Nodule";
				public static LocString DESC = "...";
			}

			public class DECORPACKB_TREXFRAGMENT
			{
				public static LocString NAME = FormatAsLink("T-Rex Fossil Fragments", GiantFossilFragmentConfigs.TREX);
				public static LocString DESC = "Mostly intact remains of an enormous carnivore from the Cretaceous. ";
			}
			public class DECORPACKB_TRICERATOPSFRAGMENT
			{
				public static LocString NAME = FormatAsLink("Triceratops Fossil Fragments", GiantFossilFragmentConfigs.TRICERATOPS);
				public static LocString DESC = "Fractured fossils of a herbivorous dinosaur. ";
			}

			public class DECORPACKB_BRONTOFRAGMENT
			{
				public static LocString NAME = FormatAsLink("Brontosaurus Fossil Fragments", GiantFossilFragmentConfigs.BRONTO);
				public static LocString DESC = "Enormous bones left behind by a long extinct creature.";
			}

			public class DECORPACKB_LIVAYATANFRAGMENT
			{
				public static LocString NAME = FormatAsLink("Livayatan Fossil Fragments", GiantFossilFragmentConfigs.LIVAYATAN);
				public static LocString DESC = "Well preerved fossils of an ancient aquatic predator whale.";
			}
		}
		public class BUILDING
		{
			public class STATUSITEMS
			{
				public class DECORPACKB_AWAITINGFUEL
				{
					public static LocString NAME = "Awaiting Fuel";
					public static LocString TOOLTIP = "This building requires {1} {0} to function. It can be delivered by hand or by pipe.";
				}
				public class DECORPACKB_FOUNTAINDRIEDOUT
				{
					public static LocString NAME = "Dried Out!";
					public static LocString TOOLTIP = "This fountain is currently dried out and awaiting liquid flow.\n\nIt looks kinda sad in this state (decor is reduced)";
				}
				public class DECORPACKB_FOUNTAINFLOWING
				{
					public static LocString NAME = "Fountain flowing";
					public static LocString TOOLTIP = "Splish splash!";
				}
				public class DECORPACKB_FOSSILRECONSTRUCTION
				{
					public static LocString NAME = "Reconstruction by {duplicantName}";
					public static LocString TOOLTIP = "This fossil has been reconstructed by {duplicantName}.\n\nDuplicants with more advanced science skills will produce more accurate reconstructions.";
				}
			}
		}

		public class BUILDINGS
		{
			public class PREFABS
			{
				public class DECORPACKB_FOSSILDISPLAY
				{
					public static LocString NAME = FormatAsLink("Fossil Display", FossilDisplayConfig.ID);
					public static LocString DESC = "Duplicants who have learned research skills can produce more accurate reconstructions.";
					public static LocString EFFECT = "Majorly increases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

					public class VARIANT
					{
						public static class HUMAN
						{
							public static LocString NAME = FormatAsLink("\"Imaginative\" Human Skeleton", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "Humans were a species that went extinct in a war against Dodos 4 million years ago. \n" +
								"They had long icky arms with 5 fingers which they used to consume their only nutrition, Cheeto, a type of orange worm. \n" +
								"\n" +
								"This seems inaccurate.";
						}

						public static class DOGGY
						{
							public static LocString NAME = FormatAsLink("\"Imaginative\" Dog Skeleton", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "" +
								"\n" +
								"This seems inaccurate.";
						}


						public static class SPIDER
						{
							public static LocString NAME = FormatAsLink("\"Imaginative\" Spider Skeleton", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "The Spider was a creature obsessed with wrapping other creatures in warm and silky blankets, as a way to show it's friendly nature.\n" +
								"Some species were well known for their incredible pastries.\n" +
								"\n" +
								"This seems inaccurate.";
						}

						public static class UNICORN
						{
							public static LocString NAME = FormatAsLink("\"Imaginative\" Unicorn Skeleton", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "..." +
								"\n" +
								"This seems inaccurate.";
						}

						public static class PACU
						{
							public static LocString NAME = FormatAsLink("Three Pacus", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "<i>Pacumidae</i> are a group of ancient fish that once lived on these Asteroids. " +
								"If only such beautiful creatures could still grace our world...";
						}

						public static class DODO
						{
							public static LocString NAME = FormatAsLink("Dodo", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class TRILOBITE
						{
							public static LocString NAME = FormatAsLink("Trilobites", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class MINIPARA
						{
							public static LocString NAME = FormatAsLink("Tiny Parasaurolophus", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "A regular Parasaurolophus skeleton which appears to have been hit by a shrink ray. " +
								"That, or the artist misjudged the scale.";
						}

						public static class BEEFALO
						{
							public static LocString NAME = FormatAsLink("Beefalo", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "Ancient humans used to settle around Beefalo herds.";
						}

						public static class HELLHOUND
						{
							public static LocString NAME = FormatAsLink("Hell Hound", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "If you listen closely,you can almost hear a distant growl still echoing... ";
						}

						public static class CATCOON
						{
							public static LocString NAME = FormatAsLink("Catcoon", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class GLOMMER
						{
							public static LocString NAME = FormatAsLink("Oddly cute ancient insect", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}


						public static class VOLGUS
						{
							public static LocString NAME = FormatAsLink("Volgus", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = $"There are small pieces of {FormatAsLink("Neutronium", SimHashes.Unobtanium.ToString())} stuck to these remains, similar to that related to the Temporal Tear. It's as if this entity came from an alternate reality that was never to be.";
						}

						public static class MICRORAPTOR
						{
							public static LocString NAME = FormatAsLink("Microraptor", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "The remains of an ancient theropod with well visible plumage, stuck in hard Amber.";
						}

						public static class AMMONITE
						{
							public static LocString NAME = FormatAsLink("Ammonite", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class ANCIENTSPECIMENT
						{
							public static LocString NAME = FormatAsLink("Ancient Specimen", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class PAWPRINTS
						{
							public static LocString NAME = FormatAsLink("Mudprints", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "A family heirloom that was passed on across many generations of critters.";
						}

						public static class ANCIENTSPECIMENAMBER
						{
							public static LocString NAME = FormatAsLink("Ancient Amber Inclusion", FossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}
					}
				}

				public class DECORPACKB_GIANTFOSSILDISPLAY
				{
					public static LocString NAME = FormatAsLink("Giant Fossil Display", GiantFossilDisplayConfig.ID);
					public static LocString DESC = "Duplicants who have learned research skills can produce more accurate reconstructions.";
					public static LocString EFFECT = "Majorly increases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

					public class VARIANT
					{
						public static class BRONTO
						{
							public static LocString NAME = FormatAsLink("Brontosaurus", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class TREX
						{
							public static LocString NAME = FormatAsLink("Tyrannosaurus Rex", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class TRICERATOPS
						{
							public static LocString NAME = FormatAsLink("Triceratops", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class PTERODACTYL
						{
							public static LocString NAME = FormatAsLink("Pterodactyl", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class SPINOSAURUS
						{
							public static LocString NAME = FormatAsLink("Spinosaurus", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class DEERCLOPS
						{
							public static LocString NAME = FormatAsLink("Deerclops", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class PUGALISK
						{
							public static LocString NAME = FormatAsLink("Pugalisk", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "...";
						}

						public static class LIVAYATAN
						{
							public static LocString NAME = FormatAsLink("Livayatan", GiantFossilDisplayConfig.ID);
							public static LocString DESCRIPTION = "Livayatan Melville was one of the largest predators to have ever existed. They were a type of whale, named after the mythical Leviathan and the author of Moby Dick, Herman Melville. Livayatans most likely went extinct in the late Miocene due to globally declining temperatures.";
						}
					}
				}

				public class DECORPACKB_POT
				{
					public static LocString NAME = FormatAsLink("Clay Block", PotConfig.ID);
					public static LocString DESC = "Duplicants who have learned art skills can produce more decorative clay pottery.";
					public static LocString EFFECT = "Stores materials, while also increases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

					public class VARIANT
					{
						public class ANGRYLETTUCE
						{
							public static LocString NAME = FormatAsLink("Lettuce at peace", PotConfig.ID);
							public static LocString DESCRIPTION = "This pip has found inner piece in being a clay pot.";
						}

						public class HATCH
						{
							public static LocString NAME = FormatAsLink("Hatch Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "A gaggle of hatches decorate this intricate pottery piece.";
						}

						public class MORB
						{
							public static LocString NAME = FormatAsLink("Slimy Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "The extra shiny glaze gives these morbs an eerily realistic look.";
						}

						public class SWIRLIES
						{
							public static LocString NAME = FormatAsLink("Fun Swirly Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "This abstract pot design is elegant in its simplicity.";
						}

						public class SWIRLIES_PURPLE
						{
							public static LocString NAME = FormatAsLink("Purple Swirly Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "The purples really bring out the beauty of the Mealwood Seeds inside the pot.";
						}

						public class SWIRLIES_BLUEGOLD
						{
							public static LocString NAME = FormatAsLink("Blue-Gold Swirly Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "Shimmering hypnotizing swirls on a Clay Pot.";
						}

						public class PINKYFLUFFYLETTUCE
						{
							public static LocString NAME = FormatAsLink("Huggable Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "Somehow the duplicant who crafted this was able to achieve a soft clay surface.";
						}

						public class GENERIC_TALL
						{
							public static LocString NAME = FormatAsLink("Tall Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "A simple tall pot.";
						}

						public class MUCKROOT
						{
							public static LocString NAME = FormatAsLink("Mucky Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "Truly, the most prestigeous patterns of all time: The Muckroot.";
						}

						public class RECTANGULAR
						{
							public static LocString NAME = FormatAsLink("Cubular Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "The artist didn't really have any inspiration for this one...";
						}

						public class WEIRD
						{
							public static LocString NAME = FormatAsLink("Weird Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "A strangely shaped pot. Surely, an intentional design by the artist to express their deepest emotions.";
						}

						public class RED
						{
							public static LocString NAME = FormatAsLink("Red Pot", PotConfig.ID);
							public static LocString DESCRIPTION = "A pot made to look intentionally old and ancient. Or so the artist says.";
						}
					}
				}

				public class DECORPACKB_FOUNTAIN
				{
					public static LocString NAME = FormatAsLink("Fountain Kit", "");
					public static LocString DESC = "Duplicants who have learned art skills can produce more decorative sculptures.";
					public static LocString EFFECT = "Majorly increases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";
					public static LocString POORQUALITYNAME = "\"Abstract\" Fountain";
					public static LocString AVERAGEQUALITYNAME = "Mediocre Fountain";
					public static LocString GENIUSQUALITYNAME = "Genius Fountain";

					public static class VARIANT
					{
						public static class FISH
						{
							public static LocString NAME = FormatAsLink("Fancy Fish Fountain", FountainConfig.ID);
							public static LocString DESCRIPTION = "A triplet of regal Pacus.";
						}

						public static class BOWLS
						{
							public static LocString NAME = FormatAsLink("Bowls Fountain", FountainConfig.ID);
							public static LocString DESCRIPTION = "A classic fountain design, the stack of cereal bowls is always a safe choice.";
						}

						public static class ROLLER_SNAKE
						{
							public static LocString NAME = FormatAsLink("Rolling Fountain", FountainConfig.ID);
							public static LocString DESCRIPTION = "The eternal cycle of the water, the eternal cycle of the Roller Snake... These things really make you ponder.";
						}

						public static class ABSTRACT
						{
							public static LocString NAME = FormatAsLink("Pipedreams", FountainConfig.ID);
							public static LocString DESCRIPTION = "A post modernist creation for the thinking minds, making grandiose statements about our existence on this planet.";
						}
					}
				}

				public class DECORPACKB_OILLANTERN
				{
					public static LocString NAME = FormatAsLink("Oil Lantern", OilLanternConfig.ID);
					public static LocString DESC = "Light reduces Duplicant stress and is required to grow certain plants.";
					public static LocString EFFECT = $"Provides {FormatAsLink("Light", "LIGHT")} when supplied with {FormatAsLink("Oil", GameTags.UnrefinedOil.ToString())}.\n" +
						$"\n" +
						$"Duplicants can operate buildings more quickly when the building is lit.";
				}

				public class DECORPACKB_FLOORLIGHT
				{
					public static LocString NAME = FormatAsLink("Floorlamp", FloorLightConfig.ID);
					public static LocString DESC = "Light reduces Duplicant stress and is required to grow certain plants.";
					public static LocString EFFECT = "Provides " + FormatAsLink("Light", "LIGHT") + " when supplied with Power.\n" +
						"\n" +
						"Duplicants can operate buildings more quickly when the building is lit.";
				}
			}
		}

		public class MISC
		{
			public static class DECORPACKB
			{
				public static LocString FOSSIL_FRAGMENT = $"Fossil Fragments can be used to build {FormatAsLink("Giant Fossil Displays", GiantFossilDisplayConfig.ID)}.";
			}

			public class TAGS
			{
				public static LocString FOSSILBUILDING = "Fossil";
				public static LocString FOSSIL = "Fossil";
				public static LocString DECORPACKB_BUILDINGFOSSILNODULE = "Fossil Nodule";
				public static LocString DECORPACKB_BUILDINGFOSSILNODULE_DESC = "An important hint for the construction of a larger structure.";
				public static LocString DECORPACKB_FOSSILMATERIAL = "Fossil";
				public static LocString DECORPACKB_FOSSILMATERIAL_DESC = "Fossilized imprints";
				public static LocString DECORPACKB_FLOORLAMPFRAMEMATERIAL = "Frame";
				public static LocString DECORPACKB_FLOORLAMPPANEMATERIAL = "Pane";
			}
		}

		public class UI
		{
			public class DECORPACKB
			{
				public class BUILD_LOCATION_RULE_COMPLAINTS
				{
					public static LocString WALLS = "Must be attached to a wall on any of the four sides.";
					public static LocString HANGABLE = "Must be built on solid floor or hung from a ceiling.";
				}
			}

			public class KLEI_INVENTORY_SCREEN
			{
				public class SUBCATEGORIES
				{
					public static LocString DECORPACKB_FOSSILS = "Fossils";
					public static LocString DECORPACKB_POTS = "Pottery";
					public static LocString DECORPACKB_FOUNTAINS = "Fountains";
				}
			}

			public static class TOOLTIPS
			{
				public static LocString HELP_BUILDLOCATION_ON_ANY_WALL = "Must be placed on the ground, ceiling, or next to a wall.";
			}
		}
		public static class DUPLICANTS
		{
			public static class MODIFIERS
			{
				public static class DECORPACKB_INSPIRED_LOW
				{
					public static LocString NAME = "Midly Curious";
					public static LocString TOOLTIP = "This duplicant has seen a somewhat interesting thing.";
				}

				public static class DECORPACKB_INSPIRED_OKAY
				{
					public static LocString NAME = "Inspired";
					public static LocString TOOLTIP = "This duplicant has seen a decently interesting thing.";
				}

				public static class DECORPACKB_INSPIRED_GREAT
				{
					public static LocString NAME = "Awestruck";
					public static LocString TOOLTIP = "This duplicant has seen amazing discoveries and cannot wait to learn more about their world.";
				}

				public static class DECORPACKB_INSPIRED_GIANT
				{
					public static LocString NAME = "Expanded mind";
					public static LocString TOOLTIP = "This duplicant is greatly inspired by the wonders of this world.";
				}
			}
		}
		public class DECORPACKB_CONFIG
		{
			public class FUNCTIONALFOSSILS
			{
				public static LocString NAME = "Fossile Researching Mechanic";
				public static LocString TOOLTIP = "Fossils require a researcher to assemble, but will provide a researching buff to duplicants";
			}
			public class OILLANTERN_FLICKERS
			{
				public static LocString NAME = "Oil Lantern Flickers";
				public static LocString TOOLTIP = "Oil lanterns will have some flickering in their light intensity (like a real oil lamp)";
			}
			public class POTCAPACITY
			{
				public static LocString NAME = "Pot Capacity";
				public static LocString TOOLTIP = "Storage capacity of Pots";
			}
		}
	}
}
