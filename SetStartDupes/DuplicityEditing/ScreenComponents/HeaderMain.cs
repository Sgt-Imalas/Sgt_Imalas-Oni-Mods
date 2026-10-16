namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
	internal class HeaderMain : KMonoBehaviour
	{
		public string Text;
		LocText label;
		public override void OnPrefabInit()
		{
			base.OnSpawn();
			label = transform.Find("Label").GetComponent<LocText>();
		}
		public override void OnSpawn()
		{
			label.SetText(Text);
			base.OnSpawn();
		}
	}
}
