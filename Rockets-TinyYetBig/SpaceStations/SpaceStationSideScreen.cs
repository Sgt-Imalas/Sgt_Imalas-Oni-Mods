using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace Rockets_TinyYetBig.SpaceStations
{
    public class SpaceStationSideScreen : SideScreenContent
    {
        public Image icon;
        public KButton viewButton;
        private SpaceStation targetEntity;

        public override void OnSpawn()
        {
            this.viewButton.onClick += new System.Action(this.OnClickView);
        }


        public override float GetSortKey() => 21f;

        public override bool IsValidForTarget(GameObject target) => (UnityEngine.Object)target.GetComponent<SpaceStation>() != (UnityEngine.Object)null;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            titleKey = "STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONSIDESCREEN.TITLE";
        }
        public override void SetTarget(GameObject target)
        {
            UIUtils.ListAllChildren(transform);

            viewButton = UIUtils.TryFindComponent<KButton>(transform, "Contents/TopPanel/Buttons/ViewInteriorButton");
            icon = UIUtils.TryFindComponent<Image>(transform, "Contents/TopPanel/Image");
            UIUtils.TryChangeText(viewButton.transform, "Label", STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONSIDESCREEN.VIEW_WORLD_DESC);

            base.SetTarget(target);
            this.targetEntity = target.GetComponent<SpaceStation>();
            this.icon.sprite = this.targetEntity.GetUISprite();
            if (this.targetEntity == null)
                return;
            WorldContainer component = ClusterManager.Instance.GetWorld(targetEntity.SpaceStationInteriorId);
            bool flag = (UnityEngine.Object)component != (UnityEngine.Object)null;
            this.viewButton.isInteractable = true;
            
            this.viewButton.GetComponent<ToolTip>().SetSimpleTooltip((string)STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONSIDESCREEN.VIEW_WORLD_TOOLTIP);
        }

        private void OnClickView()
        {
            WorldContainer component = ClusterManager.Instance.GetWorld(targetEntity.SpaceStationInteriorId);
            if (!component.IsDupeVisited)
                component.LookAtSurface();
            ClusterManager.Instance.SetActiveWorld(component.id);
            ManagementMenu.Instance.CloseAll();
        }
    }
}
