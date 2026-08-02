using System.Text;
using Allure.Net.Commons;

namespace GamePlatform.Tests.Infrastructure.Reporting;

public sealed class AllureReportStepTracer : IReportStepTracer
{
    public Task RunStepAsync(string name, Func<Task> action) =>
        RunStepAsync(name, async () =>
        {
            await action();
            return true;
        });

    public Task<T> RunStepAsync<T>(string name, Func<Task<T>> action) =>
        AllureApi.Step(name, action);

    public void AttachText(string name, string content) =>
        AllureApi.AddAttachment(name, "text/plain", Encoding.UTF8.GetBytes(content));
}
