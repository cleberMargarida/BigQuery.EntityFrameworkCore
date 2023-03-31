using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class TakeExpression : TakeSkipExpression
    {
        public TakeExpression(MethodCallExpression node) : base(node)
        {
            Source = node.Arguments[0];
        }

        public Expression Source { get; }

        public static explicit operator TakeExpression(MethodCallExpression node)
        {
            return new TakeExpression(node);
        }
    }
}