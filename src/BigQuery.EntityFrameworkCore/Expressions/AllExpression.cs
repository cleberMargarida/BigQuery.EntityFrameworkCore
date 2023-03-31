using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class AllExpression : SourceSeletorExpression
    {
        public AllExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator AllExpression(MethodCallExpression node)
        {
            return new AllExpression(node);
        }
    }
}