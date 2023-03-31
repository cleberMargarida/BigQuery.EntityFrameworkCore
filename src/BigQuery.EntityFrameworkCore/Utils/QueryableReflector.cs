using System.Linq.Expressions;
using System.Reflection;

namespace BigQuery.EntityFrameworkCore.Utils
{
    internal static class QueryableReflector
    {
        private static readonly Type _queryableType = typeof(Queryable);
        private static readonly MethodInfo[] _methods = _queryableType.GetMethods();

        public static Type Type => _queryableType;
        public static MethodInfo[] Methods => _methods;

        internal static MethodInfo CreateMethod(string name, params Type[] types)
        {
            if (types.Length == 1)
            {
                return Methods.First(x => x.Name == name && x.ContainsGenericParameters == true).MakeGenericMethod(types);
            }

            return Methods.First(x => x.Name == name && x.GetGenericArguments().Length == types.Length).MakeGenericMethod(types);
        }
    }
}
