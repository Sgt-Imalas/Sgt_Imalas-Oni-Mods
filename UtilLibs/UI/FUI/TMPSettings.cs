using Newtonsoft.Json;
using System;
using TMPro;

namespace UtilLibs.UIcmp //Source: Aki
{
    [Serializable]
    public class TMPSettings
    {
        [JsonProperty]
        public string Font { get; set; } = "NotoSans-Regular";

        [JsonProperty]
        public FontStyles FontStyle { get; set; }

        [JsonProperty]
        public float FontSize { get; set; }

        [JsonProperty]
        public TextAlignmentOptions Alignment { get; set; }

        [JsonProperty]
        public int MaxVisibleLines { get; set; }

        [JsonProperty]
        public bool EnableWordWrapping { get; set; }

        [JsonProperty]
        public bool AutoSizeTextContainer { get; set; }

        [JsonProperty]
        public string Content { get; set; }

        [JsonProperty]
        public float X { get; set; }

        [JsonProperty]
        public float Y { get; set; }

        [JsonProperty]
        public float[] Color { get; set; }
    }
}
