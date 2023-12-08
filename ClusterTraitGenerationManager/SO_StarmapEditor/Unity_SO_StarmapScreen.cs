using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtilLibs.UIcmp;
using static KInputController;
using static STRINGS.DUPLICANTS.PERSONALITIES;
using static STRINGS.UI.CLUSTERMAP.ROCKETS;

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
        ScrollRect ScrollRect;

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
        //float speed = 0.1f;

        //public void FixedUpdate()
        //{
        //    if (leftPan == 0 && rightPan == 0 && upPan == 0 && downPan == 0) return;

        //    float contentHeight = ScrollRect.content.sizeDelta.y;
        //    float contentWidth = ScrollRect.content.sizeDelta.x;
        //    float contentShiftVert = speed * (upPan+downPan) * Time.deltaTime;
        //    float contentShiftHoriz = speed * (leftPan+rightPan) * Time.deltaTime;
        //    ScrollRect.verticalNormalizedPosition += contentShiftVert / contentHeight;
        //    ScrollRect.horizontalNormalizedPosition += contentShiftHoriz / contentHeight;



        //}


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

            ScrollRect = transform.Find("HorizontalLayout/ItemInfo/ScrollArea").gameObject.GetComponent<ScrollRect>();



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
