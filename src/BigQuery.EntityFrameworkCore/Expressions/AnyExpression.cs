using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class AnyExpression : SourceSeletorExpression
    {
        public AnyExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator AnyExpression(MethodCallExpression node)
        {
            return new AnyExpression(node);
        }
    }
}