using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class DistinctExpression : MethodCallExpressionWrapper
    {
        public DistinctExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator DistinctExpression(MethodCallExpression node)
        {
            return new DistinctExpression(node);
        }
    }
}