using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ClusterTraitGenerationManager.SettinPrefabComps
{
    internal class CycleHandler: KMonoBehaviour,ICustomPlanetoidSetting
    {
        KButton next;
        KButton prev;
        LocText label;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            UIUtils.FindAndRemove<NewGameSettingList>(transform);
            next = UIUtils.TryFindComponent<KButton>(transform, "Arrow_Right");
            prev = UIUtils.TryFindComponent<KButton>(transform, "Arrow_Left");
            label = UIUtils.TryFindComponent<LocText>(transform, "Label");
            next.ClearOnClick();
            prev.ClearOnClick();
        }

        public void SetActions(System.Action prevAction, System.Action nextAction)
        {
            next.onClick += nextAction;
            prev.onClick += prevAction;
        }
        public override void OnCleanUp()
        {

            base.OnCleanUp();
        }

        public void HandleData(object data)
        {
            label.text= data.ToString();
        }

        public void ToggleInteractable(bool interactable)
        {
            next.interactable= interactable;
            prev.interactable= interactable;
        }
    }
}
