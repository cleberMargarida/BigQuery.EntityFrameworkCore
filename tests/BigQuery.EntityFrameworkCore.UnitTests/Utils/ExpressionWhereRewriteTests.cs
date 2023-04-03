using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.UnitTests.Utils
{
    using static ExpressionWhereRewrite;

    public class ExpressionWhereRewriteTests
    {
        [Fact]
        public void RewriteWhere_TwoWhere_ShouldCombine()
        {
            Expression<Func<Table<Product>, IQueryable<Product>>> expression = xs => xs.Where(x => x.Id == 1).Where(x => x.Id == 2);
            Expression<Func<Table<Product>, IQueryable<Product>>> expected = xs => xs.Where(x => x.Id == 1 && x.Id == 2);
            var actual = Rewrite(expression);

            Assert.Equivalent(expected, actual);
        }
        
        [Fact]
        public void RewriteWhere_ThreeWhere_ShouldCombine()
        {
            Expression<Func<Table<Product>, IQueryable<Product>>> expression = xs => xs.Where(x => x.Id == 1).Where(x => x.Id == 2).Where(x => x.Id == 3);
            Expression<Func<Table<Product>, IQueryable<Product>>> expected = xs => xs.Where(x => x.Id == 1 && x.Id == 2 && x.Id == 3);
            var actual = Rewrite(expression);

            Assert.Equivalent(expected, actual);
        }
    }
}
