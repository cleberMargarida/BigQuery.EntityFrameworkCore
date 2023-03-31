using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class ThenByDescendingExpression : ThenByExpression
    {
        public ThenByDescendingExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator ThenByDescendingExpression(MethodCallExpression node)
        {
            return new ThenByDescendingExpression(node);
        }
    }
}
