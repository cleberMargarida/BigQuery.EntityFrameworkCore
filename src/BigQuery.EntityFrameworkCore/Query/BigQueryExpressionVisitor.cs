using BigQuery.EntityFrameworkCore.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("BigQuery.EntityFrameworkCore.UnitTests", AllInternalsVisible = true)]
namespace BigQuery.EntityFrameworkCore
{
    public interface IExpressionPrinter
    {
        Expression? Visit(Expression? expression);
        internal string Print(Expression expression);
    }

    public class BigQueryExpressionVisitor : ExpressionVisitor, IExpressionPrinter
    {
        protected readonly StringBuilder _stringBuilder = new StringBuilder();
        private static readonly Dictionary<ExpressionType, string> _binaryOperandMap = new Dictionary<ExpressionType, string>
        {
            {
                ExpressionType.Assign,
                " = "
            },
            {
                ExpressionType.Equal,
                " = "
            },
            {
                ExpressionType.NotEqual,
                " != "
            },
            {
                ExpressionType.GreaterThan,
                " > "
            },
            {
                ExpressionType.GreaterThanOrEqual,
                " >= "
            },
            {
                ExpressionType.LessThan,
                " < "
            },
            {
                ExpressionType.LessThanOrEqual,
                " <= "
            },
            {
                ExpressionType.OrElse,
                " || "
            },
            {
                ExpressionType.AndAlso,
                " AND "
            },
            {
                ExpressionType.Coalesce,
                " ?? "
            },
            {
                ExpressionType.Add,
                " + "
            },
            {
                ExpressionType.Subtract,
                " - "
            },
            {
                ExpressionType.Multiply,
                " * "
            },
            {
                ExpressionType.Divide,
                " / "
            },
            {
                ExpressionType.Modulo,
                " % "
            },
            {
                ExpressionType.And,
                " & "
            },
            {
                ExpressionType.Or,
                " | "
            },
            {
                ExpressionType.ExclusiveOr,
                " ^ "
            }
        };

        internal string Print(Expression expression)
        {
            Visit(expression);
            return _stringBuilder.ToString().FormatSql();
        }

        protected string? LastTable { get; set; }
        internal string? LastCall { get; private set; }

        protected BigQueryExpressionVisitor GetNewVisitor() => new BigQueryExpressionVisitor();

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
            if (node.GetCallChainMethodsName().LastButOne() is nameof(Queryable.Select))
            {
                node = ExpressionUnraveler.Rewrite(node);
            }

            switch (node.Method.Name)
            {
                case nameof(Enumerable.Select): VisitSelect(node); LastCall = nameof(Enumerable.Select); break;
                case nameof(Enumerable.Where): VisitWhere(node); LastCall = nameof(Enumerable.Where); break;
                case nameof(Enumerable.OrderBy): VisitOrderBy(node); LastCall = nameof(Enumerable.OrderBy); break;
                case nameof(Enumerable.OrderByDescending): VisitOrderByDescending(node); LastCall = nameof(Enumerable.OrderByDescending); break;
                case nameof(Enumerable.ThenBy): VisitThenBy(node); LastCall = nameof(Enumerable.ThenBy); break;
                case nameof(Enumerable.ThenByDescending): VisitThenByDescending(node); LastCall = nameof(Enumerable.ThenByDescending); break;
                case nameof(Enumerable.Take): VisitTake(node); LastCall = nameof(Enumerable.Take); break;
                case nameof(Enumerable.Skip): VisitSkip(node); LastCall = nameof(Enumerable.Skip); break;
                case nameof(Enumerable.First): VisitFirst(node); LastCall = nameof(Enumerable.First); break;
                case nameof(Enumerable.FirstOrDefault): VisitFirst(node); LastCall = nameof(Enumerable.FirstOrDefault); break;
                case nameof(Enumerable.Single): VisitFirst(node); LastCall = nameof(Enumerable.Single); break;
                case nameof(Enumerable.SingleOrDefault): VisitFirst(node); LastCall = nameof(Enumerable.SingleOrDefault); break;
                case nameof(Enumerable.Last): VisitLast(node); LastCall = nameof(Enumerable.Last); break;
                case nameof(Enumerable.LastOrDefault): VisitLast(node); LastCall = nameof(Enumerable.LastOrDefault); break;
                case nameof(Enumerable.All): VisitAll(node); LastCall = nameof(Enumerable.All); break;
                case nameof(Enumerable.Any): VisitAny(node); LastCall = nameof(Enumerable.Any); break;
                case nameof(Enumerable.Count): VisitCount(node); LastCall = nameof(Enumerable.Count); break;
                case nameof(Enumerable.Distinct): VisitDistinct(node); LastCall = nameof(Enumerable.Distinct); break;
                case nameof(Enumerable.Max): VisitMax(node); LastCall = nameof(Enumerable.Max); break;
                case nameof(Enumerable.Min): VisitMin(node); LastCall = nameof(Enumerable.Min); break;
                case nameof(Enumerable.Sum): VisitSum(node); LastCall = nameof(Enumerable.Sum); break;
                case nameof(Enumerable.Average): VisitAverage(node); LastCall = nameof(Enumerable.Average); break;
                case nameof(Enumerable.Join): VisitJoin(node); LastCall = nameof(Enumerable.Join); break;
                default: throw new NotSupportedException(string.Format(ErrorMessages.CallExpressionNotSupported, node));
            }

