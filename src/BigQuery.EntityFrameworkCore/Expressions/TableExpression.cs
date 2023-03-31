using BigQuery.EntityFrameworkCore;
using BigQuery.EntityFrameworkCore.Utils;

namespace System.Linq.Expressions
{
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
}