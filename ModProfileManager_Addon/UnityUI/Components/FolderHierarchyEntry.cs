using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.UIcmp;

namespace ModProfileManager_Addon.UnityUI.Components
{
    internal class FolderHierarchyEntry : KMonoBehaviour
    {
        public SaveGameModList folder;

        public System.Action OnEntryClicked;
        FButton button;
        LocText Label;

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Label = transform.Find("Label").gameObject.GetComponent<LocText>();
            button = gameObject.AddComponent<FButton>();

            if (folder != null)
            {
                Label.SetText(folder.ModlistPath);
                button.OnClick += OnEntryClicked;
            }
        }
    }
}
