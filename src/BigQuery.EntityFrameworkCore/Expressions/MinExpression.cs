using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class MinExpression : SourceSeletorExpression
    {
        public MinExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator MinExpression(MethodCallExpression node)
        {
            return new MinExpression(node);
        }
    }
}