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

        public static string Serialize(Structure value)
        {
            return "f" + value.FrameRate
                + ";w" + value.StartingWidth
                + ";h" + value.StartingHeight
                + ";c" + value.StartingConfig.Replace(";", "")
                + ";a" + value.Actions;
        }

        public static Structure Deserialize(string value)
        {
            Structure structure = new Structure();

            string[] parameters = value.Split(';');
            foreach (string parameter in parameters)
            {
                string parameterTrimmed = parameter.Trim();
                if (parameterTrimmed.Length == 0)
                {
                    continue;
                }

                string data = parameterTrimmed.Substring(1);
                switch (parameterTrimmed[0])
                {
                    case 'f':
                        if (int.TryParse(data, out int frameRate))
                        {
                            structure.FrameRate = frameRate;
                        }
                        break;
                    case 'w':
                        if (double.TryParse(data, out double startingWidth))
                        {
                            structure.StartingWidth = startingWidth;
                        }
                        break;
                    case 'h':
                        if (double.TryParse(data, out double startingHeight))
                        {
                            structure.StartingHeight = startingHeight;
                        }
                        break;
                    case 'c':
                        structure.StartingConfig = data;
                        break;
                    case 'a':
                        structure.Actions = data;
                        break;
                }
            }

            return structure;
        }
    }
}