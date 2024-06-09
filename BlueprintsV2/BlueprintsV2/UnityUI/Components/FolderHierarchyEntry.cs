using BlueprintsV2.BlueprintsV2.BlueprintData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.UIcmp;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components
{
    internal class FolderHierarchyEntry : KMonoBehaviour
    {
        public BlueprintFolder folder;

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
                Label.SetText(folder.Name);
                button.OnClick += OnEntryClicked;
            }
        }
    }
}
