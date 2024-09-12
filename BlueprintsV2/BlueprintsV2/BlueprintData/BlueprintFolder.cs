
using System;
using System.Collections.Generic;
using System.IO;
using static BlueprintsV2.ModAssets;

namespace BlueprintsV2.BlueprintData
{
	/// <summary>
	/// Represents a folder of unsorted blueprints. 
	/// Abstracts away the actual backing list, providing comfort methods to add, remove and cycle (next/previous) blueprints.
	/// </summary>
	public sealed class BlueprintFolder : IEquatable<BlueprintFolder>
	{
		/// <summary>
		/// The name of the folder.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The number of blueprints inside the folder.
		/// </summary>
		public int BlueprintCount => contents.Count;

		/// <summary>
		/// Are there an blueprints inside this folder?
		/// </summary>
		public bool HasBlueprints => contents.Count > 0;


		///Backing list storing the contents of the folder.
		private HashSet<Blueprint> contents = new();
		public HashSet<Blueprint> Blueprints => contents;

		/// <summary>
		/// Create a new blueprint folder with the given name.
		/// </summary>
		/// <param name="name">The name for the folder</param>
		public BlueprintFolder(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Add a blueprint to the folder.
		/// </summary>
		/// <param name="blueprint">The blueprint to add</param>
		public void AddBlueprint(Blueprint blueprint)
		{
			contents.Add(blueprint);
		}


		/// <summary>
		/// Remove a blueprint from the folder based upon equality with a blueprint inside the list.
		/// </summary>
		/// <param name="blueprint">The blueprint to remove</param>
		/// <param name="deleteIfEmpty">Whether to delete the folder if it is made empty by this removal</param>
		public void RemoveBlueprint(Blueprint blueprint, bool deleteIfEmpty = true)
		{
			contents.Remove(blueprint);

			if (deleteIfEmpty && BlueprintCount == 0)
			{

				//Delete the backing folder if it is not the root folder.
				if (Name != "")
				{
					//Remove the folder from the global blueprint folder list.

					BlueprintFileHandling.BlueprintFolders.Remove(this);
					string path = Path.Combine(BlueprintFileHandling.GetBlueprintDirectory(), Name);

					if (Directory.Exists(path))
					{
						Directory.Delete(path);
					}
				}
			}
		}

		public bool ContainsBlueprint(Blueprint blueprint) => contents.Contains(blueprint);

		public bool Equals(BlueprintFolder other)
		{
			return other.Name == Name;
		}
		public override bool Equals(object obj) => obj is BlueprintFolder other && Equals(other);

		public static bool operator ==(BlueprintFolder a, BlueprintFolder b) => a?.Name == b?.Name;
		public static bool operator !=(BlueprintFolder a, BlueprintFolder b) => !(a == b);
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}
