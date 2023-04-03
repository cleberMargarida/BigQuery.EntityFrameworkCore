using Google.Cloud.BigQuery.V2;
using System.Collections;
using System.ComponentModel;

namespace BigQuery.EntityFrameworkCore.Utils
{
    internal class BigQueryRowParser
    {
        public TResult? Parse<TResult>(BigQueryResults rows)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(TResult)) && !typeof(TResult).IsString())
            {
                return ParseEnumerable<TResult>(rows);
            }

            return Parse<TResult>(rows.First());
        }

        public TResult? Parse<TResult>(BigQueryRow row)
        {
            return (TResult?)Parse(row, typeof(TResult));
        }

        public object? Parse(BigQueryRow row, Type type)
        {
            if (type.IsPrimitive)
            {
                return ParsePrimitive(row, type);
            }

            if (type.IsString())
            {
                return ParseString(row);
            }

            return ParseClass(row, type);
        }

        public object ParseString(BigQueryRow row)
        {
            return row.RawRow.F[0].V.ToString()!;
        }

        public object? ParsePrimitive(BigQueryRow row, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFromInvariantString(row.RawRow.F[0].V.ToString()!);
        }

        public TResult? ParseEnumerable<TResult>(BigQueryResults rows)
        {
            Type type = typeof(TResult).GenericTypeArguments[0];
            var rowArr = rows.ToArray();

            var results = Array.CreateInstance(type, (int)rows.TotalRows!);

            for (int i = 0; i < rowArr.Length; i++)
            {
                results.SetValue(Parse(rowArr[i], type), i);
            }

            return (TResult)results.SyncRoot;
        }

        public object? ParseClass(BigQueryRow row, Type type)
        {
            var result = Activator.CreateInstance(type);
            var typeProperties = type.GetProperties();

            foreach (var (field, value) in (row.Schema.Fields, row.RawRow.F))
            {
                var matchingProperty = typeProperties.FirstOrDefault(x => x.Name.ToLower() == field.Name.ToLower());

                if (matchingProperty == null || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                var converter = TypeDescriptor.GetConverter(matchingProperty.PropertyType);
                var convertedValue = converter.ConvertFromInvariantString(value);
                matchingProperty.SetValue(result, convertedValue);
            }

            return result;
        }
    }
}