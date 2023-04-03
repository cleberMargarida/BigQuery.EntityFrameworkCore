using BigQuery.EntityFrameworkCore;
using BigQuery.EntityFrameworkCore.Utils;
using System.Collections.ObjectModel;

namespace System.Linq.Expressions;

internal abstract class MethodCallExpressionWrapper : Expression
{
    private readonly MethodCallExpression _internalCall;

    public MethodCallExpressionWrapper(MethodCallExpression node)
    {
        _internalCall = Call(node.Method, node.Arguments);
    }

    protected ReadOnlyCollection<Expression> Arguments => _internalCall.Arguments;

    public override ExpressionType NodeType => _internalCall.NodeType;
    public override bool CanReduce => _internalCall.CanReduce;
    public override Type Type => _internalCall.Type;
    public override string ToString() => _internalCall.ToString();
}

internal abstract class SourcePredicateExpression : MethodCallExpressionWrapper
{
    public SourcePredicateExpression(MethodCallExpression node) : base(node)
    {
        Source = node.Arguments[0];
        Predicate = node.Arguments[1];
    }

    public Expression Source { get; }
    public Expression Predicate { get; }
}

internal abstract class TakeSkipExpression : MethodCallExpressionWrapper
{
    public TakeSkipExpression(MethodCallExpression node) : base(node)
    {
        Number = node.Arguments[1];
    }

    public Expression Number { get; }
}

internal sealed class AllExpression : SourcePredicateExpression
{
    public AllExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator AllExpression(MethodCallExpression node)
    {
        return new AllExpression(node);
    }
}

internal sealed class AnyExpression : SourcePredicateExpression
{
    public AnyExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator AnyExpression(MethodCallExpression node)
    {
        return new AnyExpression(node);
    }
}

internal sealed class AverageExpression : SourcePredicateExpression
{
    public AverageExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator AverageExpression(MethodCallExpression node)
    {
        return new AverageExpression(node);
    }
}

internal sealed class CountExpression : MethodCallExpressionWrapper
{
    public CountExpression(MethodCallExpression node) : base(node)
    {
        Source = node.Arguments[0];

        if (node.Arguments[0] is MethodCallExpression methodCallExpression && methodCallExpression.Method.Name is nameof(Queryable.Select))
        {
            Source = methodCallExpression.Arguments[0];
        }

        if (node.Arguments.Count > 1)
        {
            Predicate = node.Arguments[1];
        }
    }

    public static explicit operator CountExpression(MethodCallExpression node)
    {
        return new CountExpression(node);
    }

    public Expression Source { get; }
    public Expression? Predicate { get; }
}

internal sealed class DistinctExpression : MethodCallExpressionWrapper
{
    public DistinctExpression(MethodCallExpression node) : base(node)
    {
        Source = node.Arguments[0];
    }

    public static explicit operator DistinctExpression(MethodCallExpression node)
    {
        return new DistinctExpression(node);
    }

    public Expression Source { get; }
}

internal sealed class FirstExpression : MethodCallExpressionWrapper
{
    public FirstExpression(MethodCallExpression node) : base(node)
    {
        Source = node.Arguments[0];
    }

    public Expression Source { get; }

    public static explicit operator FirstExpression(MethodCallExpression node)
    {
        return new FirstExpression(node);
    }
}

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

internal sealed class LastExpression : MethodCallExpressionWrapper
{
    public LastExpression(MethodCallExpression node) : base(node)
    {
        Source = node.Arguments[0];
    }

    public Expression? Source { get; }

    public static explicit operator LastExpression(MethodCallExpression node)
    {
        return new LastExpression(node);
    }
}

internal sealed class MaxExpression : SourcePredicateExpression
{
    public MaxExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator MaxExpression(MethodCallExpression node)
    {
        return new MaxExpression(node);
    }
}

internal sealed class MinExpression : SourcePredicateExpression
{
    public MinExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator MinExpression(MethodCallExpression node)
    {
        return new MinExpression(node);
    }
}

internal sealed class OrderByDescendingExpression : OrderByExpression
{
    public OrderByDescendingExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator OrderByDescendingExpression(MethodCallExpression node)
    {
        return new OrderByDescendingExpression(node);
    }
}

internal class OrderByExpression : SourcePredicateExpression
{
    public OrderByExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator OrderByExpression(MethodCallExpression node)
    {
        return new OrderByExpression(node);
    }
}

internal sealed class SelectExpression : SourcePredicateExpression
{
    public SelectExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator SelectExpression(MethodCallExpression node)
    {
        return new SelectExpression(node);
    }
}

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

internal sealed class SumExpression : SourcePredicateExpression
{
    public SumExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator SumExpression(MethodCallExpression node)
    {
        return new SumExpression(node);
    }
}

internal sealed class TableExpression : Expression
{
    public TableExpression(ExpressionType nodeType, Type type, Table value, Type tableType)
    {
        NodeType = nodeType;
        Type = type;
        Value = value;
        TableType = tableType;
    }

    public sealed override ExpressionType NodeType { get; }
    public override Type Type { get; }
    public Type TableType { get; }
    public Table Value { get; }

    public override string ToString()
    {
        return string.Format(" FROM " + "{0}.{1} AS {2}", Value.DatasetName, Value.TableName, TableType.Name);
    }

    public static explicit operator TableExpression(ConstantExpression node)
    {
        if (node.Value is not Table table)
        {
            throw new ArgumentException(ErrorMessages.ExpressionDoesntContainValidTable);
        }

        return new TableExpression(node.NodeType, node.Type, table, node.Type.GenericTypeArguments[0]);
    }

    public static explicit operator ConstantExpression(TableExpression node)
    {
        return Constant(node.Value);
    }
}

internal sealed class TakeExpression : TakeSkipExpression
{
    public TakeExpression(MethodCallExpression node) : base(node)
    {
        Source = node.Arguments[0];
    }

    public Expression Source { get; }

    public static explicit operator TakeExpression(MethodCallExpression node)
    {
        return new TakeExpression(node);
    }
}

internal class ThenByExpression : MethodCallExpressionWrapper
{
    public ThenByExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator ThenByExpression(MethodCallExpression node)
    {
        return new ThenByExpression(node);
    }
}

internal sealed class ThenByDescendingExpression : ThenByExpression
{
    public ThenByDescendingExpression(MethodCallExpression node) : base(node)
    {
    }

    public static explicit operator ThenByDescendingExpression(MethodCallExpression node)
    {
        return new ThenByDescendingExpression(node);
    }
}

internal sealed class WhereExpression : MethodCallExpressionWrapper
{
    public WhereExpression(MethodCallExpression node) : base(node)
    {
        Source = node.Arguments[0];
        Condition = node.Arguments[1];
    }

    public static explicit operator WhereExpression(MethodCallExpression node)
    {
        return new WhereExpression(node);
    }

    public Expression Source { get; }
    public new Expression Condition { get; }
}