using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class OrderByDescendingExpression : OrderByExpression
    {
        public OrderByDescendingExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator OrderByDescendingExpression(MethodCallExpression node)
        {
            return new OrderByDescendingExpression(node);
        }
    }
}