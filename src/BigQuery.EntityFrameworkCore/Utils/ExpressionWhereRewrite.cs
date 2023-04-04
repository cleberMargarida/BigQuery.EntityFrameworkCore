using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.Utils;

internal class ExpressionWhereRewrite : ExpressionVisitor
{
    private readonly Expression _root;
    private Expression? _result;

    public ExpressionWhereRewrite(Expression root)
    {
        _root = root;
    }

    internal static LambdaExpression Rewrite(LambdaExpression node)
    {
        var instance = new ExpressionWhereRewrite(node);
        instance.Visit(node);
        return (instance._result ?? instance._root).Cast<LambdaExpression>();
    }

    internal static MethodCallExpression Rewrite(MethodCallExpression node)
    {
        var instance = new ExpressionWhereRewrite(node);
        instance.Visit(node);
        return (instance._result ?? instance._root).Cast<MethodCallExpression>();
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var methods = node.GetCallStack();

        if (methods.Count(m => m == nameof(Queryable.Where)) < 2)
        {
            return base.VisitMethodCall(node);
        }

        var predicates = node.FindAll<LambdaExpression>();

        if (predicates == null || predicates.Count < 2)
        {
            return base.VisitMethodCall(node);
        }

        var parameter = predicates[0].Find<ParameterExpression>(o => o.StopOnLast);
        var tableExpression = (ConstantExpression?)BigQueryExpressionVisitorFactory.FactorySingleton.TableVisitor.VisitTable(node);
        var source = tableExpression as Expression ?? node.Find<ParameterExpression>(o => o.StopOnFirst)!;
        var combinedPredicate = CombinePredicates(predicates, parameter);

        var newExpression = Expression.Call(
            node.Method,
            source,
            combinedPredicate
        );

        SetResult(source, newExpression);

        return node;

        void SetResult(Expression source, MethodCallExpression newExpression)
        {
            if (_root is MethodCallExpression)
            {
                _result = newExpression;
            }

            if (_root is LambdaExpression)
            {
                _result = Expression.Lambda(
                     newExpression,
                     (ParameterExpression)source
                 );
            }
        }

        LambdaExpression CombinePredicates(List<LambdaExpression> predicates, ParameterExpression? parameter)
        {
            var combinedPredicate = Expression.Lambda(
                Expression.AndAlso(predicates[0].Body, predicates[1].Body),
                parameter);

            foreach (var predicate in predicates.Skip(2))
            {
                combinedPredicate = Expression.Lambda(
                Expression.AndAlso(combinedPredicate.Body, predicate.Body),
                parameter);
            }

            return combinedPredicate;
        }
    }
}
