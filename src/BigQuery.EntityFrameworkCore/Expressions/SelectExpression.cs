using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class SelectExpression : SourceSeletorExpression
    {
        public SelectExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator SelectExpression(MethodCallExpression node)
        {
            return new SelectExpression(node);
        }
    }
}