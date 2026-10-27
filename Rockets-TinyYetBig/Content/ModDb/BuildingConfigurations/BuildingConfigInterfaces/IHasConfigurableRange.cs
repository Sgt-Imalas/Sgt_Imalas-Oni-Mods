namespace Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations.BuildingConfigInterfaces
{
	public interface IHasConfigurableRange
	{
		int GetTileRange();
		void SetTileRange(int tiles);
#nullable enable
		string GetDescriptorTextRange();
		public Tuple<int, int>? GetLimitsRange();
		string RangeUnitLabel();
		string RangeTooltip();
#nullable disable
	}
}
