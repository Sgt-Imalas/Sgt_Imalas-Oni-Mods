namespace ClusterTraitGenerationManager
{
	internal class STRINGS
	{
		public class MODCONFIG
		{
			public class MANUALCLUSTERPRESETS
			{
				public static LocString NAME = "Automated Cluster Preset";
				public static LocString TOOLTIP = "Each time you start a new CGM cluster, a cluster preset will be created automatically.\nif deactivated, there will be a button for manual creation on the start screen instead.";
			}
			public class CHALLENGEASTEROIDS
			{
				public static LocString NAME = "Enable Challenge Asteroids";
				public static LocString TOOLTIP = "Generate more challenging start asteroids like superconductive and regolith moonlet";
			}
		}
		public class WORLD_TRAITS
		{
			public class CGM_RANDOMTRAIT
			{
				public static LocString NAME = "Randomized Traits";
				public static LocString DESCRIPTION = "Chooses between 1 and 3 Traits at random.\n(Between 0 and 2 for random planets)\nMutually exclusive with other selectable Traits.";
			}
		}
		public class UI
		{
			public class GENERIC_YESNO
			{
				public static LocString YES = "Yes";
				public static LocString NO = "No";
			}
			public class INFOTOOLTIPS
			{
				public static LocString INFO_ONLY = "These are for info only and are not configurable.";
			}
			public class GENERATIONWARNING
			{
				public static LocString WINDOWNAME = "Potential Generation Errors detected!";
				public static LocString DESCRIPTION = "You have selected more than 6 outer planets, which can lead to placement failures.\n Automatically adjust cluster size and placements?";
				public static LocString YES = "Yes";
				public static LocString NOMANUAL = "No, let me do it manually.";
			}

			public class CGM_MAINSCREENEXPORT
			{
				public class CATEGORIES
				{
					public class HEADER
					{
						public static LocString LABEL = "Starmap Item Categories";

					}
					public class FOOTERCONTENT
					{
						public class TITLE
						{
							public static LocString LABEL = "World Settings";
						}
						public class STORYTRAITS
						{
							public static LocString TOOLTIP = "Open the story traits selection.";
						}
						public class GAMESETTINGS
						{
							public static LocString TOOLTIP = "Open the game settings.";
						}
						public class MIXINGSETTINGS
						{
							public static LocString TOOLTIP = "Open the mixing settings.";
						}
					}
				}
				public class ITEMSELECTION
				{
					public class HEADER
					{
						public static LocString LABEL = "[STARMAPITEMTYPEPL] in this category:";
					}
					public class STARITEMCONTENT
					{
						public class INPUT
						{
							public class TEXTAREA
							{
								public static LocString PLACEHOLDER = "Enter text to filter asteroids...";
							}
						}
					}
					public class VANILLASTARMAPCONTENT
					{
						public class VANILLASTARMAPCONTAINER
						{
							public class ADDMISSINGPOI
							{
								public static LocString LABEL = "Add another POI group";
								public static LocString LABEL_BASEGAME = "Add missing POIs ({0} missing)";
								public static LocString TOOLTIP = "The following POI types are currently missing:{0}";
							}
							public class VANILLASTARMAPENTRYPREFAB
							{
								public class MININGWORLDSCONTAINER
								{
									public class SCROLLAREA
									{
										public class CONTENT
										{
											public class ADDPOI
											{
												public static LocString LABEL = "Add new POI";
											}
										}
									}
								}
							}

							public class ADDNEWDISTANCEBUTTONCONTAINER
							{
								public class ADDDISTANCEROW
								{
									public static LocString LABEL = "Increase max. Distance";
								}
								public class REMOVEDISTANCEROW
								{
									public static LocString LABEL = "Reduce max. Distance";
								}
							}
						}
					}
					public class FOOTER
					{
						public class TOOLBOX
						{
							public class TRASHCANCONTAINER
							{
								public class SHOWPERMALABELS
								{
									public static LocString LABEL = "Show Names";
								}
								public static LocString LABEL = "Drag POI here to delete.";
								public class INPUT
								{
									public static LocString TEXT = "";
								}
							}
							public class BOXOFPOI
							{
								public static LocString LABEL = "Drag POI here to delete.";
								public static LocString POINOTINSTARMAP = "This POI is currently not present on the starmap";
								public class INPUT
								{
									public class TEXTAREA
									{
										public static LocString TEXT = "";
										public static LocString PLACEHOLDER = "Enter Text to filter POIs";
									}
								}
							}
						}
					}
				}
				public class DETAILS
				{
					public class HEADER
					{
						public static LocString LABEL = "Currently selected [STARMAPITEMTYPE]: {0}";
						public static LocString LABEL_LOCATION = "Current [STARMAPITEMTYPE]: {0} at {1}";
					}


					public class CONTENT
					{
						public class SCROLLRECTCONTAINER
						{
							public class SO_POIGROUP_CONTAINER
							{
								public class GROUPHEADER
								{
									public static LocString LABEL = "POIs in this Group:";
								}
								public class POICONTAINER
								{
									public class SCROLLAREA
									{
										public class CONTENT
										{
											public class NOPOIS
											{
												public static LocString LABEL = "No POIs in this group...";
											}
										}
									}
								}
								public class ADDPOIBUTTON
								{
									public static LocString LABEL = "Add new POI to group";
								}
							}
							public class POI_ALLOWDUPLICATES
							{
								public static LocString LABEL = "Allow Spawning Duplicates:";
								public static LocString TOOLTIP = "When enabled, a POI from the POI pool can generate multiple times from this POI group.";
							}
							public class POI_AVOIDCLUMPING
							{
								public static LocString LABEL = "Avoid Clumping:";
								public static LocString TOOLTIP = "When enabled, POIs generated from this group cannot generate adjacent to other POIs.";
							}
							public class POI_GUARANTEE
							{
								public static LocString LABEL = "Guarantee all POIs:";
								public static LocString TOOLTIP = "When enabled, all POIs will be spawned, even if their placement fails by regular placement rules.";
							}
							public class SO_POIGROUP_REMOVE
							{
								public static LocString LABEL = "Delete this POI Group";
								public static LocString TOOLTIP = "Delete the currently selected POI group from the custom cluster.";

							}
							public class STORYTRAIT
							{
								public class STORYTRAITENABLED
								{
									public static LocString LABEL = "Generate Story Trait:";
									public static LocString TOOLTIP = "Should this Story Trait be generated?";
								}
								public class STORYTRAITBLACKLIST
								{
									public class DESCRIPTOR
									{
										public static LocString LABEL = "Allow Story Traits on these Asteroids:";
										public static LocString TOOLTIP = "Allows you to block the game from spawning this story trait on certain asteroids.\n\nBlocking too many asteroids can prevent the worldgen from succeeding!";
									}
								}
							}
							public class VANILLAPOI_RESOURCES
							{
								public static LocString NONESELECTED = "None";
								public static LocString SELECTEDDISTANCE = "{0} at {1} {2}";
								public static LocString SELECTEDDISTANCE_SO = "{0} in Group {1}";
								public static LocString DISTANCELABEL_DLC = "POI-Group {0}\nSpawns {1} of these POIs at Distance {2} to {3}:";
								public class RESOURCEHEADER
								{
									public static LocString LABEL = "Resources:";
								}
								public class CONTENT
								{
									public class RESOURCECONTAINER
									{
										public class SCROLLAREA
										{
											public class CONTENT
											{
												public class NORESOURCES
												{
													public static LocString LABEL = "No Resources here..";
												}
												public class LISTVIEWENTRYPREFAB
												{
													public class BIOLABEL
													{
														public static LocString LABEL = "Bio-Resource";
														public static LocString TOOLTIP = "This resource requires a biological cargo bay to collect.";
													}
												}
											}
										}
									}
								}
								public class VANILLAPOI_REMOVE
								{
									public class DELETEPOI
									{
										public static LocString TEXT = "Remove this POI";
									}
								}

								public class MODIFYPOIBTN
								{
									public class MODIFYPOI
									{
										public static LocString TEXT = "Modify this POI Type";
									}
								}
								public class CAPACITY
								{
									public static LocString LABEL = "Mineable Mass:";
								}
								public class REPLENISMENT
								{
									public static LocString LABEL = "Replenishment per cycle:";
								}
								public class VANILLAPOI_ARTIFACT
								{
									public static LocString LABEL = "Artifact Rarity:";
									public class ARTIFACTRATES
									{
										public static LocString NONE = "None";
										public static LocString BAD = "Bad";
										public static LocString MEDIOCRE = "Mediocre";
										public static LocString GOOD = "Good";
										public static LocString GREAT = "Great";
										public static LocString AMAZING = "Amazing";
										public static LocString PERFECT = "Perfect";
										public static LocString DLC_NO = "No Artifacts";
										public static LocString DLC_YES = "Produces Artifacts";
									}
								}
							}

							public class STARMAPITEMENABLED
							{
								public static LocString LABEL = "Generate [STARMAPITEMTYPE]:";
								public static LocString TOOLTIP = "Should this starmap item be generated at all?";
							}
							public class AMOUNTSLIDER
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Number to generate:";
									public static LocString TOOLTIP = "How many instances of this starmap item should be generated.\nValues that aren't full numbers represent a chance to generate for POIs.\n(f.e. 0.8 = 80% chance to generate this POI)";
									public static LocString OUTPUT = "0";
									public class INPUT
									{
										public static LocString TEXT = "";

									}
								}
							}
							public class AMOUNTOFCLASSICPLANETS
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Max. Number of Classic planets:";
									public static LocString TOOLTIP = "How many of the random planets are allowed to be \"classic\" size.\nA large number of classic size asteroids can impact performance.\nThis check includes start and warp planet in its calculcations.";
									public static LocString OUTPUT = "0";
									public class INPUT
									{
										public static LocString TEXT = "";

									}
								}
							}
							public class MINMAXDISTANCE
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Distance to Center:";
									public static LocString TOOLTIP = "The minimum and maximum distance to the center of the starmap the starmap item can generate with.\nSetting the range to 0 - 0 will always spawn the starmap item the center of the map.\nSetting the range to 3 - 8 will spawn the starmap item randomly between 3 and 8 hexes away from the center of the starmap.";

									public static LocString FORMAT = "Between {0} and {1}";
									public static LocString OUTPUT = "0";
									public class INPUT
									{
										public static LocString TEXT = "";
									}
								}
							}
							public class BUFFERSLIDER
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Buffer Distance:";
									public static LocString TOOLTIP = "The minimum distance this asteroid has to other asteroids.";
									public class INPUT
									{
										public static LocString TEXT = "";

									}
									public static LocString OUTPUT = "0";
								}
							}

							public class ASTEROIDMIXINGTARGETSELECTOR
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Asteroid Remix Target:";
									public static LocString TOOLTIP = "The target asteroid that is getting remixed.";
								}
							}

							public class ASTEROIDSKY
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Asteroid Sky:";
									public static LocString TOOLTIP = "The space behavior of this asteroid";
								}
								public class CONTENT
								{

									public class SUNLIGHTCYCLE
									{
										public static LocString LABEL = "Sunlight:";
									}
									public class RADIATIONCYCLE
									{
										public static LocString LABEL = "Space Radiation:";
									}
									public class NORTHERNLIGHTSCYCLE
									{
										public static LocString LABEL = "Northern Lights:";
									}
								}

							}

							public class ASTEROIDSIZE
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Asteroid Size:";
									public static LocString TOOLTIP = "The dimensions of this asteroid.";
								}
								public class CONTENT
								{
									public class INFO
									{
										public class HEIGHTLABEL
										{
											public static LocString LABEL = "Height:";
										}
										public class WIDTHLABEL
										{
											public static LocString LABEL = "Width:";
										}
									}
								}
								public static LocString SIZEWARNING = "Warning!\nThe planet size you have selected has {0}% more area than a normal vanilla size asteroid.\nThis might lead to low game performance!";
								public static LocString BIOMEMISSINGWARNING = "Warning!\nThe selected planet dimensions are too small, making it very likely to fail worldgen.\nIncrease them to avoid that!";


								public class SIZESELECTOR
								{

									public static LocString NEGSIZE0 = "Tiny";
									public static LocString NEGSIZE0TOOLTIP = "The asteroid is at 30% of its usual size.";
									public static LocString NEGSIZE1 = "Smaller";
									public static LocString NEGSIZE1TOOLTIP = "The asteroid is at 45% of its usual size.";
									public static LocString NEGSIZE2 = "Small";
									public static LocString NEGSIZE2TOOLTIP = "The asteroid is at 60% of its usual size.";
									public static LocString NEGSIZE3 = "Slightly Smaller";
									public static LocString NEGSIZE3TOOLTIP = "The asteroid is at 80% of its usual size.";

									public static LocString SIZE0 = "Normal";
									public static LocString SIZE0TOOLTIP = "The asteroid is at its usual size.";
									public static LocString SIZE1 = "Slightly Larger";
									public static LocString SIZE1TOOLTIP = "The asteroid is 25% larger than normal.";
									public static LocString SIZE2 = "Large";
									public static LocString SIZE2TOOLTIP = "The asteroid is 50% larger than normal.";
									public static LocString SIZE3 = "Huge";
									public static LocString SIZE3TOOLTIP = "The asteroid has twice its usual size.";
									public static LocString SIZE4 = "Massive";
									public static LocString SIZE4TOOLTIP = "The asteroid has three times its usual size.";
									public static LocString SIZE5 = "Enormous";
									public static LocString SIZE5TOOLTIP = "The asteroid has four times its usual size.";
								}
								public class RATIOSELECTOR
								{

									public static LocString NORMAL = "Normal Shape";
									public static LocString NORMALTOOLTIP = "The asteroid has its usual shape.";
									public static LocString WIDE1 = "Slightly Wider";
									public static LocString WIDE1TOOLTIP = "The asteroid is a bit wider than normal.";
									public static LocString WIDE2 = "Wider";
									public static LocString WIDE2TOOLTIP = "The asteroid is wider than normal.";
									public static LocString WIDE3 = "Much Wider";
									public static LocString WIDE3TOOLTIP = "The asteroid is a lot wider than normal.";

									public static LocString HEIGHT1 = "Slightly Taller";
									public static LocString HEIGHT1TOOLTIP = "The asteroid is a bit taller than normal.";

									public static LocString HEIGHT2 = "Taller";
									public static LocString HEIGHT2TOOLTIP = "The asteroid is taller than normal.";

									public static LocString HEIGHT3 = "Much Taller";
									public static LocString HEIGHT3TOOLTIP = "The asteroid is a lot taller than normal.";

								}


								public class INPUT
								{
									public static LocString TEXT = "";

								}
							}

							public class ASTEROIDTRAITS
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Asteroid Traits:";
								}
								public class CONTENT
								{
									public class TRAITCONTAINER
									{
										public class SCROLLAREA
										{
											public class CONTENT
											{
												public class NOTRAITS
												{
													public static LocString LABEL = "No Traits";
												}
												public class LISTVIEWENTRYPREFAB
												{
													public class AWAILABLERANDOMTRAITS
													{
														public static LocString LABEL = "Blacklist Traits";
														public static LocString TOOLTIP = "Disable Traits you want to not show up as random traits.";
													}
												}
											}
										}
									}
									public class ADDSEASONBUTTON
									{
										public static LocString TEXT = "Add Trait";
									}
								}
								public class ADDTRAITBUTTON
								{
									public static LocString TEXT = "Add Trait";
								}
							}

							public class ASTEROIDGEYSERS
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Asteroid Geysers:";
									public static LocString NONE = "No available geysers";
								}
								public class CONTENT
								{
									public class GUARANTEED
									{
										public class DESCRIPTOR
										{
											public static LocString LABEL = "Overrides:";
											public static LocString INFOTOOLTIP = "Geyser overrides can be used to force specific geysers to appear in place of fully randomly generated geysers.\nSemi-Random, pre-picked geysers cannot be changed.\nfully random geysers also include those added by the \"Geoactive\" trait.";
										}
										public class SCROLLAREA
										{
											public class CONTENT
											{
												public class NONE
												{
													public static LocString LABEL = "No geyser overrides";
												}
											}
										}
										public class ADDGEYSERBTN
										{
											public static LocString TEXT = "Add Geyser Override";
										}
									}
									public class BLACKLIST
									{
										public class DESCRIPTOR
										{
											public static LocString LABEL = "Blacklist:";
											public static LocString LABEL_SHARED = "Blacklist (shared):";
											public static LocString INFOTOOLTIP = "Prevent geyser types from generating as fully random geysers.\nThese are then replaced with random (generic) geysers.\nHas no effect on semi-random, curated geysers unless toggled on.\nAsteroids will share a central blacklist unless toggled off.";
										}
										public class BLACKLISTAFFECTNONGENERICS
										{
											public static LocString LABEL = "Blacklist affects all geysers";
											public static LocString TOOLTIP = "If enabled, the blacklist will also replace predetermined and curated geysers from the blacklist with random geysers.";
										}
										public class SHAREDBLACKLIST
										{
											public static LocString LABEL = "Use shared blacklist for asteroid";
											public static LocString TOOLTIP = "If disabled, the asteroid will use an individual blacklist.\nOtherwise it will share a blacklist with other asteroids";
										}
										public class SCROLLAREA
										{
											public class CONTENT
											{
												public class NONE
												{
													public static LocString LABEL = "No blacklisted geysers";
												}
											}
										}
										public class BLACKLISTBUTTON
										{
											public static LocString TEXT = "Add geyser to blacklist";
										}
									}
								}
							}

							public class METEORSEASONCYCLE
							{
								public class DESCRIPTOR
								{
									public static LocString LABEL = "Meteors:";
									public static LocString TOOLTIP = "What kind of meteors should come down on this asteroid?";
								}

								public static LocString NOSEASONSSELECTED = "No season types selected";
								public static LocString ADDNEWSEASON = "Add additional meteor season";
								public static LocString ADDNEWSEASONTOOLTIP = "Add another type of meteor season type to this asteroid.\nWarning: Meteor season types are all active at the same time,\nleading to a high volume of meteors at the same time if multiple are added.\nNormal asteroids have usually one season type.";
								public static LocString ACTIVESEASONSELECTORLABEL = "Active Meteor Seasons:";
								public static LocString ACTIVEMETEORSLABEL = "Active Meteor Showers:";

								public class CONTENT
								{
									public class SEASONS
									{
										public class SEASONSCROLLAREA
										{
											public class CONTENT
											{
												public class NOSEASONSELECTED
												{
													//public static LocString LABEL = "No Seasons selected";
													public static LocString LABEL = "";
												}
												public class ADDSEASONBUTTON
												{
													public static LocString TEXT = "Add new season type";
												}
											}
										}
									}
									public class ASTEROIDS
									{
										public class SCROLLAREA
										{
											public class CONTENT
											{
												public class NOMETEORSAVAILABLE
												{
													public static LocString LABEL = "No Meteor Showers";
												}
											}
										}
									}

									public static LocString TITLE = "Available Meteor Season Types:";
									public static LocString NOSEASONTYPESAVAILABLE = "No more available Season Types";
									public static LocString SEASONTYPETOOLTIP = "This season type contains the following shower types:";
									public static LocString SEASONTYPENOMETEORSTOOLTIP = "This season type does not contain any meteor shower types.\nThis might change in the future if Klei decides to add some showers to this season type.\nFor this reason the season type is listed here.";
								}

								public static LocString SWITCHTOOTHERSEASONTOOLTIP = "Swap this season to a different type";
								public static LocString REMOVESEASONTOOLTIP = "Remove this season type";

								public static LocString SHOWERTOOLTIP = "Meteor type present in these shower types:";
								public static LocString VANILLASEASON = "Vanilla Meteor Showers";
								public static LocString FULLERENETOOLTIP = "One time shower event when opening the tear.";
							}

						}

					}

					public class FOOTER
					{
						public class CLUSTERSIZESLIDER
						{
							public class DESCRIPTOR
							{
								public static LocString TOOLTIP = "The radius of the starmap.";
								public static LocString LABEL = "Cluster Size (Radius):";
								public static LocString OUTPUT = "0";
								public class INPUT
								{
									public static LocString TEXT = "";

								}
							}
						}

						public class BUTTONS
						{
							public class RESETCLUSTERBUTTON
							{
								public static LocString TEXT = "Reset Everything";
								public static LocString TOOLTIP = "Undo all changes you have made by reloading the cluster preset.";
								public static LocString TOOLTIP_VANILLA = "Undo all changes you have made by reloading the world preset.";
							}
							public class RESETSELECTIONBUTTON
							{
								public static LocString TEXT = "Reset Selected Item";
								public static LocString TOOLTIP = "Undo all changes you have made to the currently selected item.";
							}
							public class STARMAPBUTTON
							{
								public static LocString TEXT = "Reset Starmap";
								public static LocString TOOLTIP = "Undo all changes you have made to the starmap.";
							}
							public class RETURNBUTTON
							{
								public static LocString TEXT = "Return";
								public static LocString TOOLTIP = "Return to the previous screen.";
							}
							public class PRESETBUTTON
							{
								public static LocString TEXT = "Cluster Presets";
								public static LocString TOOLTIP = "Create new or load your existing cluster presets";
							}
							public class GENERATECLUSTERBUTTON
							{
								public static LocString TEXT = "Start modified Game";
								public static LocString TOOLTIP = "Start generating a modified Cluster based on selected parameters.\nModified Cluster Generation is only activated if this button here is used.";
								public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED = "The current asteroid placement rules do not allow a spot for all asteroids on the starmap.\nPlease adjust your asteroid placement rules or reduce the total amount of asteroids!";
								public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED_ADJACENTASTEROIDS = "You currently have at least 2 asteroids directly adjacent to each other.\nThis will lead to crashes when a rocket tries landing on either of them with roundtrip enabled.\nPlease adjust your asteroids positions so they are no longer directly adjacent!";
								public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED_ASTEROID = "The current asteroid placement rules for the {0} currently fail to place the asteroid on the starmap.\nPlease adjust its placements or reduce the total amount of asteroids!";
								public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED_COMETS = "The current asteroid placement rules do not allow meteors to reach the {0}.\nThis would cause a crash on the spawn of the first meteor shower that tries to target this unreachable asteroid.\nPlease adjust your starmap placements!";
								public static LocString TOOLTIP_CLUSTERPLACEMENTFAILED_TEAR = "There is currently no temporal tear on the starmap.\nPlease add a temporal tear to the starmap!";
							}
						}
					}
				}
			}


			public class CGMEXPORT_SIDEMENUS
			{
				public class PRESETWINDOWCGM
				{
					public class DELETEWINDOW
					{
						public static LocString TITLE = "Delete {0}";
						public static LocString DESC = "You are about to delete the preset \"{0}\".\nDo you want to continue?";
						public static LocString YES = "Confirm Deletion";
						public static LocString CANCEL = "Cancel";

					}

					public static LocString TITLE = "Cluster Presets";

					public class HORIZONTALLAYOUT
					{
						public class OBJECTLIST
						{
							public class SCROLLAREA
							{
								public class CONTENT
								{
									public class NOPRESETSAVAILABLE
									{
										public static LocString LABEL = "No presets available";
									}
									public class PRESETENTRYPREFAB
									{
										public class ADDTHISTRAITBUTTON
										{
											public static LocString TEXT = "Load Preset";
											public static LocString TOOLTIP = "Load this preset to the preview";

										}

										public static LocString RENAMEPRESETTOOLTIP = "Rename Preset";
										public static LocString DELETEPRESETTOOLTIP = "Delete Preset";

									}
								}
							}

							internal class SEARCHBAR
							{
								public static LocString CLEARTOOLTIP = "Clear search bar";
								public static LocString OPENFOLDERTOOLTIP = "Open the folder where the presets are stored.";
								internal class INPUT
								{
									public class TEXTAREA
									{
										public static LocString PLACEHOLDER = "Enter text to filter presets...";
										public static LocString TEXT = "";
									}
								}
							}
						}
						public class ITEMINFO
						{
							public class BUTTONS
							{
								public class CLOSEBUTTON
								{
									public static LocString TEXT = "Return";
									public static LocString TOOLTIP = "Close this preset window";
								}
								public class GENERATEFROMCURRENT
								{
									public static LocString TEXT = "Generate new Preset";
									public static LocString TEXT_STARTSCREEN = "Cluster Preset";
									public static LocString TOOLTIP = "Save the currently loaded cluster configuration to a new preset.";
								}
								public class APPLYPRESETBUTTON
								{
									public static LocString TEXT = "Apply Preset";
									public static LocString TOOLTIP = "Apply the preset thats currently displayed in the preview to the custom cluster.";
								}
							}
						}
					}
				}
				public class TRAITPOPUP
				{
					public static LocString TEXT = "available Traits:";
					public class TOGGLE
					{
						public static LocString LABEL = "Override asteroid trait rules:";
						public static LocString TOOLTIP = "Force overrides the trait rules for the asteroids,\nallowing to add traits that would not be allowed by vanilla generation.\nCaution; might increase worldgen failure rates or result in weird asteroid layouts, depending on the trait.";
					}
					public class SCROLLAREA
					{
						public class CONTENT
						{
							public class NOTRAITAVAILABLE
							{
								public static LocString LABEL = "No Traits available";

							}
							public class LISTVIEWENTRYPREFAB
							{
								public static LocString LABEL = "trait label";
								public class ADDTHISTRAITBUTTON
								{
									public static LocString TEXT = "Add this trait";

								}
								public class TOGGLETRAITBUTTON
								{
									public static LocString ADDTOBLACKLIST = "Disable as Random";
									public static LocString ADDTOBLACKLISTTOOLTIP = "Prevent the Trait from generating as a random trait";
									public static LocString REMOVEFROMBLACKLIST = "Enable as Random";
									public static LocString REMOVEFROMBLACKLISTTOOLTIP = "Allows the Trait generating as a random trait";

								}
							}
						}
					}
					public class CANCELBUTTON
					{
						public static LocString TEXT = "Close";
					}
				}
			}


			public class CGMBUTTON
			{
				public static LocString DESC = "Start customizing the currently selected cluster.";
			}
			public static class CATEGORYENUM
			{
				public static LocString START = "Start Asteroid";
				public static LocString WARP = "Teleport Asteroid";
				public static LocString OUTER = "Outer Asteroids";
				public static LocString POI = "Points of Interest";
				public static LocString VANILLASTARMAP = "Starmap";
			}
			public static class STARMAPITEMDESCRIPTOR
			{
				public static LocString ASTEROID = "Asteroid";
				public static LocString ASTEROIDPLURAL = "Asteroids";

				public static LocString POI = "Point of Interest";
				public static LocString POI_GROUP = "POI Group";
				public static LocString POI_GROUP_PLURAL = "POI Groups";
				public static LocString POIPLURAL = "Points of Interest";

				public static LocString STORYTRAIT = "Story Trait";
				public static LocString STORYTRAITPLURAL = "Story Traits";


				public static LocString NOPOISAVAILABLE = "No more available POI types.";
			}

			public class SEEDLOCK
			{
				public static LocString NAME = "Seed rerolling affects traits";
				public static LocString NAME_SHORT = "reroll Traits:";
				public static LocString NAME_STARMAP = "reroll Starmap:";
				public static LocString NAME_MIXING = "reroll Mixings:";
				public static LocString SEED_PLACEHOLDER = "Enter Seed...";
				public static LocString TOOLTIP = "When enabled, rerolling the seed will also reroll the planet traits to those of the new seed.\nDisable to reroll the seed without affecting the traits.\nOnly blocks trait rerolling for the seed setting above.";
				public static LocString TOOLTIP_STARMAP = "When enabled, rerolling the seed will also reroll the starmap to the new seed.\nDisable to reroll the seed without affecting the starmap.";
			}

			public class SPACEDESTINATIONS
			{
				public static LocString MODDEDPLANET = "(Modded)";
				public static LocString MODDEDPLANETDESC = "Added by the mod \"{0}\"";
				public static class CGM_RANDOM_STARTER
				{
					public static LocString NAME = "Random Start Asteroid";
					public static LocString DESCRIPTION = "The starting asteroid will be picked at random";
				}
				public static class CGM_RANDOM_WARP
				{
					public static LocString NAME = "Random Teleporter Asteroid";
					public static LocString DESCRIPTION = "The teleporter asteroid will be picked at random";
				}
				public static class CGM_RANDOM_OUTER
				{
					public static LocString NAME = "Random Outer Asteroid(s)";
					public static LocString DESCRIPTION = "Choose an amount of random outer asteroids.\n\nEach asteroid can only generate once";
				}
				public class CGM_RANDOM_POI
				{
					public static LocString NAME = "Random POI";
					public static LocString DESCRIPTION = "Chooses a random POI during worldgen.\n\nDoes not roll unique POIs\n(Temporal Tear, Russel's Teapot)";
				}
			}
		}
		public class CLUSTER_NAMES
		{
			public class CGM
			{
				public static LocString NAME = "CGM Custom Cluster";
				public static LocString DESCRIPTION = "This cluster has been handcrafted in the Cluster Generation Manager";
			}
		}
		public class ERRORMESSAGES
		{
			public static LocString PLANETPLACEMENTERROR = "Starmap Generation Error!\n{0} could not be placed on the star map. Increase the maximum distance of of this asteroid to fix this issue.";
			public static LocString MISSINGWORLD = "Missing Worlds!\nThe preset cannot be loaded since its start world is missing.";
			public static LocString MISSINGWORLDS_TITLE = "Missing Worlds!";
			public static LocString MISSINGWORLDS_TEXT = "There were {0} worlds in the preset that\ndon't exist in the current game!";
		}
	}
}

