namespace DupePrioPresetManager.Serializables
{
	internal interface IUtilityPreset
	{
		public string IFileName { get; set; }
		public string IConfigName { get; set; }
		public string ParentFolder { get;}

		public void OpenPopUpToChangeName();
	}
}
