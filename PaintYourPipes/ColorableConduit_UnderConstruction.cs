using KSerialization;

namespace PaintYourPipes
{
	internal class ColorableConduit_UnderConstruction : KMonoBehaviour
	{
		public static string BuildFromColor = "FFFFFF";
		public static bool HasColorOverride = false;


		[Serialize]
		public string ColorHex = string.Empty;
		[Serialize]
		public bool HasData = false;

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (HasColorOverride)
			{
				HasData = true;
				ColorHex = BuildFromColor;
			}

		}
	}
}
