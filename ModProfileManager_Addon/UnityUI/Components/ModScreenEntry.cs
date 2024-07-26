
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace ModProfileManager_Addon.UnityUI.Components
{
    internal class ModScreenEntry : KMonoBehaviour
    {
        FButton ChangeSortOrderBt;
        FToggle2 ModEnabled;
        public KMod.Mod TargetMod;
        public KMod.Label? MissingLabel = null;
        LocText ModName;

        GameObject PlibConfigHighlight;
        ToolTip plibTooltip;

        public string Name = string.Empty;


        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ModName = transform.Find("Label").gameObject.GetComponent<LocText>();
            ModEnabled = transform.Find("Background").gameObject.AddComponent<FToggle2>();
            ModEnabled.SetCheckmark("Checkmark");
            PlibConfigHighlight = transform.Find("HasPLibData").gameObject;
            plibTooltip = UIUtils.AddSimpleTooltipToObject(PlibConfigHighlight, "");
            ChangeSortOrderBt = transform.Find("ReorderButton").gameObject.AddComponent<FButton>();

            var TypeGO = transform.Find("ModType").gameObject;
            if (TargetMod != null)
            {
                Name = TargetMod.label.title;
                this.gameObject.name = Name;

                ChangeSortOrderBt.OnClick += () =>
                {
                    ModAssets.ShowModIndexShiftDialogue(TargetMod, ChangeSortOrderBt.gameObject);
                };

                var bt =
                    TypeGO.AddOrGet<FButton>();
                bt.OnClick += TargetMod.on_managed;
                bt.normalColor = TargetMod.IsLocal ? ModAssets.Colors.Blue : ModAssets.Colors.Red;
                bt.hoverColor = TargetMod.IsLocal ? UIUtils.Lighten(ModAssets.Colors.Blue, 20):UIUtils.Lighten(ModAssets.Colors.Red, 20);

                ModName?.SetText(Name);
                var label = transform.Find("ModType/Label").gameObject.GetComponent<LocText>();
                if (TargetMod.IsLocal)
                {
                    TypeGO.gameObject.GetComponent<Image>().color = ModAssets.Colors.Blue;
                    label.SetText(STRINGS.UI.LOCAL_MOD);
                }
                else
                {
                    label.SetText(global::STRINGS.UI.PLATFORMS.STEAM);
                }
                ModEnabled.OnClick += (active) =>
                {
                    ModAssets.ToggleModActive(TargetMod.label, active);
                };
            }
            else if (MissingLabel != null)
            {
                var m_missingLabel = MissingLabel.Value;
                bool isMissingSteam = ulong.TryParse(m_missingLabel.id, out ulong steamId);
                ChangeSortOrderBt.SetInteractable(false);

                Name = m_missingLabel.title;
                this.gameObject.name = Name;

                ModName?.SetText(Name);
                TypeGO.GetComponent<Image>().color = isMissingSteam ? ModAssets.Colors.DarkRed : ModAssets.Colors.DarkBlue;

                var label = transform.Find("ModType/Label").gameObject.GetComponent<LocText>();
                label.SetText(STRINGS.UI.MISSING);
                UIUtils.AddSimpleTooltipToObject(label.gameObject, isMissingSteam ? STRINGS.UI.STEAM_MISSING_TOOLTIP : STRINGS.UI.LOCAL_MISSING_TOOLTIP);

                if (isMissingSteam)
                {
                    var bt =
                    TypeGO.AddOrGet<FButton>();
                    bt.OnClick += () => ModAssets.SubToMissingMod(steamId);
                    bt.normalColor = ModAssets.Colors.DarkRed;
                    bt.hoverColor = UIUtils.Lighten(ModAssets.Colors.Red, 10);
                }

                ModEnabled.SetInteractable(false);
            }
        }

        public void Refresh(bool enabled, bool hasPlibConfig, string plibData)
        {
            PlibConfigHighlight.SetActive(hasPlibConfig);
            if (hasPlibConfig)
            {
                plibTooltip.SetSimpleTooltip(STRINGS.UI.PLIB_CONFIG_FOUND + "\n" + plibData);
            }
            ModEnabled.SetOnFromCode(enabled);
            ChangeSortOrderBt.SetInteractable(enabled);

        }
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
    }
}