            return node;
        }

        protected Expression VisitSelect(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitWhere(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitOrderBy(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitOrderByDescending(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitThenBy(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitThenByDescending(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitTake(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitSkip(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitFirst(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitLast(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitAll(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitAny(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitCount(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitDistinct(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitMin(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitMax(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitSum(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitAverage(MethodCallExpression node)
        {
            return node;
        }

        protected Expression VisitJoin(MethodCallExpression node)
        {
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node) => node.Value switch
        {
            Table table => VisitConstantTable(table, node),
            String str => VisitConstantString(str, node),
            DateTime dateTime => VisitConstantDateTime(dateTime, node),
            TimeSpan timeSpan => VisitConstantTimeSpan(timeSpan, node),
            _ => VisitConstantDefault(node)
        };

        protected Expression VisitConstantDefault(ConstantExpression node)
        {
            _stringBuilder.Append(node.Value);
            return node;
        }

        protected Expression VisitConstantTimeSpan(TimeSpan timeSpan, ConstantExpression node)
        {
            _stringBuilder.Append(timeSpan.ToString(Formats.TimeOnlyFormat));
            return node;
        }

        protected Expression VisitConstantDateTime(DateTime dateTime, ConstantExpression node)
        {
            _stringBuilder.Append(dateTime.ToString(Formats.DefaultDateTimeFormat));
            return node;
        }

        protected Expression VisitConstantString(string str, ConstantExpression node)
        {
            _stringBuilder.AppendWithQuotationAround(str);
            return node;
        }

        protected Expression VisitConstantTable(Table table, ConstantExpression node)
        {
            return node;
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

        protected override Expression VisitMember(MemberExpression node)
        {
            var source = node.Expression.Type.Name;
            var member = node.Member.GetCustomAttribute<ColumnAttribute>()?.Name ?? node.Member.Name;
            _stringBuilder.Append(source + "." + member);
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);

            if (_binaryOperandMap.TryGetValue(node.NodeType, out var value))
            {
                _stringBuilder.Append(value);
            }

            Visit(node.Right);
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            foreach (var argument in node.Arguments.SkipLast(1))
            {
                Visit(argument);
                _stringBuilder.Append(", ");
            }

            Visit(node.Arguments.Last());

            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node;
        }

        protected string[] GetColumnsFromProperties(IEnumerable<PropertyInfo> propertyInfos, string? lastTable = null)
        {
            var prefix = lastTable ?? LastTable;

            return propertyInfos.Select
                (p => prefix + "." +
                (p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name))
                .ToArray();
        }

        #region interface methods
        Expression? IExpressionPrinter.Visit(Expression? expression)
        {
            return this.Visit(expression);
        }

        string IExpressionPrinter.Print(Expression expression)
        {
            return this.Print(expression);
        }
        #endregion
    }
}
