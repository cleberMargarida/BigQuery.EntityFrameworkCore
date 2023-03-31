using BigQuery.EntityFrameworkCore.Utils;
using Google.Cloud.BigQuery.V2;
using System.Collections;
using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore
{
    public abstract class Table : IQueryable
    {
        public Table(IQueryProvider provider,
                     IExpressionPrinter visitor,
                     string datasetName,
                     string tableName)
        {
            Provider = provider;
            Visitor = visitor;
            DatasetName = datasetName;
            TableName = tableName;
        }

        public abstract Type ElementType { get; }
        public IQueryProvider Provider { get; }
        public string DatasetName { get; }
        public string TableName { get; }
        public Expression Expression => Expression.Constant(this);

        protected IExpressionPrinter Visitor { get; }

        public IEnumerator GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable>(this.Expression).GetEnumerator();
        }

        string _command;
        public string ToQueryString()
        {
            return _command ??= Visitor.Print(Expression.Constant(this));
        }
    }

    public class Table<TSource> : Table, IQueryable<TSource>
    {
        public Table(IQueryProvider provider,
                     IExpressionPrinter visitor,
                     string datasetName,
                     string tableName) : 
            base(provider,
                 visitor,
                 datasetName,
                 tableName)
        {
        }

        public override Type ElementType => typeof(TSource);

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<TSource>>(this.Expression).GetEnumerator();
        }
    }
}
