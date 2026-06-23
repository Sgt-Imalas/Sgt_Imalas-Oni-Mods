using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KUI = STRINGS.UI;

namespace AkisBioInksReborn
{

	public class STRINGS
	{
		public static class BUILDINGS
		{
			public static class STATUSITEMS
			{
				public static class PPR_PRINTREADY
				{
					public static LocString NAME = "Ink ready";
				}
			}
		}

		public static class EFFECTS
		{

		}

		public static class TWITCH
		{
			public static class PRINTING_POD_LEAK
			{
				public static LocString NAME = "Leaky Printing Pod!";
				public static LocString DESC = "Your printing pod is leaking ink everywhere!";
			}

			public static class FLOOR_UPGRADE
			{
				public static LocString NAME = "Floor upgrade";
				public static LocString TOAST_GENERIC = "Your {RoomType} floor is being upgraded!";
				public static LocString TOAST_STABLE = "Critters can now also enjoy nice decor.";
			}

			public static class PRINTING_POD_LEEK
			{
				public static LocString NAME = "Leeky Printing Pod!";
				public static LocString DESC = "* The author is profusely apologizing for the typo. *";
			}

			public static class USELESS_PRINTS
			{
				public static LocString NAME = "Useless Care Package";
				public static LocString TOAST = "Useless prints are ready!";
				public static LocString DESC_QUEUED = "Useless prints have been queued.";
			}

			public static class HELPFUL_PRINTS
			{
				public static LocString NAME = "Helpful Care Package";
				public static LocString TOAST = "Helpful prints are ready!";
				public static LocString DESC_QUEUED = "Helpful prints have been queued.";
				public static LocString DESC_READY = "New printables are ready to print.";
			}

			public static class WACKY_DUPE
			{
				public static LocString NAME = "Spawn a Wacky Dupe";
				public static LocString TOAST = "Spawned Duplicant";
				public static LocString DESC = "{0} has been brought into the world!";
			}
		}


		public class MISC
		{
			public class TAGS
			{
				public static LocString PPR_BIOINK = "Bio-Ink";
			}

			public static LocString NAME_PREFIXES = "Ma,Jo,Mi,Ell,Ban,Stin,Cata,Nis,Ru,Le,Bubb,Na,Ma,Goss,Lind,De,Re,Fran,A,Li,Bu,Tra,Ha,Ro,O,Tur,Ni,Mee,Je,Cam,Ash,Ste,Ama,Pe,Qui,Jor";

			public static LocString NAME_SUFFIXES = "inn,i,ri,ve,kan,mille,an,eep,la,ner,to,wan,x,rold,valdo,e,am,shua,ky,ssan,hi,kie,n,von,say,rie,mann,ils,-Ma,les,ra,by,lie,sbet,lina,ge";
		}

		public static class ITEMS
		{
			public static class BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Bio-Ink", BioInkConfig.DEFAULT);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Allows immediate recharging of a Printing Pod.";
			}

			public static class BIONIC_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Bionic Bio-Ink", BionicBioInkConfig.ID);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Prints Bionic Duplicants or accessories.";
			}

