namespace Config;

public interface IBuilderConfig
{
    string? ConnectionString { get; }
    double LoopIntervalSeconds { get; }
    string ContentDir { get; }
    string ChatId { get; }
    string AccessToken { get; }
}