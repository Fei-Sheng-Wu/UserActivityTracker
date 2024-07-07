using System.Text.Json.Serialization;

namespace UserActivityTracker.FileFormat
{
    public class Structure
    {
        [JsonPropertyName("f")]
        public int FrameRate { get; set; }

        [JsonPropertyName("w")]
        public double StartingWidth { get; set; }

        [JsonPropertyName("h")]
        public double StartingHeight { get; set; }

        [JsonPropertyName("c")]
        public string StartingConfig { get; set; }

        [JsonPropertyName("a")]
        public string Actions { get; set; }
    }
}