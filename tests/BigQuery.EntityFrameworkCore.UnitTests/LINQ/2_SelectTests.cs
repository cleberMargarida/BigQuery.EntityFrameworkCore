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
            Assert.Equal("SELECT Id Id FROM data.Product AS Product", query);
        }

        [Fact]
        public void DataProducts_SelectIdAndNameToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => new { x.Id, x.Name }).ToString();
            Assert.Equal("SELECT Id Id, ProductName Name FROM data.Product AS Product", query);
        }

        [Fact]
        public void DataProducts_SelectIdWhereIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Select(x => x.Id).Where(Id => Id == 1).ToString();
            Assert.Equal(@"SELECT Id Id FROM data.Product AS Product WHERE Id = 1", query);
        }

        //[Fact] revisit after
        //public void Test()
        //{
        //    var query = _context.Data.Marrieds.Select(x => x.Wife).ToString();
        //    Assert.Equal("SELECT Wifes.Id Id, Wifes.Name Name " +
        //                 "FROM data.Marrieds AS Marrieds " +
        //                 "INNER JOIN data.Wifes as Wifes " +
        //                 "ON Marrieds.WifeId = Id", query);
        //}
    }
}
