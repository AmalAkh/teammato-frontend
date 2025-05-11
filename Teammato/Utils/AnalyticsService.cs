namespace Teammato.Utils;

public interface IAnalyticsService
{
    void LogEvent(string eventName, Dictionary<string, string> parameters);
}