﻿using BigQuery.EntityFrameworkCore.Utils;
using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore
{
    public interface IExpressionPrinter
    {
        Expression? Visit(Expression? expression);
        internal string Print(Expression expression);
    }

    internal partial class BigQueryExpressionVisitor : IExpressionPrinter
    {
        internal string Print(Expression expression)
        {
            Visit(expression);
            return _stringBuilder.ToString().FormatSql();
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
