using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class WhereTests
    {
        private readonly BqTestContext _context;

        public WhereTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void DataProducts_WhereIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Where(x => x.Id == 1).ToString();
            Assert.Equal("SELECT Id Id, ProductName Name FROM data.Product AS Product WHERE Id = 1", query);
        }

        [Fact]
        public void DataProducts_WhereIdAndNameToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Where(x => x.Id == 1 && x.Name == "some").ToString();
            Assert.Equal(@"SELECT Id Id, ProductName Name FROM data.Product AS Product WHERE Id = 1 AND ProductName = ""some""", query);
        }

        [Fact]
        public void DataProducts_WhereIdSelectIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Where(x => x.Id == 1).Select(x => x.Id).ToString();
            Assert.Equal(@"SELECT Id Id FROM data.Product AS Product WHERE Id = 1", query);
        }
    }
}
