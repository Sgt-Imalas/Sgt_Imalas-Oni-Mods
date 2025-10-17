Ronivan's Legacy 1.8

New:

- The starting asteroid now tries spawning an extra liquid ammonia vent. It will be spawned after story traits, thus not affecting the wordlgen of other geysers. Only active with chemical processing - I.O.
- zinc ore is now classified as a starting ore; this adds a power bank recipe for zinc
- modded elements now show up in care packages
- added chlorine filter building; works like co2 filter, but for chlorine gas

Changes & Tweaks:

- decreased oil consumption of algae grower basin
- partially reverted algae vat rates to original ratio, with reduced water consumption
- structure tiles now allow overlap with heavy-duty rails and heavy watt wires
- increased average outputs of several modded geysers
- all 3 wooden tiles now allow wood as primary material, additionally to raw minerals
- doubled electric boiler throughput (1kg -> 2kg), decreased power requirement (850W -> 720W)
- added pWater output to bioplastic printer and recipe; this brings back the remaning water that is otherwise lost on a melting transition of bioplastic
- selective arc furnace internal storages are now insulated; this prevents ingredients like sulphur from melting inside the machine
- increased large freezer default storage capacities (200kg -> 300kg)
- removed magic salt from Ammonium Water recipe chain; altered thermal desalinator recipe
- reduced decor penalty of concrete (25% -> 15%)
- tweaked nitric acid synthesizer: it now has 2 chemically accurate recipes to choose between
- non-staged petrochemical refinery buildings now stack on top of each other, but require require floor otherwise

Fixes:

- fixed mercury vent and raw natural gas vent hitbox
- sour water output of staged oil refinery now comes out at building temperature
- isopropane recipe now produces the gas at a set temperature
- fixed missing floor requirements of ammonia breaker and sour water stripper
- endothermic unit now allows delivery of material when disabled
- fixed various machine fetches being classified as life support tasks
- fixed SHC and TC of solid nitrogen
- fixed meter for plastifiable liquids on custom plastic printers not working
- fixed MK2 drill ignoring logic inputs
- fixed small home fridge costing as much as a large fridge (200kg ref. metal -> 100kg ref. metal)
- fixed cost of med. reservoirs being the same as small reservoirs. Its now 3/4 of a vanilla reservoir (100kg -> 300kg), also reduced cost of wall reservoirs to 80kg
- fixed Ammonium Water transition percentages being flipped (intended rate: 89% water, 11% ammonia, this was inverted previously)
- fixed Codex page of fullerene crashing when chemical processing - I.O. was not enabled
