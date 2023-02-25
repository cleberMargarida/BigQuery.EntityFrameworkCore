using Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class TakeTests
    {
        private readonly BqTestContext _context;

        public TakeTests(BqTestContext bqTestContext)
        {
            _context = bqTestContext;
        }

        [Fact]
        public void DataProducts_Take20ToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Take(20).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName FROM data.Product AS Product LIMIT 20", query);
        }
    }
}
