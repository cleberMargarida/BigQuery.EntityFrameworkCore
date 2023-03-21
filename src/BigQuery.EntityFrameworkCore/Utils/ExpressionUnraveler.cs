using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.Utils
{
    internal class ExpressionUnraveler : ExpressionVisitor
    {
        private readonly Expression _root;

        private MethodCallExpression? _lastCall;
        private Expression? _result;
        private Expression? _member;

        public ExpressionUnraveler(Expression root)
        {
            _root = root;
        }

        internal static LambdaExpression Rewrite(LambdaExpression node)
        {
            var instance = new ExpressionUnraveler(node);
            instance.Visit(node);

            return (instance._result ?? instance._root).Cast<LambdaExpression>();
        }

        internal static MethodCallExpression Rewrite(MethodCallExpression node)
        {
            var instance = new ExpressionUnraveler(node);
            instance.Visit(node);

            return (instance._result ?? instance._root).Cast<MethodCallExpression>();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (_lastCall == null)
            {
                _lastCall = node;
                return base.VisitMethodCall(node);
            }

            if (_lastCall.Arguments.Count < 2)
            {
                return base.VisitMethodCall(node);
            }

            Expression? argument = GetArgument(node);

            if (argument == null)
            {
                return base.VisitMethodCall(node);
            }

            var seletor = node.Arguments.Last();

            var seletorOperand = seletor
                .Cast<UnaryExpression>().Operand
                .Cast<LambdaExpression>();

            var predicate = BuildPredicate(
                node, 
                seletorOperand
            );
            
            bool isQueryableMethod = (_lastCall.Method.DeclaringType?.Equals(typeof(Queryable))).GetValueOrDefault();

            if (isQueryableMethod)
            {
                bool isWhereMethod = _lastCall.Method.Name is nameof(Queryable.Where);
                bool isAllMethod = _lastCall.Method.Name is nameof(Queryable.All);
                bool isPredicatedMethod = QueryableReflector.PredicatedMethodsName.Contains(_lastCall.Method.Name) && _lastCall.Method.GetParameters().Skip(1).Any(p => p.Name is "predicate");

                if (isWhereMethod) SetResultWhere(node, argument, seletor, predicate);

                else if (isAllMethod) SetResultAll(node, argument, predicate);
                else if (isPredicatedMethod) SetResultPredicated(node, argument, seletor, seletorOperand, predicate);
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _member ?? base.VisitParameter(node);
        }

        private void SetResultPredicated(MethodCallExpression node, Expression? argument, Expression seletor, LambdaExpression seletorOperand, UnaryExpression predicate)
        {
            var lastCallMethod = _lastCall!.Method.DeclaringType?
                .GetMethods()
                .FirstOrDefault(m => m.Name == _lastCall.Method.Name)
                .GetGenericMethodDefinition()
                .MakeGenericMethod(
                seletorOperand.ReturnType
            );

            var whereMethod = QueryableReflector.WhereMethod.MakeGenericMethod(
                argument!.Type.GenericTypeArguments
            );

            var whereMethodCall = Expression.Call(
                method: whereMethod,
                argument,
                predicate
            );

            var selectMethodCall = Expression.Call(
                method: node.Method,
                whereMethodCall,
                seletor
            );

            var newMethodCall = Expression.Call(
                method: lastCallMethod,
                selectMethodCall
            );

            SetResult(argument, newMethodCall);
        }
        private void SetResultAll(MethodCallExpression node, Expression? argument, UnaryExpression predicate)
        {
            var lastCallMethod = QueryableReflector.AllMethod.MakeGenericMethod(
                node.Method.GetGenericArguments()[0]
            );

            var innerCall = Expression.Call(
                method: lastCallMethod,
                argument,
                predicate
            );

            SetResult(argument!, innerCall);
        }
        private void SetResultWhere(MethodCallExpression node, Expression? argument, Expression seletor, UnaryExpression predicate)
        {
            var lastCallMethod = QueryableReflector.WhereMethod.MakeGenericMethod(
                node.Method.GetGenericArguments()[0]
            );

            var innerCall = Expression.Call(
                method: lastCallMethod,
                argument,
                predicate
            );

            var newExpression = Expression.Call(
                method: node.Method,
                innerCall,
                seletor
            );

            SetResult(argument!, newExpression);
        }
        private static Expression? GetArgument(MethodCallExpression node) => node.Arguments.First() switch
        {
            ParameterExpression parameter => parameter,
            ConstantExpression constant => constant,
            _ => null,
        };
        private void SetResult(Expression argument, MethodCallExpression newExpression)
        {
            if (_root is MethodCallExpression)
            {
                _result = newExpression;
                return;
            }

            if (_root is LambdaExpression && argument is ParameterExpression parameter)
            {
                var lambda = Expression.Lambda(
                    delegateType: _root!.Type,
                    body: newExpression,
                    parameters: parameter
                );

                _result = lambda;
                return;
            }
        }
        private UnaryExpression BuildPredicate(MethodCallExpression node, LambdaExpression seletorOperand)
        {
            _member = node.Arguments.Last()
                .Cast<UnaryExpression>().Operand
                .Cast<LambdaExpression>().Body;

            var predicateBody = Visit(
                _lastCall?.Arguments.Last()
                .Cast<UnaryExpression>().Operand
                .Cast<LambdaExpression>().Body
            );

            _member = null;

            var predicate = Expression.Quote(Expression.Lambda(
                predicateBody,
                seletorOperand.Parameters
            ));
            return predicate;
        }
    }
}
