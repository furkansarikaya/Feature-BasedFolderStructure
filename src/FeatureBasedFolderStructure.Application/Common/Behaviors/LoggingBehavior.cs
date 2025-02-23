using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Handling {Name}", typeof(TRequest).Name);
        var response = await next();
        stopwatch.Stop();

        Console.WriteLine($"İşlem süresi: {stopwatch.ElapsedMilliseconds} ms");
        return response;
    }
}