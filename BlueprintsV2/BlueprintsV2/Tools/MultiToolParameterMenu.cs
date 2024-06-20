
using BlueprintsV2.BlueprintData;
using PeterHan.PLib.UI;
using ProcGen.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.Tools
{
    [AddComponentMenu("KMonoBehaviour/scripts/ToolParameterMenu")]
    public class MultiToolParameterMenu : KMonoBehaviour
    {
        public delegate void SyncChanged(bool synced);

        public static MultiToolParameterMenu Instance;

        public event SyncChanged OnSyncChanged;

        private readonly Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();
        private GameObject content;
        private GameObject widgetContainer;
        private Dictionary<string, ToolParameterMenu.ToggleState> parameters;

        private MultiToggle syncMultiToggle;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();

            GameObject baseContent = ToolMenu.Instance.toolParameterMenu.content;
            GameObject baseWidgetContainer = ToolMenu.Instance.toolParameterMenu.widgetContainer;

            content = Util.KInstantiateUI(baseContent, baseContent.transform.parent.gameObject);
            content.transform.GetChild(1).gameObject.SetActive(false);

            var buttonsPanel = new PRelativePanel
            {
                BackColor = PUITuning.Colors.ButtonPinkStyle.inactiveColor
            };

            var allButton = new PButton
            {
                Text = "All",
                OnClick = (_) => SetAll(ToolParameterMenu.ToggleState.On)
            }.SetKleiPinkStyle();

            var noneButton = new PButton
            {
                Text = "None"
            };

            noneButton.OnClick += source =>
            {
                Instance.SetAll(ToolParameterMenu.ToggleState.Off);
            };

            PCheckBox syncCheckBox = new PCheckBox
            {
                Text = "Auto. Sync"
            };

            syncCheckBox.SetKleiPinkStyle();
            syncCheckBox.OnRealize += realized =>
            {
                syncMultiToggle = realized.GetComponent<MultiToggle>();
            };
            syncCheckBox.OnChecked += (source, state) =>
            {
                if (state == PCheckBox.STATE_UNCHECKED)
                {
                    syncMultiToggle.ChangeState(PCheckBox.STATE_CHECKED);
                }

                else if (state == PCheckBox.STATE_CHECKED)
                {
                    syncMultiToggle.ChangeState(PCheckBox.STATE_UNCHECKED);
                }

                OnSyncChanged?.Invoke(syncMultiToggle.CurrentState == PCheckBox.STATE_CHECKED);
            };

            buttonsPanel.AddChild(allButton);
            buttonsPanel.SetLeftEdge(allButton, 0)
                .SetRightEdge(allButton, 0.25F);

            buttonsPanel.AddChild(noneButton);
            buttonsPanel.SetLeftEdge(noneButton, 0.25F)
                .SetRightEdge(noneButton, 0.5F);

            buttonsPanel.AddChild(syncCheckBox);
            buttonsPanel.SetLeftEdge(syncCheckBox, 0.5F)
                .SetRightEdge(syncCheckBox, 1F);

            widgetContainer = Util.KInstantiateUI(baseWidgetContainer, content, true);
            buttonsPanel.AddTo(content, 3);

            content.SetActive(false);
        }

        public void PopulateMenu(Dictionary<string, ToolParameterMenu.ToggleState> inputParameters)
        {
            ClearMenu();
            parameters = new Dictionary<string, ToolParameterMenu.ToggleState>(inputParameters);

            foreach (KeyValuePair<string, ToolParameterMenu.ToggleState> parameter in inputParameters)
            {
                GameObject widetPrefab = Util.KInstantiateUI(ToolMenu.Instance.toolParameterMenu.widgetPrefab, widgetContainer, true);
                widetPrefab.GetComponentInChildren<LocText>().text = Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + parameter.Key);

                MultiToggle toggle = widetPrefab.GetComponentInChildren<MultiToggle>();
                switch (parameter.Value)
                {
                    case ToolParameterMenu.ToggleState.On:
                        toggle.ChangeState(1);
                        break;

                    case ToolParameterMenu.ToggleState.Disabled:
                        toggle.ChangeState(2);
                        break;

                    default:
                        toggle.ChangeState(0);
                        break;
                }

                toggle.onClick += () =>
                {
                    foreach (KeyValuePair<string, GameObject> widget in widgets)
                    {
                        if (widget.Value == toggle.transform.parent.gameObject)
                        {
                            if (parameters[widget.Key] == ToolParameterMenu.ToggleState.Disabled)
                            {
                                break;
                            }

                            if (parameters[widget.Key] == ToolParameterMenu.ToggleState.On)
                            {
                                parameters[widget.Key] = ToolParameterMenu.ToggleState.Off;
                            }

                            else
                            {
                                parameters[widget.Key] = ToolParameterMenu.ToggleState.On;
                            }

                            OnChange();
                            break;
                        }
                    }
                };

                widgets.Add(parameter.Key, widetPrefab);
            }

            content.SetActive(true);
        }

        public void SetAll(ToolParameterMenu.ToggleState toggleState)
        {
            foreach (string key in parameters.Keys.ToList())
            {
                parameters[key] = toggleState;
            }

            OnChange();
        }

        public virtual Dictionary<string, ToolParameterMenu.ToggleState> GetParameters()
        {
            return new Dictionary<string, ToolParameterMenu.ToggleState>(parameters);
        }
        public bool BuildingDefAllowedWithCurrentFilters(BuildingDef def)
        {
            //SgtLogger.l(def.ObjectLayer.ToString());
            return AllowedLayer(def.ObjectLayer) || AllowedOverlay(def.ViewMode);
        }
        public bool AllowedOverlay(HashedString viewMode)
        {
            if (viewMode == OverlayModes.Power.ID)
            {
                return AllowedLayer(ObjectLayer.Wire);
            }

            else if (viewMode == OverlayModes.LiquidConduits.ID)
            {
                return AllowedLayer(ObjectLayer.LiquidConduit);
            }

            else if (viewMode == OverlayModes.GasConduits.ID)
            {
                return AllowedLayer(ObjectLayer.GasConduit);
            }

            else if (viewMode == OverlayModes.SolidConveyor.ID)
            {
                return AllowedLayer(ObjectLayer.SolidConduit);
            }

            else if (viewMode == OverlayModes.Logic.ID)
            {
                return AllowedLayer(ObjectLayer.LogicGate);
            }
            return false;
        }

        public bool AllowedLayer(ObjectLayer objectLayer)
        {
            var currentParams = GetParameters();
            ToolParameterMenu.ToggleState toggleState = default;
            switch (objectLayer)
            {
                case ObjectLayer.Building:
                case ObjectLayer.FoundationTile:
                case ObjectLayer.AttachableBuilding:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.BUILDINGS, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case ObjectLayer.Backwall:
                case ObjectLayer.Canvases:
                case ObjectLayer.ReplacementBackwall:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.BACKWALL, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case ObjectLayer.GasConduit:
                case ObjectLayer.GasConduitConnection:
                case ObjectLayer.GasConduitTile:
                case ObjectLayer.ReplacementGasConduit:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case ObjectLayer.LiquidConduit:
                case ObjectLayer.LiquidConduitConnection:
                case ObjectLayer.LiquidConduitTile:
                case ObjectLayer.ReplacementLiquidConduit:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case ObjectLayer.SolidConduit:
                case ObjectLayer.SolidConduitConnection:
                case ObjectLayer.SolidConduitTile:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case ObjectLayer.Wire:
                case ObjectLayer.WireConnectors:
                case ObjectLayer.WireTile:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.WIRES, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case ObjectLayer.LogicGate:
                case ObjectLayer.LogicWire:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.LOGIC, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case ObjectLayer.DigPlacer:
                    return currentParams.TryGetValue(ToolParameterMenu.FILTERLAYERS.DIGPLACER, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;

                case SolidTileFiltering.ObjectLayerFilterKey: //for auto digging
                    return currentParams.TryGetValue(SolidTileFiltering.StoreNonSolidsOptionID, out toggleState) && toggleState == ToolParameterMenu.ToggleState.On;
            }
            return false;
        }

        public void SetOverlaySync(bool synced)
        {
            if (syncMultiToggle != null)
            {
                syncMultiToggle.ChangeState(synced ? PCheckBox.STATE_CHECKED : PCheckBox.STATE_UNCHECKED);
            }
        }

        private void OnChange()
        {
            foreach (KeyValuePair<string, GameObject> widget in widgets)
            {
                switch (parameters[widget.Key])
                {
                    case ToolParameterMenu.ToggleState.On:
                        widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(1);
                        continue;

                    case ToolParameterMenu.ToggleState.Off:
                        widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(0);
                        continue;

                    case ToolParameterMenu.ToggleState.Disabled:
                        widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(2);
                        continue;
                }
            }
        }

        public void ClearMenu()
        {
            content.SetActive(false);

            foreach (KeyValuePair<string, GameObject> widget in widgets)
            {
                Util.KDestroyGameObject(widget.Value);
            }

            widgets.Clear();
        }

        public void ShowMenu()
        {
            content.SetActive(true);
        }

        public void HideMenu()
        {
            content.SetActive(false);
        }

        public static void CreateInstance()
        {
            GameObject parameterMenu = new GameObject("", typeof(MultiToolParameterMenu));
            parameterMenu.transform.SetParent(ToolMenu.Instance.toolParameterMenu.transform.parent);
            parameterMenu.gameObject.SetActive(true);
            parameterMenu.gameObject.SetActive(false);

            Instance = parameterMenu.GetComponent<MultiToolParameterMenu>();
        }

        public static void DestroyInstance()
        {
            Instance = null;
        }

    }
}
