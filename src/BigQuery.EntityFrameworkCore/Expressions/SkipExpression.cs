using BigQuery.EntityFrameworkCore.Utils;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal sealed class SkipExpression : TakeSkipExpression
    {
        public SkipExpression(MethodCallExpression node) : base(node)
        {
            if (node.Arguments[0] is not MethodCallExpression argument ||
                argument.Method.Name is not nameof(Enumerable.Take))
            {
                throw new InvalidOperationException(ErrorMessages.OffsetClauseWithoutLimitErrorMessage);
            }

            Take = argument;
        }

        public Expression Take { get; }

        public static explicit operator SkipExpression(MethodCallExpression node)
        {
            return new SkipExpression(node);
        }
    }
}