using BigQuery.EntityFrameworkCore.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BigQuery.EntityFrameworkCore.DependencyInjection", AllInternalsVisible = true)]
namespace BigQuery.EntityFrameworkCore
{
    internal class BigQueryProvider : IQueryProvider
    {
        private readonly IExecuteQuery _executeQuery;

        public BigQueryProvider(IExecuteQuery executeQuery)
        {
            this._executeQuery = executeQuery;
        }

        public BigQueryExpressionVisitor GetBigQueryVisitor()
        {
            return new BigQueryExpressionVisitor();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new BigQueryQueryable<TElement>(this, expression, GetBigQueryVisitor());
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var visitor = GetBigQueryVisitor();
            var query = visitor.Print(expression);

            TResult? result = _executeQuery.GetResult<TResult>(query);

            if (visitor.LastCall is nameof(Enumerable.FirstOrDefault) or 
                                    nameof(Enumerable.LastOrDefault) or 
                                    nameof(Enumerable.SingleOrDefault)) 
            {
                return result!;
            }

            return result ?? throw new InvalidOperationException(ErrorMessages.NoSequenceErrorMessage);
        }

        public IQueryable CreateQuery(Expression expression) => this.CreateQuery(expression);
        public object Execute(Expression expression) => this.Execute(expression);
    }
}
