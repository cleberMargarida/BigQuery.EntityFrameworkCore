using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class JoinExpression : MethodCallExpressionWrapper
    {
        public JoinExpression(MethodCallExpression node) : base(node)
        {
        }

        public static explicit operator JoinExpression(MethodCallExpression node)
        {
            return new JoinExpression(node);
        }
    }
}