			public static class FOOD_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Nutritious Bio-Ink", BioInkConfig.FOOD);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Makes new printables only edible items.";
			}

			public static class SHAKER_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Suspicious Bio-Ink", BioInkConfig.SHAKER);
				public static LocString DESC = "Collected unused printing material of a Printing pod, mixed with a questionable selection of other ingredients. Allows immediate recharging of a Printing Pod, and causes new printables be exclusively Duplicants. \n\n" +
					"<i>But something appears not quite right...</i>";
			}

			public static class TWITCH_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Useless Bio-Ink", BioInkConfig.TWITCH);
				public static LocString DESC = "<i>From Twitch chat, with love <3</i>";
			}

			public static class MEDICINAL_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Medicinal Bio-Ink", BioInkConfig.MEDICINAL);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Makes new printables contain cures and medicines.";
			}

			public static class METALLIC_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Metallic Bio-Ink", BioInkConfig.METALLIC);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Allows immediate recharging of a Printing Pod, and causes new printables to be metals and metal ores.";
			}

			public static class VACILLATING_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Vacillating Bio-Ink", BioInkConfig.VACILLATING);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Allows immediate recharging of a Printing Pod, and causes new printables be exclusively Duplicants. " +
					"These duplicants are likely to have much higher stats than normal, and can have Vacillator exclusive starting traits.";
			}

			public static class GERMINATED_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Eggy Bio-Ink", BioInkConfig.GERMINATED);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Allows immediate recharging of a Printing Pod, and causes the new selection to only include eggs and baby creatures.";
			}

			public static class SEEDED_BIO_INK
			{
				public static LocString NAME = KUI.FormatAsLink("Seeded Bio-Ink", BioInkConfig.SEEDED);
				public static LocString DESC = "Collected unused printing material of a Printing pod. Allows immediate recharging of a Printing Pod, and causes the new printables to be seeds of plants.";
			}

			public static class CAT_DRAWING
			{
				public static LocString NAME = KUI.FormatAsLink("A drawing of a cat", CatDrawingConfig.ID);
				public static LocString DESC = "A neat crayon drawing of a cat. (Can be displayed on a pedestal.)";
			}

			public static class BOOK_OF_SELF_IMPROVEMENT_VOL2
			{
				public static LocString NAME = KUI.FormatAsLink("Book of self improvement Vol II", SelfImprovablesConfig.BOOK_VOL2);
				public static LocString DESC = "The second volume for a handbook about improving and loving one's self. \n\n" +
					"Improves the lowest stat of a duplicant who reads it.\n\n" +
					"<b>Single Use</b>";
			}

			public static class MANGA
			{
				public static LocString NAME = KUI.FormatAsLink("Manga", SelfImprovablesConfig.MANGA);
				public static LocString DESC = "A Duplicant can read this.\n\n" +
					"<b>Single Use</b>";
			}

			public static class BOOK_OF_SELF_IMPROVEMENT
			{
				public static LocString NAME = KUI.FormatAsLink("Book of self improvement Vol I", SelfImprovablesConfig.BOOK_VOL2);
				public static LocString DESC = "A handy handbook about improving and loving one's self. \n\n" +
					"Removes the first negative trait from a duplicant assigned to read it.\n\n" +
					"<b>Single Use</b>";
			}

			public static class D8
			{
				public static LocString NAME = KUI.FormatAsLink("D8", SelfImprovablesConfig.D8);
				public static LocString DESC = "Rerolls all skills of a duplicant (such as digging, athletics, strength, etc.).\n\n" +
					"<b>Single Use</b>";
			}

			public static class D4
			{
				public static LocString NAME = KUI.FormatAsLink("D4", SelfImprovablesConfig.D4);
				public static LocString DESC = "Rerolls all traits of a duplicant (such as Narcoleptic, Starry Eyed, Buff, etc.).\n\n" +
					"<b>Single Use</b>";
			}

			public static class D100
			{
				public static LocString NAME = KUI.FormatAsLink("D100", SelfImprovablesConfig.D100);
				public static LocString DESC = "Rerolls the traits and skills of a duplicant.\n\n" +
					"<b>Single Use</b>";
			}

			public static class D6
			{
				public static LocString NAME = KUI.FormatAsLink("D6", D6Config.ID);
				public static LocString DESC = "Rerolls a printing pod offer. Consumable.";
			}

			public class STATUSITEMS
			{
				public class PRINTINGPODRECHARGE_ASSIGNEDTO
				{
					public static LocString NAME = $"Assigned to {{Assignee}} ({KUI.FormatAsAutomationState("{Data}", KUI.AutomationState.Standby)})";
					public static LocString TOOLTIP = $"When {{Assignee}} reads this book, their {KUI.FormatAsAutomationState("{Data}", KUI.AutomationState.Standby)} will be removed permanently.";
				}
			}

			public static class FOOD
			{
				public static class PRINTINGPODRECHARGE_LEEK
				{
					public static LocString NAME = KUI.FormatAsLink("Leek", LeekConfig.ID);
					public static LocString DESC = "Tasty greens.";
				}
			}
		}

		public static class UI
		{
			public static class D6BUTTON
			{
				public static LocString LABEL = "Reroll ({0})";
				public static LocString TOOLTIP = "Reroll all offers, uses 1 die. You have {0} dice left.";
			}

			public static class SANBOXTOOLS
			{
				public static class FILTERS
				{
					public static LocString BIO_INKS = "Bio-Inks";
				}
			}

			public static class DUPE_TIERS
			{
				public static LocString BAD_2 = "Terrible";
				public static LocString BAD_1 = "Meh";
				public static LocString OKAY = "Okay";
				public static LocString GOOD_1 = "Great";
				public static LocString GOOD_2 = "Super-Duper-Dupe";
			}

			public static class BIOINKSIDESCREEN
			{
				public static class CONTENTS
				{
					public static class BUTTONS
					{
						public static class DELIVER
						{
							public static LocString TEXT = "Deliver Bio-Ink";
						}

						public static class ACTIVATE
						{
							public static LocString TEXT = "Print";
						}
					}
				}
			}

			public static class SETTINGSDIALOG
			{
				public static class TITLE
				{
					public static LocString TITLETEXT = "Bio-Inks Settings";
				}

				public static class BUTTONS
				{
					public static class STEAMBUTTON
					{
						public static LocString TEXT = "Steam Workshop";
					}

					public static class CANCELBUTTON
					{
						public static LocString TEXT = "Cancel";
					}

					public static class GITHUBBUTTON
					{
						public static LocString TEXT = "Github";
					}

					public static class OK
					{
						public static LocString TEXT = "Apply";
					}
				}

				public static class CONTENT
				{
					public static class SLIDERPANEL
					{
						public static class SLIDER
						{
							public static LocString LABEL = "Random Dupe chance";
							public static LocString RANGELABEL = "{0}";
							public static LocString TOOLTIP = "% chance of randomized dupes showing up in prints, not using the Suspicious Ink.";
						}
					}

					public static class REFUND
					{
						public static LocString LABEL = "Refund Quantity:";
						public static LocString QUANTITY = "{0}";
						public static LocString TOOLTIP = "How much ink to refund for a rejected print, in kgs.";
					}

					public static class REFUNDACTIVETOGGLE
					{
						public static LocString LABEL = "Refund Active Ink";
						public static LocString TOOLTIP = "If enabled, when rejecting a print the refunded ink will be the same type as the print.";
					}

					public static class DEBUGMODETOGGLE
					{
						public static LocString LABEL = "Debug mode (for testing)";
						public static LocString TOOLTIP = "If enabled, an additional panel will show with debug tools. Do not use for gameplay.";
					}

					public static class TWITCHINTEGRATION
					{
						public static LocString LABEL = "Twitch Integration";
						public static LocString TOOLTIP = "Enables several new events for Twitch Integration.";
					}

					public static class COLOREDMEEPS
					{
						public static LocString LABEL = "Colored Meeps {0}";
						public static LocString MEEP_MISSING = "(mod Meep not installed)";
						public static LocString TOOLTIP = "Should randomly generated meeps have random hair colors, if Meep mod is installed.";
					}

					public static class REFUNDCYCLE
					{
						public static LocString LABEL = "Ink Refund";
						public static LocString CHOICELABEL = "N/A";
						public static LocString TOOLTIP = "What happens when you reject a print offer";

						public class OPTIONS
						{
							public static LocString MATCHING = "Matching";
							public static LocString MATCHING_DESCRIPTION = "Refund the same type of ink as was active.";
							public static LocString DEFAULT = "Default";
							public static LocString DEFAULT_DESCRIPTION = "Always refund default ink.";
							public static LocString NONE = "None";
							public static LocString NONE_DESCRIPTION = "Do not refund. (No way to get inks).";
						}
					}

					public static class RANDODUPEPRESET
					{
						public static LocString LABEL = "Suspicious Dupe Preset";
						public static LocString CHOICELABEL = "N/A";
						public static LocString TOOLTIP = "Easy tweaks to Rando dupe generation settings. Overrides .json packs unless set to \"Default\".";

						public class TIERS
						{
							public static LocString TERRIBLE = "Terrible";
							public static LocString VANILLAISH = "Vanillaish";
							public static LocString DEFAULT = "Default";
							public static LocString GENEROUS = "Generous";
							public static LocString WACKY = "Wacky";
							public static LocString QUIRKY = "Quirky";
						}
					}
				}
			}
		}
	}
}
