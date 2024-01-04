## Sgt_Imalas-Oni-Mods
Mods for Oxygen Not Included and ONI - Spaced Out.
 | This List is probably not up to date)

## How to build this Repository
1. clone/download repository and open it in visual studio
2. adjust the variables "ModFolder" and "GameLibsFolder" inside of Directory.Build.Props to reference your local dev folder and your game folder. This will relink all references to the game assemblies.
3. run "clean" on the 1_CycleComma project. This runs the publicise task to create publicised versions of the game assembly (all variables and functions are made public). If it does not work, try restoring NuGet packages on that mod.
4. Done. All mods should now be able to compile properly and be copied to the dev-folder on completion. If you want to make a new mod and use this repository as a base, make sure to allow "Unsafe Code" in the new mods project settings as this is required for using publiciser


## released Mods in this Repository
| Mod ID | Mod Title | Steam Release |
|-|-|-|
| RoboRockets |AI Controlled Rockets  | https://steamcommunity.com/sharedfiles/filedetails/?id=2765256496 | 
| Radiator_Mod |Space Radiator  | https://steamcommunity.com/sharedfiles/filedetails/?id=2795878144 | 
| CannedFoods | Canned Food  | https://steamcommunity.com/sharedfiles/filedetails/?id=2818855295 | 
| Rocketry Expanded (folder: Rockets-TinyYetBig) |Rocketry Expanded | https://steamcommunity.com/sharedfiles/filedetails/?id=2837919908 | 
| LogicSatellites |Scan & Relay Satellites | https://steamcommunity.com/sharedfiles/filedetails/?id=2840104621 | 
| Cryopod | Buildable Cryopod | https://steamcommunity.com/sharedfiles/filedetails/?id=2847621082 | 
| SaveGameModLoader|Mod Profile Manager | https://steamcommunity.com/sharedfiles/filedetails/?id=2854869130 | 
| SetStartDupes |Duplicant Stat Selector [DSS]  | https://steamcommunity.com/sharedfiles/filedetails/?id=2856555858 | 
| CrittersShedFurOnBrush|Critter Shedding  | https://steamcommunity.com/sharedfiles/filedetails/?id=2915844170 | 
| MineralizerReborn|Mineralizer Reborn  | https://steamcommunity.com/sharedfiles/filedetails/?id=2936854563 | 
| BawoonFwiend|Balloon Dispenser  | https://steamcommunity.com/sharedfiles/filedetails/?id=2938304376 | 
| DrywallPatternColours|Drywall Colours  | https://steamcommunity.com/sharedfiles/filedetails/?id=2942359329 | 
| ClusterTraitGenerationManager|Cluster Generation Manager [CGM]  | https://steamcommunity.com/sharedfiles/filedetails/?id=2947429297 | 
| CritterTraitsReborn|Critter Traits  | https://steamcommunity.com/sharedfiles/filedetails/?id=2954005789 | 
| MeteorDrops|Meteor Drops  | https://steamcommunity.com/sharedfiles/filedetails/?id=2961505956 | 
| NeutroniumTrashCan|Trash Cans  | https://steamcommunity.com/sharedfiles/filedetails/?id=3052222549 | 
| DupePrioPresetManager|Preset Manager  | https://steamcommunity.com/sharedfiles/filedetails/?id=2977728791 | 
| CustomGameSettingsModifier|Modify Difficulty Settings  | https://steamcommunity.com/sharedfiles/filedetails/?id=2962220957 | 
| SGTIM_NotificationManager|Warning Notification Manager  | https://steamcommunity.com/sharedfiles/filedetails/?id=2968008945 | 
| DailyRoutine|Daily Routine  | https://steamcommunity.com/sharedfiles/filedetails/?id=2863383492 | 
| RotatableRadboltStorage|Rotatable Radbolt Chamber Output [DLC] | https://steamcommunity.com/sharedfiles/filedetails/?id=2886144715 | 
| MoveDupeHere |"Move Here" Command Tile  | https://steamcommunity.com/sharedfiles/filedetails/?id=2901813453 | 
| ClaimNotification|Claim Notification  | https://steamcommunity.com/sharedfiles/filedetails/?id=2955304767 | 
| MeteorMigration|Meteor Migration [DLC]  | https://steamcommunity.com/sharedfiles/filedetails/?id=2955350267 | 
| UtilityGlass|Utility Glass  | https://steamcommunity.com/sharedfiles/filedetails/?id=3121297403 | 
| DebugButton|Debug Buttons  | https://steamcommunity.com/sharedfiles/filedetails/?id=3120193648 | 
| UL_UniversalLyzer|Universal Electrolyzer  | https://steamcommunity.com/sharedfiles/filedetails/?id=3052201693 | 
| hiabmripantscolf (folder: BrokenRocketInteriorPortFix)|Help I accidentally broke my rocket interior ports and now the save crashes on load - Fix  | https://steamcommunity.com/sharedfiles/filedetails/?id=3021827424 | 
| AmogusMorb|Amorbus  | https://steamcommunity.com/sharedfiles/filedetails/?id=2899109675 | 
| WeebPacu | Weeb Pacu  | https://steamcommunity.com/sharedfiles/filedetails/?id=2896083956 | 

## unreleased Mods in this Repository
| Mod Name | Notes |
|-|-|
|OniRetroEdition|brings back old, obsolete, removed features to make the game feel like an old version|
|Nuclear Expansion| Nuclear bomblets that can explode, nuclear pulse rocket engine, world destruction via big nuke|
|Chaos Twitch| extra events for twitch integration, already in test by certain streamers|
|Conveyor Tiles| factorio belt tiles|
|light bridge|portal 2 lightbridge|
|subplanetary transport | cargo railgun that shoots from one end of the asteroid to the other|
|tile of interest overlay | show tile of interest for each building/interactable.. very crashy|
|yamltemplate export fix| fixes offset inside rocket interiors when creating templates|
|storedreamjournals | released, but obsolete|
|ReactorMeterFix | fixes the nuclear reactor meter to use the full meter instead of 1/3 of it|
|RadOceanRevamp|some basic strings for that Germany-Guy|
|NeuralVaccilatorExpanded| more vaccilator traits, ability to reroll them with databanks (not implemented yet)|
|LostContentTestingGround|Test mod for experimenting with removed/unused content|
|LaserMeteorBlaster | laser powered meteor blaster|
|invertedItemFilter | random stuff for personal playthroughs|
|DupeStations|pill dispenser, clothing locker that dupe interact with when passing by|
|DupePodRailgun|shoot dupes to other planets with giant, multibuilding railguns. basically unimplemented|
|DontDisableBlamedMods| no mods get disabled on crash|
|DirtyBrine | gives brine a coler between salt water and polluted water|
|crownmouldskin | code backing for 3GuBs Moulding skins|
|chaineddeconfix|fixed port of chained deconstruction|
|betterCritterMorpher|put any critter into the critter morpher (morphs too), critter gambling, adjustable cooldowns|
|1_cycleComma| shows more numbers behind a comma in the asteroid clock, mainly used for clean task|
|_worldgenstatecapture| TNI-like world state capture, currently with json dump|



For direct downloads, see [here] | https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases)
