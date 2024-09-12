namespace BlueprintsV2.Visualizers
{
	public interface ICleanableVisual
	{
		int DirtyCell { get; }
		void Clean();
	}
}
