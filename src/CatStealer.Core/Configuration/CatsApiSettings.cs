namespace CatStealer.Core.Configuration
{
    public class CatsApiSettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int FetchCount { get; set; } = 25;
    }
}
