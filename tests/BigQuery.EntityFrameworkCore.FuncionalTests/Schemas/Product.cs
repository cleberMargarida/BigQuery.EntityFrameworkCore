using System.ComponentModel.DataAnnotations.Schema;

namespace Schemas
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }

        [Column("ProductName")]
        public string Name { get; set; }
    }

    [Table("ProductMetadata")]
    public class ProductMetadata : Product
    {
    }

    [Table("Store")]
    public class Store
    {
        public int Id { get; set; }
        [Column("Name")]
        public string StoreName { get; set; }
    }
}
