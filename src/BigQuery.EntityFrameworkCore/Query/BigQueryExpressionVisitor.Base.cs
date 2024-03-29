﻿using BigQuery.EntityFrameworkCore.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore;
internal abstract class BigQueryExpressionVisitorBase : ExpressionVisitor
{
    private static readonly string[] CaseSelectRewrite = new string[]
    {
         nameof(Queryable.Where),
         nameof(Queryable.Count),
         nameof(Queryable.LongCount),
         nameof(Queryable.Last),
         nameof(Queryable.LastOrDefault),
         nameof(Queryable.First),
         nameof(Queryable.FirstOrDefault),
         nameof(Queryable.Single),
         nameof(Queryable.SingleOrDefault),
         nameof(Queryable.Any),
         nameof(Queryable.Max),
         nameof(Queryable.Min),
         nameof(Queryable.All),
         nameof(Queryable.Sum),
         nameof(Queryable.Average),
         nameof(Queryable.OrderBy),
         nameof(Queryable.OrderByDescending),
         nameof(Queryable.ThenBy),
         nameof(Queryable.ThenByDescending)
    };

    protected string[] CallStack { get; private set; } = new string[0];

    [return: NotNullIfNotNull("expression")]
    public override Expression? Visit(Expression? expression)
    {
        if (expression == null)
        {
            return null;
        }

        switch (expression!.NodeType)
        {
            case ExpressionType.Add:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.ArrayIndex:
            case ExpressionType.Coalesce:
            case ExpressionType.Divide:
            case ExpressionType.Equal:
            case ExpressionType.ExclusiveOr:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.Modulo:
            case ExpressionType.Multiply:
            case ExpressionType.NotEqual:
            case ExpressionType.Or:
            case ExpressionType.OrElse:
            case ExpressionType.Subtract:
            case ExpressionType.Assign:
                VisitBinary((BinaryExpression)expression);
                break;
            case ExpressionType.Block:
                VisitBlock((BlockExpression)expression);
                break;
            case ExpressionType.Conditional:
                VisitConditional((ConditionalExpression)expression);
                break;
            case ExpressionType.Constant:
                VisitConstant((ConstantExpression)expression);
                break;
            case ExpressionType.Lambda:
                base.Visit(expression);
                break;
            case ExpressionType.Goto:
                VisitGoto((GotoExpression)expression);
                break;
            case ExpressionType.Label:
                VisitLabel((LabelExpression)expression);
                break;
            case ExpressionType.MemberAccess:
                VisitMember((MemberExpression)expression);
                break;
            case ExpressionType.MemberInit:
                VisitMemberInit((MemberInitExpression)expression);
                break;
            case ExpressionType.Call:
                VisitMethodCall((MethodCallExpression)expression);
                break;
            case ExpressionType.New:
                VisitNew((NewExpression)expression);
                break;
            case ExpressionType.NewArrayInit:
                VisitNewArray((NewArrayExpression)expression);
                break;
            case ExpressionType.Parameter:
                VisitParameter((ParameterExpression)expression);
                break;
            case ExpressionType.Convert:
            case ExpressionType.Not:
            case ExpressionType.Quote:
            case ExpressionType.TypeAs:
            case ExpressionType.Throw:
                VisitUnary((UnaryExpression)expression);
                break;
            case ExpressionType.Default:
                VisitDefault((DefaultExpression)expression);
                break;
            case ExpressionType.Try:
                VisitTry((TryExpression)expression);
                break;
            case ExpressionType.Index:
                VisitIndex((IndexExpression)expression);
                break;
            case ExpressionType.TypeIs:
                VisitTypeBinary((TypeBinaryExpression)expression);
                break;
            case ExpressionType.Switch:
                VisitSwitch((SwitchExpression)expression);
                break;
            case ExpressionType.Invoke:
                VisitInvocation((InvocationExpression)expression);
                break;
            case ExpressionType.Extension:
                VisitExtension(expression);
                break;
        }

        return expression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        CallStack = node.GetCallStack();
        string lastMethodName = CallStack.Last();
        string lastButOneMethodName = CallStack.LastButOne();

        if (CallStack.Length > 1 && 
            CaseSelectRewrite.Contains(lastMethodName) && 
            lastButOneMethodName.Equals(nameof(Queryable.Select)))
        {
            node = ExpressionSelectRewrite.Rewrite(node);
        }

        if (CallStack.Length > 1 && 
            lastMethodName is nameof(Queryable.Where) && 
            lastButOneMethodName is nameof(Queryable.Where))
        {
            node = ExpressionWhereRewrite.Rewrite(node);
        }

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

    protected abstract Expression VisitAll(AllExpression node);
    protected abstract Expression VisitAny(AnyExpression node);
    protected abstract Expression VisitAverage(AverageExpression node);
    protected abstract Expression VisitConstantTable(TableExpression node);
    protected abstract Expression VisitCount(CountExpression node);
    protected abstract Expression VisitDistinct(DistinctExpression node);
    protected abstract Expression VisitFirst(FirstExpression node);
    protected abstract Expression VisitJoin(JoinExpression node);
    protected abstract Expression VisitLast(LastExpression node);
    protected abstract Expression VisitMax(MaxExpression node);
    protected abstract Expression VisitMin(MinExpression node);
    protected abstract Expression VisitOrderBy(OrderByExpression node);
    protected abstract Expression VisitOrderByDescending(OrderByDescendingExpression node);
    protected abstract Expression VisitSelect(SelectExpression node);
    protected abstract Expression VisitSkip(SkipExpression node);
    protected abstract Expression VisitSum(SumExpression node);
    protected abstract Expression VisitTake(TakeExpression node);
    protected abstract Expression VisitThenBy(ThenByExpression node);
    protected abstract Expression VisitThenByDescending(ThenByDescendingExpression node);
    protected abstract Expression VisitWhere(WhereExpression node);
}
