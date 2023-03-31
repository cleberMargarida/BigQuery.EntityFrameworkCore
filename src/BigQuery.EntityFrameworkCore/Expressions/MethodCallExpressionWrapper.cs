using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal abstract class MethodCallExpressionWrapper : Expression
    {
        private readonly MethodCallExpression _internalCall;

        public MethodCallExpressionWrapper(MethodCallExpression node)
        {
            _internalCall = Call(node.Method, node.Arguments);
        }

        protected ReadOnlyCollection<Expression> Arguments => _internalCall.Arguments;

        public override string ToString() => _internalCall.ToString();
    }
}