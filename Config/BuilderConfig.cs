using Microsoft.Extensions.Configuration;

namespace Config;

public class BuilderConfig(string pathConfig) : IBuilderConfig
{
    private readonly IConfigurationRoot _config = new ConfigurationBuilder()
            .AddJsonFile(pathConfig)
            .AddEnvironmentVariables()
            .Build();

    public string ConnectionString => _config["connectionString"] ?? "";
    public double LoopIntervalSeconds => double.Parse(_config["loopIntervalSeconds"] ?? string.Empty);
    public string ContentDir => _config["contentDir"] ?? "";
    public string ChatId => _config["chatId"] ?? "";
    public string AccessToken => _config["accessToken"] ?? "";
}
