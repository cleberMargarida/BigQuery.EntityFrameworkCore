using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal class OrderByExpression : SourceSeletorExpression
    {
        public OrderByExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator OrderByExpression(MethodCallExpression node)
        {
            return new OrderByExpression(node);
        }
    }
}