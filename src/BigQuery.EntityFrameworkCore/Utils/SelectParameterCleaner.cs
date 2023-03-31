using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.Utils
{
    internal class SelectParameterCleaner : ExpressionVisitor
    {
        private readonly Expression _root;

        private MethodCallExpression? _lastCall;
        private Expression? _result;
        private Expression? _member;

        public SelectParameterCleaner(Expression root)
        {
            _root = root;
        }

        internal static LambdaExpression Rewrite(LambdaExpression node)
        {
            var instance = new SelectParameterCleaner(node);
            instance.Visit(node);

            return (instance._result ?? instance._root).Cast<LambdaExpression>();
        }

        internal static MethodCallExpression Rewrite(MethodCallExpression node)
        {
            var instance = new SelectParameterCleaner(node);
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

            if (!isQueryableMethod)
            {
                return base.VisitMethodCall(node);
            }

            switch (_lastCall.Method.Name)
            {
                case nameof(Queryable.Where):
                    SetResultWhere(node, argument, seletor, predicate); break;
                case nameof(Queryable.Count):
                case nameof(Queryable.LongCount):
                case nameof(Queryable.Last):
                case nameof(Queryable.LastOrDefault):
                case nameof(Queryable.First):
                case nameof(Queryable.FirstOrDefault):
                case nameof(Queryable.Single):
                case nameof(Queryable.SingleOrDefault):
                    SetResultNoPredicated(node, argument, seletor, seletorOperand, predicate); break;
                case nameof(Queryable.Any):
                case nameof(Queryable.All):
                case nameof(Queryable.Max):
                case nameof(Queryable.Min):
                case nameof(Queryable.Average):
                case nameof(Queryable.Sum):
                    SetResultPredicated(node, argument, seletor, predicate); break;
                case nameof(Queryable.OrderBy):
                case nameof(Queryable.OrderByDescending):
                case nameof(Queryable.ThenBy):
                case nameof(Queryable.ThenByDescending):
                    SetResultOrderPredicated(node, argument, seletor, predicate); break;
            }

            return base.VisitMethodCall(node);

            void SetResultNoPredicated(MethodCallExpression node, Expression? argument, Expression seletor, LambdaExpression seletorOperand, UnaryExpression predicate)
            {
                var lastCallMethod = _lastCall!.Method.DeclaringType?
                    .GetMethods()
                    .FirstOrDefault(m => m.Name == _lastCall.Method.Name && m.GetParameters().Length == 1)
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(
                    seletorOperand.ReturnType
                );

                var whereMethod = QueryableReflector.CreateMethod(
                    nameof(Queryable.Where),
                    types: argument!.Type.GenericTypeArguments
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

            void SetResultPredicated(MethodCallExpression node, Expression? argument, Expression seletor, UnaryExpression predicate)
            {
                var genericArguments = GetGenericArguments(node);

                var lastCallMethod = QueryableReflector.CreateMethod(
                    _lastCall.Method.Name,
                    genericArguments
                 );

                var newExpression = Expression.Call(
                    method: lastCallMethod,
                    argument,
                    predicate
                );

                SetResult(argument!, newExpression);
            }
            
            void SetResultOrderPredicated(MethodCallExpression node, Expression? argument, Expression seletor, UnaryExpression predicate)
            {
                var genericArguments = GetGenericArguments(node);

                var lastCallMethod = QueryableReflector.CreateMethod(
                    _lastCall.Method.Name,
                    genericArguments
                 );

                var newExpression = Expression.Call(
                    method: node.Method,
                    Expression.Call(
                        method: lastCallMethod,
                        argument,
                        predicate
                    ),
                    seletor
                );

                SetResult(argument!, newExpression);
            }

            void SetResultWhere(MethodCallExpression node, Expression? argument, Expression seletor, UnaryExpression predicate)
            {
                var lastCallMethod = QueryableReflector.CreateMethod(
                    nameof(Queryable.Where),
                    types: node.Method.GetGenericArguments()[0]
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

            static Expression? GetArgument(MethodCallExpression node) => node.Arguments.First() switch
            {
                ParameterExpression parameter => parameter,
                ConstantExpression constant => constant,
                _ => null,
            };

            void SetResult(Expression argument, MethodCallExpression newExpression)
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

            UnaryExpression BuildPredicate(MethodCallExpression node, LambdaExpression seletorOperand)
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

            Type[] GetGenericArguments(MethodCallExpression node)
            {
                Type[] types = _lastCall.Method.GetGenericArguments();
                int genericArgumentsCount = types.Length;
                var genericArguments = new Type[genericArgumentsCount];
                genericArguments[0] = node.Method.GetGenericArguments()[0];
                for (int i = 1; i < genericArgumentsCount; i++)
                {
                    genericArguments[i] = types[i];
                }

                return genericArguments;
            }
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _member ?? base.VisitParameter(node);
        }
    }
}
