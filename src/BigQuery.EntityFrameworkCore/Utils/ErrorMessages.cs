namespace BigQuery.EntityFrameworkCore.Utils
{
    public class ErrorMessages
    {
        public const string DuplicateTableErrorMessage = "The table {0} was registered more than 1 time. Tables with same type are not allowed to differents Datasets.";
        public const string WasNotPossibleCreateErrorMessage = "Was not possible create {0}";
        public const string OffsetClauseWithoutLimitErrorMessage = "The OFFSET clause is only valid when used in conjunction with the LIMIT clause in a SELECT statement in BigQuery. Consider use Take() before Skip() in LINQ call chain.";
        public const string CallExpressionNotSupported = "The LINQ expression '{0}' could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable ', 'ToList ', or 'ToArray '. See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.";
        public const string KeyColumnNotFounded = "The key column was not founded, try mark the key property with KeyAttribute on type {0}.";
        public const string NoSequenceErrorMessage = "The source sequence is empty.";
    }
}
