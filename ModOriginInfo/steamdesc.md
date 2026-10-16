[h1]Mod Origin Info Display[/h1]

What mod is that from? - Ask that question no longer:
This mod puts a label on buildings, critters, items and recipes added by other mods in various places, listing their origin mod name.

[img]https://raw.githubusercontent.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/refs/heads/master/Compat_All.png[/img]

[h1]Current latest Version: 1.1.3[/h1]

[hr][/hr]

[h1]Features[/h1]
Adds mod origin label to the following locations:
[list][*] Build Menu info screen (buildings)
[*] Info Screen (buildings, entities, critters)
[*] Codex Entry (buildings, entities)
[*] Research Screen Entry (buildings)
[*] Recipe Selection Screen (recipes)
[/list]

[h2]Limitations[/h2]
Dynamically generated buildings not added by the default implementation via IBuildingConfig class cannot be detected (should only affect a minory of mods).
Re-Enabled vanilla buildings are still considered vanilla.
Mods that use plib improperly without il-merging might not be detected properly.
Does not track modded elements or geysers.

[h2]Localization[/h2]
As there are only 3 strings in this mod, there is no translation template this time, 
as I handle them in code. If you want your language to be added (or one of the machine translations is in need of improvements),
please comment the translations, I'll add them the next update
[code]"Modded Building, added by:"
"Modded Content, added by:"
"Modded Recipe, added by:"[/code] 

[h1]Bug Reports & Local Download[/h1]
You can find direct downloads for my mods [url=https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases]here[/url].

Please post bug reports and issues on [url=https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/issues]GitHub[/url] or on my [url=https://discord.gg/5n7peBCnWZ]Discord Server[/url]. 
[b]Always include the full player.log in a report: [/b] https://github.com/aki-art/ONI-Mods/wiki/How-to-send-a-log

[b]Make sure you do NOT have the mod "Debug Console" enabled as it breaks the game's logging and makes the log useless for fixing bugs![/b] 

Bugs in the games' mod updating system can leading to mods not updating, always use [url=https://steamcommunity.com/sharedfiles/filedetails/?id=2018291283]Mod Updater[/url] to force updates in that case!
If you get a restart required loop, ignore it by clicking the escape key, then use another force update.

[hr][/hr]

Do you like the mods I made? If you want, you can support me on [url=https://ko-fi.com/sgtimalas]Kofi[/url] :D
