using Blueprints;
using BlueprintsV2.BlueprintsV2.ModAPI;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Actions;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using PeterHan.PLib.PatchManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static Blueprints.Integration;

namespace BlueprintsV2
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(Config));
            new PPatchManager(harmony).RegisterPatchClass(typeof(Integration));
            new PLocalization().Register();

            BlueprintsCreateAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_CREATE_KEY,
                BlueprintsStrings.ACTION_CREATE_TITLE, new PKeyBinding());
            BlueprintsUseAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_USE_KEY,
                BlueprintsStrings.ACTION_USE_TITLE, new PKeyBinding());
            BlueprintsCreateFolderAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_CREATEFOLDER_KEY,
                BlueprintsStrings.ACTION_CREATEFOLDER_TITLE, new PKeyBinding(KKeyCode.Home));
            BlueprintsRenameAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_RENAME_KEY,
                BlueprintsStrings.ACTION_RENAME_TITLE, new PKeyBinding(KKeyCode.End));
            BlueprintsCycleFoldersNextAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_CYCLEFOLDERS_NEXT_KEY,
                BlueprintsStrings.ACTION_CYCLEFOLDERS_NEXT_TITLE, new PKeyBinding(KKeyCode.UpArrow));
            BlueprintsCycleFoldersPrevAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_CYCLEFOLDERS_PREV_KEY,
                BlueprintsStrings.ACTION_CYCLEFOLDERS_PREV_TITLE, new PKeyBinding(KKeyCode.DownArrow));
            BlueprintsCycleBlueprintsNextAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_CYCLEBLUEPRINTS_NEXT_KEY,
                BlueprintsStrings.ACTION_CYCLEBLUEPRINTS_NEXT_TITLE, new PKeyBinding(KKeyCode.RightArrow));
            BlueprintsCycleBlueprintsPrevAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_CYCLEBLUEPRINTS_PREV_KEY,
                BlueprintsStrings.ACTION_CYCLEBLUEPRINTS_PREV_TITLE, new PKeyBinding(KKeyCode.LeftArrow));
            BlueprintsSnapshotAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_SNAPSHOT_KEY,
                BlueprintsStrings.ACTION_SNAPSHOT_TITLE, new PKeyBinding());
            BlueprintsDeleteAction = new PActionManager().CreateAction(BlueprintsStrings.ACTION_DELETE_KEY,
                BlueprintsStrings.ACTION_DELETE_TITLE, new PKeyBinding(KKeyCode.Delete));

            Utilities.AttachFileWatcher();

            new PVersionCheck().Register(this, new SteamVersionChecker());
            Debug.Log("Blueprints fixed loaded: Version " + Assembly.GetExecutingAssembly().GetName().Version + " mod.label.id:" + mod.label.id);


        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            API_Methods.RegisterExtraData();
        }
    }
}
