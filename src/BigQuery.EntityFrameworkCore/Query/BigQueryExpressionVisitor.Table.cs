using BigQuery.EntityFrameworkCore.Utils;
using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore;

internal partial class BigQueryTableExpressionVisitor : BigQueryExpressionVisitorBase
{
    private TableExpression? _table;

    protected override Expression VisitSelect(SelectExpression node)
    {
        Visit(node.Predicate);
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitWhere(WhereExpression node)
    {
        Visit(node.Source);
        Visit(node.Condition);
        return node;
    }

    protected override Expression VisitOrderBy(OrderByExpression node)
    {
        Visit(node.Source);
        Visit(node.Predicate);
        return node;
    }

    protected override Expression VisitOrderByDescending(OrderByDescendingExpression node)
    {
        VisitOrderBy(node);
        return node;
    }

    protected override Expression VisitThenBy(ThenByExpression node)
    {
        return node;
    }

    protected override Expression VisitThenByDescending(ThenByDescendingExpression node)
    {
        return node;
    }

    protected override Expression VisitTake(TakeExpression node)
    {
        Visit(node.Source);
        Visit(node.Number);
        return node;
    }

    protected override Expression VisitSkip(SkipExpression node)
    {
        Visit(node.Take);
        Visit(node.Number);
        return node;
    }

    protected override Expression VisitFirst(FirstExpression node)
    {
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitLast(LastExpression node)
    {
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitAll(AllExpression node)
    {
        Visit(node.Predicate);
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitAny(AnyExpression node)
    {
        Visit(node.Predicate);
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitCount(CountExpression node)
    {
        Visit(node.Source);

        if (node.Predicate != null)
        {
            Visit(node.Predicate);
        }

        return node;
    }

    protected override Expression VisitDistinct(DistinctExpression node)
    {
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitMin(MinExpression node)
    {
        Visit(node.Predicate);
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitMax(MaxExpression node)
    {
        Visit(node.Predicate);
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitSum(SumExpression node)
    {
        Visit(node.Predicate);
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitAverage(AverageExpression node)
    {
        Visit(node.Predicate);
        Visit(node.Source);
        return node;
    }

    protected override Expression VisitJoin(JoinExpression node)
    {
        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node) => node.Value switch
    {
        Table => VisitConstantTable((TableExpression)node),
        _ => VisitConstantDefault(node)
    };

    protected Expression VisitConstantDefault(ConstantExpression node)
    {
        return node;
    }

    protected override Expression VisitConstantTable(TableExpression node)
    {
        return (_table ??= node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        switch (node.NodeType)
        {
            case ExpressionType.Quote:
                Visit(node.Operand);
                break;
        }

        return node;
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        Visit(node.Body);
        return node;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        Visit(node.Left);
        Visit(node.Right);
        return node;
    }

    protected override Expression VisitNew(NewExpression node)
    {
        foreach (var argument in node.Arguments.SkipLast(1))
        {
            Visit(argument);
        }

        Visit(node.Arguments.Last());

        return node;
    }

    public TableExpression VisitTable(Expression expression)
    {
        Visit(expression);
        return _table!;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        switch (node.Method.Name)
        {
            case nameof(Queryable.Select):
                VisitSelect((SelectExpression)node);
                break;
            case nameof(Queryable.Where):
                VisitWhere((WhereExpression)node);
                break;
            case nameof(Queryable.OrderBy):
                VisitOrderBy((OrderByExpression)node);
                break;
            case nameof(Queryable.OrderByDescending):
                VisitOrderByDescending((OrderByDescendingExpression)node);
                break;
            case nameof(Queryable.ThenBy):
                VisitThenBy((ThenByExpression)node);
                break;
            case nameof(Queryable.ThenByDescending):
                VisitThenByDescending((ThenByDescendingExpression)node);
                break;
            case nameof(Queryable.Take):
                VisitTake((TakeExpression)node);
                break;
            case nameof(Queryable.Skip):
                VisitSkip((SkipExpression)node);
                break;
            case nameof(Queryable.First):
            case nameof(Queryable.FirstOrDefault):
            case nameof(Queryable.Single):
            case nameof(Queryable.SingleOrDefault):
                VisitFirst((FirstExpression)node);
                break;
            case nameof(Queryable.Last):
            case nameof(Queryable.LastOrDefault):
                VisitLast((LastExpression)node);
                break;
            case nameof(Queryable.All):
                VisitAll((AllExpression)node);
                break;
            case nameof(Queryable.Any):
                VisitAny((AnyExpression)node);
                break;
            case nameof(Queryable.Count):
                VisitCount((CountExpression)node);
                break;
            case nameof(Queryable.Distinct):
                VisitDistinct((DistinctExpression)node);
                break;
            case nameof(Queryable.Max):
                VisitMax((MaxExpression)node);
                break;
            case nameof(Queryable.Min):
                VisitMin((MinExpression)node);
                break;
            case nameof(Queryable.Sum):
                VisitSum((SumExpression)node);
                break;
            case nameof(Queryable.Average):
                VisitAverage((AverageExpression)node);
                break;
            case nameof(Queryable.Join):
                VisitJoin((JoinExpression)node);
                break;
            default: throw new NotSupportedException(string.Format(ErrorMessages.CallExpressionNotSupported, node));
        }

        return node;
    }
}
