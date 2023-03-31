using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class FirstExpression : MethodCallExpressionWrapper
    {
        public FirstExpression(MethodCallExpression node) : base(node)
        {
            Source = node.Arguments[0];
        }

        public Expression Source { get; }

        public static explicit operator FirstExpression(MethodCallExpression node)
        {
            return new FirstExpression(node);
        }
    }
}