using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.UnitTests.Query;

public class BigQueryExpressionVisitorTests
{
    private readonly IExpressionPrinter _tranlator;

    public BigQueryExpressionVisitorTests(IExpressionPrinter tranlator)
    {
        _tranlator = tranlator;
    }

    [Fact]
    public void TranslatorPrint_NormalDateTime_ShouldBeEqualExpected()
    {
        var datetime = new DateTime(2022,02,02, 02, 02, 02);
        var expression = _tranlator.Print(Expression.Constant(datetime));
        Assert.Equal(datetime.ToString(Formats.DefaultDateTimeFormat), expression);
    }
}
