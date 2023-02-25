namespace BigQuery.EntityFrameworkCore.Utils
{
    internal static class StringExt
    {
        public static string FormatSql(this string source)
        {
            return new SqlFormatter(source).Format();
        }
    }
}
