using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ClusterTraitGenerationManager.SettinPrefabComps
{
    internal class CheckBoxHandler:KMonoBehaviour, ICustomPlanetoidSetting
    {
        MultiToggle CheckBox;
        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            UIUtils.FindAndRemove<NewGameSettingToggle>(transform);
            CheckBox = UIUtils.TryFindComponent<MultiToggle>(transform, "Checkbox");
            CheckBox.onClick = null;
        }

        public void SetAction(System.Action action)
        {
            CheckBox.onClick = action;

        }

        public override void OnCleanUp()
        {
            base.OnCleanUp();
        }

        public void HandleData(object data)
        {
            bool value = (bool)data;
            CheckBox.ChangeState(value ? 1 : 0);
        }
    }
}
