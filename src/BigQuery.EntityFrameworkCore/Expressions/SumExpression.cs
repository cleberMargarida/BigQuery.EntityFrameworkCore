using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class SumExpression : SourceSeletorExpression
    {
        public SumExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator SumExpression(MethodCallExpression node)
        {
            return new SumExpression(node);
        }
    }
}