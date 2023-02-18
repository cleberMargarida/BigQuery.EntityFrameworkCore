using Schemas;

namespace BigQuery.EntityFrameworkCore.FuncionalTests
{
    public class BigQueryContextTests
    {
        private readonly BqTestContext _context;

        public BigQueryContextTests(BqTestContext context)
        {
            _context = context;
        }

        [Fact]
        public void Test()
        {
            var product = _context.Data.Products.Where(x => x.Name == "Product-1").Any(x => x.Id > 0);
        }
    }
}
