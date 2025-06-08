## Sgt_Imalas-Oni-Mods
Mods for Oxygen Not Included and ONI - Spaced Out.
 (The List of mods is probably not up to date)

## How to build this Repository
1. clone/download repository and open it in visual studio
2. make a copy of the `Directory.Build.props.default` file and name it `Directory.Build.props.user`, then adjust the variables "ModFolder" and "GameLibsFolder" inside of the copy to reference your local dev folder and your game folder. This will relink all references to the game assemblies.
3. run "clean" on the 1_CycleComma project. This runs the publicise task to create publicised versions of the game assembly (all variables and functions are made public). If it does not work, try restoring NuGet packages on that mod.
4. Done. All mods should now be able to compile properly and be copied to the dev-folder on completion. If you want to make a new mod and use this repository as a base, make sure to allow "Unsafe Code" in the new mods project settings as this is required for using publiciser (Update; this should now happen automatically via solution setting). A project template file is included with "UpdatedOniTemplate.zip".


## Mods in this Repository
My mods can be found on the [Steam Workshop](https://steamcommunity.com/id/Sgt_Imalas/myworkshopfiles/?p=1&numperpage=30).

For direct downloads, there are both [nightly builds](https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/tag/AllMods_Automated_Build_Nightly) and [full releases](https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases/tag/AllMods_Automated_Build_FullRelease).


Downloads for older ONI versions can also be found in [releases](https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/releases).
