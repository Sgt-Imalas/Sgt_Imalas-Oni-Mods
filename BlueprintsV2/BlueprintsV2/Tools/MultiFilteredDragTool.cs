using System.Collections.Generic;
using System.Linq;

namespace BlueprintsV2.Tools
{
    public class MultiFilteredDragTool : DragTool
    {
        public virtual Dictionary<string, ToolParameterMenu.ToggleState> DefaultParameters { get; set; } = new Dictionary<string, ToolParameterMenu.ToggleState>();
        public virtual bool OverlaySynced { get; set; }

        private Dictionary<string, ToolParameterMenu.ToggleState> cachedParameters;
        private bool isSynced;

        public override void OnActivateTool()
        {
            base.OnActivateTool();

            MultiToolParameterMenu.Instance.PopulateMenu(DefaultParameters);
            MultiToolParameterMenu.Instance.SetOverlaySync(OverlaySynced);
            MultiToolParameterMenu.Instance.OnSyncChanged += OnSyncChanged;
            MultiToolParameterMenu.Instance.ShowMenu();

            OverlayScreen.Instance.OnOverlayChanged += OnOverlayChanged;
            OnOverlayChanged(OverlayScreen.Instance.mode);
        }

        public override void OnDeactivateTool(InterfaceTool newTool)
        {
            base.OnDeactivateTool(newTool);

            cachedParameters = null;
            OverlayScreen.Instance.OnOverlayChanged -= OnOverlayChanged;

            MultiToolParameterMenu.Instance.HideMenu();
            MultiToolParameterMenu.Instance.ClearMenu();
            MultiToolParameterMenu.Instance.OnSyncChanged -= OnSyncChanged;
        }

        public virtual void OnSyncChanged(bool synced)
        {
            OverlaySynced = synced;

            if (!synced)
            {
                SetSynced(false);
            }
        }

        public virtual void OnOverlayChanged(HashedString overlay)
        {
            if (OverlaySynced)
            {
                if (overlay == null)
                {
                    SetSynced(false);
                }

                else
                {
                    string filter = null;

                    if (overlay == OverlayModes.Power.ID)
                    {
                        filter = ToolParameterMenu.FILTERLAYERS.WIRES;
                    }

                    else if (overlay == OverlayModes.LiquidConduits.ID)
                    {
                        filter = ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT;
                    }

                    else if (overlay == OverlayModes.GasConduits.ID)
                    {
                        filter = ToolParameterMenu.FILTERLAYERS.GASCONDUIT;
                    }

                    else if (overlay == OverlayModes.SolidConveyor.ID)
                    {
                        filter = ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT;
                    }

                    else if (overlay == OverlayModes.Logic.ID)
                    {
                        filter = ToolParameterMenu.FILTERLAYERS.LOGIC;
                    }

                    if (filter == null)
                    {
                        SetSynced(false);
                    }

                    else
                    {
                        Dictionary<string, ToolParameterMenu.ToggleState> parameters = new Dictionary<string, ToolParameterMenu.ToggleState>(DefaultParameters);
                        foreach (string parameter in parameters.Keys.ToArray())
                        {
                            parameters[parameter] = parameter == filter ? ToolParameterMenu.ToggleState.On : ToolParameterMenu.ToggleState.Disabled;
                        }

                        SetSynced(true);
                        MultiToolParameterMenu.Instance.PopulateMenu(parameters);
                    }
                }
            }
        }

        private void SetSynced(bool synced)
        {
            if (synced == isSynced)
            {
                return;
            }

            if (synced)
            {
                cachedParameters = MultiToolParameterMenu.Instance.GetParameters();
            }

            else
            {
                if (cachedParameters == null)
                {
                    MultiToolParameterMenu.Instance.PopulateMenu(DefaultParameters);
                }

                else
                {
                    MultiToolParameterMenu.Instance.PopulateMenu(cachedParameters);
                    cachedParameters = null;
                }
            }

            isSynced = synced;
        }
    }
}
