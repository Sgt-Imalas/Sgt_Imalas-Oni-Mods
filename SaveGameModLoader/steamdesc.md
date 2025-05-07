[h1]Mod Profile Manager [MPM][/h1]

The best Mod Manager supplement for Mod Updater.
This mod allows you to synchronize your enabled mods to those used in a save game.
It stores the individual mod configs of your save games and allows you to switch between different configurations with deviating mod lists with one click.
You can also manually create and apply your own custom Mod Presets, called Mod Profiles.

To generate such a mod profile, just load your savegame you want the modlist from or save your active game with this mod enabled, the profile is created automatically.
You can also import new mod profiles from Steam Collections or manually create your own from the current mod loadout.

[b]This mod wont do any mod updating, install mod updater for that![/b] 

[img]https://i.imgur.com/oNBUpGe.png[/img] [img]https://i.imgur.com/Cd1HwbV.png[/img] [img]https://i.imgur.com/pa6jsyD.png[/img]
[h1]Current latest Version: 7.1[/h1]

[hr][/hr]
[h1]Features[/h1]
[h2]Mod Screen Improvements:[/h2][list]
[*] Massively improved Mod screen performance when there is a large number of mods installed
[*] Searchbar that filters for Mod Name and Author (code incorporated from asquared mod filter, if that mod is enabled aswell it will take priority)
[*] Pin Feature to pin your favourite mods to the top of the list
[*] Extensive Mod Filter options, each individually toggleable (Hide mods that are: dev, local, steam, active, inactive, incompatible)
[*] Easier changing of the load order position
[*] Custom Tag Filters
[*] Blue highlighting for local mods for better distinction
[*] The Mod screen will now stretch based on the screen resolution and will generally allow showing more mods at the same time.
[*] Workshop links from the "Subscription" buttons and the "Steam Workshop" button will now open in the steam overlay instead of the browser (this can be disabled by setting the parameter "Workshop_Browser_Choice" in the KPlayerPrefs file to "1")
[*] Custom Mod Profiles
[*] Mod Config Transfer
[/list] 


[h2]Mod Profile Features:[/h2][list]
[*] Loading a Savegame will copy the mod-config stored in that savegame and will enable syncing to that.
[*] Saving the game stores the profiles of currently enabled mods as the mod config of that save.
[*]  "Sync and Resume" - button on the main menu: allows you to synchronize your active mods to those active in your "Resume Game" - save 
[*] Load Menu has a new "Sync"-Button, it becomes enabled if there is a stored mod config. Clicking that sync-button opens the MOD SYNC MENU:
[list]
    [*] Will list all mods that are not in sync:[list]
        [*] enabled mods that are disabled in the stored mod config
        [*] disabled mods that are enabled in the stored mod config[/list] 
    [*] Clicking "Sync All and Load" will synchronize all mods to the stored config, afterwards it will restart the game and automatically load the save
    [*] Clicking "Sync Current Config and Load" will load the state the mod profile is on click, allowing you to manually select the mods you want to have enabled/disabled
    [*] If all mods are in sync, the menu will be empty and will have an extra green button telling that.
    [*] If there are mods in the mod config that are not installed, the menu will get a red button that will display them in a popup with a redirection to the steam workshop page of that mod[/list] 
[*] New Sub menu that gives an overview of all your existing mod profiles with individual access to each.
[*] Mod Config Transfer for "Sync and Resume"
[*] Mod Profiles in this overview allow two applying modes:[list]
    [*] Apply Profile: All mods stored in the profile will be enabled, all not in the profile get disabled
    [*] Make all active: All mods stored in the profile will be enabled, any enabled mods not in the profile STAY enabled[/list] 
[*] Subscribe to missing mods from within a mod profile 
[*] Create new Mod Profiles by importing Steam Collections
[*] Manually create new custom Mod Profiles from your active modlist.
[/list] 

[h2]How to import a Steam Collection:[/h2]
[img]https://i.imgur.com/uQKWLa0.gif[/img]

[h2]For other modders:[/h2]
you can give the import tool the file link to a player log, it will parse the player log and create a profile from it. - good for debugging user reports

[h1]Disclaimer[/h1]
[list]
[*]Mod Syncing is only supported in the main menu. I disabled it in the ingame load menu, since using that causes a base game bug which reinstalls and shuffles the entire mod profile.
[*]Crashes that happen while loading WILL wrongfully blame this mod for the crash. This happens due to the fact that mod profiles get created during loading. It is save to keep the mod enabled after such a crash, since these crashes often occur due to missing/not enabled mods.
[*] Incompatible with Ony Mod manager
[/list]


[h1]Complimentary Mods:[/h1]
[list]
[*][url=https://steamcommunity.com/sharedfiles/filedetails/?id=2018291283]Mod Updater[/url]: Fixes 99% of all mod updating issues, I consider this mod to be mandatory for myself.
[/list] 


[h1]Localisations[/h1]
[list]
[*]English
[*]German
[*]French (provided by Pholith)
[*]Chinese (provided by 『KRS』山城张学友)
[/list] 

[h1]Credits[/h1]
Virtual Scroll Code for a performant mod screen copied from FastTrack by Stephen under MIT License
Searchbar Code copied and slightly adjusted from Asquared31415s [url=https://steamcommunity.com/sharedfiles/filedetails/?id=2196347807]Mod Filter[/url] with her permission.

[hr][/hr]

[h1]Bug Reports & Local Download[/h1]
You can find direct downloads for my mods [url=https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases]here[/url] (please notify me if I forgot to release new versions).

Please post bug reports and issues on [url=https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/issues] GitHub[/url] or on my [url=https://discord.gg/5n7peBCnWZ]Discord Server[/url]. 
[b]Always include the full player.log in a report: [/b] https://github.com/aki-art/ONI-Mods/wiki/How-to-send-a-log

[b]Make sure you do NOT have the mod "Debug Console" enabled as it breaks the game's logging and makes the log useless for debugging![/b] 

[hr][/hr]

Do you like the mods I made? If you want, you can support me on [url=https://www.buymeacoffee.com/sgtimalas]Buy me a coffe[/url] or [url=https://ko-fi.com/sgtimalas]Kofi[/url] :D.

