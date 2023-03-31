using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class MaxExpression : SourceSeletorExpression
    {
        public MaxExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator MaxExpression(MethodCallExpression node)
        {
            return new MaxExpression(node);
        }
    }
}