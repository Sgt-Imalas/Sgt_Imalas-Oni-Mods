using System;

public class BlueprintsV2_API_Tags
{
	/// <summary>
	/// Prevents the "Preconfigure Building" button to show up on your building.
	/// </summary>
	public static readonly Tag SkipPreconfiguration = TagManager.Create("Blueprints_SkipPreconfiguration");
}
public class BlueprintsV2.ModAPI.API_Methods
{

		/// <summary>
		/// Register a Building Recipe Tag that has NonSolid materials selectable, example: decor pack I stained glass tiles
		/// </summary>
		public static void RegisterNonSolidTag(Tag tag)

		/// <summary>
		/// Register a separate check for buildings to be considered capturable by blueprints
		/// </summary>
		/// <param name="ID">unique id for the check</param>
		/// <param name="AlwaysAllowIfTrue">true/false, 
		/// if true: if at least one of these conditions is fulfilled, the building is considered constructable (OR chaining)
		/// if false: only if all of these conditions are fulfilled, the building is considered constructable (AND chaining)
		/// </param>
		/// <param name="Check"> function that returns a boolean value and takes in the buildingDef</param>
		public static void RegisterAdditionalBuildableCheck(string ID, bool AlwaysAllowIfTrue, Func<BuildingDef, bool> Check)
}