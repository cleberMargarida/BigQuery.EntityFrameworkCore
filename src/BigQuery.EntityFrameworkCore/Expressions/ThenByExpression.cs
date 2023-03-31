using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal class ThenByExpression : MethodCallExpressionWrapper
    {
        public ThenByExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator ThenByExpression(MethodCallExpression node)
        {
            return new ThenByExpression(node);
        }
    }
}