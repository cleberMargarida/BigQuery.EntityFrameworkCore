using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BigQuery.EntityFrameworkCore.Utils
{
    internal static class QueryableReflector
    {
        private static readonly string[] _predicatedMethods = new[]
        {
            nameof(Queryable.Count),
            nameof(Queryable.LongCount),
            nameof(Queryable.Last),
            nameof(Queryable.LastOrDefault),
            nameof(Queryable.First),
            nameof(Queryable.FirstOrDefault),
            nameof(Queryable.Single),
            nameof(Queryable.SingleOrDefault),
            nameof(Queryable.Any),
            nameof(Queryable.All),
            nameof(Queryable.Where),
        };

        private readonly static MethodInfo _where = typeof(Queryable)
            .GetMethods()
            .FirstOrDefault(m => m.Name is nameof(Queryable.Where));

        private readonly static MethodInfo _all = typeof(Queryable)
            .GetMethods()
            .FirstOrDefault(m => m.Name is nameof(Queryable.All));

        public static MethodInfo WhereMethod => _where;
        public static MethodInfo AllMethod => _all;
        public static string[] PredicatedMethodsName => _predicatedMethods;
    }
}
