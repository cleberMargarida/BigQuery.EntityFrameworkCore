using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.EntityFrameworkCore.Utils
{
    public class ErrorMessages
    {
        public const string DuplicateTableErrorMessage = "The table {0} was registered more than 1 time. Tables with same type are not allowed to differents Datasets.";
        public const string WasNotPossibleCreateErrorMessage = "Was not possible create {0}";
        public const string OffsetClauseWithoutLimitErrorMessage = "The OFFSET clause is only valid when used in conjunction with the LIMIT clause in a SELECT statement in BigQuery. Consider use Take() before Skip() in LINQ call chain.";
        public const string CallExpressionNotSupported = "The Expression {0} called was not supported. Try get the enumerator then append the .{0}";
        public const string KeyColumnNotFounded = "The key column was not founded, try mark the key property with KeyAttribute on type {0}.";
        public const string NoSequenceErrorMessage = "The source sequence is empty.";
    }
}
