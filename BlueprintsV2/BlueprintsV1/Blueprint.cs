using BlueprintsV2.BlueprintsV2;
using BlueprintsV2.BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.ModAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.CLUSTERMAP;

namespace Blueprints
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
                    string path = Path.Combine(Utilities.GetBlueprintDirectory(), Name);

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

    /// <summary>
    /// A blueprint.
    /// Provides facilities to save to and read from disk as well as comfort methods for renaming the blueprint and for interacting with folders.
    /// </summary>
    public class Blueprint
    {
        /// <summary>
        /// The name of the blueprint.
        /// </summary>
        public string FriendlyName { get; set; } = "unnamed";

        /// <summary>
        /// The location of the blueprint on the file system.
        /// </summary>
        public string FilePath { get; private set; } = "";

        /// <summary>
        /// The folder that contains the blueprint.
        /// Set to null to indicate no folder.
        /// </summary>
        public string Folder { get; private set; }

        /// <summary>
        /// The buildings contained inside the blueprint.
        /// </summary>
        public List<BuildingConfig> BuildingConfiguration { get; } = new List<BuildingConfig>();

        /// <summary>
        /// The true cost of the blueprint given the current game's configuration and the contents of the blueprint.
        /// </summary>
        public Dictionary<Tag, float> BlueprintCost { get; private set; } = new Dictionary<Tag, float>();

        /// <summary>
        /// The dig locations contained inside the blueprint.
        /// </summary>
        public List<Vector2I> DigLocations { get; } = new();

        /// <summary>
        /// Create a new blueprint at the given location.
        /// This constructor assumes the name of the blueprint from the location.
        /// </summary>
        /// <param name="fileLocation">The location for the blueprint on the file system</param>
        public Blueprint(string fileLocation)
        {
            int blueprintsDirectoryLength = Utilities.GetBlueprintDirectory().Length + 1;
            int folderLength = fileLocation.Length - (blueprintsDirectoryLength + Path.GetFileName(fileLocation).Length + 1);

            FilePath = fileLocation;
            Folder = fileLocation.Substring(blueprintsDirectoryLength, Mathf.Max(0, folderLength)).ToLower();
            InferFriendlyName();
        }

        /// <summary>
        /// Creates a new blueprint with the given name and folder.
        /// </summary>
        /// <param name="friendlyName">The name for the blueprint</param>
        /// <param name="folder">The folder for the blueprint</param>
        public Blueprint(string friendlyName, string folder)
        {
            FriendlyName = friendlyName;
            Folder = SanitizeFolder(folder).ToLower();

            InferFileLocation();
        }

        /// <summary>
        /// "Sanitizes" a blueprint's folder path, converting it to a standard form to prevent any issues.
        /// </summary>
        /// <param name="folder">The folder path to sanitize</param>
        /// <returns>The sanitized, standardized folder path</returns>
        public static string SanitizeFolder(string folder)
        {
            //If the blueprint is in the default folder there's nothing to be sanitized.
            if (folder == "")
            {
                return "";
            }

            //Replace all different directory seperators ("/" and "\" for player entries and the alternative system character for redundancy) with the system's directory separator character.
            folder = folder.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string returnString = "";

            //Sanitize sections (invidual folders and files) of the blueprint's path.
            string[] folderSections = folder.Split(Path.DirectorySeparatorChar);
            foreach (string folderSection in folderSections)
            {
                //Skip any repeating seperator characters. Empty folder names are not possible for obvious reasons.
                if (folderSection.Trim().Length > 0)
                {
                    returnString += SanitizeFile(folderSection) + Path.DirectorySeparatorChar;
                }
            }

            return returnString.TrimEnd(Path.DirectorySeparatorChar).ToLowerInvariant();
        }

        /// <summary>
        /// "Sanitizes" a blueprint's file name, removing any invalid characters for the host operating system.
        /// Since the name of the blueprint is stored inside the file not based upon the file name this is harmless.
        /// </summary>
        /// <param name="file">The file name to sanitize</param>
        /// <returns>The sanitized file name</returns>
        public static string SanitizeFile(string file)
        {
            string returnString = "";

            //Remove any OS-dependant invalid characters, replacing them with an '_'
            //Perhaps this should be improved to account for if '_' is an invalid character. However, I do not know of any operating systems that have this.
            for (int i = 0; i < file.Length; ++i)
            {
                char character = file[i];
                returnString += BlueprintsAssets.BLUEPRINTS_FILE_DISALLOWEDCHARACTERS.Contains(character) ? '_' : character;
            }

            return returnString.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Reads the contents of a binary-formatted file and adds its contents to the blueprint.
        /// </summary>
        /// <returns>True if the read succeeded, false otherwise</returns>
        public bool ReadBinary()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
                    {
                        FriendlyName = reader.ReadString();

                        int buildingCount = reader.ReadInt32();
                        for (int i = 0; i < buildingCount; ++i)
                        {
                            BuildingConfig buildingConfig = new BuildingConfig();
                            if (!buildingConfig.ReadBinary(reader))
                            {
                                SgtLogger.l("failed at building");
                                return false;
                            }

                            BuildingConfiguration.Add(buildingConfig);
                        }

                        int digLocationCount = reader.ReadInt32();
                        for (int i = 0; i < digLocationCount; ++i)
                        {
                            DigLocations.Add(new Vector2I(reader.ReadInt32(), reader.ReadInt32()));
                        }
                    }

                    CacheCost();
                    return true;
                }

                catch (System.Exception exception)
                {
                    Debug.Log("Error when loading blueprint: " + FilePath + ",\n" + nameof(exception) + ": " + exception.Message);
                }
            }

            return false;
        }

        /// <summary>
        /// Reads the contents of a JSON-formatted file and adds it to the blueprint.
        /// </summary>
        public virtual void ReadJson()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    using StreamReader reader = File.OpenText(FilePath);
                    using JsonTextReader jsonReader = new JsonTextReader(reader);

                    JObject rootObject = (JObject)JToken.ReadFrom(jsonReader).Root;

                    JToken friendlyNameToken = rootObject.SelectToken("friendlyname");
                    JToken buildingsToken = rootObject.SelectToken("buildings");
                    JToken digCommandsToken = rootObject.SelectToken("digcommands");

                    if (friendlyNameToken != null && friendlyNameToken.Type == JTokenType.String)
                    {
                        FriendlyName = friendlyNameToken.Value<string>();
                    }

                    if (buildingsToken != null)
                    {
                        JArray buildingTokens = buildingsToken.Value<JArray>();

                        if (buildingTokens != null)
                        {
                            foreach (JToken buildingToken in buildingTokens)
                            {
                                BuildingConfig buildingConfig = new BuildingConfig();
                                buildingConfig.ReadJson((JObject)buildingToken);

                                BuildingConfiguration.Add(buildingConfig);
                            }
                        }
                    }

                    if (digCommandsToken != null)
                    {
                        JArray digCommandTokens = digCommandsToken.Value<JArray>();

                        if (digCommandTokens != null)
                        {
                            foreach (JToken digCommandToken in digCommandTokens)
                            {
                                JToken xToken = digCommandToken.SelectToken("x");
                                JToken yToken = digCommandToken.SelectToken("y");

                                if (xToken != null && xToken.Type == JTokenType.Integer || yToken != null && yToken.Type == JTokenType.Integer)
                                {
                                    DigLocations.Add(new(xToken == null ? 0 : xToken.Value<int>(), yToken == null ? 0 : yToken.Value<int>()));
                                }

                                else if (xToken == null && yToken == null)
                                {
                                    DigLocations.Add(new(0, 0));
                                }
                            }
                        }
                    }

                    CacheCost();
                }

                catch (System.Exception exception)
                {
                    Debug.Log("Error when loading blueprint: " + FilePath + ",\n" + nameof(exception) + ": " + exception.Message);
                }
            }
        }

        /// <summary>
        /// Writes a blueprint, selecting the correct format (binary or JSON) based upon user configuration.
        /// </summary>
        public void Write()
        {
            BlueprintsAssets.BLUEPRINTS_AUTOFILE_IGNORE.Add(FilePath);
            string folder = Path.GetDirectoryName(FilePath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder!);
            }

            ////Use the smaller, binary format if blueprint compression is enabled, use JSON otherwise.
            //if (BlueprintsAssets.Options.CompressBlueprints)
            //{
            //    WriteBinary();
            //}

            //else
            //{
            WriteJson();
            //}
        }

        /// <summary>
        /// Writes the blueprint to a file using binary formatting.
        /// </summary>
        public void WriteBinary()
        {
            using BinaryWriter binaryWriter = new BinaryWriter(File.Open(FilePath, FileMode.OpenOrCreate));

            binaryWriter.Write(FriendlyName);

            binaryWriter.Write(BuildingConfiguration.Count);
            BuildingConfiguration.ForEach(buildingConfig => buildingConfig.WriteBinary(binaryWriter));

            binaryWriter.Write(DigLocations.Count);
            DigLocations.ForEach(digLocation => { binaryWriter.Write(digLocation.x); binaryWriter.Write(digLocation.y); });
        }


        /// <summary>
        /// Writes the blueprint to a file using JSON formatting.
        /// </summary>
        public virtual void WriteJson()
        {
            SgtLogger.l("writing json to " + FilePath);
            using TextWriter textWriter = File.CreateText(FilePath);
            using JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented
            };

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("friendlyname");
            jsonWriter.WriteValue(FriendlyName);

            if (BuildingConfiguration.Count > 0)
            {
                jsonWriter.WritePropertyName("buildings");
                jsonWriter.WriteStartArray();

                foreach (BuildingConfig buildingConfig in BuildingConfiguration)
                {
                    buildingConfig.WriteJson(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }

            if (DigLocations.Count > 0)
            {
                jsonWriter.WritePropertyName("digcommands");
                jsonWriter.WriteStartArray();

                foreach (var digLocation in DigLocations)
                {
                    jsonWriter.WriteStartObject();

                    if (digLocation.x != 0)
                    {
                        jsonWriter.WritePropertyName("x");
                        jsonWriter.WriteValue(digLocation.x);
                    }

                    if (digLocation.y != 0)
                    {
                        jsonWriter.WritePropertyName("y");
                        jsonWriter.WriteValue(digLocation.y);
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }

        /// <summary>
        /// Deletes the blueprint from disk.
        /// </summary>
        /// <returns>True if a file was deleted, false otherwise</returns>
        public bool DeleteFile()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Changes the parent folder of the blueprint, creating a new folder if necessary.
        /// </summary>
        /// <param name="newFolder">The new folder to place the blueprint into</param>
        /// <param name="rewrite">Whether to delete the old blueprint file and write a new one to reflect the new folder on disk</param>
        public void SetFolder(string newFolder, bool rewrite = true)
        {
            if (rewrite)
            {
                DeleteFile();
            }

            //Remove the blueprint from its parent folder.
            for (int i = 0; i < BlueprintsState.LoadedBlueprints.Count; ++i)
            {
                if (BlueprintsState.LoadedBlueprints[i].Name == Folder)
                {
                    BlueprintsState.SelectedFolder.RemoveBlueprint(this);
                    break;
                }
            }

            //Generate the new folder and file path.
            Folder = SanitizeFolder(newFolder);
            InferFileLocation();

            //Place the blueprint into its new folder.
            Utilities.PlaceIntoFolder(this);

            if (rewrite)
            {
                Write();
            }
        }

        /// <summary>
        /// Changes the name of the blueprint.
        /// </summary>
        /// <param name="newFriendlyName">The new name for the blueprint</param>
        /// <param name="rewrite">Whether to delete the old blueprint file and write a new one to reflect the new folder on disk</param>
        public void Rename(string newFriendlyName, bool rewrite = true)
        {
            if (rewrite)
            {
                DeleteFile();
            }

            FriendlyName = newFriendlyName;
            InferFileLocation();

            if (rewrite)
            {
                Write();
            }
        }

        /// <summary>
        /// Infers the blueprint's file's location based upon the blueprint's name and folder.
        /// <para>This method makes sure to account for existing files. 
        /// For example, if "someblueprint.blueprint" exists and this blueprint would share its path with it, the generated name will be "someblueprint-0.blueprint".</para>
        /// </summary>
        public void InferFileLocation()
        {
            FilePath = GetFileLocation(-1);
            int index = 0;

            //Continue to generate files until the generated path doesn't exist.
            while (File.Exists(FilePath))
            {
                FilePath = GetFileLocation(index);
                ++index;
            }
        }

        /// <summary>
        /// Generates a file location based upon an index. This is used by <see cref="InferFileLocation"/> to infer file locations.
        /// </summary>
        /// <param name="index">The repetition index of the file, -1 for no repetition</param>
        /// <returns>The generated file location</returns>
        private string GetFileLocation(int index)
        {
            string sanitizedFriendlyName = SanitizeFile(FriendlyName);

            //If the index is -1, there's no repetition.
            if (index == -1)
            {
                return Path.Combine(Path.Combine(Utilities.GetBlueprintDirectory(), Folder), sanitizedFriendlyName + ".blueprint");
            }

            //There is repetition. Append -x to the end of the name, where x is the index.
            return Path.Combine(Path.Combine(Utilities.GetBlueprintDirectory(), Folder), sanitizedFriendlyName + '-' + index + ".blueprint");
        }

        /// <summary>
        /// Infers the name of the blueprint based upon the path of the blueprint.
        /// </summary>
        public void InferFriendlyName()
        {
            FileInfo fileInfo = new FileInfo(FilePath);
            FriendlyName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
        }

        /// <summary>
        /// Returns true if the blueprint contains no instructions (no buildings or dig commands.)    
        /// </summary>
        /// <returns>True if the blueprint is empty, false otherwise</returns>
        public bool IsEmpty()
        {
            return BuildingConfiguration.Count == 0 && DigLocations.Count == 0;
        }

        public void CacheCost()
        {
            BlueprintCost.Clear();

            foreach (BuildingConfig buildingConfig in BuildingConfiguration)
            {
                Recipe buildingRecipe = buildingConfig.BuildingDef.CraftRecipe;

                if (buildingRecipe != null)
                {
                    foreach (Recipe.Ingredient ingredient in buildingRecipe.Ingredients)
                    {
                        if (BlueprintCost.ContainsKey(ingredient.tag))
                        {
                            BlueprintCost[ingredient.tag] += ingredient.amount;
                        }

                        else
                        {
                            BlueprintCost.Add(ingredient.tag, ingredient.amount);
                        }
                    }
                }
            }
        }

        public bool CanAffordToPlace(out Dictionary<Tag, float> remaining)
        {
            //Dictionary<Tag, float> accessibleResources = WorldInventory.Instance.GetAccessibleAmounts();

            WorldInventory instance = ClusterManager.Instance.activeWorld.worldInventory;
            Dictionary<Tag, float> accessibleResources = instance.GetAccessibleAmounts();

            remaining = BlueprintCost;

            foreach (KeyValuePair<Tag, float> accessibleResource in accessibleResources)
            {
                if (remaining.ContainsKey(accessibleResource.Key))
                {
                    remaining[accessibleResource.Key] -= accessibleResource.Value;

                    if (remaining[accessibleResource.Key] <= 0)
                    {
                        remaining.Remove(accessibleResource.Key);

                        if (remaining.Count == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Describes an individual building inside of a blueprint.
    /// </summary>
    public sealed class BuildingConfig : System.IEquatable<BuildingConfig>
    {
        /// <summary>
        /// The offset from the bottom left of a blueprint.
        /// </summary>
        public Vector2I Offset { get; set; } = new Vector2I(0, 0);

        /// <summary>
        /// The <see cref="BuildingDef"/> of the blueprint this represents.
        /// </summary>
        public BuildingDef BuildingDef { get; set; }

        /// <summary>
        /// The elements the building is to be constructed from.
        /// </summary>
        public List<Tag> SelectedElements { get; } = new List<Tag>();

        /// <summary>
        /// The orientation of the building.
        /// </summary>
        public Orientation Orientation { get; set; } = 0;

        ///// <summary>
        ///// Multi use flags for describing more complex elements of the building, such as pipe connections.
        ///// </summary>
        //public int Flags { get; set; }

        /// <summary>
        /// Replaces Flags in V2 BPss for better versatility
        /// </summary>
        public Dictionary<string, JObject> AdditionalBuildingData = null;

        public bool TryGetDataValue(string id, out JObject data)
        {
            data = null;
            return AdditionalBuildingData != null && AdditionalBuildingData.TryGetValue(id, out data);
        }

        /// <summary>
        /// Appends the building config to the given binary writer.
        /// </summary>
        /// <param name="binaryWriter">The <see cref="BinaryWriter"/> encsapsulating the stream to write to</param>
        [Obsolete]
        public void WriteBinary(BinaryWriter binaryWriter)
        {
            //To prevent crashes. Should never actually happen, though,
            if (BuildingDef == null)
            {
                Debug.Log("Error when writing building config: No building definition.");
                return;
            }

            binaryWriter.Write(Offset.X);
            binaryWriter.Write(Offset.y);
            binaryWriter.Write(BuildingDef.PrefabID);
            binaryWriter.Write(SelectedElements.Count);
            SelectedElements.ForEach(selectedElement => binaryWriter.Write(selectedElement.GetHash()));
            binaryWriter.Write((int)Orientation);
            binaryWriter.Write(-1);
            binaryWriter.Write(AdditionalBuildingData.Count);
            AdditionalBuildingData.ToList().ForEach(entry =>
            {
                binaryWriter.Write(entry.Key);
                binaryWriter.Write(entry.Value.ToString());
            });
        }

        /// <summary>
        /// Writes the building config to the given JSON writer.
        /// Doesn't write unnecessary information to save space.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> encsapsulating the stream to write to</param>
        public void WriteJson(JsonWriter jsonWriter)
        {
            if (BuildingDef == null)
            {
                Debug.Log("Error when writing building config: No building definition.");
                return;
            }

            jsonWriter.WriteStartObject();

            if (Offset.x != 0 || Offset.y != 0)
            {
                jsonWriter.WritePropertyName("offset");
                jsonWriter.WriteStartObject();

                if (Offset.x != 0)
                {
                    jsonWriter.WritePropertyName("x");
                    jsonWriter.WriteValue(Offset.x);
                }

                if (Offset.y != 0)
                {
                    jsonWriter.WritePropertyName("y");
                    jsonWriter.WriteValue(Offset.y);
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WritePropertyName("buildingdef");
            jsonWriter.WriteValue(BuildingDef.PrefabID);

            jsonWriter.WritePropertyName("selected_elements");
            jsonWriter.WriteStartArray();
            SelectedElements.ForEach(elementTag => jsonWriter.WriteValue(elementTag.GetHash()));
            jsonWriter.WriteEndArray();

            if (Orientation != 0)
            {
                jsonWriter.WritePropertyName("orientation");
                jsonWriter.WriteValue((int)Orientation);
            }

            //if (Flags != 0) {
            //    jsonWriter.WritePropertyName("flags");
            //    jsonWriter.WriteValue(Flags);
            //}
            if (AdditionalBuildingData != null)
            {
                jsonWriter.WritePropertyName("buildingData");
                jsonWriter.WriteStartArray();
                AdditionalBuildingData.ToList().ForEach(dataEntry =>
                {
                    if (dataEntry.Key == null || dataEntry.Value == null)
                        return;
                    JObject data = new JObject()
                    {
                    new JProperty("Key",dataEntry.Key),
                    new JProperty ("Value",dataEntry.Value)
                    };
                    data.WriteTo(jsonWriter);
                }
                );
                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }

        /// <summary>
        /// Reads a portion of a binary stream to populate this building config.
        /// OBSOLETE!
        /// </summary>
        /// <param name="binaryReader">The <see cref="BinaryReader"/> encapsulating the binary information to read</param>
        /// <returns>True if the read succeeded, false otherwise</returns>

        public bool ReadBinary(BinaryReader binaryReader)
        {
            try
            {
                Offset = new Vector2I(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                BuildingDef = Assets.GetBuildingDef(binaryReader.ReadString());

                int selectedElementCount = binaryReader.ReadInt32();
                for (int i = 0; i < selectedElementCount; ++i)
                {
                    Tag elementTag;

                    //Only add the tag to the list if it describes a valid element in game.
                    if (ElementLoader.GetElement(elementTag = new Tag(binaryReader.ReadInt32())) != null)
                    {
                        SelectedElements.Add(elementTag);
                    }
                }

                Orientation = (Orientation)binaryReader.ReadInt32();
                int oldFlagSystemValue = binaryReader.ReadInt32();
                if (oldFlagSystemValue != -1)
                {
                    SetUtilityFlags(oldFlagSystemValue);
                }
                return true;
                int buildingDataCount = binaryReader.ReadInt32();
                string id, value;
                for (int i = 0; i < buildingDataCount; ++i)
                {
                    id = binaryReader.ReadString();
                    value = binaryReader.ReadString();
                }
                return true;

            }

            catch (System.Exception e)
            {
                SgtLogger.error(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Reads a JSON object to populate this building config.
        /// </summary>
        /// <param name="rootObject">The <see cref="JObject"/> to use to read from</param>
        public void ReadJson(JObject rootObject)
        {
            JToken offsetToken = rootObject.SelectToken("offset");
            JToken buildingDefToken = rootObject.SelectToken("buildingdef");
            JToken selectedElementsToken = rootObject.SelectToken("selected_elements");
            JToken orientationToken = rootObject.SelectToken("orientation");
            JToken flagsToken = rootObject.SelectToken("flags");
            JToken buildingDataToken = rootObject.SelectToken("buildingData");

            if (offsetToken != null && offsetToken.Type == JTokenType.Object)
            {
                JToken xToken = offsetToken.SelectToken("x");
                JToken yToken = offsetToken.SelectToken("y");

                if (xToken != null && xToken.Type == JTokenType.Integer || yToken != null && yToken.Type == JTokenType.Integer)
                {
                    Offset = new Vector2I(xToken == null ? 0 : xToken.Value<int>(), yToken == null ? 0 : yToken.Value<int>());
                }
            }

            if (buildingDefToken != null && buildingDefToken.Type == JTokenType.String)
            {
                BuildingDef = Assets.GetBuildingDef(buildingDefToken.Value<string>());
            }

            if (selectedElementsToken != null && selectedElementsToken.Type == JTokenType.Array)
            {
                JArray selectedElementTokens = selectedElementsToken.Value<JArray>();

                if (selectedElementTokens != null)
                {
                    foreach (JToken selectedElement in selectedElementTokens)
                    {
                        Tag elementTag;

                        if (selectedElement.Type == JTokenType.Integer && ElementLoader.GetElement(elementTag = new Tag(selectedElement.Value<int>())) != null)
                        {
                            SelectedElements.Add(elementTag);
                        }
                    }
                }
            }

            if (orientationToken != null && orientationToken.Type == JTokenType.Integer)
            {
                Orientation = (Orientation)orientationToken.Value<int>();
            }

            if (flagsToken != null && flagsToken.Type == JTokenType.Integer)
            {
                var value = flagsToken.Value<int>();
                if (value != -1)
                {
                    SetUtilityFlags(value);
                }
            }
            if (buildingDataToken != null)
            {
                JArray additionalDataTokens = buildingDataToken.Value<JArray>();

                if (additionalDataTokens != null)
                {
                    foreach (JObject dataToken in additionalDataTokens)
                    {
                        string key = dataToken.GetValue("Key").Value<string>();
                        JObject value = dataToken.GetValue("Value").Value<JObject>();
                        if (key == null || key.Length == 0 || value == null)
                            continue;

                        AddBuildingData(key, value);
                    }
                }
            }
        }

        /// <summary>
        /// Tests two <see cref="BuildingConfig"/> for equality.
        /// </summary>
        /// <param name="otherBuildingConfig">The other <see cref="BuildingConfig"/> to test for equality</param>
        /// <returns>True if the two objects are equal, false otherwise</returns>
        public bool Equals(BuildingConfig otherBuildingConfig)
        {
            return otherBuildingConfig != null && Offset == otherBuildingConfig.Offset && BuildingDef == otherBuildingConfig.BuildingDef && Orientation == otherBuildingConfig.Orientation;
        }
        internal void AddBuildingData(string Id, JObject data)
        {
            if (Id.IsNullOrWhiteSpace() || data == null)
                return;

            if (AdditionalBuildingData == null)
                AdditionalBuildingData = new();
            AdditionalBuildingData[Id] = data;

        }
        internal void SetUtilityFlags(int flag)
        {
            AddBuildingData(API_Consts.PipeOrientationID,
                new JObject()
                {
                    { API_Consts.PipeOrientationID, flag }
                });
        }
        internal bool GetPipeFlags(out int flags)
        {
            flags = -1;
            if (AdditionalBuildingData != null && AdditionalBuildingData.TryGetValue(API_Consts.PipeOrientationID, out var value)
                && value.SelectToken(API_Consts.PipeOrientationID) != null)
            {
                JToken token = value.SelectToken(API_Consts.PipeOrientationID);

                if (token != null && token.Type == JTokenType.Integer)
                {
                    flags = token.Value<int>();
                    return true;
                }
            }
            return flags != -1;
        }
    }
}
