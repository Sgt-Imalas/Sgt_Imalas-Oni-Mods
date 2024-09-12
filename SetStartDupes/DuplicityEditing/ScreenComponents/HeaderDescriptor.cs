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
			label.SetText(TextLeft);
			labelRight = transform.Find("Output").GetComponent<LocText>();
			labelRight.SetText(TextRight);
		}
	}
}
