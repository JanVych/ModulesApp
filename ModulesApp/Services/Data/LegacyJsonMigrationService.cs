using Microsoft.EntityFrameworkCore;
using ModulesApp.Data;
using ModulesApp.Helpers;
using System.Data.Common;
using System.Globalization;
using System.Text.Json;

namespace ModulesApp.Services.Data;

public sealed class LegacyJsonMigrationService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<LegacyJsonMigrationService> _logger;

    public LegacyJsonMigrationService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<LegacyJsonMigrationService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var connection = context.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var total = 0;
        total += await NormalizeColumnAsync(connection, "DashBoardEntity", "Id", "Data", cancellationToken);
        total += await NormalizeColumnAsync(connection, "Module", "Id", "Data", cancellationToken);
        total += await NormalizeColumnAsync(connection, "BackgroundService", "Id", "ConfigurationData", cancellationToken);
        total += await NormalizeColumnAsync(connection, "BackgroundService", "Id", "MessageData", cancellationToken);
        total += await NormalizeColumnAsync(connection, "Action", "Id", "Value", cancellationToken);
        total += await NormalizeColumnAsync(connection, "GlobalValue", "Id", "Value", cancellationToken);

        total += await NormalizeTimestampColumnAsync(connection, "Task", "Id", "LastRun", "1970-01-01 00:00:00", cancellationToken);
        total += await NormalizeTimestampColumnAsync(connection, "Module", "Id", "LastResponse", null, cancellationToken);

        _logger.LogInformation("Legacy migration done. Updated rows: {Total}", total);
    }

    private static async Task<int> NormalizeColumnAsync(
        DbConnection connection,
        string table,
        string idColumn,
        string column,
        CancellationToken cancellationToken)
    {
        var updated = 0;

        await using var select = connection.CreateCommand();
        select.CommandText = $"""SELECT "{idColumn}", "{column}" FROM "{table}";""";

        await using var reader = await select.ExecuteReaderAsync(cancellationToken);
        var rows = new List<(long Id, string Json)>();

        while (await reader.ReadAsync(cancellationToken))
        {
            if (reader.IsDBNull(1))
            {
                continue;
            }

            var id = reader.GetInt64(0);
            var json = reader.GetString(1);
            rows.Add((id, json));
        }

        await reader.CloseAsync();

        if (rows.Count == 0)
        {
            return 0;
        }

        await using var update = connection.CreateCommand();
        update.CommandText = $"""UPDATE "{table}" SET "{column}" = @value WHERE "{idColumn}" = @id;""";
        var idParam = update.CreateParameter();
        idParam.ParameterName = "@id";
        update.Parameters.Add(idParam);

        var valueParam = update.CreateParameter();
        valueParam.ParameterName = "@value";
        update.Parameters.Add(valueParam);

        foreach (var row in rows)
        {
            if (IsValidJson(row.Json))
            {
                continue;
            }

            var normalized = LegacyJsonNormalizer.Normalize(row.Json);
            if (!IsValidJson(normalized))
            {
                continue;
            }

            idParam.Value = row.Id;
            valueParam.Value = normalized;
            updated += await update.ExecuteNonQueryAsync(cancellationToken);
        }

        return updated;
    }

    private static async Task<int> NormalizeTimestampColumnAsync(
        DbConnection connection,
        string table,
        string idColumn,
        string column,
        string? fallbackTimestamp,
        CancellationToken cancellationToken)
    {
        var updated = 0;

        await using var select = connection.CreateCommand();
        select.CommandText = $"""SELECT "{idColumn}", "{column}"::text FROM "{table}" WHERE "{column}" IS NOT NULL;""";

        await using var reader = await select.ExecuteReaderAsync(cancellationToken);
        var rows = new List<(long Id, string Value)>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt64(0);
            var value = reader.GetString(1);
            rows.Add((id, value));
        }

        await reader.CloseAsync();

        if (rows.Count == 0)
        {
            return 0;
        }

        await using var update = connection.CreateCommand();
        update.CommandText = $"""UPDATE "{table}" SET "{column}" = @value WHERE "{idColumn}" = @id;""";
        var idParam = update.CreateParameter();
        idParam.ParameterName = "@id";
        update.Parameters.Add(idParam);

        var valueParam = update.CreateParameter();
        valueParam.ParameterName = "@value";
        update.Parameters.Add(valueParam);

        var fallbackValue = fallbackTimestamp == null
            ? (object)DBNull.Value
            : DateTime.Parse(fallbackTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        foreach (var row in rows)
        {
            if (IsValidDateTimeText(row.Value))
            {
                continue;
            }

            idParam.Value = row.Id;
            valueParam.Value = fallbackValue;
            updated += await update.ExecuteNonQueryAsync(cancellationToken);
        }

        return updated;
    }

    private static bool IsValidDateTimeText(string value)
    {
        if (string.Equals(value, "infinity", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "-infinity", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out _);
    }

    private static bool IsValidJson(string json)
    {
        try
        {
            using var _ = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}