using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal abstract class TakeSkipExpression : MethodCallExpressionWrapper
    {
        public TakeSkipExpression(MethodCallExpression node) : base(node)
        {
            Number = node.Arguments[1];
        }

        public Expression Number { get; }
    }
}
