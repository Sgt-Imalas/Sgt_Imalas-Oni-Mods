Ronivan's Legacy 1.11

New:

- thermium is now considered a hardened alloy (steel replacement)
- added a "crude" steel recipe back to the normal metal refinery, this comes at a reduced yield to encourage using the proper machine for it (selective arc furnace).
- thermal fudge values of metal refinery, advanced metal refinery and selective arc furnace can now be configured in the mod config
- high pressure reduction valves now can alternatively be built from Tungsten

Changes & Tweaks:

- wolframite to tungsten recipe in the metal refinery is now considered a crude recipe (reduced yield), to bring it in line with the original variant of Chemical Processing (wolframite recipe is only found on the advanced metal refinery)
- slightly reduced ammonia vent average output
- tweaked permendur recipe
- rebalanced mineral and augur drill; increased time of mineral drill recipes, increased steel recipe costs; vastly improved yields of mk2 drill recipes
- reduced syngas yield of oil shale, increased petroleum yield

Selective Arc Furnace Revamp: 
- the building now takes some insulating material in its construction
- recipes now produce heat energy again (like metal refinery)
- building now has a "Thermal Capacitor", an insulated, internal heat storage that takes in heat produced by recipes
- added coolant ports to the building. this acts like a thermal bridge, continuously cycling coolant
- the thermal capacitor will disperse heat by bringing coolant piped through the building close to its boiling point (but not exceed it). If the coolant is already close to its boiling point, it will offer very little heat dispersal.
- if the thermal capacitor exceeds its capacity due to insufficient cooling, the building will be rendered unusable until all heat has been dispersed. While the capacitor is above 100%, the building will take heat damage.
- added a configurable capacitor heat threshold logic output port
- added a "building is fabricating" logic output port

Fixes:

- geyser types are now always generated in the background to avoid geysers despawning when a world with them is loaded while chemical processing is disabled. (does not affect worldgen when turned off)
- fixed some typos
- fixed plasma furnace considering itself overpressurized while unloading liquid outputs during continuous operation
- added an icon for "filtration medium" in codex recipe panels
