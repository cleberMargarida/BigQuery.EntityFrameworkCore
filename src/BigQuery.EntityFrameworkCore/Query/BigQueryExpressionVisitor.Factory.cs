namespace BigQuery.EntityFrameworkCore;

public interface IExpressionPrinterFactory
{
    IExpressionPrinter Create(string? methodName = "");
}

internal class BigQueryExpressionVisitorFactory : IExpressionPrinterFactory
{
    private static readonly BigQueryExpressionVisitorFactory _instance = new();

    private BigQueryExpressionVisitorFactory() { }

    public static BigQueryExpressionVisitorFactory FactorySingleton => _instance;

    internal BigQueryExpressionVisitor Create(string? methodName = "") => methodName switch
    {
        nameof(Queryable.Join) => JoinVisitor,
        _ => DefaultVisitor
    };

    internal BigQueryJoinExpressionVisitor JoinVisitor => new();
    internal BigQueryTableExpressionVisitor TableVisitor => new();
    internal BigQueryExpressionVisitor DefaultVisitor => new();

    IExpressionPrinter IExpressionPrinterFactory.Create(string? methodName) => Create(methodName);
}
