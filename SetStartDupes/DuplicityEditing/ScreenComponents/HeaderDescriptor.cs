namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
	internal class HeaderDescriptor : KMonoBehaviour
	{
		public string TextLeft, TextRight;
		LocText label, labelRight;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			label = transform.Find("Label").GetComponent<LocText>();
			labelRight = transform.Find("Output").GetComponent<LocText>();
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			labelRight.SetText(TextRight);
			label.SetText(TextLeft);
		}
	}
}
