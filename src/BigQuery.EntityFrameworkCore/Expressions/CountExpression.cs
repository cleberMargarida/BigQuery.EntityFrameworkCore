using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class CountExpression : SourceSeletorExpression
    {
        public CountExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator CountExpression(MethodCallExpression node)
        {
            return new CountExpression(node);
        }
    }
}