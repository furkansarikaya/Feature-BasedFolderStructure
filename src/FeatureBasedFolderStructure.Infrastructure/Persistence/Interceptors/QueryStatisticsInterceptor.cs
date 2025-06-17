using System.Data.Common;
using FeatureBasedFolderStructure.Domain.Common.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Interceptors;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 80)]
public class QueryStatisticsInterceptor(
    IHttpContextAccessor httpContextAccessor,
    ILogger<QueryStatisticsInterceptor> logger)
    : DbCommandInterceptor
{
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        RecordQueryStatistics(command, eventData);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }
    
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        RecordQueryStatistics(command, eventData);
        return base.ReaderExecuted(command, eventData, result);
    }
    
    /// <summary>
    /// Query statistics'i HttpContext'e record ediyor.
    /// </summary>
    private void RecordQueryStatistics(DbCommand command, CommandExecutedEventData eventData)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) return;
        
        // HttpContext'ten mevcut statistics al veya yeni oluştur
        var statsKey = "QueryStats";
        var stats = httpContext.Items[statsKey] as Dictionary<string, object> 
                   ?? new Dictionary<string, object>
                   {
                       ["QueriesCount"] = 0,
                       ["ExecutionTimeMs"] = 0L,
                       ["ExecutedQueries"] = new List<string>()
                   };
        
        // Statistics update et
        stats["QueriesCount"] = (int)stats["QueriesCount"] + 1;
        stats["ExecutionTimeMs"] = (long)stats["ExecutionTimeMs"] + (long)eventData.Duration.TotalMilliseconds;
        
        var executedQueries = (List<string>)stats["ExecutedQueries"];
        executedQueries.Add(command.CommandText.Length > 100 
            ? command.CommandText.Substring(0, 100) + "..." 
            : command.CommandText);
        
        // Updated statistics'i HttpContext'e store et
        httpContext.Items[statsKey] = stats;
        
        logger.LogDebug("Query executed in {Duration}ms. Total queries: {Count}", 
            eventData.Duration.TotalMilliseconds, (int)stats["QueriesCount"]);
    }
}