using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
    internal class DeletableListEntry:KMonoBehaviour
    {
        public string Text, Tooltip;
        LocText label;
        public System.Action OnDeleteClicked;
        FButton deleteButton;
        public UnityEngine.Color? backgroundColor =null;

        public override void OnPrefabInit()
        {
            base.OnSpawn();
            label = transform.Find("Label").GetComponent<LocText>();
            label.SetText(Text);

            deleteButton = transform.Find("DeleteButton").gameObject.AddOrGet<FButton>();
            deleteButton.OnClick += OnDeleteClicked;

            if(Tooltip != null && Tooltip.Length > 0)
            {
                UIUtils.AddSimpleTooltipToObject(this.gameObject, Tooltip);
            }

            if (backgroundColor.HasValue)
            {
                transform.Find("Background").gameObject.GetComponent<Image>().color = backgroundColor.Value;
            }
        }
    }
}
