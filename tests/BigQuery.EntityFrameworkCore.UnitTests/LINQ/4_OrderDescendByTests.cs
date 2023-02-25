using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class OrderByDescendingTests
    {
        private readonly BqTestContext _context;

        public OrderByDescendingTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void DataProducts_OrderByDescendinIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.OrderByDescending(x => x.Id).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName FROM data.Product AS Product ORDER BY Product.Id DESC", query);
        }
    }
}
