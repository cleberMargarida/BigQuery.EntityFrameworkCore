using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class DistinctTests
    {
        private readonly BqTestContext _context;

        public DistinctTests(BqTestContext bqTestContext)
        {
            _context = bqTestContext;
        }

        [Fact]
        public void DataProducts_DistinctToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Distinct().ToString();
            Assert.Equal("SELECT DISTINCT Id Id, ProductName Name FROM data.Product AS Product", query);
        }
    }
}
