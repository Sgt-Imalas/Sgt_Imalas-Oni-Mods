
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BlueprintsV2.ModAssets;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
    /// <summary>
    /// Represents a folder of unsorted blueprints. 
    /// Abstracts away the actual backing list, providing comfort methods to add, remove and cycle (next/previous) blueprints.
    /// </summary>
    public sealed class BlueprintFolder
    {
        /// <summary>
        /// The name of the folder.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The selected blueprint.
        /// </summary>
        public Blueprint SelectedBlueprint => contents[selectedBlueprintIndex];

        /// <summary>
        /// The number of blueprints inside the folder.
        /// </summary>
        public int BlueprintCount => contents.Count;

        //Backing variable for "SelectedBlueprintIndex" below.
        private int selectedBlueprintIndex;

        /// <summary>
        /// The zero-based index of the selected blueprint in the backing list.
        /// </summary>
        public int SelectedBlueprintIndex
        {
            get => selectedBlueprintIndex;

            set => selectedBlueprintIndex = Mathf.Clamp(value, 0, contents.Count - 1);
        }

        //Backing list storing the contents of the folder.
        private readonly List<Blueprint> contents = new List<Blueprint>();

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
            SelectedBlueprintIndex = contents.Count - 1;
        }

        /// <summary>
        /// Remove a blueprint from the folder based upon equality with a blueprint inside the list.
        /// </summary>
        /// <param name="blueprint">The blueprint to remove</param>
        /// <param name="deleteIfEmpty">Whether to delete the folder if it is made empty by this removal</param>
        public void RemoveBlueprint(Blueprint blueprint, bool deleteIfEmpty = true)
        {
            contents.Remove(blueprint);
            selectedBlueprintIndex = Mathf.Clamp(SelectedBlueprintIndex, 0, contents.Count - 1);

            if (deleteIfEmpty && BlueprintCount == 0)
            {
                //Remove the folder from the global blueprint folder list.

                BlueprintsState.LoadedBlueprints.Remove(this);
                BlueprintsState.SelectedBlueprintFolderIndex = Mathf.Clamp(BlueprintsState.SelectedBlueprintFolderIndex, 0, BlueprintsState.LoadedBlueprints.Count - 1);

                //Delete the backing folder if it is not the default folder.
                if (Name != "")
                {
                    string path = Path.Combine(BlueprintFileHandling.GetBlueprintDirectory(), Name);

                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path);
                    }
                }
            }
        }

        /// <summary>
        /// Remove a blueprint from the folder based upon its index.
        /// </summary>
        /// <param name="index">The index of the blueprint to remove</param>
        /// <param name="deleteIfEmpty">Whether to delete the folder if it is made empty by this removal</param>
        public void RemoveBlueprint(int index, bool deleteIfEmpty = true)
        {
            RemoveBlueprint(contents[index], deleteIfEmpty);
        }

        /// <summary>
        /// Moves to the next blueprint.
        /// </summary>
        /// <returns>Returns true if the selected blueprint was changed by the call, otherwise false</returns>
        public bool NextBlueprint()
        {
            //If the call would not change anything just return false.
            if (contents.Count < 2)
            {
                return false;
            }

            //Increment the index and loop back to the beginning if necessary.
            selectedBlueprintIndex++;
            if (selectedBlueprintIndex >= BlueprintCount)
            {
                selectedBlueprintIndex = 0;
            }

            return true;
        }

        /// <summary>
        /// Moves to the previous blueprint.
        /// </summary>
        /// <returns>Returns true if the selected blueprint was changed by the call, otherwise false</returns>
        public bool PreviousBlueprint()
        {
            //If the call would not change anything just return false.
            if (contents.Count < 2)
            {
                return false;
            }

            //Decrement the index and loop back to the end if necessary.
            selectedBlueprintIndex--;
            if (selectedBlueprintIndex < 0)
            {
                selectedBlueprintIndex = BlueprintCount - 1;
            }

            return true;
        }
    }
}
