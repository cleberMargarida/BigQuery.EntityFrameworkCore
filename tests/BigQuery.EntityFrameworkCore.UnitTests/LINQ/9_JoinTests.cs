using Schemas;

namespace BigQuery.EntityFrameworkCore.UnitTests.LINQ
{
    public class JoinTests
    {
        private readonly BqTestContext _context;

        public JoinTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void DataProducts_JoinMetadataProductsMetadataToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Join(_context.Metadata.ProductsMetadata, x => x.Id, x => x.Id, (x, y) => new { x, y }).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName, ProductMetadata.Id, ProductMetadata.ProductName FROM data.Product AS Product INNER JOIN Metadata.ProductMetadata AS ProductMetadata ON Product.Id = ProductMetadata.Id", query);
        }

        [Fact]
        public void DataProducts_JoinMetadataProductsMetadataIdToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Join(_context.Metadata.ProductsMetadata, x => x.Id, x => x.Id, (x, y) => new { x, y.Id }).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName, ProductMetadata.Id FROM data.Product AS Product INNER JOIN Metadata.ProductMetadata AS ProductMetadata ON Product.Id = ProductMetadata.Id", query);
        }

        [Fact]
        public void DataProducts_JoinMetadataProductsMetadataIdAliasProductMetadataToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Join(_context.Metadata.ProductsMetadata, x => x.Id, x => x.Id, (x, y) => new { x, y.Id, y }).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName, ProductMetadata.Id, ProductMetadata.Id, ProductMetadata.ProductName FROM data.Product AS Product INNER JOIN Metadata.ProductMetadata AS ProductMetadata ON Product.Id = ProductMetadata.Id", query);
        }

        [Fact]
        public void DataProducts_JoinMetadataProductsMetadataWhereToString_ShouldReturnExpected()
        {
            var query = _context.Data.Products.Join(_context.Metadata.ProductsMetadata.Where(x => x.Id == 1), x => x.Id, x => x.Id, (x, y) => new { x, y }).ToString();
            Assert.Equal("SELECT Product.Id, Product.ProductName, ProductMetadata.Id, ProductMetadata.ProductName FROM data.Product AS Product INNER JOIN (SELECT ProductMetadata.Id, ProductMetadata.ProductName FROM Metadata.ProductMetadata AS ProductMetadata WHERE ProductMetadata.Id = 1) ON Product.Id = ProductMetadata.Id", query);
        }
    }
}
