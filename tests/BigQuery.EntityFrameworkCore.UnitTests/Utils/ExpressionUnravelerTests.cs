using System.Linq.Expressions;

namespace BigQuery.EntityFrameworkCore.UnitTests.Utils
{
    public class ExpressionUnravelerTests
    {
        [Fact]
        public void RewriteSelect_SelectWhere_ShouldSwap()
        {
            Expression<Func<Table<Product>, IQueryable<int>>> expression = xs => xs.Select(x => x.Id).Where(x => x == 1);
            Expression<Func<Table<Product>, IQueryable<int>>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id);

            var actual = ExpressionUnraveler.Rewrite(expression);

            Assert.Equivalent(expected, actual);
        }       
        
        [Fact]
        public void RewriteSelect_SelectWhereWithMoreThanOneArgument_ShouldSwap()
        {
            Expression<Func<Table<Product>, IQueryable<int>>> expression = xs => xs.Select(x => x.Id).Where(x => x == 1 || x == 2);
            Expression<Func<Table<Product>, IQueryable<int>>> expected = xs => xs.Where(x => x.Id == 1 || x.Id == 2).Select(x => x.Id);

            var actual = ExpressionUnraveler.Rewrite(expression);

            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectCount_ShouldSwap()
        {
            Expression<Func<Table<Product>, int>> expression = xs => xs.Select(x => x.Id).Count(x => x == 1);
            Expression<Func<Table<Product>, int>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).Count();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectAny_ShouldSwap()
        {
            Expression<Func<Table<Product>, bool>> expression = xs => xs.Select(x => x.Id).Any(x => x == 1);
            Expression<Func<Table<Product>, bool>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).Any();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectLongCount_ShouldSwap()
        {
            Expression<Func<Table<Product>, long>> expression = xs => xs.Select(x => x.Id).LongCount(x => x == 1);
            Expression<Func<Table<Product>, long>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).LongCount();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectFirst_ShouldSwap()
        {
            Expression<Func<Table<Product>, int>> expression = xs => xs.Select(x => x.Id).First(x => x == 1);
            Expression<Func<Table<Product>, int>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).First();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectFirstOrDefault_ShouldSwap()
        {
            Expression<Func<Table<Product>, int>> expression = xs => xs.Select(x => x.Id).FirstOrDefault(x => x == 1);
            Expression<Func<Table<Product>, int>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).FirstOrDefault();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectLast_ShouldSwap()
        {
            Expression<Func<Table<Product>, int>> expression = xs => xs.Select(x => x.Id).Last(x => x == 1);
            Expression<Func<Table<Product>, int>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).Last();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectLastOrDefault_ShouldSwap()
        {
            Expression<Func<Table<Product>, int>> expression = xs => xs.Select(x => x.Id).LastOrDefault(x => x == 1);
            Expression<Func<Table<Product>, int>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).LastOrDefault();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectSingle_ShouldSwap()
        {
            Expression<Func<Table<Product>, int>> expression = xs => xs.Select(x => x.Id).Single(x => x == 1);
            Expression<Func<Table<Product>, int>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).Single();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectSingleOrDefault_ShouldSwap()
        {
            Expression<Func<Table<Product>, int>> expression = xs => xs.Select(x => x.Id).SingleOrDefault(x => x == 1);
            Expression<Func<Table<Product>, int>> expected = xs => xs.Where(x => x.Id == 1).Select(x => x.Id).SingleOrDefault();
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void RewriteSelect_SelectAll_ShouldSwap()
        {
            Expression<Func<Table<Product>, bool>> expression = xs => xs.Select(x => x.Id).All(x => x == 1);
            Expression<Func<Table<Product>, bool>> expected = xs => xs.All(x => x.Id == 1);
            var actual = ExpressionUnraveler.Rewrite(expression);
            Assert.Equivalent(expected, actual);
        }
    }
}
