using System;

namespace BlueprintsV2.ModAPI
{
	internal class API_Consts
	{
		//Obsolete!, lots of overhang for nothing, reverting back to regular conduit flags
		public const string ConduitFlagID = "ConduitFlags";

		/// <summary>
		/// Prevents the "Preconfigure Building" button to show up on your building.
		/// </summary>
		public static readonly Tag SkipPreconfiguration = TagManager.Create("Blueprints_SkipPreconfiguration");
	}
}
