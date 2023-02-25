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
            Assert.Equal("SELECT Product.Id, Product.ProductName FROM data.Product AS Product", query);
        }
    }
}
