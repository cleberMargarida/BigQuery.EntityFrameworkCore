using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal abstract class SourceSeletorExpression : MethodCallExpressionWrapper
    {
        public SourceSeletorExpression(MethodCallExpression node) : base(node)
        {
            Source = node.Arguments[0];
            Seletor = node.Arguments[1];
        }

        public Expression Source { get; }
        public Expression Seletor { get; }
    }
}
