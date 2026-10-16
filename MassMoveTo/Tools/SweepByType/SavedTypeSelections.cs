/*
 * Copyright 2026 Peter Han
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using KSerialization;
using System.Collections.Generic;

namespace MassMoveTo.Tools.SweepByType
{
	/// <summary>
	/// Stores a list of the types selected to sweep in the save.
	/// </summary>
	[SerializationConfig(MemberSerialization.OptIn)]
	public sealed class SavedTypeSelections : KMonoBehaviour
	{
		/// <summary>
		/// The number of available presets, excluding "All".
		/// </summary>
		public const int PRESET_COUNT = 5;

		/// <summary>
		/// The currently active preset.
		/// </summary>
		[Serialize]
		public int index;

		/// <summary>
		/// The saved presets for types to sweep.
		/// </summary>
		[Serialize]
		private List<SavedTypePreset> presets;

		/// <summary>
		/// The types to sweep last chosen by the user - for compatibility with previous
		/// versions of the mod only!
		/// </summary>
		[Serialize]
		private List<Tag> types;

		internal SavedTypeSelections() { }

		/// <summary>
		/// Initializes the type list if it is uninitialized.
		/// </summary>
		private void InitPresets()
		{
			if (presets == null)
			{
				presets = new List<SavedTypePreset>(PRESET_COUNT);
				if (types != null && types.Count > 0)
				{
					// Import from old versions of the mod
					var preset = new SavedTypePreset();
					preset.SetSavedTypes(types);
					presets.Add(preset);
				}
			}
			while (presets.Count < PRESET_COUNT)
			{
				var preset = new SavedTypePreset();
				preset.InitTypes();
				presets.Add(preset);
			}
		}

		/// <summary>
		/// Gets the list of saved presets.
		/// </summary>
		/// <returns>The list of saved presets, intializing them to defaults if necessary.</returns>
		public IList<SavedTypePreset> GetSavedPresets()
		{
			InitPresets();
			return presets;
		}
	}

	/// <summary>
	/// An individual preset for types to sweep.
	/// </summary>
	[SerializationConfig(MemberSerialization.OptIn)]
	public sealed class SavedTypePreset : KMonoBehaviour
	{
		/// <summary>
		/// The types to sweep last chosen by the user.
		/// </summary>
		[Serialize]
		private List<Tag> types;

		/// <summary>
		/// The name of this preset.
		/// </summary>
		[Serialize]
		public string name;

		internal SavedTypePreset() { }

		/// <summary>
		/// Initializes the type list if it is uninitialized.
		/// </summary>
		internal void InitTypes()
		{
			if (types == null)
				types = new List<Tag>(64);
		}

		/// <summary>
		/// Gets the list of saved types.
		/// </summary>
		/// <returns>The list of saved types, or an empty list if they are uninitialized.</returns>
		public ICollection<Tag> GetSavedTypes()
		{
			InitTypes();
			return types;
		}

		/// <summary>
		/// Sets the list of saved types.
		/// </summary>
		/// <param name="newTypes">The types selected.</param>
		public void SetSavedTypes(IEnumerable<Tag> newTypes)
		{
			if (newTypes != null)
			{
				InitTypes();
				types.Clear();
				types.AddRange(newTypes);
			}
		}
	}
}
