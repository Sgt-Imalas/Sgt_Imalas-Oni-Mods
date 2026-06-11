Ronivan's Legacy 1.16

New:
- added codex information for all types of fertilization (nitrogen atmosphere, divergents, mimika, micronutrient fertilizer)
- metal refinement recipes now have integration with the 'Rephysicalized' mod
- adv. metal refinery now allows other (non-supercoolant) coolants, at the drawback of reduced production speed
- added spanish localization by Dey2121
- argentite now has a unique element texture
- modded liquids now make use of the new liquid shaders
- the plasma furnace now also has gaseous outputs with optional piping
- added siltstone and basalt gambling recipes to both ball crusher mills
- added bioplastic gaskets
- all buildings of the mod in the research screen now have a dlc-like banner
- added wood type selection to wood generator, wood heater and ethanol distiller
- ModIntegration: added plib registry locations other mods can write their conduit capacity adjustments to for better integration with HPA

Changes & Tweaks:
- adjusted buildings with "transparent" material to use "glassy"
- removed galena, molten zinc and gaseous zinc, as these elements collided with the new vanilla versions
- reduced sulphur percentage in argentite to 13% for more accuracy (and bc galena is no longer a silver source)
- pyrite is now physically accurate and composed of sulphur and iron
- old modded zinc ore is now Aurichalcite, composed of zinc and copper
- old refined zinc is now legacy zinc and can no longer be obtained
- toxic slurry is no longer considered a water-like substance
- several buildings had their recipes adjusted to using gaskets instead of plastic; rate 50kg->1kg
- buildng a flocculation sieve now opens pipe overlay automatically
- added config setting for DupesLogistics for sweeper and loader to turn off heat production again

Fixes:
- methane generator patch integrates better with other mods now
- fixed several instances of overlapping text in codex panels
- fixed emulsifier not piping out phyto oil from veg. oil+water recipe
- added additional crash prevention+logging to other mods fiddling with the oil well