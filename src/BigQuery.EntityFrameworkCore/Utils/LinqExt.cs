using Google.Apis.Bigquery.v2.Data;
using System.Collections;

namespace System.Linq
{
    internal static class LinqExt
    {
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
