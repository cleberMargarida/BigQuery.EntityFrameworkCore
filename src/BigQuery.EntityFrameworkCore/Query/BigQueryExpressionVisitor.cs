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
        protected BigQueryExpressionVisitor NewVisitor => new BigQueryExpressionVisitor();
        protected BigQueryExpressionVisitor NewJoinVisitor => new BigQueryJoinExpressionVisitor();


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
            switch (node.Method.Name)
            {
                case nameof(Enumerable.Select): TranslateSelect(node); LastCall = nameof(Enumerable.Select); break;
                case nameof(Enumerable.Where): TranslateWhere(node); LastCall = nameof(Enumerable.Where); break;
                case nameof(Enumerable.OrderBy): TranslateOrderBy(node); LastCall = nameof(Enumerable.OrderBy); break;
                case nameof(Enumerable.OrderByDescending): TranslateOrderByDescending(node); LastCall = nameof(Enumerable.OrderByDescending); break;
                case nameof(Enumerable.ThenBy): TranslateThenBy(node); LastCall = nameof(Enumerable.ThenBy); break;
                case nameof(Enumerable.ThenByDescending): TranslateThenByDescending(node); LastCall = nameof(Enumerable.ThenByDescending); break;
                case nameof(Enumerable.Take): TranslateTake(node); LastCall = nameof(Enumerable.Take); break;
                case nameof(Enumerable.Skip): TranslateSkip(node); LastCall = nameof(Enumerable.Skip); break;
                case nameof(Enumerable.First): TranslateFirst(node); LastCall = nameof(Enumerable.First); break;
                case nameof(Enumerable.FirstOrDefault): TranslateFirst(node); LastCall = nameof(Enumerable.FirstOrDefault); break;
                case nameof(Enumerable.Single): TranslateFirst(node); LastCall = nameof(Enumerable.Single); break;
                case nameof(Enumerable.SingleOrDefault): TranslateFirst(node); LastCall = nameof(Enumerable.SingleOrDefault); break;
                case nameof(Enumerable.Last): TranslateLast(node); LastCall = nameof(Enumerable.Last); break;
                case nameof(Enumerable.LastOrDefault): TranslateLast(node); LastCall = nameof(Enumerable.LastOrDefault); break;
                case nameof(Enumerable.All): TranslateAll(node); LastCall = nameof(Enumerable.All); break;
                case nameof(Enumerable.Any): TranslateAny(node); LastCall = nameof(Enumerable.Any); break;
                case nameof(Enumerable.Count): TranslateCount(node); LastCall = nameof(Enumerable.Count); break;
                case nameof(Enumerable.Distinct): TranslateDistinct(node); LastCall = nameof(Enumerable.Distinct); break;
                case nameof(Enumerable.Max): TranslateMax(node); LastCall = nameof(Enumerable.Max); break;
                case nameof(Enumerable.Min): TranslateMin(node); LastCall = nameof(Enumerable.Min); break;
                case nameof(Enumerable.Sum): TranslateSum(node); LastCall = nameof(Enumerable.Sum); break;
                case nameof(Enumerable.Average): TranslateAverage(node); LastCall = nameof(Enumerable.Average); break;
                case nameof(Enumerable.Join): TranslateJoin(node); LastCall = nameof(Enumerable.Join); break;
                default: throw new NotSupportedException(string.Format(ErrorMessages.CallExpressionNotSupported, node));
            }

            return node;
        }

        protected void TranslateSelect(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT ");
            _stringBuilder.Append(NewVisitor.Print(node.Arguments.Last()));
            Visit(node.Arguments.First());
        }

        protected void TranslateWhere(MethodCallExpression node)
        {
            Visit(node.Arguments.First());
            _stringBuilder.Append(" WHERE ");
            Visit(node.Arguments.Last());
        }

        protected void TranslateOrderBy(MethodCallExpression node)
        {
            Visit(node.Arguments.First());
            _stringBuilder.Append(" ORDER BY ");
            Visit(node.Arguments.Last());
        }

        protected void TranslateOrderByDescending(MethodCallExpression node)
        {
            TranslateOrderBy(node);
            _stringBuilder.Append(" DESC");
        }

        protected void TranslateThenBy(MethodCallExpression node)
        {
            TranslateOrderBy(node);
            _stringBuilder.Append(" DESC");
        }

        protected void TranslateThenByDescending(MethodCallExpression node)
        {
            TranslateOrderBy(node);
            _stringBuilder.Append(" DESC");
        }

        protected void TranslateTake(MethodCallExpression node)
        {
            Visit(node.Arguments.First());
            _stringBuilder.Append(" LIMIT ");
            Visit(node.Arguments.Last());
        }

        protected void TranslateSkip(MethodCallExpression node)
        {
            if (node.Arguments.First() is not MethodCallExpression argument ||
                argument.Method.Name is not nameof(Enumerable.Take))
            {
                throw new InvalidOperationException(ErrorMessages.OffsetClauseWithoutLimitErrorMessage);
            }

            TranslateTake(argument);
            _stringBuilder.Append(" OFFSET ");
            Visit(node.Arguments.Last());
        }

        protected void TranslateFirst(MethodCallExpression node)
        {
            Visit(node.Arguments.First());
            _stringBuilder.Append(" LIMIT 1");
        }

        protected void TranslateLast(MethodCallExpression node)
        {
            Visit(node.Arguments.First());

            if (LastCall is nameof(Enumerable.Where))
                _stringBuilder.Append(" AND ");
            else
                _stringBuilder.Append(" WHERE ");

            string primaryKey = node.Type.KeyColumn();

            _stringBuilder.Append(primaryKey);
            _stringBuilder.Append(" = ");
            _stringBuilder.Append('(');
            _stringBuilder.Append("SELECT MAX(");
            _stringBuilder.Append(primaryKey);
            _stringBuilder.Append(')');
            Visit(node.Arguments.First());
            _stringBuilder.Append(')');
        }

        protected void TranslateAll(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT COUNTIF (");
            Visit(node.Arguments.Last());
            _stringBuilder.Append(')');
            _stringBuilder.Append(" = COUNT(*)");
            _stringBuilder.Append(" FROM (");
            _stringBuilder.Append(NewVisitor.Print(node.Arguments.First()));
            _stringBuilder.Append(')');
        }

        protected void TranslateAny(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT COUNTIF (");
            Visit(node.Arguments.Last());
            _stringBuilder.Append(')');
            _stringBuilder.Append(" > 0");
            _stringBuilder.Append(" FROM (");
            _stringBuilder.Append(NewVisitor.Print(node.Arguments.First()));
            _stringBuilder.Append(')');
        }

        protected void TranslateCount(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT COUNT(*)");
            Visit(node.Arguments.First());
        }

        protected void TranslateDistinct(MethodCallExpression node)
        {
            if (_stringBuilder.Length is not 0)
            {
                return;
            }

            LastTable = node.Type.GenericTypeArguments[0].Name;

            _stringBuilder.Append("SELECT DISTINCT ");
            var columns = GetColumnsFromProperties(node.Type.GenericTypeArguments[0].GetProperties());
            _stringBuilder.Append(string.Join(", ", columns));

            Visit(node.Arguments.First());
        }

        protected void TranslateMin(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT MIN(");
            Visit(node.Arguments.Last());
            _stringBuilder.Append(')');
            Visit(node.Arguments.First());
        }

        protected void TranslateMax(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT MAX(");
            Visit(node.Arguments.Last());
            _stringBuilder.Append(')');
            Visit(node.Arguments.First());
        }

        protected void TranslateSum(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT SUM(");
            Visit(node.Arguments.Last());
            _stringBuilder.Append(')');
            Visit(node.Arguments.First());
        }

        protected void TranslateAverage(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT AVG(");
            Visit(node.Arguments.Last());
            _stringBuilder.Append(')');
            Visit(node.Arguments.First());
        }

        protected void TranslateJoin(MethodCallExpression node)
        {
            _stringBuilder.Append("SELECT ");
            Visit(node.Arguments[4]);
            Visit(node.Arguments[0]);
            _stringBuilder.Append(" INNER JOIN ");
            _stringBuilder.Append(NewJoinVisitor.Print(node.Arguments[1]));
            _stringBuilder.Append(" ON ");
            Visit(node.Arguments[2]);
            _stringBuilder.Append(" = ");
            Visit(node.Arguments[3]);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return node.Value switch
            {
                Table table => VisitConstantTable(table),
                String str => VisitConstantString(str),
                DateTime dateTime => VisitConstantDateTime(dateTime),
                TimeSpan timeSpan => VisitConstantTimeSpan(timeSpan),
                _ => VisitConstantDefault()
            };

            Expression VisitConstantTable(Table table)
            {
                Type tableType = node.Type.GenericTypeArguments[0];
                LastTable = tableType.Name;

                if (_stringBuilder.Length is 0)
                {
                    _stringBuilder.Append("SELECT ");

                    var columns = GetColumnsFromProperties(tableType.GetProperties());

                    _stringBuilder.Append(string.Join(", ", columns));
                    _stringBuilder.Append(string.Format(" FROM " + "{0}.{1} AS {2}", table.DatasetName, table.TableName, tableType.Name));
                }
                else
                {
                    _stringBuilder.Append(string.Format(" FROM " + "{0}.{1} AS {2}", table.DatasetName, table.TableName, tableType.Name));
                }

                return node;
            }
            Expression VisitConstantString(string str)
            {
                _stringBuilder.AppendWithQuotationAround(str);
                return node;
            }
            Expression VisitConstantDateTime(DateTime dateTime)
            {
                _stringBuilder.Append(dateTime.ToString(Formats.DefaultDateTimeFormat));
                return node;
            }
            Expression VisitConstantTimeSpan(TimeSpan timeSpan)
            {
                _stringBuilder.Append(timeSpan.ToString(Formats.TimeOnlyFormat));
                return node;
            }
            Expression VisitConstantDefault()
            {
                _stringBuilder.Append(node.Value);
                return node;
            }
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
            var parameterIsRoot = LastTable is null;

            if (parameterIsRoot)
            {
                var columns = GetColumnsFromProperties(node.Type.GetProperties(), node.Type.Name);
                _stringBuilder.Append(string.Join(", ", columns));
                return node;
            }

            _stringBuilder.Append(LastTable);
            _stringBuilder.Append('.');
            _stringBuilder.Append(node.Name);
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

    internal class BigQueryJoinExpressionVisitor : BigQueryExpressionVisitor
    {
        private bool _methodCallVisited;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _methodCallVisited = true;

            _stringBuilder.Append('(');
            var result = base.VisitMethodCall(node);
            _stringBuilder.Append(')');
            return result;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return node.Value switch
            {
                Table table => VisitConstantTable(table),
                _ => base.VisitConstant(node),
            };

            Expression VisitConstantTable(Table table)
            {
                Type tableType = node.Type.GenericTypeArguments[0];
                LastTable = tableType.Name;

                var columns = GetColumnsFromProperties(tableType.GetProperties());

                if (_methodCallVisited)
                {
                    _stringBuilder.Append("SELECT ");
                    _stringBuilder.Append(string.Join(", ", columns));
                    _stringBuilder.Append(" FROM ");
                }

                _stringBuilder.Append(string.Format("{0}.{1} AS {2}", table.DatasetName, table.TableName, tableType.Name));

                return node;
            }

        }
    }
}
