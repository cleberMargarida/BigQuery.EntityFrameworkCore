using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class TableTests
    {
        private readonly BqTestContext _context;

        public TableTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void DataProducts_ToString_ShouldReturnTheExpectedQuery()
        {
            var query = _context.Data.Products.ToString();
            Assert.Equal("SELECT Id Id, ProductName Name FROM data.Product AS Product", query);
        }

        [Fact]
        public void DataProducts_ToString_ShouldNotReturnTheExpectedQuery()
        {
            var query = _context.Data.Products.ToString();
            Assert.NotEqual("SELECT Id Id, ProductName Name FROM ********* AS Product", query);
        }
    }
}
