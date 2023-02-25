using Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class SelectTests
    {
        private readonly BqTestContext _context;

        public SelectTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void DataProducts_SelectIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => x.Id).ToString();
            Assert.Equal("SELECT Product.Id FROM data.Product AS Product", query);
        }

        [Fact]
        public void DataProducts_SelectIdAndNameToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => new { x.Id, x.Name }).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName FROM data.Product AS Product", query);
        }

        [Fact]
        public void DataProducts_SelectIdWhereIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => x.Id).Where(Id => Id == 1).ToString();
            Assert.Equal("SELECT Product.Id FROM data.Product AS Product WHERE Product.Id = 1", query);
        }

        [Fact]
        public void DataProducts_SelectIdOrderByIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => x.Id).OrderBy(Id => Id).ToString();
            Assert.Equal("SELECT Product.Id FROM data.Product AS Product ORDER BY Product.Id", query);
        }

        [Fact]
        public void DataProducts_SelectIdOrderByDescendingIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => x.Id).OrderByDescending(Id => Id).ToString();
            Assert.Equal("SELECT Product.Id FROM data.Product AS Product ORDER BY Product.Id DESC", query);
        }

        [Fact]
        public void DataProducts_SelectIdTake1ToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => x.Id).Take(1).ToString();
            Assert.Equal("SELECT Product.Id FROM data.Product AS Product LIMIT 1", query);
        }

        [Fact]
        public void DataProducts_SelectIdTake1Skip1ToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => x.Id).Take(1).Skip(1).ToString();
            Assert.Equal("SELECT Product.Id FROM data.Product AS Product LIMIT 1 OFFSET 1", query);
        }
    }
}
