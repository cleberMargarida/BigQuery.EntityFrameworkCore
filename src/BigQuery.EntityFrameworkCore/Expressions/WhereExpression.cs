using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{

    internal sealed class WhereExpression : MethodCallExpressionWrapper
    {
        public WhereExpression(MethodCallExpression node) : base(node)
        {
            Source = node.Arguments[0];
            Condition = node.Arguments[1];
        }

        public static explicit operator WhereExpression(MethodCallExpression node)
        {
            return new WhereExpression(node);
        }

        public Expression Source { get; }
        public new Expression Condition { get; }
    }
}