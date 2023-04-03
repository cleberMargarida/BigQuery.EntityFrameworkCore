using Google.Apis.Bigquery.v2.Data;
using System.Linq.Expressions;

namespace System.Linq
{
    internal static class LinqExt
    {
        public static T LastButOne<T>(this IEnumerable<T> source)
        {
            if (source.Count() == 1)
            {
                return source.First();
            }

            return source.Reverse().Skip(1).First(); 
        }
        
        public static T? LastButOneOrDefault<T>(this IEnumerable<T> source)
        {
            if (source.Count() == 1)
            {
                return source.FirstOrDefault();
            }

            return source.Reverse().Skip(1).FirstOrDefault(); 
        }

        public static TExpression Cast<TExpression>(this Expression expression) where TExpression : Expression
        {
            return (TExpression)expression;
        }
    }

    public struct FieldEnumerator
    {
        private readonly IEnumerator<(TableFieldSchema Field, string? Value)> _enumerator;
        public FieldEnumerator(IEnumerable<(TableFieldSchema Field, string? Value)> enumerable) => _enumerator = enumerable.GetEnumerator();
        public (TableFieldSchema Field, string? Value) Current => _enumerator.Current;
        public bool MoveNext() => _enumerator.MoveNext();
    }

    public static class FieldEnumeratorExt
    {
        public static FieldEnumerator GetEnumerator(this (IList<TableFieldSchema> TableFieldSchemas, IList<TableCell> TableCells) tuple)
        {
            var values = tuple.TableCells.Select(x => x.V?.ToString());
            var enumerable = tuple.TableFieldSchemas.Zip(values);
            return new FieldEnumerator(enumerable);
        }
    }
}
