using Newtonsoft.Json;

namespace RiotSharp.Endpoints.SpectatorEndpoint
{
    /// <summary>
    /// Class representing a GameCustomizationObject in the API.
    /// </summary>
    public class GameCustomizationObject
    {
        /// <summary>
        /// Category identifier for Game Customization
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// Game Customization content
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
