using BigQuery.EntityFrameworkCore.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;

namespace BigQuery.EntityFrameworkCore;

public delegate BigQueryClient BigQueryClientFactory(string projectId, GoogleCredential googleCredential);

public interface IExecuteQuery
{
    TResult? GetResult<TResult>(string query);
}

internal class ExecuteQuery : IExecuteQuery
{
    private readonly BqContextOptions _options;
    private readonly BigQueryClientFactory _factory;
    private readonly BigQueryRowParser _parser;

    public ExecuteQuery(BigQueryClientFactory factory, BqContextOptions options, BigQueryRowParser parser)
    {
        _factory = factory;
        _options = options;
        _parser = parser;
    }

    public List<BigQueryParameter> EmptyBigQueryParameters { get; } = new List<BigQueryParameter>(0);

    public TResult? GetResult<TResult>(string query)
    {
        using var client = GetBigQueryClient();
        var response = client.ExecuteQuery(query, EmptyBigQueryParameters);

        if (response.TotalRows.GetValueOrDefault() == 0)
        {
            return default;
        }

        return _parser.Parse<TResult>(response);
    }

    private BigQueryClient GetBigQueryClient()
    {
        return _factory.Invoke(_options.ProjectId, _options.GoogleCredential);
    }
}