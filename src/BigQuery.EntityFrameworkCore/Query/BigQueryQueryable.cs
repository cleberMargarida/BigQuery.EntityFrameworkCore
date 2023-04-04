using System.Collections;
using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore;

internal class BigQueryQueryable<TSource> : IOrderedQueryable<TSource>
{
    private readonly IExpressionPrinter _visitor;

    public Type ElementType => typeof(TSource);
    public Expression? Expression { get; }
    public IQueryProvider Provider { get; }

    private BigQueryQueryable(IQueryProvider provider, IExpressionPrinter visitor)
    {
        this.Provider = provider;
        _visitor = visitor;
    }

    public BigQueryQueryable(IQueryProvider provider, IQueryable<TSource> innerSource, BigQueryExpressionVisitor visitor) : this(provider, visitor)
    {
        this.Expression = Expression.Constant(innerSource);
    }

    public BigQueryQueryable(IQueryProvider provider, Expression expression, IExpressionPrinter visitor) : this(provider, visitor)
    {
        this.Expression = expression;
    }

    public IEnumerator<TSource> GetEnumerator()
    {
        return this.Provider.Execute<IEnumerable<TSource>>(this.Expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    string? _command;
    public override string ToString()
    {
        return _command ??= _visitor.Print(this.Expression ?? throw new ArgumentNullException());
    }
}
