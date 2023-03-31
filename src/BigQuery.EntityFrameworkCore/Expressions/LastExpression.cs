using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class LastExpression : MethodCallExpressionWrapper
    {
        public LastExpression(MethodCallExpression node) : base(node)
        {
            Source = node.Arguments[0];
        }

        public Expression? Source { get; }

        public static explicit operator LastExpression(MethodCallExpression node)
        {
            return new LastExpression(node);
        }
    }
}