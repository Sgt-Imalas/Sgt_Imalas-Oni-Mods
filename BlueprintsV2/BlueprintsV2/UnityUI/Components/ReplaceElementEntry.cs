using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
    internal class ReplaceElementEntry:KMonoBehaviour
    {
        public Tag targetTag;
        public System.Action<Tag> OnSelectElement;
        LocText ElementName;
        Image ElementIcon;
        FButton button;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            ElementName = transform.Find("Label").gameObject.GetComponent<LocText>();
            ElementIcon = transform.Find("CarePackageSprite").gameObject.GetComponent<Image>();
            button = gameObject.AddComponent<FButton>();
            if (targetTag != null)
            {
                var prefab = Assets.TryGetPrefab(targetTag);
                if(prefab.TryGetComponent<KBatchedAnimController>(out var kbac))
                {
                    ElementIcon.sprite = Def.GetUISpriteFromMultiObjectAnim(kbac.animFiles[0]); 
                }

                ElementName?.SetText(prefab.GetProperName());
                button.OnClick += OnClick;
                UIUtils.AddSimpleTooltipToObject(this.gameObject, GameUtil.GetMaterialTooltips(targetTag));
            }
        }
        void OnClick()
        {
            OnSelectElement?.Invoke(targetTag);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }
    }
}
