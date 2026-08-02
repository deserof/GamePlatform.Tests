namespace GamePlatform.Tests.Infrastructure.Reporting;

public interface IReportStepTracer
{
    Task RunStepAsync(string name, Func<Task> action);

    Task<T> RunStepAsync<T>(string name, Func<Task<T>> action);

    void AttachText(string name, string content);
}
