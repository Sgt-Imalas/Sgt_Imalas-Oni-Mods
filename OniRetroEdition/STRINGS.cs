using static STRINGS.UI;

namespace OniRetroEdition
{
	internal class STRINGS
	{
		public class EQUIPMENT
		{
			public class PREFABS
			{
				public class OXYGEN_MASK_RETRO
				{
					public static LocString NAME = FormatAsLink(global::STRINGS.UI.StripLinkFormatting(global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.NAME), nameof(OXYGEN_MASK_RETRO));
					public static LocString DESC = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.DESC;
					public static LocString EFFECT = "Improves " + FormatAsLink("Decor", "DECOR") + " and reduces " + FormatAsLink("Stress", "STRESS") + " by providing " + FormatAsLink("light", "LIGHT") + ".";
				}
			}
		}
		public class BUILDINGS
		{
			public class PREFABS
			{
				public class RETROONI_BATTERYLARGE
				{
					public static LocString NAME = FormatAsLink("Huge Battery", "RETROONI_BATTERYLARGE");

					public static LocString DESC = "Huge batteries hold even more power and keep systems running longer before recharging.";

					public static LocString EFFECT = "Stores " + FormatAsLink("Power", "POWER") + " from generators, then provides that power to buildings.\n\nSlightly loses charge over time.";
				}

				public class RETROONI_WALLLAMP
				{
					public static LocString NAME = FormatAsLink("Wall Lamp", nameof(RETROONI_WALLLAMP));
					public static LocString DESC = "The light helps imitate the Duplicants' natural aboveground habitat.\nSort of. Maybe.";
					public static LocString EFFECT = "Improves " + FormatAsLink("Decor", "DECOR") + " and reduces " + FormatAsLink("Stress", "STRESS") + " by providing " + FormatAsLink("light", "LIGHT") + ".";
				}
			}
		}
		public class MISC
		{
			public class PLACERS
			{
				public class SLURPPLACER
				{
					public static LocString NAME = "Pump";
					public static LocString TOOL_NAME = "Pumping Tool";
					public static LocString ACTION_NAME = "Pump Liquids";

					public static LocString SLURPBUTTON = "Command duplicants to pump liquid\nRequires a duplicant with plumbing skill";
				}
			}
		}
		public class ITEMS
		{
			public class RETROONI_BONES
			{
				public static LocString NAME = "Bones";
				public static LocString DESC = "The last remains of a lost friend.";
				public static LocString RECIPEDESC = "Crushing bones into " + global::STRINGS.ELEMENTS.LIME.NAME;
			}
		}
		public class CREATURES
		{
			public class SPECIES
			{
				public static LocString SHOCKWORMSPECIES = "Shock Worm";
			}
			public class FAMILY_PLURAL
			{
				public static LocString SHOCKWORMSPECIES = "Shock Worms";
			}
		}
		public class UI
		{
			public class RETRO_OVERLAY
			{
				public class TOXICITY
				{
					public static LocString SLIGHTLYTOXIC = "Slightly Toxic";
					public static LocString VERYYTOXIC = "Very Toxic";
				}
				public class SOUND
				{
					public static LocString OVERLAYNAME = "NOISE";
					public static LocString TOOLTIP1 = "Total noise Level: {0} dB";
					public static LocString TOOLTIP2 = "Noise Sources:";
				}

			}
			public class KLEI_INVENTORY_SCREEN
			{
				public class SUBCATEGORIES
				{
					public static LocString BUILDING_ONI_RETRO = "ONI Retro Skins";
				}

			}
			public class NEWBUILDCATEGORIES
			{
				public static class WORKSTATIONS
				{
					public static LocString NAME = "Skill Assignment";
					public static LocString BUILDMENUTITLE = "Skill Assignment";
					public static LocString TOOLTIP = "";
				}
			}
			public class TOOLTIPS
			{
				public static LocString MANAGEMENTMENU_REQUIRES_SKILL_STATION_RETRO = ("Build a Skills Board to unlock this menu\n\nThe " + (string)global::STRINGS.BUILDINGS.PREFABS.ROLESTATION.NAME + " can be found in the " + global::STRINGS.UI.FormatAsBuildMenuTab("Stations Tab", Action.Plan10) + " of the Build Menu");
			}

		}
	}
}
