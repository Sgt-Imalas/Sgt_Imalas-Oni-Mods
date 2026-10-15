Ronivan's Legacy 1.20:

New:
- HPA: added conductive high pressure pipe variants
- HPA: added config setting to allow heavy-duty rails inside walls (disabled by default)
- Plasma Furnace Gambling: you can now gamble with low grade and base grade metal sands in the plasma furnace
- Mineral Processing Mining: added frosty, prehistoric and aquatic strata as mining destinations, offering more targeted access at the resources of those dlcs ("dlc"-elements are not actually dlc bound and always available in this game, aside from radioactive elements in SO)
- Dupes Refrigeration: added sterile atmosphere gas input port to hightech fridges (config setting, enabled by default). 
	- sterile gases are dynamically determined by game config and are compatible with any sterile modded gases (metal gases are excluded from this)
	- while there is sterile gas in storage, it will be slowly consumed (1g/s, configurable) and all stored items will use it as atmosphere instead of whats outside the fridge.
	- any germs on the fridge contents will follow their exposure rules with that gas (pumping chlorine will kill food poisoning for example).
	- This was inspired by the addon mod "HighTech Fridge Upgrade" by Rafael Pierangeli

Changes & Tweaks:
- hardened alloy substitution in recipes is now only active if chemproc-IO is enabled. this can be force activated for HPA via mod config (config setting, disabled by default). If that option is turned on, Thermium is also now available for HPA pipes
- high pressure vents now have their internal storage marked as insulated, this prevents an edge case vanilla bug when working with small pipe content amounts

Fixes:
- drywall hides pipes option no longer hides plug slugs and sticker bombs
- fixed a few broken links in translations