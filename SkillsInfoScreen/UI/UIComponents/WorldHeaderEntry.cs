using UnityEngine.UI;
using UtilLibs.UI.FUI;

namespace SkillsInfoScreen.UI.UIComponents
{
	internal class WorldHeaderEntry : KMonoBehaviour
	{
		public LocText Label;
		public Image Icon;
		public WorldContainer World;

		public FExpandToggle Toggle;

		public bool Expanded => Toggle.Expanded;

		internal void Init(WorldContainer worldContainer, System.Action RefreshAction)
		{
			Label = GetComponentInChildren<LocText>();
			Icon = transform.Find("IconContainer/Icon").gameObject.GetComponent<Image>();
			World = worldContainer;
			Toggle = transform.Find("SortButton").gameObject.AddOrGet<FExpandToggle>();
			Toggle.OnChange = _ => RefreshAction.Invoke();
			Refresh();
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			Refresh();
		}

		internal void Refresh()
		{
			Label?.SetText(World.GetProperName());
			Icon?.sprite = World.GetComponent<ClusterGridEntity>()?.GetUISprite();
		}
	}
}
