using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs.UIcmp;
using static STRINGS.DUPLICANTS.PERSONALITIES;

namespace ClusterTraitGenerationManager.SO_StarmapEditor
{
    internal class Unity_SO_StarmapScreen : FScreen
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // Remove unused private members
        new bool ConsumeMouseScroll = true; // do not remove!!!!
#pragma warning restore CS0414 // Remove unused private members
#pragma warning restore IDE0051 // Remove unused private members
        public static Unity_SO_StarmapScreen Instance = null;
        public HexGrid Grid;

        GameObject HexGridContainer;

        public bool CurrentlyActive = false;
        private bool HoveringPrio = false;


        public static void ShowWindow()
        {
            if (Instance == null)
            {
                var screen = Util.KInstantiateUI(ModAssets.SO_StarmapScreen, FrontEndManager.Instance.gameObject, true);
                Instance = screen.AddOrGet<Unity_SO_StarmapScreen>();
                Instance.Init();
            }
            Instance.Show(true);
            Instance.ConsumeMouseScroll = true;
            Instance.transform.SetAsLastSibling();
        }

        private bool init;
        private System.Action OnCloseAction;

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.TryConsume(Action.MouseRight))
            {
                this.Show(false);
            }
            if (e.TryConsume(Action.Escape))
            {
                this.Show(false);
            }
            if (e.TryConsume(Action.PanUp))
            {
                scroll
            }
            
            base.OnKeyDown(e);
        }

        static async Task ExecuteWithDelay(int ms, System.Action action)
        {
            await Task.Delay(ms);
            action.Invoke();
        }

        private void Init()
        {
            HexGridContainer = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content").gameObject;
            Grid = HexGridContainer.AddComponent<HexGrid>();
          
            Grid.EntryPrefab =       transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/HexagonBG").gameObject;
            Grid.DraggablePrefab = transform.Find("HorizontalLayout/ItemInfo/ScrollArea/Content/HexDrag").gameObject;

            Grid.EntryPrefab.SetActive(false);
            Grid.DraggablePrefab.SetActive(false);



            init = true;
        }

        public override void OnShow(bool show)
        {
            base.OnShow(show);
            var currentLayout = CGSMClusterManager.GeneratedLayout;
            Grid.MapRadius = currentLayout.numRings;
            Grid.InitGrid();
            Grid.InitPositions();
            if (!init)
            {
                Init();
            }

            if (show)
            {
                CurrentlyActive = show;
            }
            else
            {
                DeactivateStatusWithDelay(600);
            }
        }
        async Task DeactivateStatusWithDelay(int ms)
        {
            await Task.Delay(ms);
            CurrentlyActive = false;
        }
    }
}
