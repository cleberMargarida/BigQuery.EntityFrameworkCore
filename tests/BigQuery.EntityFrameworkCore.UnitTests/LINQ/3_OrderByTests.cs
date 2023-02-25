using Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class OrderByTests
    {
        private readonly BqTestContext _context;

        public OrderByTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void DataProducts_OrderByIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.OrderBy(x => x.Id).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName FROM data.Product AS Product ORDER BY Product.Id", query);
        }
    }
}
