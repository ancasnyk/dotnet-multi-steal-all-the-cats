namespace CatsStealer.WebApi.Extension
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder RegisterConfiguration(this IConfigurationBuilder configurationBuilder, HostBuilderContext hostingContext)
        {
            configurationBuilder.Sources.Clear();
            return configurationBuilder
                .ConfigureEnvironmentVariables()
                .ConfigureJsonProvider(hostingContext);
        }

        public static IConfigurationBuilder ConfigureEnvironmentVariables(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddEnvironmentVariables(prefix: "CatsStealer_");

            return configurationBuilder;
        }

        public static IConfigurationBuilder ConfigureJsonProvider(this IConfigurationBuilder configurationBuilder, HostBuilderContext hostingContext)
        {
            var env = hostingContext.HostingEnvironment;

            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                    optional: true, reloadOnChange: true);

            return configurationBuilder;
        }
    }
}
