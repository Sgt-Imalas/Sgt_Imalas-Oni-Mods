using BlueprintsV2.BlueprintData;
using UtilLibs.UIcmp;

namespace BlueprintsV2.UnityUI.Components
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
