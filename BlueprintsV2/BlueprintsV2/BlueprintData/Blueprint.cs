
using BlueprintsV2;
using BlueprintsV2.ModAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static BlueprintsV2.ModAssets;
using static STRINGS.UI.CLUSTERMAP;

namespace BlueprintsV2.BlueprintData
{

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
        public List<BuildingConfig> BuildingConfigurations { get; } = new List<BuildingConfig>();

        /// <summary>
        /// The true cost of the blueprint given the current game's configuration and the contents of the blueprint.
        /// </summary>
        public Dictionary<BlueprintSelectedMaterial, float> BlueprintCost { get; private set; } = new ();

        /// <summary>
        /// Tag cost including replacements
        /// </summary>
        public Dictionary<Tag, float> CachedAbsTagCost { get; private set; } = new();


        //public Dictionary<Tag, Tag> SelectedBlueprintMaterials { get; private set; } = new();

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
            FilePath = fileLocation;
            string parentFolder = Path.GetDirectoryName(fileLocation);
            if (parentFolder == BlueprintFileHandling.GetBlueprintDirectory())
                Folder = string.Empty;
            else
                Folder = Path.GetFileName(Path.GetDirectoryName(fileLocation)).ToLowerInvariant();
            InferFriendlyName(); 
        }

        void CalculateDimensions()
        {
            int x = 0;
            int y = 0;
            foreach(var building in BuildingConfigurations)
            {
                var offset = building.Offset;
                if (offset.y > y)
                    y = offset.y;
                if(offset.x > x)
                    x = offset.x;
            }
            foreach(var digSpot in DigLocations)
            {
                if (digSpot.y > y)
                    y = digSpot.y;
                if (digSpot.x > x)
                    x = digSpot.x;
            }
            _dimensionX = x; _dimensionY = y;
        }

        /// <summary>
        /// tile of interest dimensions of the blueprint
        /// </summary>
        [JsonIgnore]
        public Vector2I Dimensions
        {
            get 
            {
                return new(_dimensionX,_dimensionY); 
            }
        }
        private int _dimensionX,_dimensionY;

        /// <summary>
        /// Creates a new blueprint with the given name and folder.
        /// </summary>
        /// <param name="friendlyName">The name for the blueprint</param>
        /// <param name="folder">The folder for the blueprint</param>
        public Blueprint(string friendlyName, string folder)
        {
            FriendlyName = friendlyName;
            Folder = SanitizeFolder(folder).ToLowerInvariant();
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
                returnString += ModAssets.BLUEPRINTS_FILE_DISALLOWEDCHARACTERS.Contains(character) ? '_' : character;
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
                                return false;
                            }

                            BuildingConfigurations.Add(buildingConfig);
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

                catch (Exception exception)
                {
                    //Debug.Log("Error when loading blueprint: " + FilePath + ",\n" + nameof(exception) + ": " + exception.Message);
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

                                BuildingConfigurations.Add(buildingConfig);
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

                catch (Exception exception)
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
            ModAssets.BLUEPRINTS_AUTOFILE_IGNORE.Add(FilePath);
            string folder = Path.GetDirectoryName(FilePath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder!);
            }

            //if (Config.Instance.CompressBlueprints)
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
        /// retired for good
        /// </summary>
        [Obsolete]
        public void WriteBinary()
        {
            using BinaryWriter binaryWriter = new BinaryWriter(File.Open(FilePath, FileMode.OpenOrCreate));

            binaryWriter.Write(FriendlyName);

            binaryWriter.Write(BuildingConfigurations.Count);
            BuildingConfigurations.ForEach(buildingConfig => buildingConfig.WriteBinary(binaryWriter));

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
            jsonWriter.WritePropertyName("blueprintVersion");
            jsonWriter.WriteValue(2);

            jsonWriter.WritePropertyName("friendlyname");
            jsonWriter.WriteValue(FriendlyName);

            if (BuildingConfigurations.Count > 0)
            {
                jsonWriter.WritePropertyName("buildings");
                jsonWriter.WriteStartArray();

                foreach (BuildingConfig buildingConfig in BuildingConfigurations)
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

                    jsonWriter.WritePropertyName("x");
                    jsonWriter.WriteValue(digLocation.x);

                    jsonWriter.WritePropertyName("y");
                    jsonWriter.WriteValue(digLocation.y);

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
            RemoveFromFolder();

            //Generate the new folder and file path.
            Folder = SanitizeFolder(newFolder);
            InferFileLocation();

            //Place the blueprint into its new folder.
            BlueprintFileHandling.AddOrGetFolder(Folder).AddBlueprint(this);

            if (rewrite)
            {
                Write();
            }
        }

        public void RemoveFromFolder()
        {
            if (BlueprintFileHandling.TryGetFolder(this, out var folder))
            {
                folder.RemoveBlueprint(this);
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
                return Path.Combine(Path.Combine(BlueprintFileHandling.GetBlueprintDirectory(), Folder), sanitizedFriendlyName + ".blueprint");
            }

            //There is repetition. Append -x to the end of the name, where x is the index.
            return Path.Combine(Path.Combine(BlueprintFileHandling.GetBlueprintDirectory(), Folder), sanitizedFriendlyName + '-' + index + ".blueprint");
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
            return BuildingConfigurations.Count == 0 && DigLocations.Count == 0;
        }


        public void CacheCost()
        {
            CalculateDimensions();
            BlueprintCost.Clear();
            CachedAbsTagCost.Clear();

            foreach (BuildingConfig buildingConfig in BuildingConfigurations)
            {
                Recipe buildingRecipe = buildingConfig.BuildingDef.CraftRecipe;
                List<Tag> selectedElements = buildingConfig.SelectedElements;

                if (buildingRecipe != null)
                {
                    for(int i = 0; i < buildingRecipe.Ingredients.Count; i++)
                    {
                        var ingredient = buildingRecipe.Ingredients[i];
                        Tag selectedElement = null;
                        if(i < selectedElements.Count)
                        {
                            selectedElement = selectedElements[i];
                        }
                        else
                        {
                            selectedElement = ingredient.tag;
                        }
                        var key = new BlueprintSelectedMaterial(selectedElement,ingredient.tag);
                        
                        if (BlueprintCost.ContainsKey(key))
                            BlueprintCost[key] += ingredient.amount;
                        else
                            BlueprintCost.Add(key, ingredient.amount);

                        if (CachedAbsTagCost.ContainsKey(selectedElement))
                            CachedAbsTagCost[selectedElement] += ingredient.amount;
                        else
                            CachedAbsTagCost.Add(selectedElement, ingredient.amount);
                    }
                }
            }
        }

        //public bool CanAffordToPlace(out Dictionary<Tuple<Tag,Tag>, float> remaining)
        //{
        //    //Dictionary<Tag, float> accessibleResources = WorldInventory.Instance.GetAccessibleAmounts();

        //    WorldInventory instance = ClusterManager.Instance.activeWorld.worldInventory;
        //    Dictionary<Tag, float> accessibleResources = instance.GetAccessibleAmounts();

        //    remaining = BlueprintCost;

        //    foreach (KeyValuePair<Tag, float> accessibleResource in accessibleResources)
        //    {
        //        if (remaining.ContainsKey(accessibleResource.Key))
        //        {
        //            remaining[accessibleResource.Key] -= accessibleResource.Value;

        //            if (remaining[accessibleResource.Key] <= 0)
        //            {
        //                remaining.Remove(accessibleResource.Key);

        //                if (remaining.Count == 0)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}
