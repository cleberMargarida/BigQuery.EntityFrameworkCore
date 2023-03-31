using BigQuery.EntityFrameworkCore.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("BigQuery.EntityFrameworkCore.UnitTests", AllInternalsVisible = true)]
namespace BigQuery.EntityFrameworkCore
{
    partial class BigQueryExpressionVisitor : BigQueryExpressionVisitorBase
    {
        protected readonly StringBuilder _stringBuilder = new StringBuilder();
        private static readonly Dictionary<ExpressionType, string> _binaryOperandMap = new()
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

        protected List<TableExpression> Tables { get; private set; } = new();

        protected override Expression VisitSelect(SelectExpression node)
        {
            _stringBuilder.Append("SELECT ");
            Visit(node.Seletor);
            Visit(node.Source);
            return node;
        }

        protected override Expression VisitWhere(WhereExpression node)
        {
            Visit(node.Source);
            _stringBuilder.Append(" WHERE ");
            Visit(node.Condition);
            return node;
        }

        protected override Expression VisitOrderBy(OrderByExpression node)
        {
            Visit(node.Source);
            _stringBuilder.Append(" ORDER BY ");
            Visit(node.Seletor);
            return node;
        }

        protected override Expression VisitOrderByDescending(OrderByDescendingExpression node)
        {
            VisitOrderBy(node);
            _stringBuilder.Append(" DESC");
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
            _stringBuilder.Append(" LIMIT ");
            Visit(node.Number);
            return node;
        }

        protected override Expression VisitSkip(SkipExpression node)
        {
            Visit(node.Take);
            _stringBuilder.Append(" OFFSET ");
            Visit(node.Number);
            return node;
        }

        protected override Expression VisitFirst(FirstExpression node)
        {
            Visit(node.Source);
            _stringBuilder.Append(" LIMIT 1");
            return node;
        }

        protected override Expression VisitLast(LastExpression node)
        {
            Visit(node.Source);

            if (CallStack.Contains(nameof(VisitWhere)))
            {
                _stringBuilder.Append(" AND ");
            }
            else
            {
                _stringBuilder.Append(" WHERE ");
            }

            var lastSource = Tables.Last();
            var keyColumn = lastSource.TableType.Name + "." + lastSource.TableType.KeyColumn();

            _stringBuilder.Append(keyColumn);
            _stringBuilder.Append(" = ");
            _stringBuilder.Append('(');
            _stringBuilder.Append("SELECT MAX(");
            _stringBuilder.Append(keyColumn);
            _stringBuilder.Append(')');
            VisitConstantTable(lastSource);
            _stringBuilder.Append(')');
            return node;
        }

        protected override Expression VisitAll(AllExpression node)
        {
            _stringBuilder.Append("SELECT COUNTIF (");
            Visit(node.Seletor);
            _stringBuilder.Append(')');
            _stringBuilder.Append(" = COUNT(*)");
            Visit(node.Source);
            return node;
        }

        protected override Expression VisitAny(AnyExpression node)
        {
            _stringBuilder.Append("SELECT COUNTIF (");
            Visit(node.Seletor);
            _stringBuilder.Append(')');
            _stringBuilder.Append(" > 0");
            Visit(node.Source);
            return node;
        }

        protected override Expression VisitCount(CountExpression node)
        {
            _stringBuilder.Append("SELECT COUNT(*)");
            Visit(node.Source);
            return node;
        }

        protected override Expression VisitDistinct(DistinctExpression node)
        {
            return node;
        }

        protected override Expression VisitMin(MinExpression node)
        {
            _stringBuilder.Append("SELECT MIN(");
            Visit(node.Seletor);
            _stringBuilder.Append(')');
            Visit(node.Source);

            return node;
        }

        protected override Expression VisitMax(MaxExpression node)
        {
            _stringBuilder.Append("SELECT MAX(");
            Visit(node.Seletor);
            _stringBuilder.Append(')');
            Visit(node.Source);

            return node;
        }

        protected override Expression VisitSum(SumExpression node)
        {
            _stringBuilder.Append("SELECT SUM(");
            Visit(node.Seletor);
            _stringBuilder.Append(')');
            Visit(node.Source);

            return node;
        }

        protected override Expression VisitAverage(AverageExpression node)
        {
            _stringBuilder.Append("SELECT AVG(");
            Visit(node.Seletor);
            _stringBuilder.Append(')');
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

        protected override Expression VisitConstantTable(TableExpression node)
        {
            var nodeString = node.ToString();
            Tables.Add(node);

            if (!_stringBuilder.ToString().StartsWith("SELECT "))
            {
                _stringBuilder.Append("SELECT ");
                _stringBuilder.Append(string.Join(", ", GetColumns(node.TableType)));
            }

            _stringBuilder.Append(nodeString);
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
            var column = source + "." + member;
            _stringBuilder.Append(column);
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

        protected string[] GetColumns(Type tableType)
        {
            var prefix = tableType.Name;
            var propertyInfos = tableType.GetProperties();
            return propertyInfos.Select(p => prefix + "." + (p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name)).ToArray();
        }
    }
}
