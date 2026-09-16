Ronivan's Legacy 1.19 - The Great Compression:

New:
- all animations and texture assets are now compressed, reducing VRAM consumption and mod size
- added Chlorine Compressor building (marked as newly added)
- bioplastic printer now allows to alternatively print gaskets directly
- acids now kill germs
- added integration and fallback for NoManualDelivery mod
- all generator buildings + custom steam turbine now have a rate multiplier setting in the building config that affects wattage and element throughput
- added previously missing Metallurgy exclusive recipe to the Soil mixer (Phosphorite+Dirt+Sulphur+pWater->Fertilizer)

Changes & Tweaks:
- tweaked cnc machine animation, it now displays the main ingredient during crafting
- wood selection for vanilla buildings is now disabled if Sanchozz "choose ingredient" mod is active (it adds the same feature but with multi select)
- soda fountain change to soda gas input (nitrogen+co2) is now only active if chemproc is turned on
- adjusted raw gas refinery port coloring
- dupes are now more avoidant of stepping into acids

Fixes:
- fixed bioplastic printer progress meter
- fixed modded rails not allowing manual emptying via "empty pipe" tool
- fixed preview port visualizers showing up in the wrong overlays (ball crusher mill & flocculation sieve)
- fixed overpressurized regular pipes never taking damage from it
- fixed brass recipe description
- fixed buildings with selectable ingredients sometimes dropping their stored materials on load