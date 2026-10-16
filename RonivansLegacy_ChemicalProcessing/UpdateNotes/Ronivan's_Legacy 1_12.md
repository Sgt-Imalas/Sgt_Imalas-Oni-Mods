Ronivan's Legacy 1.12

New:

- new chromium refinement chain elements: chromite (ore), ferrochrome, chromium (and molten/gaseous variants), Stainless Steel (elemental bonus values are still somewhat WIP)
- chromite can be acquired from iron meteor showers, as deep drilling results and from higher tier metal sands
- new alloy: Invar (nickel + iron)
- buildings in the building configuration editor now display a status tooltip about their enabled state
- you can now "force enable" buildings in the editor, even if all of their source mods are disabled
- wood boiler now has an element selector that allows switching between wood and plywood
- Simple Oil Refinery, Carbon Boiler and wood boiler now also have a "Any" option in their element selector
- added a "Compressed biomass to plywood" recipe to the plywood press
- added plywood as an alternative ingredient to wood to a few recipes
- added plywood as alternative "burning material" to all recipes that had wood as an option
- added plant husks to the anaerobic digester; total power yield is balanced against the old gas grass rates
- added renewable diesel as alternative fuel for the new biodiesel engine
- Brick is now an alternative building material to ceramic
- CNC machine now works without a duplicant
- added "Simple Drillbit" recipes to crafting table and CNC machine
- added "copy settings" for arc furnace threshold

Changes & Tweaks:

- chemical processing ball crusher mill now opens the liquid pipe overlay automatically on placement start
- ammonia breaker now opens the gas pipe overlay automatically on placement start
- arc furnace now opens the liquid pipe overlay automatically on placement start
- increased building costs of the chemical ball crusher mill to be more in line with the mineral ball crusher mill; added ceramic requirement (600 steel, 400 ceramic)
- reduced steel requirement of mineral ball crusher mill, added refined metal as filler (1000steel -> 600steel, 300 refined metal)
- moved mk2 drillrig and cnc machine to its own dedicated late game tech
- changed mk1 drill to work with new "Simple Drillbits" instead of refined metal ingredients; this allows alternative ingredients to be used with the same drilling recipe
- uninsulated rail compressor storage
- internal water storage of the high tech fridge is now insulated and acts as a source (bottle emptiers only draw from it with auto bottling enabled))
- removed obsolete dlc restrictions on random element results (since Klei removed them in the PPP update)
- increased thermal capacitor of Arc furnace by 10%
- reduced metal sand gambling recipe times (50s -> 30s)
- removed "too heavy" item dropping from Heavy Duty Rails 

Fixes:

- the high pressure gas pumps "can pump" detection will no trigger on gases from the other side of a liquid lock when it is in a vacuum
- fixed glass foundry desc string 
- fixed placement port previews rendering in front of actual ports in their respective overlays
- fixed sulphur recipe selection of sulphuric acid synthesizer not switching the recipe
- fixed mk1 drill refunding its ingredients when the recipe was canceled but had already yielded materials
- removed building insulation from compressor buildings; this will allow them to disperse their own heat generation
- fixed flocculation sieve not removing germs
- fixed arc furnace not working with HPA pipes
- added a fix for attachment slots displaying the tag name and not the localized string (vanilla issue fix)
