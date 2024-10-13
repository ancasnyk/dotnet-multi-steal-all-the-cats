namespace CatStealer.Core.Configuration
{
    /// <summary>
    /// The settings for the cats API.
    /// </summary>
    public class CatsApiSettings
    {
        /// <summary>
        /// The API key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The base URL for the API.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The limit of cats to fetch.
        /// </summary>
        public int FetchCount { get; set; } = 25;
    }
}
