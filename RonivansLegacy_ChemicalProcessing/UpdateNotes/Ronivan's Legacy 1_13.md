Ronivan's Legacy 1.13

New:

- "Backwalls Hide Pipes" config setting: pipes, rails and wires will be behind any backwall building when enabled (active by default)
- added optional pipe output port for rayon loom steam
- added optional "combustible liquid" input for CNC machine
- added oil well raw gas output information to the codex
- expeller press now also holds all recipes of the plant pulverizer
- added vegetable oil output multiplier in config
- added optional water input port to anaerobic digester
- added optional water input port to cement mixer

Changes & Tweaks:

- moved the water output of the custom steam turbine down one tile to make pipe construction more accessible
- "metal rich" and "metal poor" traits now affect the modded ores of the mod (only affects new worlds)
- optimized some heavy-duty rail patches and made them conditional to the mod setting (patch only runs if heavy-duty rails are enabled)
- optimized structure tile heavy watt wire bypass patches
- added additional "steel-like" substitutes in certain recipes
- added icon for "any water" in codex
- increased the base recipe duration of the plasma furnace to reduce its power positivity; 10s->20s
- decreased the power consumption of the advanced metal refinery to make it more competitive with the plasma furnace; 3kw->2.4kw
- slightly decreased the renewable diesel consumption of the biodiesel generator
- increased the throughput of the seakomb expeller press recipe; 10->25 plants per recipe
- changed the purpose of the algae digestion recipe towards algae->dirt production, reduced its NatGas output to no longer be a x6 multiplier on raw power production in the resource chain VegOil->Algae(AlgaeGrower)->NatGas(Digester)
- moved Carbon Recycling Unit into a more appropriate later game tech tier
- moved Plasma Furnace into a new later game tech tier that requires all 4 science types
- removed uncrushed rock cement recipe from cement mixer

Fixes:

- fixed solid rocket wall port not outputting at higher rate onto heavy-duty rails
- fixed vanilla and modded nuclear reactors consuming endless amounts of coolant when using mixed liquids as coolant (vanilla issue bugfix)
- fixed reactor meters not working correctly with mixed coolants
- fixed base game biodiesel engine crashing on starmap selection of the rocket
- fixed slime vat not having a dirt delivery for its secondary converter
- fixed simple steel drillbit recipe making 1 instead of 2 drillbits in the CNC machine, making it more expensive than intended
- fixed broken "Not enough material" text for steel+hardened alloy+tungsten requirement
- fixed some edge cases of the oil well producing raw gas while clogged or inactive
- fixed a broken link and some inaccuracies in the staged raw gas refinery strings
- fixed a missalignment in the tech screen in base game
- adjusted description of CNC machine to reflect change to auto operation

