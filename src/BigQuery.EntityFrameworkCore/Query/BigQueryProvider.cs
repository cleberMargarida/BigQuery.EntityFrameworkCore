using BigQuery.EntityFrameworkCore.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BigQuery.EntityFrameworkCore.DependencyInjection", AllInternalsVisible = true)]
namespace BigQuery.EntityFrameworkCore;

using static BigQueryExpressionVisitorFactory;

internal class BigQueryProvider : IQueryProvider
{
    private readonly IExecuteQuery _executeQuery;

    public BigQueryProvider(IExecuteQuery executeQuery)
    {
        _executeQuery = executeQuery;
    }

    protected TResult Execute<TResult>(MethodCallExpression expression)
    {
        var query = FactorySingleton.DefaultVisitor.Print(expression);
        var result = _executeQuery.GetResult<TResult>(query);

        return ResultCanBeDefault(expression.Method.Name) ? 
            result! : 
            result ?? throw new InvalidOperationException(ErrorMessages.NoSequenceErrorMessage);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => 
        new BigQueryQueryable<TElement>(this, expression, FactorySingleton.DefaultVisitor);

    public IQueryable CreateQuery(Expression expression) => 
        this.CreateQuery(expression);

    public TResult Execute<TResult>(Expression expression) => 
        Execute<TResult>((MethodCallExpression)expression);

    public object Execute(Expression expression) => 
        this.Execute(expression);

    private static bool ResultCanBeDefault(string methodName)
    {
        return methodName is nameof(Enumerable.FirstOrDefault) or
                             nameof(Enumerable.LastOrDefault) or
                             nameof(Enumerable.SingleOrDefault);
    }
}
