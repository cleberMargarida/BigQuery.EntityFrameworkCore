using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class AverageExpression : SourceSeletorExpression
    {
        public AverageExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator AverageExpression(MethodCallExpression node)
        {
            return new AverageExpression(node);
        }
    }
}