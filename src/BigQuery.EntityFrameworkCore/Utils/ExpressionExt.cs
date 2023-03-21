using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.Utils
{
    internal static class ExpressionExt
    {
        public static string[] GetCallChainMethodsName(this MethodCallExpression expression)
        {
            if (expression.Arguments.Any() && 
                expression.Arguments[0] is MethodCallExpression innerMethodCallExpression)
            {
                return innerMethodCallExpression
                    .GetCallChainMethodsName()
                    .Append(expression.Method.Name)
                    .ToArray();
            }

            return new[] { expression.Method.Name };
        }
    }
}
