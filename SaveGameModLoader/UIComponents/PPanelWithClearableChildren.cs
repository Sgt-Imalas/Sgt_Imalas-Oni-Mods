using PeterHan.PLib.UI;

namespace SaveGameModLoader.UIComponents
{
	internal class PPanelWithClearableChildren : PPanel
	{
		public PPanelWithClearableChildren(string name) : base(name) { }
		public void ClearChildren() => base.children.Clear();
	}
}
