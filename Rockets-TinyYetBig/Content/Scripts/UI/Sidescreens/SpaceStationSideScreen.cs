using Rockets_TinyYetBig.SpaceStations;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.UI.Sidescreens
{
    public class SpaceStationSideScreen : SideScreenContent
    {
        public Image icon;
        public KButton viewButton;
        private SpaceStation targetEntity;

        public override void OnSpawn()
        {
            viewButton.onClick += new System.Action(OnClickView);
        }


        public override float GetSortKey() => 21f;

        public override bool IsValidForTarget(GameObject target) => target.GetComponent<SpaceStation>() != null;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            titleKey = "STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONSIDESCREEN.TITLE";
        }
        public override void SetTarget(GameObject target)
        {
            //UIUtils.ListAllChildren(transform);

            viewButton = UIUtils.TryFindComponent<KButton>(transform, "Contents/TopPanel/Buttons/ViewInteriorButton");
            icon = UIUtils.TryFindComponent<Image>(transform, "Contents/TopPanel/Image");
            UIUtils.TryChangeText(viewButton.transform, "Label", STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONSIDESCREEN.VIEW_WORLD_DESC);

            base.SetTarget(target);
            targetEntity = target.GetComponent<SpaceStation>();
            icon.sprite = targetEntity.GetUISprite();
            if (targetEntity == null)
                return;
            WorldContainer component = ClusterManager.Instance.GetWorld(targetEntity.SpaceStationInteriorId);
            bool flag = component != null;
            viewButton.isInteractable = true;

            viewButton.GetComponent<ToolTip>().SetSimpleTooltip((string)STRINGS.UI_MOD.UISIDESCREENS.SPACESTATIONSIDESCREEN.VIEW_WORLD_TOOLTIP);
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
