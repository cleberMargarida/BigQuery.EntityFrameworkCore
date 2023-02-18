using BigQuery.EntityFrameworkCore.Utils;
using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.UnitTests.Query
{
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

        [Fact]
        public void TranslatorPrint_NormalDateOnly_ShouldBeEqualExpected()
        {
            var date = new DateOnly(2022, 02, 02);
            var expression = _tranlator.Print(Expression.Constant(date));
            Assert.Equal(date.ToString(Formats.DateOnlyFormat), expression);
        }

        [Fact]
        public void TranslatorPrint_NormalTimeOnly_ShouldBeEqualExpected()
        {
            var time = new TimeOnly(02, 02, 02);
            var expression = _tranlator.Print(Expression.Constant(time));
            Assert.Equal(time.ToString(Formats.TimeOnlyFormat), expression);
        }
    }
}
